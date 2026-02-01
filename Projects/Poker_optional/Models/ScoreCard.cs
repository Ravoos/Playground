using System.Collections.Immutable;

namespace Playground.Projects.Poker_optional.Models;

public record ScoreCard(ImmutableDictionary<string, int> Scores)
{
    public static ScoreCard Empty =>
        new(ImmutableDictionary<string, int>.Empty);

    public ScoreCard AddWin(string playerName) =>
        this with
        {
            Scores = Scores.SetItem(
                playerName,
                Scores.TryGetValue(playerName, out var current)
                    ? current + 1
                    : 1)
        };

    public ScoreCard AddWins(IEnumerable<string> playerNames) =>
        playerNames.Aggregate(this, (score, name) => score.AddWin(name));
}

