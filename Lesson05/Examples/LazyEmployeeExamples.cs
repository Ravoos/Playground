using Models.Employees;
using Seido.Utilities.SeedGenerator;
using System.Collections.Immutable;

namespace Playground.Lesson05.Examples;

public static class LazyEmployeeExamples
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Lazy<T> Examples with Employee ===\n");

        var seeder = new SeedGenerator();
        var employees = seeder.ItemsToList<Employee>(100);

        // Example 1: Lazy computation of employee summary
        LazyEmployeeSummaryExample(employees);

        // Example 2: Lazy filtering and expensive operations
        LazyExpensiveOperationsExample(employees);

        Console.WriteLine("\n=== End of Lazy Employee Examples ===\n");
    }

    private static void LazyEmployeeSummaryExample(IList<Employee> employees)
    {
        Console.WriteLine("1. Lazy Employee Summary Example:");

        // Create a lazy computation that won't execute until accessed
        var lazyEmployeeSummary = new Lazy<string>(() =>
        {
            Console.WriteLine("   [Computing employee summary... This is expensive!]");
            
            var summary = employees
                .GroupBy(e => e.Role)
                .Select(g => $"{g.Key}: {g.Count()} employees")
                .Aggregate((a, b) => $"{a}, {b}");
                
            return $"Employee Summary: {summary}";
        });

        Console.WriteLine("   Lazy object created, but computation not yet executed.");
        
        // The expensive computation happens here, only when we first access Value
        Console.WriteLine($"   First access: {lazyEmployeeSummary.Value}");
        
        // Subsequent accesses use the cached result
        Console.WriteLine($"   Second access: {lazyEmployeeSummary.Value}");
        Console.WriteLine();
    }

    private static void LazyExpensiveOperationsExample(IList<Employee> employees)
    {
        Console.WriteLine("2. Lazy Expensive Operations Example:");

        // Lazy computation for expensive salary calculations
        var lazyHighSalaryEmployees = new Lazy<IEnumerable<Employee>>(() =>
        {
            Console.WriteLine("   [Computing high salary employees... Expensive calculation!]");
            
            return employees
                .Where(e => e.Role == WorkRole.Management || e.Role == WorkRole.Veterinarian)
                .Where(e => SimulateExpensiveSalaryCheck(e)) // Expensive operation
                .ToList();
        });

        // Lazy computation for credit card analysis
        var lazyCreditCardAnalysis = new Lazy<Dictionary<CardIssuer, int>>(() =>
        {
            Console.WriteLine("   [Analyzing credit card data... Complex processing!]");
            
            return employees
                .SelectMany(e => e.CreditCards)
                .GroupBy(cc => cc.Issuer)
                .ToDictionary(g => g.Key, g => g.Count());
        });

        Console.WriteLine("   Lazy computations created.");
        
        // Only compute when needed
        if (employees.Count > 50)
        {
            var highSalary = lazyHighSalaryEmployees.Value;
            Console.WriteLine($"   High salary employees: {highSalary.Count()}");
        }
        
        var cardStats = lazyCreditCardAnalysis.Value;
        Console.WriteLine($"   Credit card analysis: {string.Join(", ", cardStats.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
        Console.WriteLine();
    }

    // Helper method to simulate expensive salary calculation
    private static bool SimulateExpensiveSalaryCheck(Employee employee)
    {
        // Simulate expensive computation
        Thread.Sleep(1); // Pretend this is expensive
        return employee.HireDate < DateTime.Now.AddYears(-2);
    }
}