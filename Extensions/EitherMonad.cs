using Models.Music;
using Newtonsoft.Json;

namespace PlayGround.Generics;

// Either is a Monad because it provides:
// 1. A type constructor: Either<T1, T2> that wraps values
// 2. A return/unit operation: creates Either from a value (typically Right)
// 3. A bind operation: chains computations that can fail, propagating Left (errors) 
//    through the chain while applying operations only to Right (success) values
// This enables composable error handling without exceptions or nested if-statements
public abstract class Either<T1, T2>
{

}
public class Left<T1, T2> : Either<T1, T2>
{
    public Left(T1 value)
    {
        Value = value;
    }

    public T1 Value { get; init; }
}

public class Right<T1, T2> : Either<T1, T2>
{
    public Right(T2 value)
    {
        Value = value;
    }

    public T2 Value { get; init; }
}