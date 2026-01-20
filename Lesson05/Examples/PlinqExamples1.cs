using System.Diagnostics;

namespace Playground.Lesson12;

public static class PlinqExamples1
{
    public static void RunExamples()
    {
        var timer = new Stopwatch();

        // Primes Count Example
        //PrimesExample(timer);

        // Actual Primes Example
        ActualPrimesExample(timer);
    }

    static public void PrimesExample(Stopwatch timer)
    {
        var count = 10_000_000; // test without debugger 1, 10 million and see the time difference

        System.Console.WriteLine($"Finding primes between 2 and {count:N0} using Linq and PLinq...");
        timer.Start();
        int primeCountLinq = GetPrimesCount_Linq(2, count);
        timer.Stop();
        System.Console.WriteLine($"Linq: Found {primeCountLinq:N0} primes in {timer.ElapsedMilliseconds} ms");
        timer.Reset();
        timer.Start();
        int primeCountPLinq = GetPrimesCount_Plinq(2, count);
        timer.Stop();
        System.Console.WriteLine($"PLinq: Found {primeCountPLinq:N0} primes in {timer.ElapsedMilliseconds} ms");   

    }
    static public int GetPrimesCount_Linq(int start, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
        if (start < 2) throw new ArgumentOutOfRangeException(nameof(start), "Start must be at least 2.");
        
        return Enumerable.Range(start, count).Count(n =>
            Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
    }
    static public int GetPrimesCount_Plinq(int start, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
        if (start < 2) throw new ArgumentOutOfRangeException(nameof(start), "Start must be at least 2.");

        return Enumerable.Range(start, count).AsParallel().Count(n =>
            Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
    }

    static public void ActualPrimesExample(Stopwatch timer)
    {
        var count = 10_000_000; // test without debugger - try larger numbers to see performance difference

        System.Console.WriteLine($"\nFinding actual prime numbers between 2 and {count:N0} using Linq and PLinq...");
        
        timer.Start();
        var primesLinq = GetPrimes_Linq(2, count).ToList(); // ToList() to force evaluation
        timer.Stop();
        System.Console.WriteLine($"Linq: Found {primesLinq.Count:N0} primes in {timer.ElapsedMilliseconds} ms");
        System.Console.WriteLine($"First 10 primes: {string.Join(", ", primesLinq.Take(10))}");
        System.Console.WriteLine($"Last 10 primes: {string.Join(", ", primesLinq.TakeLast(10))}");
        
        timer.Reset();
        timer.Start();
        var primesPLinq = GetPrimes_Plinq(2, count).ToList(); // ToList() to force evaluation
        timer.Stop();
        System.Console.WriteLine($"PLinq: Found {primesPLinq.Count:N0} primes in {timer.ElapsedMilliseconds} ms");
        System.Console.WriteLine($"First 10 primes: {string.Join(", ", primesPLinq.Take(10))}");
        System.Console.WriteLine($"Last 10 primes: {string.Join(", ", primesPLinq.TakeLast(10))}");
    }

    static public IEnumerable<int> GetPrimes_Linq(int start, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
        if (start < 2) throw new ArgumentOutOfRangeException(nameof(start), "Start must be at least 2.");
        
        System.Console.WriteLine("GetPrimes_Linq called");
        return Enumerable.Range(start, count).Where(n =>
            Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
    }

    static public IEnumerable<int> GetPrimes_Plinq(int start, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
        if (start < 2) throw new ArgumentOutOfRangeException(nameof(start), "Start must be at least 2.");

        System.Console.WriteLine("GetPrimes_Plinq called");
        return Enumerable.Range(start, count).AsParallel().Where(n =>
            Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
    }
}
