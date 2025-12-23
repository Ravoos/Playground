using Models.Music;
using Newtonsoft.Json;

namespace PlayGround.Extensions;

public static partial class FunctionalExtensions
{
    public static T Tap<T>(this T @this, Action<T> action)
    {
        action(@this);
        return @this;
    }
}