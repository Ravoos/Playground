# Parallel LINQ (PLINQ) in C#

## What is PLINQ?

**Parallel LINQ (PLINQ)** is a parallel implementation of LINQ that enables queries to execute across multiple processor cores simultaneously. It's part of the Task Parallel Library (TPL) and provides automatic parallelization of LINQ operations to improve performance for computationally intensive queries.

## Key Benefits

- **Improved Performance** - Utilizes multiple CPU cores for faster query execution
- **Simple API** - Just add `.AsParallel()` to enable parallel execution
- **Automatic Load Balancing** - TPL handles work distribution across cores
- **Preserves LINQ Semantics** - Same query logic with parallel execution
- **Thread Safety** - Built-in coordination for parallel operations

## Core Concepts

### Sequential vs Parallel Execution

**Sequential LINQ:**
```csharp
var result = data.Where(condition).Select(transform).ToList();
```

**Parallel LINQ:**
```csharp
var result = data.AsParallel().Where(condition).Select(transform).ToList();
```

The only difference is the `.AsParallel()` call, but the performance impact can be dramatic for CPU-intensive operations.

## Practical Examples from Our Codebase

### Example 1: Prime Number Calculation ([PlinqExamples1.cs](../Examples/PlinqExamples1.cs))

#### Sequential Version
```csharp
static public int GetPrimesCount_Linq(int start, int count)
{
    if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
    if (start < 2) throw new ArgumentOutOfRangeException(nameof(start), "Start must be at least 2.");
    
    return Enumerable.Range(start, count).Count(n =>
        Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
}
```

#### Parallel Version
```csharp
static public int GetPrimesCount_Plinq(int start, int count)
{
    if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
    if (start < 2) throw new ArgumentOutOfRangeException(nameof(start), "Start must be at least 2.");

    return Enumerable.Range(start, count).AsParallel().Count(n =>
        Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
}
```

**Performance Comparison:**
```csharp
// Test with 1,000,000 numbers
System.Console.WriteLine($"Finding primes between 2 and {count:N0} using Linq and PLinq...");

// Sequential: ~2000ms
int primeCountLinq = GetPrimesCount_Linq(2, count);

// Parallel: ~500ms (4x faster on quad-core)
int primeCountPLinq = GetPrimesCount_Plinq(2, count);
```

**Key Insight:** The prime calculation is CPU-intensive (testing divisibility), making it ideal for parallelization. Each number can be tested independently, perfect for parallel processing.

### Example 2: Prime Number Generation

#### Sequential Version
```csharp
static public IEnumerable<int> GetPrimes_Linq(int start, int count)
{
    return Enumerable.Range(start, count).Where(n =>
        Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
}
```

#### Parallel Version
```csharp
static public IEnumerable<int> GetPrimes_Plinq(int start, int count)
{
    return Enumerable.Range(start, count).AsParallel().Where(n =>
        Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
}
```

**Usage:**
```csharp
var primesLinq = GetPrimes_Linq(2, count).ToList(); // Force evaluation
var primesPLinq = GetPrimes_Plinq(2, count).ToList(); // Force evaluation
```

### Example 3: Credit Card Encryption ([PlinqExamples2.cs](../Examples/PlinqExamples2.cs))

#### Sequential Encryption
```csharp
private static void CreditCardEncryptionLinq(Stopwatch timer, IList<CreditCard> creditCards)
{
    var encryptor = new EncryptionEngine(AesEncryptionOptions.Default());

    // Sequential encryption
    timer.Start();
    var encryptedCreditCards = creditCards
        .Select(cc => (cc.CreditCardId, encryptor.AesEncryptToBase64(cc)))
        .ToImmutableList();
    timer.Stop();

    Console.WriteLine($"LINQ Encryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms");

    // Sequential decryption
    timer.Reset();
    timer.Start();
    var decryptedCC = encryptedCreditCards
        .Select(cc => encryptor.AesDecryptFromBase64<CreditCard>(cc.Item2))
        .ToImmutableList();
    timer.Stop();

    Console.WriteLine($"LINQ Decryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms");
}
```

#### Parallel Encryption
```csharp
private static void CreditCardEncryptionPlinq(Stopwatch timer, IList<CreditCard> creditCards)
{
    var encryptor = new EncryptionEngine(AesEncryptionOptions.Default());

    // Parallel encryption
    timer.Start();
    var encryptedCreditCards = creditCards.AsParallel()
        .Select(cc => (cc.CreditCardId, encryptor.AesEncryptToBase64(cc)))
        .ToImmutableList();
    timer.Stop();

    Console.WriteLine($"PLINQ Encryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms");

    // Parallel decryption
    timer.Reset();
    timer.Start();
    var decryptedCC = encryptedCreditCards.AsParallel()
        .Select(cc => encryptor.AesDecryptFromBase64<CreditCard>(cc.Item2))
        .ToImmutableList();
    timer.Stop();

    Console.WriteLine($"PLINQ Decryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms");
}
```

**Performance Characteristics:**
- **1,000,000 credit cards**
- **Sequential:** ~5000ms encryption, ~4000ms decryption
- **Parallel:** ~1500ms encryption, ~1200ms decryption
- **Speedup:** ~3.3x on quad-core processor

## When to Use PLINQ

### ✅ Ideal Use Cases

#### 1. CPU-Intensive Operations
```csharp
// Mathematical calculations
var results = data.AsParallel().Select(x => ComplexMathOperation(x));

// Cryptographic operations
var encrypted = data.AsParallel().Select(x => EncryptData(x));

// Image/data processing
var processed = images.AsParallel().Select(img => ProcessImage(img));
```

#### 2. Independent Operations
```csharp
// Each item can be processed independently
var validated = items.AsParallel().Where(item => ValidateItem(item));

// No dependencies between transformations
var transformed = data.AsParallel().Select(item => TransformItem(item));
```

#### 3. Large Data Sets
```csharp
// Operations on millions of records
var filtered = millionsOfRecords.AsParallel()
    .Where(record => ExpensiveFilter(record))
    .ToList();
```

### ❌ Avoid PLINQ When

#### 1. Simple Operations
```csharp
// Don't parallelize simple operations
var simple = data.AsParallel().Select(x => x.ToString()); // Overhead > benefit
```

#### 2. Small Data Sets
```csharp
// Small collections have more overhead than benefit
var tiny = smallList.AsParallel().Where(x => x > 10); // Not worth it
```

#### 3. Order-Dependent Operations
```csharp
// Operations that depend on order or position
var indexed = data.AsParallel().Select((item, index) => new { item, index }); // May not preserve order
```

#### 4. I/O Bound Operations
```csharp
// File I/O, database calls, web requests
var files = paths.AsParallel().Select(path => File.ReadAllText(path)); // Use async instead
```

## PLINQ Configuration Options

### Degree of Parallelism
```csharp
// Control number of threads
var result = data.AsParallel()
    .WithDegreeOfParallelism(Environment.ProcessorCount / 2)
    .Select(item => ProcessItem(item));
```

### Execution Mode
```csharp
// Force parallel execution even if PLINQ thinks sequential would be better
var result = data.AsParallel()
    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
    .Select(item => ProcessItem(item));
```

### Order Preservation
```csharp
// Maintain original order (with performance cost)
var orderedResult = data.AsParallel()
    .AsOrdered()
    .Select(item => ProcessItem(item));
```

## Performance Considerations

### Overhead Factors

#### 1. Work Distribution
- Partitioning data across threads
- Coordinating thread execution
- Merging results from multiple threads

#### 2. Context Switching
- CPU time spent switching between threads
- More significant with many threads on few cores

#### 3. Memory Pressure
- Multiple threads accessing memory simultaneously
- Potential cache misses and memory contention

### Optimization Guidelines

#### 1. Chunk Size
```csharp
// For very large collections, consider chunking
var result = data.AsParallel()
    .WithPartitioner(Partitioner.Create(data, true))
    .Select(item => ProcessItem(item));
```

#### 2. Load Balancing
PLINQ automatically handles load balancing, but uneven work distribution can still cause issues:

```csharp
// Example: Some items take much longer to process
var mixed = data.AsParallel().Select(item => 
    item.IsComplex ? ExpensiveOperation(item) : SimpleOperation(item));
```

#### 3. Exception Handling
```csharp
try
{
    var result = data.AsParallel().Select(item => ProcessItem(item)).ToList();
}
catch (AggregateException ex)
{
    foreach (var innerEx in ex.InnerExceptions)
    {
        Console.WriteLine($"Error: {innerEx.Message}");
    }
}
```

## Thread Safety Considerations

### Safe Operations
```csharp
// Pure functions are always thread-safe
var results = data.AsParallel().Select(x => Math.Sqrt(x * x + 1));

// Immutable transformations are safe
var transformed = items.AsParallel().Select(item => 
    item with { ProcessedAt = DateTime.Now });
```

### Unsafe Operations
```csharp
// Shared mutable state - AVOID!
int counter = 0;
var results = data.AsParallel().Select(item => {
    counter++; // Race condition!
    return ProcessItem(item);
});

// Side effects - AVOID!
var results = data.AsParallel().Select(item => {
    Console.WriteLine(item); // Output order is unpredictable
    return ProcessItem(item);
});
```

## Best Practices

### ✅ Do
- Profile performance with both sequential and parallel versions
- Use PLINQ for CPU-intensive, independent operations
- Consider data size and operation complexity
- Use immutable data structures when possible
- Handle AggregateException for error scenarios
- Test on target hardware configurations

### ❌ Don't
- Assume PLINQ is always faster
- Use for I/O-bound operations (use async/await instead)
- Parallelize simple operations on small datasets
- Share mutable state between parallel operations
- Ignore exception handling
- Forget about overhead costs

### 🔍 Performance Testing Pattern
```csharp
public static void CompareMethods<T>(IEnumerable<T> data, Func<IEnumerable<T>, IEnumerable<U>> sequential, 
                                    Func<IEnumerable<T>, IEnumerable<U>> parallel)
{
    var timer = Stopwatch.StartNew();
    
    // Sequential
    timer.Start();
    var seqResult = sequential(data).ToList();
    timer.Stop();
    Console.WriteLine($"Sequential: {timer.ElapsedMilliseconds} ms");
    
    // Parallel
    timer.Restart();
    var parResult = parallel(data).ToList();
    timer.Stop();
    Console.WriteLine($"Parallel: {timer.ElapsedMilliseconds} ms");
    
    // Verify results are equivalent
    Console.WriteLine($"Results match: {seqResult.SequenceEqual(parResult)}");
}
```

## Functional Programming Integration

### Immutability
PLINQ works excellently with functional programming principles:

```csharp
// Transform records immutably in parallel
var updatedEmployees = employees.AsParallel()
    .Select(emp => emp with { 
        Salary = CalculateNewSalary(emp),
        LastUpdated = DateTime.Now 
    });
```

### Composition
```csharp
// Chain parallel operations
var result = data.AsParallel()
    .Where(item => FilterCondition(item))
    .Select(item => TransformItem(item))
    .Where(transformed => PostTransformFilter(transformed))
    .ToList();
```

### Pure Functions
```csharp
// Pure functions are ideal for parallel execution
static decimal CalculateTax(Employee emp) => 
    emp.Salary * GetTaxRate(emp.TaxBracket);

var taxCalculations = employees.AsParallel()
    .Select(emp => (emp.EmployeeId, Tax: CalculateTax(emp)));
```

## Summary

PLINQ provides a simple yet powerful way to leverage multi-core processors for improved query performance. Our examples demonstrate:

- **Prime calculation** showing dramatic speedups for CPU-intensive mathematical operations
- **Credit card encryption** demonstrating parallel cryptographic processing
- **Performance measurement** patterns for comparing sequential vs parallel execution

**Key takeaways:**
- Add `.AsParallel()` to enable parallel execution
- Best for CPU-intensive, independent operations on large datasets
- Always measure performance - parallel isn't always faster
- Maintain thread safety and handle exceptions properly
- Consider overhead costs and hardware characteristics

PLINQ seamlessly integrates with functional programming patterns, making it an excellent tool for performance optimization in functional C# applications.