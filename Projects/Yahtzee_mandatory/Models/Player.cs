namespace Playground.Projects.Yahtzee_mandatory.Models;

public record Player(string Name, YahzeeCup YahzeeCup, ScoreCard ScoreCard)
{
    public override string ToString() => $"Player: {Name}, Hand: {YahzeeCup}, ScoreCard: {ScoreCard}";
}
