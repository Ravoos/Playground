using Seido.Utilities.SeedGenerator;
using Models.Employees;
using Models.Music;
using PlayGround.Extensions;

namespace Playground.Lesson03;

public static class ForkExtension
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Fork Extension Method Examples ===\n");
        
        EmployeeForkExamples();
        MusicGroupForkExamples();

        Console.WriteLine("\n=== End of Fork Extension Method Examples ===\n");
    }

    static void EmployeeForkExamples()
    {
        Console.WriteLine("--- Employee Fork Examples ---");
        
        var seeder = new SeedGenerator();
        var employee = new Employee().Seed(seeder);
        
        Console.WriteLine($"Original Employee: {employee.FirstName} {employee.LastName}, Role: {employee.Role}");
        
        // Example 1: Fork to combine personal info and work info
        var employeeProfile = employee.Fork(
            e => $"{e.FirstName} {e.LastName}",           // Extract full name
            e => $"{e.Role} since {e.HireDate:yyyy}",     // Extract work info
            (name, workInfo) => $"Profile: {name} - {workInfo}"
        );
        Console.WriteLine($"Result: {employeeProfile}");
        
        // Example 2: Fork to combine tenure and credit card stats
        var employeeStats = employee.Fork(
            e => DateTime.Now.Year - e.HireDate.Year,     // Calculate tenure
            e => e.CreditCards.Count,                     // Count credit cards
            (tenure, cardCount) => new {
                YearsOfService = tenure,
                CreditCards = cardCount,
                Category = tenure > 5 && cardCount > 2 ? "Senior with High Credit" : "Standard"
            }
        );
        Console.WriteLine($"Stats: {employeeStats.YearsOfService} years, {employeeStats.CreditCards} cards, Category: {employeeStats.Category}");
        
        // Example 3: Fork to create employee summary combining different aspects
        var summary = employee.Fork(
            e => new { Name = $"{e.FirstName} {e.LastName}", Role = e.Role },
            e => new { Tenure = DateTime.Now.Year - e.HireDate.Year, Cards = e.CreditCards.Count },
            (personal, professional) => 
                $"Summary: {personal.Name} ({personal.Role}) has {professional.Tenure} years experience and {professional.Cards} credit cards"
        );
        Console.WriteLine($"Complete Summary: {summary}");
        
        // Example 4: Fork to compare salary potential vs experience
        var evaluation = employee.Fork(
            e => e.Role switch {
                WorkRole.Management => 85000,
                WorkRole.Veterinarian => 75000,
                WorkRole.ProgramCoordinator => 60000,
                WorkRole.AnimalCare => 45000,
                WorkRole.Maintenance => 40000,
                _ => 35000
            },                                            // Estimate salary based on role
            e => (DateTime.Now.Year - e.HireDate.Year) * 2000,  // Experience bonus
            (baseSalary, experienceBonus) => new {
                EstimatedSalary = baseSalary + experienceBonus,
                Base = baseSalary,
                Bonus = experienceBonus
            }
        );
        Console.WriteLine($"Salary Evaluation: Base ${evaluation.Base:N0} + Experience Bonus ${evaluation.Bonus:N0} = Total ${evaluation.EstimatedSalary:N0}");
        
        Console.WriteLine();
    }

    static void MusicGroupForkExamples()
    {
        Console.WriteLine("--- MusicGroup Fork Examples ---");
        
        var seeder = new SeedGenerator();
        var musicGroup = new MusicGroup().Seed(seeder);
        
        Console.WriteLine($"Original Group: {musicGroup.Name}, Genre: {musicGroup.Genre}, Est: {musicGroup.EstablishedYear}");
        
        // Example 1: Fork to combine group info and discography stats
        var groupOverview = musicGroup.Fork(
            g => $"{g.Name} ({g.Genre})",                 // Group identity
            g => $"{g.Albums.Count} albums, {g.Artists.Count} artists",  // Discography stats
            (identity, stats) => $"Overview: {identity} - {stats}"
        );
        Console.WriteLine($"Result: {groupOverview}");
        
        // Example 2: Fork to calculate career span and productivity
        var careerAnalysis = musicGroup.Fork(
            g => DateTime.Now.Year - g.EstablishedYear,   // Years active
            g => g.Albums.Count > 0 ? (double)g.Albums.Sum(a => a.Tracks.Count) / g.Albums.Count : 0, // Avg tracks per album
            (yearsActive, avgTracks) => new {
                CareerSpan = yearsActive,
                Productivity = avgTracks,
                Classification = yearsActive switch {
                    > 30 => "Legendary",
                    > 20 => "Veteran",
                    > 10 => "Established",
                    > 5 => "Developing",
                    _ => "New"
                }
            }
        );
        Console.WriteLine($"Career Analysis: {careerAnalysis.CareerSpan} years active, {careerAnalysis.Productivity:F1} avg tracks/album, Status: {careerAnalysis.Classification}");
        
        // Example 3: Fork to combine commercial success with artistic output
        var successMetrics = musicGroup.Fork(
            g => g.Albums.Sum(a => a.CopiesSold),         // Total sales
            g => g.Albums.Sum(a => a.Tracks.Count),       // Total tracks produced
            (totalSales, totalTracks) => new {
                TotalSales = totalSales,
                TotalTracks = totalTracks,
                SalesPerTrack = totalTracks > 0 ? totalSales / totalTracks : 0,
                SuccessLevel = totalSales switch {
                    > 1_000_000 => "Platinum Success",
                    > 500_000 => "Gold Success", 
                    > 100_000 => "Commercial Success",
                    > 10_000 => "Moderate Success",
                    _ => "Emerging"
                }
            }
        );
        Console.WriteLine($"Success Metrics: {successMetrics.TotalSales:N0} total sales, {successMetrics.TotalTracks} tracks, {successMetrics.SalesPerTrack:F0} sales/track, Level: {successMetrics.SuccessLevel}");
        
        // Example 4: Fork to analyze genre popularity vs longevity
        var marketAnalysis = musicGroup.Fork(
            g => g.Genre switch {
                MusicGenre.Rock => new { PopularityScore = 85, Timeless = true },
                MusicGenre.Jazz => new { PopularityScore = 70, Timeless = true },
                MusicGenre.Blues => new { PopularityScore = 60, Timeless = true },
                MusicGenre.Metal => new { PopularityScore = 75, Timeless = false },
                _ => new { PopularityScore = 50, Timeless = false }
            },                                            // Genre characteristics
            g => new {
                Longevity = DateTime.Now.Year - g.EstablishedYear,
                IsActive = DateTime.Now.Year - g.EstablishedYear < 50,
                AlbumFrequency = g.Albums.Count > 0 ? (DateTime.Now.Year - g.EstablishedYear) / (double)g.Albums.Count : 0
            },                                            // Career metrics
            (genreInfo, careerInfo) => new {
                GenrePopularity = genreInfo.PopularityScore,
                CareerLongevity = careerInfo.Longevity,
                MarketPosition = (genreInfo.PopularityScore * 0.6) + (careerInfo.Longevity * 2),
                PredictedFuture = genreInfo.Timeless && careerInfo.IsActive ? "Strong Future" : 
                                  genreInfo.Timeless ? "Legacy Act" : "Niche Appeal"
            }
        );
        Console.WriteLine($"Market Analysis: Genre popularity {marketAnalysis.GenrePopularity}%, {marketAnalysis.CareerLongevity} years active, Market score: {marketAnalysis.MarketPosition:F1}, Outlook: {marketAnalysis.PredictedFuture}");
        
        // Example 5: Fork to create comprehensive band profile
        var bandProfile = musicGroup.Fork(
            g => new {
                BasicInfo = $"{g.Name} ({g.Genre})",
                Timeline = $"{g.EstablishedYear}-Present",
                Experience = DateTime.Now.Year - g.EstablishedYear
            },                                            // Basic information
            g => new {
                Catalog = $"{g.Albums.Count} albums",
                Personnel = $"{g.Artists.Count} artists",
                Output = g.Albums.Sum(a => a.Tracks.Count)
            },                                            // Production statistics
            (basic, production) => 
                $"Band Profile: {basic.BasicInfo} | Active: {basic.Timeline} ({basic.Experience} years) | " +
                $"Discography: {production.Catalog}, {production.Personnel}, {production.Output} total tracks"
        );
        Console.WriteLine($"Complete Profile: {bandProfile}");
        
        Console.WriteLine();
    }
}