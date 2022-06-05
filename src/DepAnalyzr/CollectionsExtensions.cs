namespace DepAnalyzr;

internal static class CollectionsExtensions
{
    public static void Each<T>(this IEnumerable<T> @this, Action<T> act)
    {
        foreach (var item in @this) act?.Invoke(item);
    }
}