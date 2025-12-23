using Models.Music;
using Newtonsoft.Json;

namespace PlayGround.Extensions;

public static partial class FunctionalExtensions
{
    public static Func<TIn, NewTOut> Compose<TIn, OldTOut, NewTOut>(
        this Func<TIn, OldTOut> @this,
        Func<OldTOut, NewTOut> f) =>
            x => f(@this(x));
}