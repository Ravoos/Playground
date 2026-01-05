using Models.Employees;
using Models.Music;
using PlayGround.Extensions;
using PlayGround.Generics;
using Seido.Utilities.SeedGenerator;
using System.Collections.Immutable;

namespace Playground.Lesson04;

/// <summary>
/// StateExtension Examples demonstrating stateful computations with Employee and MusicGroup data.
/// The State monad manages both state transformation and value computation in a single operation.
/// State is now represented as tuples or records instead of primitives, converted to strings at presentation.
/// </summary>
public static class StateExtensionExamples
{
    // Record types for structured state representation
    public record ComputationState(int Step, int Multiplier, DateTime Timestamp);
    public record CounterState(int Current, int Previous, int Operations);
    
    public static void RunExamples()
    {
        Console.WriteLine("\n=== State Extension Method Examples (with structured state) ===\n");
        
        // Simple state example
         var result = 10.ToState(10)           // s=10, x=10
        .Tap(state => 
            Console.WriteLine($"Initial: State={state.CurrentState}, Value={state.CurrentValue}"))
        .Bind((s, x) => (s * x).WithState(s))  // s=10, x=100
        .Tap(state => 
            Console.WriteLine($"After first Bind: State={state.CurrentState}, Value={state.CurrentValue}"))
        .Bind((s, x) => (x - s).WithState(s))  // s=10, x = 90
        .Tap(state => 
            Console.WriteLine($"After second Bind: State={state.CurrentState}, Value={state.CurrentValue}"))
        .UpdateState(s => s - 5)               // s=5, x = 90
        .Tap(state => 
            Console.WriteLine($"After Update: State={state.CurrentState}, Value={state.CurrentValue}"))
        .Bind((s, x) => (x / 5).WithState(s))  // s=5, x = 18
        .Tap(state => 
            Console.WriteLine($"Final: State={state.CurrentState}, Value={state.CurrentValue}"));

        Console.WriteLine();

        // Simple tuple-based state example
        var tupleResult = (count: 0, total: 0)
            .ToState("Starting")
            .Tap(state => Console.WriteLine($"Initial: {state}"))
            .Bind((s, msg) => $"{msg} -> Added 5".WithState((s.count + 1, s.total + 5)))
            .Tap(state => Console.WriteLine($"After add: {state}"))
            .Bind((s, msg) => $"{msg} -> Added 3".WithState((s.count + 1, s.total + 3)))
            .Tap(state => Console.WriteLine($"Final: {state}"));
        
        EmployeeStateExamples();
        MusicGroupStateExamples();

        Console.WriteLine("\n=== End of State Extension Method Examples ===\n");
    }
    
    #region Employee State Examples
    
    // Record types for employee-related state management
    public record BudgetState(decimal Amount, int ProcessedCount, DateTime LastUpdate);
    public record CapacityState(int MaxCapacity, int Current, int Available);
    public record CoverageState(int RequiredHours, int AssignedHours, List<string> Schedule);
    public record ComplianceState(int MaxViolations, int CurrentViolations, List<string> Issues);
    
    static void EmployeeStateExamples()
    {
        Console.WriteLine("--- Employee State Examples ---");
        
        var seeder = new SeedGenerator();
        var employees = seeder.ItemsToList<Employee>(50).ToImmutableList();
        
        Console.WriteLine($"Working with {employees.Count} employees");
        
        // Example 1: Payroll processing (simplified)
        var totalBudget = 2_000_000m;
        var payrollState = totalBudget
            .ToState(employees)
            .Bind((budget, emps) => {
                var totalSalaries = emps.Sum(e => GetBaseSalary(e.Role));
                var remainingBudget = budget - totalSalaries;
                
                var payrollSummary = $"Processed {emps.Count} employees, Total: ${totalSalaries:N0}";
                return payrollSummary.WithState(remainingBudget);
            });
            
        Console.WriteLine($"Payroll Processing: {payrollState.CurrentValue}");
        Console.WriteLine($"Remaining budget: ${payrollState.CurrentState:N0}\n");
        
        // Example 2: Employee onboarding (simplified)
        var hiringCount = 0;
        var onboardingState = hiringCount
            .ToState(employees)
            .Bind((count, emps) => {
                var newHires = emps.Count(e => DateTime.Now.Year - e.HireDate.Year < 1);
                var updatedCount = count + newHires;
                
                var onboardingPlan = $"New hires this year: {newHires}";
                return onboardingPlan.WithState(updatedCount);
            });
            
        Console.WriteLine($"Onboarding Planning: {onboardingState.CurrentValue}");
        Console.WriteLine($"Total hiring count: {onboardingState.CurrentState}\n");
                
        // Example 3: Credit card policy compliance (simplified)
        var violationCount = 0;
        var complianceState = violationCount
            .ToState(employees)
            .Bind((violations, emps) => {
                var highRiskCount = emps.Count(e => e.CreditCards.Count > 3);
                var updatedViolations = violations + highRiskCount;
                
                var complianceReport = $"High-risk employees: {highRiskCount}";
                return complianceReport.WithState(updatedViolations);
            });
            
        Console.WriteLine($"Compliance Check: {complianceState.CurrentValue}");
        Console.WriteLine($"Total violations found: {complianceState.CurrentState}\n");
                
        // Example 4: Processing employees sequentially with accumulating state
        SequentialEmployeeProcessing(employees);
    }
    
    #region Sequential Processing Examples
    
    // Record types for sequential processing state
    public record HiringMetrics(int TotalHired, decimal CurrentBudget, List<string> HiredEmployees);
    
    static void SequentialEmployeeProcessing(ImmutableList<Employee> employees)
    {
        Console.WriteLine("--- Sequential Employee Processing ---");
        
        // Simple hiring decisions based on budget
        var initialBudget = 800_000m;
        
        var finalState = employees.Take(8).Aggregate(
            initialBudget.ToState("Starting hiring process"),
            (currentState, employee) => currentState.Bind((budget, report) => {
                var salary = GetBaseSalary(employee.Role);
                var hiringCost = salary + (salary * 0.15m); // 15% benefits overhead
                
                if (budget >= hiringCost)
                {
                    var newBudget = budget - hiringCost;
                    var newReport = $"{report}\n  ✓ Hired {employee.FirstName} {employee.LastName} - Cost: ${hiringCost:N0}";
                    return newReport.WithState(newBudget);
                }
                else
                {
                    var newReport = $"{report}\n  ✗ Rejected {employee.FirstName} {employee.LastName} - Budget insufficient";
                    return newReport.WithState(budget);
                }
            })
        );
        
        Console.WriteLine($"{finalState.CurrentValue}");
        Console.WriteLine($"Remaining budget: ${finalState.CurrentState:N0}\n");
    }
    #endregion
    
    #endregion
    
    #region MusicGroup State Examples
    
    // Record types for music-related state management
    public record LabelBudgetState(decimal Budget, decimal Revenue, decimal Reserve, int GroupsProcessed);
    public record StageTimeState(int TotalMinutes, int UsedMinutes, List<string> Lineup);
    public record PlaylistDiversityState(int Target, int CurrentScore, Dictionary<string, int> GenreBalance);
    public record TourCapacityState(int MaxCapacity, int UsedCapacity, List<string> Bookings);
    public record InvestmentMetrics(int GroupsSigned, decimal BudgetRemaining, List<string> SignedGroups);
    
    static void MusicGroupStateExamples()
    {
        Console.WriteLine("--- MusicGroup State Examples ---");
        
        var seeder = new SeedGenerator();
        var musicGroups = seeder.ItemsToList<MusicGroup>(30).ToImmutableList();
        
        Console.WriteLine($"Working with {musicGroups.Count} music groups");
        
        // Example 1: Record label budget allocation (simplified)
        var labelBudget = 5_000_000m;
        var budgetState = labelBudget
            .ToState(musicGroups)
            .Bind((budget, groups) => {
                var totalRevenue = groups.Sum(g => g.Albums.Sum(a => a.CopiesSold * 15.99m));
                var remainingBudget = budget - (totalRevenue * 0.1m); // 10% investment cost
                
                var allocation = $"Generated ${totalRevenue:N0} revenue from {groups.Count} groups";
                return allocation.WithState(remainingBudget);
            });
            
        Console.WriteLine($"Label Budget: {budgetState.CurrentValue}");
        Console.WriteLine($"Remaining budget: ${budgetState.CurrentState:N0}\n");
        
        // Example 2: Festival lineup curation (simplified)
        var totalMinutes = 480; // 8 hours
        var lineupState = totalMinutes
            .ToState(musicGroups.OrderByDescending(g => g.Albums.Sum(a => a.CopiesSold)).Take(6).ToList())
            .Bind((minutes, topGroups) => {
                var timePerGroup = minutes / Math.Max(1, topGroups.Count);
                var usedTime = topGroups.Count * timePerGroup;
                
                var lineupDetails = $"Festival Lineup ({topGroups.Count} groups, {timePerGroup}min each)";
                return lineupDetails.WithState(minutes - usedTime);
            });
            
        Console.WriteLine($"{lineupState.CurrentValue}");
        Console.WriteLine($"Remaining time: {lineupState.CurrentState}min\n");
        
        // Example 3: Sequential music group signing with investment tracking
        SequentialMusicGroupProcessing(musicGroups);
    }
    
    static void SequentialMusicGroupProcessing(ImmutableList<MusicGroup> musicGroups)
    {
        Console.WriteLine("--- Sequential Music Group Processing ---");
        
        // Simple record label signing decisions based on budget
        var initialBudget = 500_000m;
        
        var finalState = musicGroups.Take(6).Aggregate(
            initialBudget.ToState("Starting group evaluations"),
            (currentState, group) => currentState.Bind((budget, report) => {
                var totalSales = group.Albums.Sum(a => a.CopiesSold);
                var signingCost = totalSales > 100_000 ? 80_000m : 40_000m;
                
                if (budget >= signingCost)
                {
                    var newBudget = budget - signingCost;
                    var newReport = $"{report}\n  ✓ Signed {group.Name} - Cost: ${signingCost:N0}";
                    return newReport.WithState(newBudget);
                }
                else
                {
                    var newReport = $"{report}\n  ✗ Passed on {group.Name} - Budget insufficient";
                    return newReport.WithState(budget);
                }
            })
        );
        
        Console.WriteLine($"{finalState.CurrentValue}");
        Console.WriteLine($"Remaining budget: ${finalState.CurrentState:N0}\n");
    }
    
    #endregion
    
    #region Helper Methods
    
    // Helper methods for converting structured state to string representation at presentation time
    private static string BudgetStateToString(BudgetState state) =>
        $"Amount=${state.Amount:N0}, Processed={state.ProcessedCount}, Updated={state.LastUpdate:HH:mm:ss}";
    
    private static string CapacityStateToString(CapacityState state) =>
        $"Max={state.MaxCapacity}, Current={state.Current}, Available={state.Available}";
        
    private static string ComplianceStateToString(ComplianceState state) =>
        $"Violations={state.CurrentViolations}/{state.MaxViolations}, Issues=[{string.Join(", ", state.Issues.Take(2))}]";
    
    private static string HiringMetricsToString(HiringMetrics state) =>
        $"Hired={state.TotalHired}, Budget=${state.CurrentBudget:N0}, Employees=[{string.Join(", ", state.HiredEmployees.Take(3))}]";
    
    private static string LabelBudgetStateToString(LabelBudgetState state) =>
        $"Budget=${state.Budget:N0}, Revenue=${state.Revenue:N0}, Reserve=${state.Reserve:N0}, Groups={state.GroupsProcessed}";
    
    private static string StageTimeStateToString(StageTimeState state) =>
        $"Total={state.TotalMinutes}min, Used={state.UsedMinutes}min, Remaining={state.TotalMinutes - state.UsedMinutes}min, Acts={state.Lineup.Count}";
            
    private static string InvestmentMetricsToString(InvestmentMetrics state) =>
        $"Signed={state.GroupsSigned}, Budget=${state.BudgetRemaining:N0}, Groups=[{string.Join(", ", state.SignedGroups.Take(3))}]";

    // Original helper methods for salary calculations
    private static decimal GetBaseSalary(WorkRole role) => role switch
    {
        WorkRole.Management => 120_000m,
        WorkRole.Veterinarian => 95_000m,
        WorkRole.ProgramCoordinator => 65_000m,
        WorkRole.AnimalCare => 45_000m,
        WorkRole.Maintenance => 50_000m,
        _ => 40_000m
    };    
    #endregion
}