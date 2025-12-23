using Models.Employees;
using Models.Music;
using PlayGround.Extensions;
using Seido.Utilities.SeedGenerator;

namespace Playground.Lesson03;
public static class Main03
{
    public static void Entry(string[] args = null)
    {
        System.Console.WriteLine("Hello Lesson 03!");

        //Map Extension Method Examples
        MapExtension.RunExamples();

        //Fork Extension Method Examples
        ForkExtension.RunExamples();
        
        //Alt Extension Method Examples
        AltExtension.RunExamples();
        
        //Compose Extension Method Examples
        ComposeExtension.RunExamples();
        
        //Tap Extension Method Examples
        TapExtension.RunExamples();
                
        //Maybe Monad Basics Examples (Types Only)
        MaybeMonadBasics.RunExamples();
        
        //Either Monad Basics Examples (Types Only)
        EitherMonadBasics.RunExamples();

        //Home Excercise Lesson 03
        HomeExercise03.Entry();
    }
}
