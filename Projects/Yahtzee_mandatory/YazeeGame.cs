using System.Collections.Immutable;
using Playground.Projects.Yahtzee_mandatory.Extensions;
using Playground.Projects.Yahtzee_mandatory.Models;
using PlayGround.Extensions;
using YahtzeeCombo = Playground.Projects.Yahtzee_mandatory.Models.Yahtzee;

namespace Playground.Projects.Yahtzee_mandatory;

public static class YahzeeGame
{
    public static void RunSimulation()
    {
        Console.WriteLine("Testing the Cup of Dice.");

        var cupOfDice = new CupOfDice(10);
        Console.WriteLine($"Cup with 10 dice: {cupOfDice}\n");

        cupOfDice = new CupOfDice(2);
        Console.WriteLine($"Cup with 2 dice: {cupOfDice}\n");


        var yahzeeCup = new YahzeeCup();
        Console.WriteLine($"Yahzee Cup with 5 dice: {yahzeeCup}\n");

        Enumerable.Range(1, 10)
            .Aggregate(yahzeeCup, (currentCup, i) =>
            {
                var newCup = currentCup
                .ShakeAndRoll();

                newCup.Tap(cup => Console.WriteLine($"Yahzee Cup with 5 dice: {cup}"))
                .GetYahtzeeCombination()
                .Tap(ycombo => Console.WriteLine($"Yahzee Combination: {ycombo.GetType().Name}, Score: {ycombo.Score}\n"));

                return newCup;
            });

        // Added scorecard and for the test players
        ImmutableList<Player> players = ImmutableList.Create(
            new Player("Alice", new YahzeeCup(), ScoreCard.Empty),
            new Player("Bob", new YahzeeCup(), ScoreCard.Empty),
            new Player("Diana", new YahzeeCup(), ScoreCard.Empty))
            .Tap(p => Console.WriteLine(string.Join("\n", p.Select(pl => $"{pl.Name} has Yahtzee cup: {pl.YahzeeCup}"))));


        Console.WriteLine("\nYahtzee Round Simulation:");
        Console.WriteLine("Your code should implement the Yahtzee round simulation below.");
        Console.WriteLine("========================");

        // Implement a Yahtzee round simulation here using functional patterns

        // use existing monadic extensions and functional patterns
        // minimize imperative code, maximize declative code using LINQ and extension methods

        // Simulate 13 rounds of Yahtzee
        var rounds = 13;

        // Track players and their scorecards through the rounds
        _ = Enumerable.Range(1, rounds)
            .Aggregate(players, (currentPlayers, round) =>
            {
                Console.WriteLine($"\n--- Round {round} ---");

                var roundResults =
                    currentPlayers.Select(player =>
                    {
                        Console.WriteLine($"\n{player.Name}'s turn:");

                        var rolledCup = RollUpToThreeTimes(player.YahzeeCup);
                        return ApplyBestScore(player, rolledCup);
                    }
                    )
                    .Select(p =>
                        p.Tap(updated =>
                        {
                            if (updated.Box is not null && updated.Points > 0)
                            {
                                Console.WriteLine($"{updated.Player.Name} filled '{updated.Box}' box with {updated.Points} points." +
                                    $"\nNew total score: {updated.Player.ScoreCard.Total}");
                            }
                            Console.WriteLine($"{updated.Player.Name}'s current total score {updated.Player.ScoreCard.Total}");
                        })).ToImmutableList();

                // Determine the winner (winners if multiple) of the round
                var maxScore = roundResults.Max(r => r.Player.ScoreCard.Total);

                var winners = roundResults
                    .Where(r => r.Player.ScoreCard.Total == maxScore)
                    .Select(r => r.Player.Name)
                    .ToImmutableList();

                // Announce round winners
                Console.WriteLine($"\nRound {round} Winner(s): " +
                                  $"{string.Join(", ", winners)} " +
                                  $"with {maxScore} points!");

                return roundResults.Select(r => r.Player).ToImmutableList();
            });
    }

    static YahzeeCup RollUpToThreeTimes(YahzeeCup initialCup)
    {
        return Enumerable.Range(1, 3)
            .Aggregate(initialCup, (cup, _) =>
            {
                var bestCombo =
                    cup.GetAllYahtzeeCombinations()
                        .FirstOrDefault(c => c is not Chance and not NoCombination);

                ImmutableList<Die> keptDice =
                    bestCombo is null
                        ? KeepMostFrequentValue(cup)
                        : bestCombo.dice.ToImmutableList();


                // Create a new cup with kept dice with a placeholder
                var newCup = new YahzeeCup
                {
                    dice = keptDice
                        .AddRange(
                            Enumerable.Range(0, 5 - keptDice.Count)
                                .Select(_ => new Die(DiePip.One)))
                };

                return newCup.ShakeAndRoll();
            });
    }

    static (Player Player, string? Box, int Points) ApplyBestScore(Player player, YahzeeCup roll)
    {
        // Find the best available combination to score
        var availableCombos =
             roll.GetAllYahtzeeCombinations()
             .Select(c => new 
             { 
                 Combo = c, 
                 Box = c.GetType().Name 
             })
             .Where(c => player.ScoreCard.IsAvailable(c.Box))
             .ToList();

        var bestNonChance =
            availableCombos
                .Where(x => x.Box != "Chance" && x.Combo.Score > 0)
                .OrderByDescending(x => x.Combo.Score)
                .FirstOrDefault();

        // This is a fallback in case no boxes are available
        if (bestNonChance is not null)
        {
            var filledCard = player.ScoreCard.Fill(bestNonChance.Box, bestNonChance.Combo.Score);

            var finalCard = ApplyYahtzeeBonus(filledCard, bestNonChance.Combo);

            return (
                player with 
                { 
                    ScoreCard = finalCard 
                }, 
                bestNonChance.Box, 
                bestNonChance.Combo.Score);
        }

        var scrificedBox = player.ScoreCard.Boxes
            .Keys
            .FirstOrDefault(b => player.ScoreCard.IsAvailable(b));

        if (scrificedBox is not null)
        {
            Console.WriteLine($"{player.Name} has no good combinations left, sacrificing '{scrificedBox}' box.");
            
            return (player with 
            { 
                ScoreCard = player.ScoreCard.Fill(scrificedBox, 0) 
            }, 
            scrificedBox, 
            0);
        }

        Console.WriteLine($"{player.Name} has no boxes left to fill, skipping turn.");
        return (player, null, 0);
    }

    static ScoreCard ApplyYahtzeeBonus(ScoreCard card, YahzeeCup combo)
    {
        if (combo is YahtzeeCombo &&
            card.Boxes.TryGetValue("Yahtzee", out var score) &&
            score == 50)
        {
            return card with { YahtzeeBonus = card.YahtzeeBonus + 100 };
        }

        return card;
    }

    static ImmutableList<Die> KeepMostFrequentValue(YahzeeCup cup) =>
    cup.dice
        .GroupBy(d => d.Pip)
        .OrderByDescending(g => g.Count())
        .ThenByDescending(g => (int)g.Key)
        .First()
        .ToImmutableList();

}