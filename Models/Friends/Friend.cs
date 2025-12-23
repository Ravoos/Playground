using System.Collections.Immutable;
using Seido.Utilities.SeedGenerator;

namespace Models.Friends;

public record Friend(
    Guid FriendId,
    string FirstName,
    string LastName,
    string Email,
    DateTime? Birthday,
    Address Address,
    ImmutableList<Pet> Pets,
    ImmutableList<Quote> Quotes,
    bool Seeded = false
) : ISeed<Friend>
{
    public Friend() : this(default, default, default, default, default, default, default, default) { }

    #region randomly seed this instance
    public virtual Friend Seed(SeedGenerator sgen)
    {
        // Generate 0 to 4 pets
        var pets = sgen.ItemsToList<Pet>(sgen.Next(0, 5)).DistinctBy(p => p.Name).ToList();
        
        // Generate 0 to 4 quotes  
        var quotes = sgen.ItemsToList<Quote>(sgen.Next(0, 5)).DistinctBy(q => q.QuoteText).ToList();

        // Generate address 50% of the time
        var address = sgen.Bool ? new Address().Seed(sgen) : null;

        var fn = sgen.FirstName;
        var ln = sgen.LastName;

        var ret = new Friend(
            Guid.NewGuid(),
            fn,
            ln,
            sgen.Email(fn, ln),
            sgen.Bool ? sgen.DateAndTime(1970, 2000) : null,
            address,
            pets.ToImmutableList(),
            quotes.ToImmutableList(),
            true
        );
        return ret;
    }
    #endregion
}

