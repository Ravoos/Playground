using System.Collections.Immutable;
using Playground.Projects.Poker_optional.Extensions;
using Playground.Projects.Poker_optional.Models;
using PlayGround.Extensions;

namespace Playground.Projects.Poker_optional;

public static class PokerGame
{
    public static void RunSimulation()
    {
        Console.WriteLine("Testing the Deck.");

        var smallDeck = CardDeck.Create()
            .Tap(deck => Console.WriteLine("Shuffled Deck:\n" + deck))

            .Shuffle()
            .Tap(deck => Console.WriteLine("Shuffled Deck:\n" + deck))

            .Sort(cards => cards.OrderBy(c => c.Suit).ThenBy(c => c.Rank))
            .Tap(deck => Console.WriteLine("Sorted Deck:\n" + deck))

            .Keep(c => c.Rank switch {
                CardRank.Two or CardRank.Three or CardRank.Five or CardRank.Seven => true,
                _ => false
            })
            .Tap(deck => Console.WriteLine("Kept Deck (Only 2,3,5,7):\n" + deck))

            .Remove(c => c.Suit == CardSuit.Hearts)
            .Tap(deck => Console.WriteLine("Removed Hearts:\n" + deck))

            .Fork(deck => deck.AddToTop(new Card(CardSuit.Spades, CardRank.Ace)),
                  deck => deck.AddToBottom(new Card(CardSuit.Diamonds, CardRank.King)),
                  (topAdded, bottomAdded) => bottomAdded.AddDeck(topAdded).RemoveDuplicates())
            .Tap(deck => Console.WriteLine("Added Cards and Removed Duplicates:\n" + deck))

            .Draw(out var drawnCard)
            .Tap(deck => Console.WriteLine($"Drew Card: {drawnCard}\nRemaining Deck:\n" + deck));


        Console.WriteLine("Testing Player.");

        ImmutableList<Player> players = ImmutableList.Create(
            new Player("Alice", new PokerHand()),
            new Player("Bob", new PokerHand()),
            new Player("Diana", new PokerHand()))

            .Tap(p => Console.WriteLine(string.Join(", ", p.Select(pl => $"{pl.Name} has {pl.Hand.cards.Count} cards on hand"))));


        Console.WriteLine("\nPoker Round Simulation:");
        Console.WriteLine("Your code should implement the Poker round simulation below.");
        Console.WriteLine("========================");

        /*var deck = CardDeck.Create().Shuffle();

        var (deckThisRound, playersThisRound) = DealOneRound(deck, players);

        playersThisRound.Tap(
            ps =>
            {
                Console.WriteLine("\n =========== HAND DEALT ===============");
                foreach (var player in ps)
                {
                    Console.WriteLine($"{player.Name}'s hand: {player.Hand}");
                }
            });

        Console.WriteLine($"\nCards remaining in deck after dealing: {deckThisRound.cards.Count}");*/

        Console.WriteLine("\n================ RUNNING MULTIPLE ROUNDS ================");
        var finalScore = RunRounds(CardDeck.Create().Shuffle(), players, ScoreCard.Empty);

        Console.WriteLine("\n================ FINAL SCORE ================");
        foreach (var (playerName, scores) in finalScore.Scores.OrderByDescending(s => s.Value))
        {
            Console.WriteLine($"{playerName}: {scores}");
        }

        var maxScore = finalScore.Scores.Values.Max();
        var winner = finalScore.Scores
            .Where(s => s.Value == maxScore)
            .Select(s => s.Key)
            .ToImmutableList();

        Console.WriteLine(
            winner.Count == 1
                ? $"Overall Winner: {winner[0]} with {maxScore} wins!"
                : $"Overall Tie between: {string.Join(", ", winner)} with {maxScore} wins!"
        );

    }

    private static (CardDeck deck, ImmutableList<Player> players) DealOneRound(
    CardDeck deck,
    ImmutableList<Player> players)
    {
        var cardsNeeded = players.Count * 5;

        if (deck.cards.Count < cardsNeeded)
            return (deck, players);

        return players.Aggregate(
            (deck, ImmutableList<Player>.Empty),
            (state, player) =>
            {
                var (currentDeck, updatedPlayers) = state;

                var handCards = currentDeck.cards
                    .TakeLast(5)
                    .ToImmutableList();

                var newDeck = currentDeck.Map(d => d with
                {
                    cards = d.cards.RemoveRange(d.cards.Count - 5, 5)
                });

                var updatedPlayer = player with
                {
                    Hand = new PokerHand { cards = handCards }
                };

                return (newDeck, updatedPlayers.Add(updatedPlayer));
            });
    }
    private static ScoreCard RunRounds(
        CardDeck deck,
        ImmutableList<Player> players,
        ScoreCard scoreCard)
    {
        // Base case: not enough cards for another round
        if (deck.cards.Count < players.Count * 5)
            return scoreCard;

        var (nextDeck, dealtPlayers) = DealOneRound(deck, players);

        Console.WriteLine("\n--- New Round ---");

        dealtPlayers.Tap(ps =>
        {
            foreach (var p in ps)
                Console.WriteLine($"{p.Name}: {string.Join(", ", p.Hand.cards)}");
        });

        var winners = DetermineRoundWinner(dealtPlayers);

        Console.WriteLine(
            winners.Count == 1
                ? $"Winner: {winners[0].Name}"
                : $"Tie between: {string.Join(", ", winners.Select(w => w.Name))}"
        );

        var updatedScore = scoreCard.AddWins(winners.Select(w => w.Name));

        var resetPlayers = dealtPlayers
            .Select(p => p with { Hand = new PokerHand() })
            .ToImmutableList();

        return RunRounds(nextDeck, resetPlayers, updatedScore);
    }


    private static ImmutableList<Player> DetermineRoundWinner(
        ImmutableList<Player> players)
    {
        var ranked = players
            .Select(p => (Player: p, Ranked: p.Hand.GetPokerRank()))
            .ToList();

        var bestHand = ranked
            .Select(r => r.Ranked)
            .Max();

        return ranked
            .Where(r => r.Ranked.CompareTo(bestHand) == 0)
            .Select(r => r.Player)
            .ToImmutableList();
    }


}