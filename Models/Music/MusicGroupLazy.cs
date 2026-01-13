using System.Collections.Immutable;
using Seido.Utilities.SeedGenerator;
using Models.Music;

namespace Models.Music;

public record MusicGroupLazy (
    Guid MusicGroupId,
    string Name,
    int EstablishedYear,
    MusicGenre Genre,
    Lazy<ImmutableList<Album>> Albums,
    Lazy<ImmutableList<Artist>> Artists,
    bool Seeded = false
) : ISeed<MusicGroupLazy>
{
    public MusicGroupLazy() : this(default, default, default, default, default, default) {}

    #region randomly seed this instance
    public virtual MusicGroupLazy Seed(SeedGenerator seedGenerator)
    {
        // Create lazy factories for albums and artists
        var lazyAlbums = new Lazy<ImmutableList<Album>>(() =>
        {
            Console.WriteLine($"[Lazy Loading] Generating albums for music group...");
            var albums = seedGenerator.ItemsToList<Album>(seedGenerator.Next(1, 11));
            return albums.ToImmutableList();
        });

        var lazyArtists = new Lazy<ImmutableList<Artist>>(() =>
        {
            Console.WriteLine($"[Lazy Loading] Generating artists for music group...");
            var artists = seedGenerator.ItemsToList<Artist>(seedGenerator.Next(1, 6));
            return artists.ToImmutableList();
        });

        var ret = new MusicGroupLazy(
            Guid.NewGuid(),
            seedGenerator.MusicGroupName,
            seedGenerator.Next(1970, 2024),
            seedGenerator.FromEnum<MusicGenre>(),
            lazyAlbums,
            lazyArtists,
            true
        );
        return ret;
    }
    #endregion
}


