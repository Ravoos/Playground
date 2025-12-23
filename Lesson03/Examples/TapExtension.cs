using Seido.Utilities.SeedGenerator;
using Models.Employees;
using Models.Music;
using PlayGround.Extensions;
using System.Collections.Immutable;

namespace Playground.Lesson03;

public static class TapExtension
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Tap Extension Method Examples ===\n");
        
        EmployeeTapExample();
        MusicGroupTapExample();

        Console.WriteLine("\n=== End of Tap Extension Method Examples ===\n");
    }

    static void EmployeeTapExample()
    {
        Console.WriteLine("--- Employee Tap Example ---");
        
        var seeder = new SeedGenerator();
        var employee = new Employee().Seed(seeder);
        
        Console.WriteLine($"Processing employee: {employee.FirstName} {employee.LastName}");
        
        // Example: Use Tap for audit logging during employee processing pipeline
        var processedEmployee = employee
            .Tap(e => Console.WriteLine($"[AUDIT LOG] Employee loaded: ID={e.EmployeeId}, Name={e.FirstName} {e.LastName}"))
            .Map(e => e with { FirstName = e.FirstName.ToUpper(), LastName = e.LastName.ToUpper() })
            .Tap(e => Console.WriteLine($"[AUDIT LOG] Employee names capitalized: {e.FirstName} {e.LastName}"))
            .Map(e => e with { Role = e.Role == WorkRole.AnimalCare && DateTime.Now.Year - e.HireDate.Year > 5 ? WorkRole.ProgramCoordinator : e.Role })
            .Tap(e => Console.WriteLine($"[AUDIT LOG] Employee role evaluated: {e.Role} (Tenure: {DateTime.Now.Year - e.HireDate.Year} years)"))
            .Map(e => e with { CreditCards = e.CreditCards.Take(2).ToImmutableList() })
            .Tap(e => Console.WriteLine($"[AUDIT LOG] Credit cards limited to: {e.CreditCards.Count} cards"))
            .Tap(e => Console.WriteLine($"[AUDIT LOG] Employee processing completed for: {e.FirstName} {e.LastName}"));
        
        Console.WriteLine($"\nFinal result: {processedEmployee.FirstName} {processedEmployee.LastName}, Role: {processedEmployee.Role}, Cards: {processedEmployee.CreditCards.Count}");
        Console.WriteLine();
    }

    static void MusicGroupTapExample()
    {
        Console.WriteLine("--- MusicGroup Tap Example ---");
        
        var seeder = new SeedGenerator();
        var musicGroup = new MusicGroup().Seed(seeder);
        
        Console.WriteLine($"Processing music group: {musicGroup.Name}");
        
        // Example: Use Tap for performance monitoring and debugging during music group analysis
        var analyzedGroup = musicGroup
            .Tap(g => Console.WriteLine($"[DEBUG] Starting analysis for: {g.Name} ({g.Genre})"))
            .Tap(g => Console.WriteLine($"[METRICS] Initial stats - Albums: {g.Albums.Count}, Artists: {g.Artists.Count}, Est: {g.EstablishedYear}"))
            .Map(g => g with { Name = g.Name.Contains("The") ? g.Name : $"The {g.Name}" })
            .Tap(g => Console.WriteLine($"[DEBUG] Name normalized to: {g.Name}"))
            .Fork(
                g => g.Albums.Sum(a => a.CopiesSold),
                g => DateTime.Now.Year - g.EstablishedYear,
                (sales, years) => new { TotalSales = sales, YearsActive = years, Group = musicGroup }
            )
            .Tap(result => Console.WriteLine($"[METRICS] Commercial analysis - Sales: {result.TotalSales:N0}, Active: {result.YearsActive} years"))
            .Map(result => result.Group with { 
                Albums = result.Group.Albums
                    .OrderByDescending(a => a.CopiesSold)
                    .Take(5)
                    .ToImmutableList() 
            })
            .Tap(g => Console.WriteLine($"[DEBUG] Albums filtered to top 5 bestsellers: {string.Join(", ", g.Albums.Select(a => a.Name))}"))
            .Tap(g => Console.WriteLine($"[PERFORMANCE] Analysis completed for {g.Name} - Processing time: {DateTime.Now:HH:mm:ss}"));
        
        var successLevel = analyzedGroup.Albums.Sum(a => a.CopiesSold) switch
        {
            > 1_000_000 => "Platinum Success",
            > 500_000 => "Gold Success",
            > 100_000 => "Commercial Success",
            _ => "Emerging Artist"
        };
        
        Console.WriteLine($"\nFinal analysis: {analyzedGroup.Name} achieved {successLevel} with {analyzedGroup.Albums.Count} top albums");
        Console.WriteLine();
    }
}