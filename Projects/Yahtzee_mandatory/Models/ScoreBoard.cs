using System.Collections.Immutable;

namespace Playground.Projects.Yahtzee_mandatory.Models;

public record ScoreCard(
    ImmutableDictionary<string, int> Boxes,
    int YahtzeeBonus = 0)
{
    private static readonly ImmutableHashSet<string> UpperSectionBoxes = ImmutableHashSet.Create(
    "Ones", "Twos", "Threes", "Fours", "Fives", "Sixes");

    public static ScoreCard Empty =>
        new(ImmutableDictionary<string, int>.Empty);

    public bool IsAvailable(string box) =>
        !Boxes.ContainsKey(box);

    public int UpperSectionTotal =>
        Boxes.Where(kv => UpperSectionBoxes.Contains(kv.Key))
            .Sum(kv => kv.Value);

    public int UpperSectionBonus =>
        UpperSectionTotal >= 63 ? 35 : 0;

    public ScoreCard Fill(string box, int score)
    {
        if (!Boxes.ContainsKey(box))
            return this with { Boxes = Boxes.Add(box, score) };

        return this;
    }

    public int Total =>
        Boxes.Values.Sum() +
        YahtzeeBonus +
        UpperSectionBonus;
}