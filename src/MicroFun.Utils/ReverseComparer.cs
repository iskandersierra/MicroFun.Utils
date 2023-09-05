namespace MicroFun.Utils;

public class ReverseComparer<T> : IComparer<T>
{
    private readonly IComparer<T> comparer;

    public ReverseComparer(IComparer<T> comparer)
    {
        this.comparer = comparer;
    }

    public int Compare(T? x, T? y)
    {
        return comparer.Compare(y, x);
    }
}

public static class ReverseComparerExtensions
{
    public static ReverseComparer<T> Reverse<T>(this IComparer<T> comparer) => new(comparer);
}
