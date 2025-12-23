using System.Collections.Immutable;
using Seido.Utilities.SeedGenerator;

namespace Models.Friends;

public enum AnimalKind { Dog, Cat, Rabbit, Fish, Bird }
public enum AnimalMood { Happy, Hungry, Lazy, Sulky, Buzy, Sleepy }

public record Pet(
    Guid PetId,
    AnimalKind Kind,
    AnimalMood Mood,
    string Name,
    bool Seeded = false
) : ISeed<Pet>
{
    public Pet() : this(default, default, default, default) { }

    #region randomly seed this instance
    public virtual Pet Seed(SeedGenerator seedGenerator)
    {
        var ret = new Pet(
            Guid.NewGuid(),
            seedGenerator.FromEnum<AnimalKind>(),
            seedGenerator.FromEnum<AnimalMood>(),
            seedGenerator.PetName,
            true
        );
        return ret;
    }
    #endregion
}


