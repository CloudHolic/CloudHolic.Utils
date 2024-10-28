namespace CloudHolic.Utils.Extensions;

public static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T>? source, Action<T> action)
    {
        if (source == null)
            return;

        foreach (var obj in source)
            action(obj);
    }
}
