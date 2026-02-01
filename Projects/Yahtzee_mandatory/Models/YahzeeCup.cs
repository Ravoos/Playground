using System.Collections.Immutable;

namespace Playground.Projects.Yahtzee_mandatory.Models;

public record YahzeeCup : CupOfDice
{
    IEnumerable<IGrouping<DiePip, Die>> dicePipGroups => dice.GroupBy(d => d.Pip);
    IOrderedEnumerable<DiePip> sortedDicePips => dice.Select(d => d.Pip).OrderBy(v => v);

    Dictionary<string, List<DiePip>> straightsCombinations
    {
        get
        {
            return new Dictionary<string, List<DiePip>>
        {
            {"SmallStraight1", new List<DiePip> {DiePip.One, DiePip.Two, DiePip.Three, DiePip.Four}},
            {"SmallStraight2", new List<DiePip> {DiePip.Two, DiePip.Three, DiePip.Four, DiePip.Five}},
            {"SmallStraight3", new List<DiePip> {DiePip.Three, DiePip.Four, DiePip.Five, DiePip.Six}},
            {"LargeStraight1", new List<DiePip> {DiePip.One, DiePip.Two, DiePip.Three, DiePip.Four, DiePip.Five}},
            {"LargeStraight2", new List<DiePip> {DiePip.Two, DiePip.Three, DiePip.Four, DiePip.Five, DiePip.Six}}
        };
        }
    }

    public override string ToString() => base.ToString();

    public int Score => this switch
    {
        Ones => dice.Where(d => d.Pip == DiePip.One).Sum(d => (int)d.Pip),
        Twos => dice.Where(d => d.Pip == DiePip.Two).Sum(d => (int)d.Pip),
        Threes => dice.Where(d => d.Pip == DiePip.Three).Sum(d => (int)d.Pip),
        Fours => dice.Where(d => d.Pip == DiePip.Four).Sum(d => (int)d.Pip),
        Fives => dice.Where(d => d.Pip == DiePip.Five).Sum(d => (int)d.Pip),
        Sixes => dice.Where(d => d.Pip == DiePip.Six).Sum(d => (int)d.Pip),
        ThreeOfAKind => dice.Sum(d => (int)d.Pip),
        FourOfAKind => dice.Sum(d => (int)d.Pip),
        FullHouse => 25,
        SmallStraight => 30,
        LargeStraight => 40,
        Yahtzee => 50,
        Chance => dice.Sum(d => (int)d.Pip),
        _ => 0
    };
    public YahzeeCup() : base(5)
    { }

    public YahzeeCup GetYahtzeeCombination() =>
        GetAllYahtzeeCombinations().FirstOrDefault()
        ?? new NoCombination { dice = dice };

    public IEnumerable<YahzeeCup> GetAllYahtzeeCombinations()
    {
        if (dice.Count != 5)
            return Enumerable.Empty<YahzeeCup>();

        var combinations = new List<YahzeeCup>();

        bool isThreeOfAKind = dicePipGroups.Any(g => g.Count() >= 3);
        bool isFourOfAKind = dicePipGroups.Any(g => g.Count() >= 4);
        bool isFullHouse = dicePipGroups.Any(g => g.Count() == 3)
                           && dicePipGroups.Any(g => g.Count() == 2);
        bool isYahtzee = dicePipGroups.Any(g => g.Count() == 5);

        bool isSmallStraight =
            straightsCombinations
                .Where(kvp => kvp.Key.StartsWith("SmallStraight"))
                .Any(kvp => kvp.Value.All(sortedDicePips.Contains));

        bool isLargeStraight =
            straightsCombinations
                .Where(kvp => kvp.Key.StartsWith("LargeStraight"))
                .Any(kvp => kvp.Value.All(sortedDicePips.Contains));

        // Lower section
        if (isYahtzee) combinations.Add(new Yahtzee { dice = dice });
        if (isLargeStraight) combinations.Add(new LargeStraight { dice = dice });
        if (isSmallStraight) combinations.Add(new SmallStraight { dice = dice });
        if (isFullHouse) combinations.Add(new FullHouse { dice = dice });
        if (isFourOfAKind) combinations.Add(new FourOfAKind { dice = dice });
        if (isThreeOfAKind) combinations.Add(new ThreeOfAKind { dice = dice });

        // Upper section (only if present)
        if (dice.Any(d => d.Pip == DiePip.Six)) combinations.Add(new Sixes { dice = dice });
        if (dice.Any(d => d.Pip == DiePip.Five)) combinations.Add(new Fives { dice = dice });
        if (dice.Any(d => d.Pip == DiePip.Four)) combinations.Add(new Fours { dice = dice });
        if (dice.Any(d => d.Pip == DiePip.Three)) combinations.Add(new Threes { dice = dice });
        if (dice.Any(d => d.Pip == DiePip.Two)) combinations.Add(new Twos { dice = dice });
        if (dice.Any(d => d.Pip == DiePip.One)) combinations.Add(new Ones { dice = dice });

        // Chance is always valid
        combinations.Add(new Chance { dice = dice });

        return combinations
            .OrderByDescending(c => c.Score);
    }
}

//Disciminators for yahtzee combinations
public record Yahtzee : YahzeeCup
{
}
public record LargeStraight : YahzeeCup
{
}
public record SmallStraight : YahzeeCup
{
}
public record FullHouse : YahzeeCup
{
}
public record FourOfAKind : YahzeeCup
{
}
public record ThreeOfAKind : YahzeeCup
{
}
public record Sixes : YahzeeCup
{
}
public record Fives : YahzeeCup
{
}
public record Fours : YahzeeCup
{
}
public record Threes : YahzeeCup
{
}
public record Twos : YahzeeCup
{
}
public record Ones : YahzeeCup
{
}
public record Chance : YahzeeCup
{
}
public record NoCombination : YahzeeCup
{
}