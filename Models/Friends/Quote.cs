using System.Collections.Immutable;
using Seido.Utilities.SeedGenerator;

namespace Models.Friends;

public record Quote(
    Guid QuoteId,
    string QuoteText,
    string Author,
    bool Seeded = false
) : ISeed<Quote>
{
    public Quote() : this(default, default, default) { }

    public Quote(SeededQuote goodQuote) : this(
        Guid.NewGuid(),
        goodQuote.Quote,
        goodQuote.Author,
        true
    ) { }

    #region randomly seed this instance
    public virtual Quote Seed(SeedGenerator seedGenerator)
    {
        var quote = seedGenerator.Quote;
        var ret = new Quote(
            Guid.NewGuid(),
            quote.Quote,
            quote.Author,
            true
        );
        return ret;
    }
    #endregion
}


