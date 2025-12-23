using Seido.Utilities.SeedGenerator;
using Models.Employees;
using Models.Music;
using PlayGround.Generics;

namespace Playground.Lesson03;

public static class EitherMonadBasics
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Either Monad Examples ===\n");
        
        EmployeeEitherExamples();
        MusicGroupEitherExamples();

        Console.WriteLine("\n=== End of Either Monad Examples ===\n");
    }

    static void EmployeeEitherExamples()
    {
        Console.WriteLine("--- Employee Either Examples ---");
        
        var seeder = new SeedGenerator();
        var employee = new Employee().Seed(seeder);
        
        Console.WriteLine($"Original Employee: {employee.FirstName} {employee.LastName}, Role: {employee.Role}");
        
        // Example 1: Employee validation - Either<string, Employee> (Error or Success)
        var validationResult = ValidateEmployee(employee);
        
        switch (validationResult)
        {
            case Left<string, Employee> error:
                Console.WriteLine($"Employee validation failed: {error.Value}");
                break;
            case Right<string, Employee> success:
                Console.WriteLine($"Employee validation passed: {success.Value.FirstName} {success.Value.LastName}");
                break;
        }
        
        // Example 2: Salary calculation - Either<string, decimal> (Error message or Salary amount)
        var salaryResult = CalculateEmployeeSalary(employee);
        
        switch (salaryResult)
        {
            case Left<string, decimal> error:
                Console.WriteLine($"Salary calculation error: {error.Value}");
                break;
            case Right<string, decimal> success:
                Console.WriteLine($"Employee salary calculated: ${success.Value:N0}");
                break;
        }
        
        // Example 3: Promotion check - Either<string, string> (Reason for no promotion or Promotion details)
        var promotionResult = CheckEmployeePromotion(employee);
        
        if (promotionResult is Left<string, string> noPromotion)
        {
            Console.WriteLine($"No promotion available: {noPromotion.Value}");
        }
        else if (promotionResult is Right<string, string> promotion)
        {
            Console.WriteLine($"Promotion opportunity: {promotion.Value}");
        }
        Console.WriteLine();
    }

    static void MusicGroupEitherExamples()
    {
        Console.WriteLine("--- MusicGroup Either Examples ---");
        
        var seeder = new SeedGenerator();
        var musicGroup = new MusicGroup().Seed(seeder);
        
        Console.WriteLine($"Original Group: {musicGroup.Name}, Genre: {musicGroup.Genre}, Albums: {musicGroup.Albums.Count}");
        
        // Example 1: Group validation - Either<string, MusicGroup>
        var validationResult = ValidateMusicGroup(musicGroup);
        
        switch (validationResult)
        {
            case Left<string, MusicGroup> error:
                Console.WriteLine($"Group validation failed: {error.Value}");
                break;
            case Right<string, MusicGroup> success:
                Console.WriteLine($"Group validation passed: {success.Value.Name} ({success.Value.Genre})");
                break;
        }
        
        // Example 2: Revenue calculation - Either<string, decimal>
        var revenueResult = CalculateGroupRevenue(musicGroup);
        
        switch (revenueResult)
        {
            case Left<string, decimal> error:
                Console.WriteLine($"Revenue calculation error: {error.Value}");
                break;
            case Right<string, decimal> revenue:
                Console.WriteLine($"Estimated group revenue: ${revenue.Value:N0}");
                break;
        }
        
        // Example 3: Contract negotiation - Either<string, string> (Rejection reason or Contract terms)
        var contractResult = NegotiateRecordDeal(musicGroup);
        
        if (contractResult is Left<string, string> rejection)
        {
            Console.WriteLine($"Record deal rejected: {rejection.Value}");
        }
        else if (contractResult is Right<string, string> contract)
        {
            Console.WriteLine($"Record deal terms: {contract.Value}");
        }
                
        Console.WriteLine();
    }

    // Helper methods for Employee examples
    static Either<string, Employee> ValidateEmployee(Employee employee)
    {
        if (string.IsNullOrEmpty(employee.FirstName))
            return new Left<string, Employee>("First name is required");
        
        if (string.IsNullOrEmpty(employee.LastName))
            return new Left<string, Employee>("Last name is required");
        
        if (employee.HireDate > DateTime.Now)
            return new Left<string, Employee>("Hire date cannot be in the future");
        
        if (employee.HireDate < DateTime.Now.AddYears(-50))
            return new Left<string, Employee>("Hire date seems too old - please verify");
        
        return new Right<string, Employee>(employee);
    }

    static Either<string, decimal> CalculateEmployeeSalary(Employee employee)
    {
        var tenure = DateTime.Now.Year - employee.HireDate.Year;
        
        if (tenure < 0)
            return new Left<string, decimal>("Invalid tenure calculation");
        
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
            return new Left<string, decimal>($"No salary information available for role: {employee.Role}");
        
        var experienceBonus = tenure * 2000m;
        var totalSalary = baseSalary + experienceBonus;
        
        return new Right<string, decimal>(totalSalary);
    }

    static Either<string, string> CheckEmployeePromotion(Employee employee)
    {
        var tenure = DateTime.Now.Year - employee.HireDate.Year;
        
        if (tenure < 1)
            return new Left<string, string>("Employee must have at least 1 year of service");
        
        var promotion = (employee.Role, tenure) switch
        {
            (WorkRole.AnimalCare, >= 5) => "Senior Animal Care Specialist - $52,000 base salary",
            (WorkRole.AnimalCare, >= 3) => "Lead Animal Care Technician - $48,000 base salary",
            (WorkRole.Veterinarian, >= 8) => "Chief Veterinarian - $95,000 base salary",
            (WorkRole.ProgramCoordinator, >= 6) => "Senior Program Manager - $72,000 base salary",
            (WorkRole.Maintenance, >= 10) => "Facilities Manager - $55,000 base salary",
            (WorkRole.Management, >= 7) => "Senior Management - $100,000 base salary",
            _ => null
        };
        
        if (promotion == null)
            return new Left<string, string>($"No promotion opportunities available for {employee.Role} with {tenure} years experience");
        
        return new Right<string, string>(promotion);
    }


    // Helper methods for MusicGroup examples
    static Either<string, MusicGroup> ValidateMusicGroup(MusicGroup group)
    {
        if (string.IsNullOrEmpty(group.Name))
            return new Left<string, MusicGroup>("Group name is required");
        
        if (group.EstablishedYear > DateTime.Now.Year)
            return new Left<string, MusicGroup>("Established year cannot be in the future");
        
        if (group.EstablishedYear < 1900)
            return new Left<string, MusicGroup>("Established year seems too early - please verify");
        
        if (!group.Artists.Any())
            return new Left<string, MusicGroup>("Group must have at least one artist");
        
        return new Right<string, MusicGroup>(group);
    }

    static Either<string, decimal> CalculateGroupRevenue(MusicGroup group)
    {
        if (!group.Albums.Any())
            return new Left<string, decimal>("No albums available for revenue calculation");
        
        try
        {
            var digitalSales = group.Albums.Sum(a => a.CopiesSold * 8.99m); // Digital sales
            var physicalSales = group.Albums.Sum(a => a.CopiesSold * 0.3m * 15.99m); // 30% physical at higher price
            var streamingRevenue = group.Albums.Sum(a => a.Tracks.Count * 1000 * 0.003m); // Estimated streams
            
            var totalRevenue = digitalSales + physicalSales + streamingRevenue;
            
            return new Right<string, decimal>(totalRevenue);
        }
        catch (OverflowException)
        {
            return new Left<string, decimal>("Revenue calculation resulted in overflow - sales figures may be too large");
        }
    }

    static Either<string, string> NegotiateRecordDeal(MusicGroup group)
    {
        var totalSales = group.Albums.Sum(a => a.CopiesSold);
        var yearsActive = DateTime.Now.Year - group.EstablishedYear;
        
        if (yearsActive < 2)
            return new Left<string, string>("Group too new for major record deal consideration");
        
        if (totalSales < 5000)
            return new Left<string, string>("Insufficient sales history for record deal");
        
        var dealTerms = (totalSales, yearsActive) switch
        {
            (> 1_000_000, > 15) => "Major label deal: 3 albums, $2M advance, 15% royalties",
            (> 500_000, > 10) => "Independent label deal: 2 albums, $500K advance, 18% royalties",
            (> 100_000, > 5) => "Regional label deal: 1 album, $100K advance, 20% royalties",
            (> 25_000, > 3) => "Indie label deal: 1 album, $25K advance, 25% royalties",
            _ => "Demo deal: EP only, no advance, 30% royalties"
        };
        
        return new Right<string, string>(dealTerms);
    }
}