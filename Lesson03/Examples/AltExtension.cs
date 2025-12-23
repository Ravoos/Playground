using Seido.Utilities.SeedGenerator;
using Models.Employees;
using Models.Music;
using PlayGround.Extensions;

namespace Playground.Lesson03;

public static class AltExtension
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Alt Extension Method Examples ===\n");
        
        EmployeeAltExamples();
        MusicGroupAltExamples();

        Console.WriteLine("\n=== End of Alt Extension Method Examples ===\n");
    }

    static void EmployeeAltExamples()
    {
        Console.WriteLine("--- Employee Alt Examples ---");
        
        var seeder = new SeedGenerator();
        var employee = new Employee().Seed(seeder);
        var employeeWithoutCards = employee with { CreditCards = System.Collections.Immutable.ImmutableList<CreditCard>.Empty };
        
        Console.WriteLine($"Original Employee: {employee.FirstName} {employee.LastName}, Role: {employee.Role}, Cards: {employee.CreditCards.Count}");
        
        // Example 1: Alt to get employee contact preference with fallbacks
        var contactInfo = employee.Alt(
            e => e.CreditCards.Count > 2 ? "Primary cardholder - use billing address" : null,
            e => e.Role == WorkRole.Management ? "Management contact - use office phone" : null,
            e => DateTime.Now.Year - e.HireDate.Year > 5 ? "Senior employee - use direct line" : null,
            e => "Standard employee - use general contact"
        );
        Console.WriteLine($"Contact Preference: {contactInfo}");
        
        // Example 2: Alt to determine employee title with hierarchical fallbacks
        var employeeTitle = employee.Alt(
            e => e.Role == WorkRole.Management && DateTime.Now.Year - e.HireDate.Year > 10 ? "Senior Manager" : null,
            e => e.Role == WorkRole.Management && DateTime.Now.Year - e.HireDate.Year > 5 ? "Manager" : null,
            e => e.Role == WorkRole.Veterinarian && DateTime.Now.Year - e.HireDate.Year > 8 ? "Chief Veterinarian" : null,
            e => e.Role == WorkRole.Veterinarian ? "Veterinarian" : null,
            e => DateTime.Now.Year - e.HireDate.Year > 15 ? $"Senior {e.Role}" : null,
            e => DateTime.Now.Year - e.HireDate.Year > 5 ? $"Experienced {e.Role}" : null,
            e => e.Role.ToString()
        );
        Console.WriteLine($"Employee Title: {employeeTitle}");
            
        // Example 3: Testing with employee without credit cards
        Console.WriteLine($"\nEmployee without cards: {employeeWithoutCards.FirstName} {employeeWithoutCards.LastName}");
        var noCreditAssessment = employeeWithoutCards.Alt(
            e => e.CreditCards.Count > 3 && e.Role == WorkRole.Management ? "Premium Credit Profile" : null,
            e => e.CreditCards.Count > 2 && DateTime.Now.Year - e.HireDate.Year > 5 ? "Good Credit Profile" : null,
            e => e.CreditCards.Count > 1 ? "Standard Credit Profile" : null,
            e => e.CreditCards.Count == 1 ? "Basic Credit Profile" : null,
            e => "No Credit History"
        );
        Console.WriteLine($"Credit Assessment (No Cards): {noCreditAssessment}");
        Console.WriteLine();
    }

    static void MusicGroupAltExamples()
    {
        Console.WriteLine("--- MusicGroup Alt Examples ---");
        
        var seeder = new SeedGenerator();
        var musicGroup = new MusicGroup().Seed(seeder);
        var newGroup = musicGroup with { EstablishedYear = 2020, Albums = System.Collections.Immutable.ImmutableList<Album>.Empty };
        
        Console.WriteLine($"Original Group: {musicGroup.Name}, Genre: {musicGroup.Genre}, Est: {musicGroup.EstablishedYear}, Albums: {musicGroup.Albums.Count}");
        
        // Example 1: Alt to determine band status with multiple criteria
        var bandStatus = musicGroup.Alt(
            g => g.Albums.Count > 10 && DateTime.Now.Year - g.EstablishedYear > 20 ? "Legendary Band" : null,
            g => g.Albums.Count > 8 && DateTime.Now.Year - g.EstablishedYear > 15 ? "Veteran Band" : null,
            g => g.Albums.Count > 5 && DateTime.Now.Year - g.EstablishedYear > 10 ? "Established Band" : null,
            g => g.Albums.Count > 3 && DateTime.Now.Year - g.EstablishedYear > 5 ? "Active Band" : null,
            g => g.Albums.Count > 0 ? "Emerging Band" : null,
            g => "New Band"
        );
        Console.WriteLine($"Band Status: {bandStatus}");
        
        // Example 2: Alt for tour venue recommendations based on success level
        var venueRecommendation = musicGroup.Alt(
            g => g.Albums.Sum(a => a.CopiesSold) > 1_000_000 ? "Stadium Tour Ready" : null,
            g => g.Albums.Sum(a => a.CopiesSold) > 500_000 ? "Arena Tour Suitable" : null,
            g => g.Albums.Sum(a => a.CopiesSold) > 100_000 ? "Theater Circuit Appropriate" : null,
            g => g.Albums.Sum(a => a.CopiesSold) > 10_000 ? "Club Circuit Ready" : null,
            g => g.Albums.Count > 0 ? "Local Venue Performer" : null,
            g => "Demo Stage Only"
        );
        Console.WriteLine($"Venue Recommendation: {venueRecommendation}");
        
        // Example 3: Alt for record label interest level
        var labelInterest = musicGroup.Alt(
            g => g.Genre == MusicGenre.Rock && g.Albums.Sum(a => a.CopiesSold) > 500_000 ? "Major Label Interest" : null,
            g => g.Genre == MusicGenre.Jazz && DateTime.Now.Year - g.EstablishedYear > 15 ? "Specialized Jazz Label Interest" : null,
            g => g.Albums.Count > 5 && g.Albums.Sum(a => a.CopiesSold) > 100_000 ? "Independent Label Interest" : null,
            g => g.Albums.Count > 2 && g.Artists.Count >= 3 ? "Regional Label Interest" : null,
            g => g.Albums.Count > 0 ? "Demo Label Consideration" : null,
            g => "Self-Released Only"
        );
        Console.WriteLine($"Label Interest: {labelInterest}");

        // Example 4: Testing with new band (no albums)
        Console.WriteLine($"\nNew Group: {newGroup.Name}, Genre: {newGroup.Genre}, Est: {newGroup.EstablishedYear}, Albums: {newGroup.Albums.Count}");
        var newBandStatus = newGroup.Alt(
            g => g.Albums.Count > 10 && DateTime.Now.Year - g.EstablishedYear > 20 ? "Legendary Band" : null,
            g => g.Albums.Count > 8 && DateTime.Now.Year - g.EstablishedYear > 15 ? "Veteran Band" : null,
            g => g.Albums.Count > 5 && DateTime.Now.Year - g.EstablishedYear > 10 ? "Established Band" : null,
            g => g.Albums.Count > 3 && DateTime.Now.Year - g.EstablishedYear > 5 ? "Active Band" : null,
            g => g.Albums.Count > 0 ? "Emerging Band" : null,
            g => "New Band"
        );
        Console.WriteLine($"New Band Status: {newBandStatus}");
        Console.WriteLine();
    }
}