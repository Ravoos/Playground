using Seido.Utilities.SeedGenerator;
using Models.Employees;
using Models.Music;
using PlayGround.Generics;

namespace Playground.Lesson03;

public static class MaybeMonadBasics
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Maybe Monad Basics Examples ===\n");
        
        EmployeeBasicMaybeExamples();
        MusicGroupBasicMaybeExamples();

        Console.WriteLine("\n=== End of Maybe Monad Basics Examples ===\n");
    }

    static void EmployeeBasicMaybeExamples()
    {
        Console.WriteLine("--- Employee Basic Maybe Examples ---");
        
        var seeder = new SeedGenerator();
        var employee = new Employee().Seed(seeder);
        
        Console.WriteLine($"Original Employee: {employee.FirstName} {employee.LastName}, Role: {employee.Role}");
               
        // Example 1: Salary calculation with Maybe types
        var salaryResult = CalculateSalary(employee);
        
        switch (salaryResult)
        {
            case Something<decimal> salary:
                Console.WriteLine($"Employee salary: ${salary.Value:N0}");
                break;
            case Nothing<decimal>:
                Console.WriteLine("Salary information not available");
                break;
            case Error<decimal> error:
                Console.WriteLine($"Salary calculation error: {error.CapturedError.Message}");
                break;
        }
        
        // Example 2: Credit card validation
        var creditResult = ValidateCreditCards(employee);
        
        switch (creditResult)
        {
            case Something<string> status:
                Console.WriteLine($"Credit status: {status.Value}");
                break;
            case Nothing<string>:
                Console.WriteLine("No credit cards to validate");
                break;
            case Error<string> error:
                Console.WriteLine($"Credit validation failed: {error.CapturedError.Message}");
                break;
        }
                
        Console.WriteLine();
    }

    static void MusicGroupBasicMaybeExamples()
    {
        Console.WriteLine("--- MusicGroup Basic Maybe Examples ---");
        
        var seeder = new SeedGenerator();
        var musicGroup = new MusicGroup().Seed(seeder);
        
        Console.WriteLine($"Original Group: {musicGroup.Name}, Genre: {musicGroup.Genre}, Albums: {musicGroup.Albums.Count}");
              
        // Example 1: Revenue calculation
        var revenueResult = CalculateRevenue(musicGroup);
        
        switch (revenueResult)
        {
            case Something<decimal> revenue:
                Console.WriteLine($"Estimated revenue: ${revenue.Value:N0}");
                break;
            case Nothing<decimal>:
                Console.WriteLine("Revenue data not available");
                break;
            case Error<decimal> error:
                Console.WriteLine($"Revenue calculation error: {error.CapturedError.Message}");
                break;
        }
        
        // Example 3: Best selling album
        var albumResult = GetBestSellingAlbum(musicGroup);
        
        switch (albumResult)
        {
            case Something<Album> album:
                Console.WriteLine($"Best selling album: '{album.Value.Name}' - {album.Value.CopiesSold:N0} copies");
                break;
            case Nothing<Album>:
                Console.WriteLine("No albums available");
                break;
            case Error<Album> error:
                Console.WriteLine($"Album lookup error: {error.CapturedError.Message}");
                break;
        }                
        Console.WriteLine();
    }

    // Helper methods for Employee examples
    static Maybe<decimal> CalculateSalary(Employee employee)
    {
        try
        {
            // Simulate business rule that might cause error
            if (employee.HireDate > DateTime.Now)
            {
                throw new ArgumentException("Invalid hire date - cannot be in future");
            }

            var baseSalary = employee.Role switch
            {
                WorkRole.Management => 85000m,
                WorkRole.Veterinarian => 75000m,
                WorkRole.ProgramCoordinator => 60000m,
                WorkRole.AnimalCare => 45000m,
                WorkRole.Maintenance => 40000m,
                _ => 0m
            };

            if (baseSalary == 0)
            {
                return new Nothing<decimal>();
            }

            var experienceBonus = (DateTime.Now.Year - employee.HireDate.Year) * 2000m;
            return new Something<decimal>(baseSalary + experienceBonus);
        }
        catch (Exception ex)
        {
            return new Error<decimal>(ex);
        }
    }

    static Maybe<string> ValidateCreditCards(Employee employee)
    {
        try
        {
            if (!employee.CreditCards.Any())
            {
                return new Nothing<string>();
            }

            // Simulate validation that might throw
            if (employee.CreditCards.Count > 10)
            {
                throw new InvalidOperationException("Suspicious number of credit cards");
            }

            var status = employee.CreditCards.Count switch
            {
                1 => "Single card holder - Low risk",
                2 => "Dual card holder - Standard risk",
                >= 3 and <= 5 => "Multiple cards - Moderate risk",
                _ => "High activity - Enhanced monitoring"
            };

            return new Something<string>(status);
        }
        catch (Exception ex)
        {
            return new Error<string>(ex);
        }
    }

    static Maybe<decimal> CalculateRevenue(MusicGroup group)
    {
        try
        {
            if (!group.Albums.Any())
            {
                return new Nothing<decimal>();
            }

            // Simulate potential error condition
            if (group.EstablishedYear < 1900)
            {
                throw new ArgumentException("Invalid establishment year");
            }

            var totalRevenue = group.Albums.Sum(a => a.CopiesSold * 12.99m); // Average price per album
            return new Something<decimal>(totalRevenue);
        }
        catch (Exception ex)
        {
            return new Error<decimal>(ex);
        }
    }

    static Maybe<Album> GetBestSellingAlbum(MusicGroup group)
    {
        try
        {
            if (!group.Albums.Any())
            {
                return new Nothing<Album>();
            }

            var bestAlbum = group.Albums.OrderByDescending(a => a.CopiesSold).First();
            return new Something<Album>(bestAlbum);
        }
        catch (Exception ex)
        {
            return new Error<Album>(ex);
        }
    }
}