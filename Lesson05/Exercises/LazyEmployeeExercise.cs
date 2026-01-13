using Models.Employees;
using Seido.Utilities.SeedGenerator;

namespace Playground.Lesson05.Exercises;

public static class LazyEmployeeExercise
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
    }

    private static void SelectiveCreditCardLoadingExercise(SeedGenerator seeder)
    {
        Console.WriteLine("Exercise 2: Selective Credit Card Loading");
    }
}