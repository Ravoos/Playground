# Yahtzee Round Simulation Project

## Run the simulation
To run the Yahtzee round simulation, go to program.cs and make sure that:
```
	Playground.Projects.YahtzeeProject.Entry();
```
is not commented out, otherwise it will not run. 

Comment out any other simulation project in the file if you wish to only run the yahtzee simulation.

Then, run the program. The simulation will execute a single round of Yahtzee, rolling the dice and displaying the results in the console.

## Explenation of the VG advanced features
For the rolling mechanic I have this piece of code:
```
	static YahzeeCup RollUpToThreeTimes(YahzeeCup initialCup)
    {
        return Enumerable.Range(1, 3)
            .Aggregate(initialCup, (cup, _) =>
            {
                // Randomly decide how many dice to keep
                var keepCount = Random.Shared.Next(1, 6);

                var bestCombo =
                    cup.GetAllYahtzeeCombinations()
                        .FirstOrDefault(c => c is not Chance and not NoCombination);

                ImmutableList<Die> keptDice =
                    bestCombo is null
                        ? KeepMostFrequentValue(cup)
                        : bestCombo.dice.ToImmutableList();


                // Create a new cup with kept dice with a placeholder
                var newCup = new YahzeeCup
                {
                    dice = keptDice
                        .AddRange(
                            Enumerable.Range(0, 5 - keptDice.Count)
                                .Select(_ => new Die(DiePip.One)))
                };

                return newCup.ShakeAndRoll();
            });
    }
```
- What the code does is that when I roll the initial cup that I am given and potentially reroll it two times.
- I'm using Enumerable.Range and Aggregate to make new immutable versions instead of mutating the same value.
	- Inside the Aggregate I have a a function that tries to find the best combination made with the dice roll.
   		- An important note is that I try to find the best "meaningful" one. I.e, not chance or no combination.
 	- If I find no combination with the current roll, I will call a function to keep the dice/dices that are the best ones for the player to keep.
    - Once the dice/dices to keep are chosen, I will create a new YahzeeCup where I reroll the dice or don't if I already have kept the best combo.
- Finally, I return the new verion of the YahzeeCup.

All of this keeps the function immutable, use pure transformations and higher order functions.

```
	static ImmutableList<Die> KeepMostFrequentValue(YahzeeCup cup) =>
    cup.dice
        .GroupBy(d => d.Pip)
        .OrderByDescending(g => g.Count())
        .ThenByDescending(g => (int)g.Key) // prefer higher values
        .First()
        .ToImmutableList();
```
- If no combo is found, this function is called to decide how many dice/dices to keep.
- I take the dice and transform them into a group order. This keeps all the dice in a neat pile with themselves.
- The next thing I do is I sort by the most frequent result. So if I have rolled three fours, I put the fours on the top of the list.
	- ThenByDescending is to make sure I keep the highest value if I have a tie breaker.
- Lastly, I take the first group (the one now ontop) and put it in an immutable list.

All this makes for a functional way to keep dice instead of using for or while loops.

```
	static ScoreCard ApplyYahtzeeBonus(ScoreCard card, YahzeeCup combo)
    {
        if (combo is YahtzeeCombo &&
            card.Boxes.TryGetValue("Yahtzee", out var score) &&
            score == 50)
        {
            return card with { YahtzeeBonus = card.YahtzeeBonus + 100 };
        }

        return card;
    }
```
- Here I am checking if the combo I have is a YatzheeCombo (as in, all dice is the same number) AND if I already have the Yatzhee bonus score of 50.
	- If this is true, I add an extra 100 to the YahtzeeBonus on the score card for the player using the "with" syntax to create a new scorecard instead of mutating the existing one.
