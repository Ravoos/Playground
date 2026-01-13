# Lazy<T> in C#

## What is Lazy<T>?

`Lazy<T>` is a built-in generic class in .NET that provides support for lazy initialization. It ensures that an expensive computation or resource creation is deferred until it's actually needed, and then only executes once. This pattern is particularly useful for:

- **Performance optimization** - Avoiding expensive computations that might never be used
- **Memory efficiency** - Delaying resource allocation until necessary
- **Thread safety** - Built-in thread-safe lazy initialization
- **Caching results** - Ensuring expensive operations are only performed once

## Key Characteristics

### 1. Deferred Execution
The computation inside a `Lazy<T>` is not executed until the first time the `Value` property is accessed.

### 2. Single Execution
Once computed, the result is cached and reused for all subsequent accesses.

### 3. Thread Safety
By default, `Lazy<T>` is thread-safe, ensuring the factory function is called only once even in multi-threaded scenarios.

### 4. IsValueCreated Property
You can check if the lazy value has been created without triggering its creation using the `IsValueCreated` property.

## Practical Examples from Our Codebase

### Example 1: Lazy Employee Summary ([LazyEmployeeExamples.cs](../Examples/LazyEmployeeExamples.cs))

```csharp
// Create a lazy computation that won't execute until accessed
var lazyEmployeeSummary = new Lazy<string>(() =>
{
    Console.WriteLine("[Computing employee summary... This is expensive!]");
    
    var summary = employees
        .GroupBy(e => e.Role)
        .Select(g => $"{g.Key}: {g.Count()} employees")
        .Aggregate((a, b) => $"{a}, {b}");
        
    return $"Employee Summary: {summary}";
});

Console.WriteLine("Lazy object created, but computation not yet executed.");

// The expensive computation happens here, only when we first access Value
Console.WriteLine($"First access: {lazyEmployeeSummary.Value}");

// Subsequent accesses use the cached result
Console.WriteLine($"Second access: {lazyEmployeeSummary.Value}");
```

**Key Benefits:**
- The expensive summary computation only runs if and when needed
- Multiple accesses to the same result don't repeat the computation
- The computation can be conditionally avoided entirely

### Example 2: Lazy Music Group Data ([LazyMusicGroupExamples.cs](../Examples/LazyMusicGroupExamples.cs))

The `MusicGroupLazy` model demonstrates lazy loading of related data:

```csharp
public record MusicGroupLazy (
    Guid MusicGroupId,
    string Name,
    int EstablishedYear,
    MusicGenre Genre,
    Lazy<ImmutableList<Album>> Albums,    // Lazy-loaded albums
    Lazy<ImmutableList<Artist>> Artists,  // Lazy-loaded artists
    bool Seeded = false
)
```

**Usage in Practice:**
```csharp
// Create music groups - albums and artists are NOT loaded yet
var musicGroups = seeder.ItemsToList<MusicGroupLazy>(20);

// Check loading status without triggering loading
Console.WriteLine($"Albums loaded: {group.Albums.IsValueCreated}"); // False

// Only load data when actually needed
var albumCount = group.Albums.Value.Count; // NOW the albums are loaded

// Subsequent accesses use cached data
var moreAlbums = group.Albums.Value; // No additional loading
```

### Example 3: Selective Loading Pattern

```csharp
// Filter for Rock groups and load their lazy data
var rockGroups = allMusicGroups.Where(g => g.Genre == MusicGenre.Rock).ToList();

foreach (var rockGroup in rockGroups)
{
    // Load both albums and artists ONLY for Rock groups
    var albumCount = rockGroup.Albums.Value.Count;
    var artistCount = rockGroup.Artists.Value.Count;
}

// Other music groups never had their expensive data loaded!
var albumsLoaded = allMusicGroups.Count(g => g.Albums.IsValueCreated);
var artistsLoaded = allMusicGroups.Count(g => g.Artists.IsValueCreated);
```

**Memory Savings:** In this example, if you have 20 music groups but only 3 are Rock, you've saved 17 × (albums + artists) from being loaded into memory.

## Common Use Cases

### 1. Expensive Computations
```csharp
var lazyExpensiveCalculation = new Lazy<ComplexResult>(() => 
{
    // Time-consuming mathematical computation
    // Database aggregation
    // File processing
    return ComputeComplexResult();
});

// Only compute if needed
if (someCondition)
{
    var result = lazyExpensiveCalculation.Value;
}
```

### 2. Resource Initialization
```csharp
var lazyDatabaseConnection = new Lazy<IDbConnection>(() =>
{
    return new SqlConnection(connectionString);
});

// Connection only created when first used
using var db = lazyDatabaseConnection.Value;
```

### 3. Conditional Data Loading
```csharp
var lazyLargeDataset = new Lazy<List<LargeObject>>(() =>
{
    return LoadLargeDatasetFromDatabase();
});

// Only load if user requests detailed view
if (userWantsDetailedView)
{
    var data = lazyLargeDataset.Value;
    DisplayDetailedView(data);
}
```

## Best Practices

### ✅ Do
- Use `Lazy<T>` for expensive operations that might not be needed
- Check `IsValueCreated` to determine loading status without triggering creation
- Combine with filtering to avoid unnecessary computations
- Use for caching expensive, immutable results

### ❌ Don't
- Use `Lazy<T>` for simple, cheap operations
- Access `Value` in tight loops if you need the data repeatedly (store in a variable instead)
- Use for mutable objects that need to be recreated each time
- Forget that the factory function should be thread-safe if using in multi-threaded scenarios

## Performance Impact

**Without Lazy:**
```csharp
// All data loaded immediately, regardless of usage
var musicGroup = new MusicGroup(id, name, LoadAllAlbums(), LoadAllArtists());
// Memory: 100% of albums + artists loaded
// Time: Full loading time upfront
```

**With Lazy:**
```csharp
// Data loaded only when accessed
var musicGroup = new MusicGroupLazy(id, name, 
    new Lazy<>(() => LoadAllAlbums()), 
    new Lazy<>(() => LoadAllArtists()));
// Memory: Only basic info loaded initially
// Time: Deferred until first access
```

## Thread Safety

`Lazy<T>` handles thread safety automatically:

```csharp
var lazyValue = new Lazy<ExpensiveResource>(() => CreateExpensiveResource());

// Multiple threads can safely access this
// The factory function will only run once, regardless of concurrent access
Parallel.For(0, 100, i => 
{
    var resource = lazyValue.Value; // Thread-safe
});
```

## Summary

`Lazy<T>` is a powerful tool for optimizing applications by deferring expensive operations until they're actually needed. It's particularly effective when:

- Working with large datasets that might not be used
- Performing expensive computations conditionally
- Loading related data that's only sometimes needed
- Implementing caching patterns for immutable, expensive-to-create objects

The examples in our codebase demonstrate practical applications, from selective data loading in music groups to conditional expensive computations with employee data, showing how `Lazy<T>` can significantly improve both performance and memory efficiency.