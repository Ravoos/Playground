using Models.Music;
using Newtonsoft.Json;

namespace PlayGround.Extensions;

public static partial class FunctionalExtensions
{
    public static TOut Map<TIn, TOut>(this TIn @this, Func<TIn, TOut> f) =>
        f(@this);

    public static T Map<T>(this T @this, params Func<T,T>[] transformations) =>
        transformations.Aggregate(@this, (agg, x) => x(agg));
}