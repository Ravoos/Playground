using Models.Music;
using Seido.Utilities.SeedGenerator;

namespace Playground.Lesson05.Examples;

public static class LazyMusicGroupExamples
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Lazy<T> Examples with Music Groups ===\n");

        var seeder = new SeedGenerator();

        LazyMusicGroupCreationExample(seeder);

        SelectiveMusicGroupLoadingExample(seeder);

        Console.WriteLine("\n=== End of Lazy Music Group Examples ===\n");
    }

    private static void LazyMusicGroupCreationExample(SeedGenerator seeder)
    {
        Console.WriteLine("1. Lazy Music Group Creation Example:");

        // Create music groups with lazy-loaded albums and artists
        var musicGroups = seeder.ItemsToList<MusicGroupLazy>(5);

        Console.WriteLine($"   Created {musicGroups.Count} music groups.");
        Console.WriteLine("   Albums and artists are not yet loaded (lazy initialization).");

        foreach (var group in musicGroups)
        {
            Console.WriteLine($"   Group: {group.Name} ({group.Genre}, Est. {group.EstablishedYear})");
            Console.WriteLine($"   Albums loaded: {group.Albums.IsValueCreated}, Artists loaded: {group.Artists.IsValueCreated}");
        }

        Console.WriteLine("\n   Now accessing albums for first group...");
        var firstGroupAlbums = musicGroups.First();
        Console.WriteLine($"   First group now has {firstGroupAlbums.Albums.Value.Count} albums loaded.");
        Console.WriteLine($"   Albums loaded: {firstGroupAlbums.Albums.IsValueCreated}, Artists loaded: {firstGroupAlbums.Artists.IsValueCreated}");

        Console.WriteLine($"   First group now has {firstGroupAlbums.Artists.Value.Count} artists loaded.");
        Console.WriteLine($"   Albums loaded: {firstGroupAlbums.Albums.IsValueCreated}, Artists loaded: {firstGroupAlbums.Artists.IsValueCreated}");

        Console.WriteLine();
    }

    private static void SelectiveMusicGroupLoadingExample(SeedGenerator seeder)
    {
        Console.WriteLine("2. Selective Loading - Rock Groups Only Example:");

        // Create many music groups with various genres
        var allMusicGroups = seeder.ItemsToList<MusicGroupLazy>(20);
        Console.WriteLine($"   Created {allMusicGroups.Count} music groups with various genres.");

        // Show initial state - no albums/artists loaded yet
        Console.WriteLine("   Initial state - no lazy data loaded yet:");
        var genreCount = allMusicGroups.GroupBy(g => g.Genre).ToDictionary(g => g.Key, g => g.Count());
        foreach (var genre in genreCount)
        {
            Console.WriteLine($"   {genre.Key}: {genre.Value} groups");
        }

        Console.WriteLine("\n   Selectively loading albums and artists ONLY for Rock groups...");
        
        // Filter for Rock groups and load their lazy data
        var rockGroups = allMusicGroups.Where(g => g.Genre == MusicGenre.Rock).ToList();
        
        foreach (var rockGroup in rockGroups)
        {
            // Load both albums and artists for Rock groups
            var albumCount = rockGroup.Albums.Value.Count;
            var artistCount = rockGroup.Artists.Value.Count;
            Console.WriteLine($"   Rock Group: {rockGroup.Name} - Albums: {albumCount}, Artists: {artistCount}");
        }

        // Show final loading state
        Console.WriteLine("\n   Final loading state:");
        var albumsLoaded = allMusicGroups.Count(g => g.Albums.IsValueCreated);
        var artistsLoaded = allMusicGroups.Count(g => g.Artists.IsValueCreated);
        var rockGroupCount = allMusicGroups.Count(g => g.Genre == MusicGenre.Rock);

        Console.WriteLine($"   Total groups: {allMusicGroups.Count}");
        Console.WriteLine($"   Rock groups: {rockGroupCount}");
        Console.WriteLine($"   Albums loaded for: {albumsLoaded} groups (should equal Rock groups)");
        Console.WriteLine($"   Artists loaded for: {artistsLoaded} groups (should equal Rock groups)");
        Console.WriteLine($"   Memory saved: {allMusicGroups.Count - rockGroupCount} groups avoided loading expensive data");

        Console.WriteLine();
    }
}