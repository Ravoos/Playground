using System.Collections.Immutable;
using Seido.Utilities.SeedGenerator;

namespace Models.Friends;

public record Address(
    Guid AddressId,
    string StreetAddress,
    int ZipCode,
    string City,
    string Country,
    bool Seeded = false
) : ISeed<Address>
{
    public Address() : this(default, default, default, default, default) { }

    #region randomly seed this instance
    public virtual Address Seed(SeedGenerator seedGenerator)
    {
        var ret = new Address(
            Guid.NewGuid(),
            seedGenerator.StreetAddress(seedGenerator.Country),
            seedGenerator.ZipCode,
            seedGenerator.City(seedGenerator.Country),
            seedGenerator.Country,
            true
        );
        return ret;
    }
    #endregion
}


