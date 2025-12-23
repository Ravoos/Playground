using Models.Music;
using Newtonsoft.Json;

namespace PlayGround.Generics;

// Maybe is a Monad because it provides:
// 1. A type constructor: Maybe<T> that wraps values in Something, Nothing, or Error
// 2. A return/unit operation: creates Maybe from a value (typically Something)
// 3. A bind operation: chains computations that might fail, propagating Nothing/Error
//    through the chain while applying operations only to Something (success) values
// This enables safe null handling and error propagation without null reference exceptions
//Error Handling with Generics
public abstract class Maybe<T>
{
}

public class Something<T> : Maybe<T>
{
    public Something(T value)
    {
        this.Value = value;
    }

    public T Value { get; init; }
}

public class Nothing<T> : Maybe<T>
{

}

public class Error<T> : Maybe<T>
{
    public Error(Exception e)
    {
        this.CapturedError = e;
    }

    public Exception CapturedError { get; init; }
}