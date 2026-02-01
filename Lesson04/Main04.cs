using PlayGround.Lesson04.CoinFlip;

namespace Playground.Lesson04;
public static class Main04
{
    public static void Entry(string[] args = null)
    {
        Console.WriteLine("Hello Lesson 04!");

        //Maybe Bind Extension Method Examples
        MaybeExtension.RunExamples();
        
        //State Extension Method Examples
        StateExtensionExamples.RunExamples();
        
        //Simple Game Exercise: Create a Coin Flip Challenge
        CoinFlipGame.RunSimulation();
   }
}
