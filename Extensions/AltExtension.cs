using Models.Music;
using Newtonsoft.Json;

namespace PlayGround.Extensions;

public static partial class FunctionalExtensions
{
    public static TOut Alt<TIn, TOut>(this TIn @this,
        params Func<TIn, TOut>[] args) =>
        args.Select(x => x(@this))
        .First(x => x != null);
}