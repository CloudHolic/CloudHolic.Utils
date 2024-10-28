using System.Collections.ObjectModel;

namespace CloudHolic.Utils.Extensions;

public static class ObservableCollectionExtensions
{
    public static bool AddIfNotExists<T>(this ObservableCollection<T> collection, T item)
    {
        if (collection.Contains(item))
            return false;

        collection.Add(item);
        return true;
    }

    public static bool RemoveIfNotExists<T>(this ObservableCollection<T> collection, T item) =>
        collection.Contains(item) && collection.Remove(item);

    public static int RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
    {
        var itemsToRemove = collection.Where(condition).ToList();

        foreach(var item in itemsToRemove)
            collection.Remove(item);

        return itemsToRemove.Count;
    }
}
