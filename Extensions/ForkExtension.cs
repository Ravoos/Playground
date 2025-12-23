using Models.Music;
using Newtonsoft.Json;

namespace PlayGround.Extensions;

public static partial class FunctionalExtensions
{
    public static TOut Fork<TIn, T1, T2, TOut>(this TIn @this,
      Func<TIn, T1> f1,
      Func<TIn, T2> f2,
      Func<T1,T2,TOut> fout)
    {
        var p1 = f1(@this);
        var p2 = f2(@this);
        var result = fout(p1, p2);
        return result;
    }
}