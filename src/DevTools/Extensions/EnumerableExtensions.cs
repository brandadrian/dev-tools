namespace DevTools.Extensions;

/// <summary>
/// Extension methods for IEnumerable collections.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Partitions a collection into two lists based on a predicate.
    /// Single-pass operation with no pre-allocation.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection</typeparam>
    /// <param name="source">The source collection to partition</param>
    /// <param name="predicate">The condition to partition by</param>
    /// <returns>A tuple of (trueList, falseList) containing items matching and not matching the predicate</returns>
    public static (List<T>, List<T>) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var trueList = new List<T>();
        var falseList = new List<T>();

        foreach (var item in source)
        {
            (predicate(item) ? trueList : falseList).Add(item);
        }

        return (trueList, falseList);
    }
}
