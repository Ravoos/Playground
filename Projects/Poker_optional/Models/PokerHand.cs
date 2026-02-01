namespace Playground.Projects.Poker_optional.Models;
public record PokerHand : CardDeck, IComparable<PokerHand>
{
    IEnumerable<IGrouping<CardSuit, Card>> suits => cards.GroupBy(card => card.Suit);
    IEnumerable<IGrouping<CardRank, Card>> ranks => cards.GroupBy(card => card.Rank);
    IOrderedEnumerable<CardRank> sortedRanks => cards.Select(c => c.Rank).OrderBy(r => r);

    bool isRanksSequential
    {
        get
        {
            var sRanks = sortedRanks.ToList();
            return Enumerable.Range(0, 4).All(i => sRanks[i + 1] - sRanks[i] == 1);
        }
    }

    public PokerHand() { }

    public PokerHand GetPokerRank()
    {
        if (cards.Count != 5)
            return new NoPokerRank() { cards = cards };

        bool isFlush = suits.Any(suit => suit.Count() == 5);
        bool isThreeOfAKind = ranks.Any(group => group.Count() >= 3);
        bool isFourOfAKind = ranks.Any(group => group.Count() == 4);
        bool isFullHouse = ranks.Any(group => group.Count() == 3) && ranks.Any(group => group.Count() == 2);
        bool isTwoPair = ranks.Count(group => group.Count() == 2) == 2;
        bool isOnePair = ranks.Any(group => group.Count() == 2);
        bool isRoyalFlush = isFlush && isRanksSequential && sortedRanks.Last() == CardRank.Ace;
        bool isStraightFlush = isFlush && isRanksSequential && !isRoyalFlush;
        bool isStraight = isRanksSequential && !isFlush;

        return (isRoyalFlush, isStraightFlush, isFourOfAKind, isFullHouse, isFlush, isStraight, isThreeOfAKind, isTwoPair, isOnePair) switch
        {
            (true, _, _, _, _, _, _, _, _) => new RoyalFlush() { cards = cards },
            (_, true, _, _, _, _, _, _, _) => new StraightFlush() { cards = cards },
            (_, _, true, _, _, _, _, _, _) => new FourOfAKind() { cards = cards },
            (_, _, _, true, _, _, _, _, _) => new FullHouse() { cards = cards },
            (_, _, _, _, true, _, _, _, _) => new Flush() { cards = cards },
            (_, _, _, _, _, true, _, _, _) => new Straight() { cards = cards },
            (_, _, _, _, _, _, true, _, _) => new ThreeOfAKind() { cards = cards },
            (_, _, _, _, _, _, _, true, _) => new TwoPair() { cards = cards },
            (_, _, _, _, _, _, _, _, true) => new OnePair() { cards = cards },
            _ => new HighCard() { cards = cards }
        };
    }

    public virtual int RankStrength => this switch
    {
        RoyalFlush => 10,
        StraightFlush => 9,
        FourOfAKind => 8,
        FullHouse => 7,
        Flush => 6,
        Straight => 5,
        ThreeOfAKind => 4,
        TwoPair => 3,
        OnePair => 2,
        HighCard => 1,
        _ => 0 // NoPokerRank
    };

    public virtual IReadOnlyList<CardRank> TieBreakers =>
    this switch
    {
        HighCard or Flush => cards.Select(c => c.Rank)
                 .OrderByDescending(r => r)
                 .ToList(),
        OnePair =>
            ranks.OrderByDescending(g => g.Count())
                 .ThenByDescending(g => g.Key)
                 .SelectMany(g => Enumerable.Repeat(g.Key, g.Count()))
                 .ToList(),
        TwoPair => ranks.OrderByDescending(g => g.Count())
                 .ThenByDescending(g => g.Key)
                 .SelectMany(g => Enumerable.Repeat(g.Key, g.Count()))
                 .ToList(),
        ThreeOfAKind => ranks.OrderByDescending(g => g.Count())
                 .ThenByDescending(g => g.Key)
                 .SelectMany(g => Enumerable.Repeat(g.Key, g.Count()))
                 .ToList(),
        FullHouse => ranks.OrderByDescending(g => g.Count())
                 .Select(g => g.Key)
                 .ToList(),
        FourOfAKind => ranks.OrderByDescending(g => g.Count())
                 .SelectMany(g => Enumerable.Repeat(g.Key, g.Count()))
                 .ToList(),
        Straight or StraightFlush => new[] { GetStraightHighCard() },
        RoyalFlush => [],
        _ => []
    };

    private CardRank GetStraightHighCard()
    {
        var ordered = cards.Select(c => c.Rank).OrderBy(r => r).ToList();

        // Wheel straight: A-2-3-4-5
        if (ordered.SequenceEqual(new[]
        {
        CardRank.Two,
        CardRank.Three,
        CardRank.Four,
        CardRank.Five,
        CardRank.Ace
        }))
            return CardRank.Five;

        return ordered.Last();
    }


    public int CompareTo(PokerHand? other)
    {
        if (other is null) return 1;

        var rankCompare = RankStrength.CompareTo(other.RankStrength);
        if (rankCompare != 0)
            return rankCompare;

        return TieBreakers
            .Zip(other.TieBreakers)
            .Select(z => z.First.CompareTo(z.Second))
            .FirstOrDefault(c => c != 0);
    }
}

//Disciminators for poker hand types
public record RoyalFlush : PokerHand
{
}
public record StraightFlush : PokerHand
{
}
public record FourOfAKind : PokerHand
{
}
public record FullHouse : PokerHand
{
}
public record Flush : PokerHand
{
}
public record Straight : PokerHand
{
}
public record ThreeOfAKind : PokerHand
{
}
public record TwoPair : PokerHand
{
}
public record OnePair : PokerHand
{
}
public record HighCard : PokerHand
{
}
public record NoPokerRank : PokerHand
{
}