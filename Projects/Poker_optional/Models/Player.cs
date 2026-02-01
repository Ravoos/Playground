namespace Playground.Projects.Poker_optional.Models;

public record Player(string Name, PokerHand Hand)
{
    public override string ToString() => $"Player: {Name}, Hand: {Hand}";
}
