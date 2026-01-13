using System.Collections.Immutable;
using Models.Employees;
using Seido.Utilities.SeedGenerator;

namespace Playground.Lesson05.Exercises;

public record EmployeeLazy (
    Guid EmployeeId,
    string FirstName,
    string LastName,
    DateTime HireDate,
    WorkRole Role,
    Lazy<ImmutableList<CreditCard>> CreditCards,
    bool Seeded = false
) : ISeed<EmployeeLazy>
{
    public EmployeeLazy() : this(default, default, default, default, default, default) {}

    #region randomly seed this instance
    public virtual EmployeeLazy Seed(SeedGenerator seeder)
    {
        // Create lazy factory for credit cards
        var lazyCreditCards = new Lazy<ImmutableList<CreditCard>>(() =>
        {
            Console.WriteLine($"[Lazy Loading] Generating credit cards for {seeder.FirstName}...");
            var cardCount = seeder.Next(0, 4); // 0 to 3 credit cards
            var cards = seeder.ItemsToList<CreditCard>(cardCount);
            return cards.ToImmutableList();
        });

        var ret = new EmployeeLazy(
            Guid.NewGuid(),
            seeder.FirstName,
            seeder.LastName,
            seeder.DateAndTime(2000, 2024),
            seeder.FromEnum<WorkRole>(),
            lazyCreditCards,
            true
        );
        return ret;
    }
    #endregion
}

public static class LazyEmployeeExerciseAnswer
{
    public static void RunExercise()
    {
        Console.WriteLine("\n=== Lazy Employee Exercise ===\n");

        var seeder = new SeedGenerator();

        // Exercise 1: Create employees with lazy credit cards
        LazyEmployeeCreationExercise(seeder);

        // Exercise 2: Selective credit card loading
        SelectiveCreditCardLoadingExercise(seeder);

        Console.WriteLine("\n=== End of Lazy Employee Exercise ===\n");
    }

    private static void LazyEmployeeCreationExercise(SeedGenerator seeder)
    {
        Console.WriteLine("Exercise 1: Create Employees with Lazy Credit Cards");

        // TODO: Create a list of EmployeeLazy objects
        var employees = seeder.ItemsToList<EmployeeLazy>(10);

        Console.WriteLine($"   Created {employees.Count} employees with lazy credit cards.");
        Console.WriteLine("   Credit cards are not yet loaded (lazy initialization).");

        // Show that credit cards are not loaded yet
        foreach (var employee in employees.Take(3))
        {
            Console.WriteLine($"   Employee: {employee.FirstName} {employee.LastName} ({employee.Role})");
            Console.WriteLine($"   Credit cards loaded: {employee.CreditCards.IsValueCreated}");
        }

        Console.WriteLine("\n   Now accessing credit cards for first employee...");
        var firstEmployeeCreditCards = employees.First().CreditCards.Value;
        Console.WriteLine($"   First employee now has {firstEmployeeCreditCards.Count} credit cards loaded.");

        Console.WriteLine();
    }

    private static void SelectiveCreditCardLoadingExercise(SeedGenerator seeder)
    {
        Console.WriteLine("Exercise 2: Selective Credit Card Loading");

        var employees = seeder.ItemsToList<EmployeeLazy>(5);
        Console.WriteLine($"   Created {employees.Count} employees.");

        // Load credit cards only for management roles
        var managementEmployees = employees.Where(e => e.Role == WorkRole.Management);
        
        Console.WriteLine("   Loading credit cards only for Management employees...");
        foreach (var employee in managementEmployees)
        {
            var cardCount = employee.CreditCards.Value.Count;
            Console.WriteLine($"   Manager {employee.FirstName} {employee.LastName}: {cardCount} credit cards");
        }

        // Show loading status
        var cardsLoaded = employees.Count(e => e.CreditCards.IsValueCreated);
        Console.WriteLine($"   Credit cards loaded for: {cardsLoaded}/{employees.Count} employees");

        Console.WriteLine();
    }
}