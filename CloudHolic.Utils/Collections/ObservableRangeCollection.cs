using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace CloudHolic.Utils.Collections;

public class ObservableRangeCollection<T> : ObservableCollection<T>
{
    #region Private Fields

    [NonSerialized]
    private DeferredEventsCollection? _deferredEvents;

    #endregion

    #region Public Properties

    public bool AllowDuplicates { get; } = true;

    public EqualityComparer<T> Comparer { get; }

    #endregion

    #region Constructors

    public ObservableRangeCollection(bool allowDuplicates = true, EqualityComparer<T>? comparer = null)
    {
        AllowDuplicates = allowDuplicates;
        Comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public ObservableRangeCollection(IEnumerable<T> collection, bool allowDuplicates = true, EqualityComparer<T>? comparer = null) : base(collection)
    {
        AllowDuplicates = allowDuplicates;
        Comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public ObservableRangeCollection(List<T> list, bool allowDuplicates = true, EqualityComparer<T>? comparer = null) : base(list)
    {
        AllowDuplicates = allowDuplicates;
        Comparer = comparer ?? EqualityComparer<T>.Default;
    }

    #endregion

    #region Public Methods

    public int AddRange(IEnumerable<T> collection) => InsertRange(Count, collection);

    public int InsertRange(int index, IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (index < 0 || index > Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (!AllowDuplicates)
            collection = collection.Distinct(Comparer).Where(x => !Items.Contains(x, Comparer));

        var list = collection.ToList();

        switch (list.Count)
        {
            case 0:
                return 0;
            case 1:
                Add(list.First());
                return 1;
        }

        CheckReentrancy();

        var items = (List<T>)Items;
        items.InsertRange(index, list);

        OnEssentialPropertiesChanged();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));

        return list.Count;
    }

    public int RemoveAll(Predicate<T> match) => RemoveAll(0, Count, match);

    public int RemoveAll(int index, int count, Predicate<T> match)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count - index < count)
            throw new ArgumentException("Offset and length were out of bounds for the array " +
                                        "or count is greater than the number of items from index " +
                                        "to the end of the source collection.");

        ArgumentNullException.ThrowIfNull(match);

        if (Count == 0)
            return 0;

        List<T>? cluster = null;
        var (clusterIndex, removedCount) = (-1, 0);

        using(BlockReentrancy())
        using (DeferEvents())
        {
            for (var i = 0; i < count; i++, index++)
            {
                var item = Items[index];

                if (match(item))
                {
                    Items.RemoveAt(index);
                    removedCount++;

                    if (clusterIndex == index)
                    {
                        Debug.Assert(cluster is not null);
                        cluster.Add(item);
                    }
                    else
                    {
                        cluster = new List<T> { item };
                        clusterIndex = index;
                    }

                    index--;
                }
                else if (clusterIndex > -1)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
                    clusterIndex = -1;
                    cluster = null;
                }
            }

            if (clusterIndex > -1)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
        }

        if (removedCount > 0)
            OnEssentialPropertiesChanged();

        return removedCount;
    }

    public int RemoveRange(IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (Count == 0)
            return 0;

        var list = collection.ToList();
        switch (list.Count)
        {
            case 0:
                return 0;
            case 1:
                var removed = Remove(list.First());
                return removed ? 1 : 0;
        }

        CheckReentrancy();

        var removedCount = 0;

        foreach (var item in list)
        {
            var removed = Items.Remove(item);
            removedCount += removed ? 1 : 0;
        }

        if (removedCount == 0)
            return 0;

        OnEssentialPropertiesChanged();

        if (Count == 0)
            OnCollectionReset();
        else
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list));

        return removedCount;
    }

    public void RemoveRange(int index, int count)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count - index < count)
            throw new ArgumentException("Offset and length were out of bounds for the array " +
                                        "or count is greater than the number of items from index " +
                                        "to the end of the source collection.");

        if (Count == 0)
            return;

        if (Count == 1)
        {
            RemoveItem(index);
            return;
        }

        if (index == 0 && count == Count)
        {
            Clear();
            return;
        }

        var items = (List<T>)Items;
        var removedItems = items.GetRange(index, count);

        CheckReentrancy();

        items.RemoveRange(index, count);

        OnEssentialPropertiesChanged();

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
    }

    public int Replace(T item) => ReplaceRange(0, Count, new[] { item });

    public int ReplaceRange(IEnumerable<T> collection) => ReplaceRange(0, Count, collection);

    public int ReplaceRange(int index, int count, IEnumerable<T> collection)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (count < 0) 
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count - index < count)
            throw new ArgumentException("Offset and length were out of bounds for the array " +
                                        "or count is greater than the number of items from index " +
                                        "to the end of the source collection.");

        ArgumentNullException.ThrowIfNull(collection);

        var list = collection.ToList();
        if (!list.Any())
        {
            RemoveRange(index, count);
            return -count;
        }

        if (!AllowDuplicates)
            list = list.Distinct(Comparer).ToList();

        if (index + count == 0)
        {
            var added = InsertRange(0, list);
            return added;
        }

        var oldCount = Count;

        using(BlockReentrancy())
        using (DeferEvents())
        {
            var (rangedCount, addedCount) = (index + count, list.Count);

            var changesMade = false;
            List<T>? newCluster = null, oldCluster = null;

            var i = index;
            for (; i < rangedCount && i - index < addedCount; i++)
            {
                var (oldItem, newItem) = (this[i], list[i - index]);

                if (Comparer.Equals(oldItem, newItem))
                    OnRangeReplaced(i, newCluster, oldCluster);
                else
                {
                    Items[i] = newItem;

                    if (newCluster is null)
                    {
                        Debug.Assert(oldCluster is null);

                        newCluster = [newItem];
                        oldCluster = [oldItem];
                    }
                    else
                    {
                        newCluster.Add(newItem);
                        oldCluster!.Add(oldItem);
                    }

                    changesMade = true;
                }
            }

            OnRangeReplaced(i, newCluster, oldCluster);

            if (count != addedCount)
            {
                var items = (List<T>)Items;

                if (count > addedCount)
                {
                    var removedCount = rangedCount - addedCount;
                    var removed = new T[removedCount];

                    items.CopyTo(i, removed, 0, removed.Length);
                    items.RemoveRange(i, removedCount);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, i));
                }
                else
                {
                    var k = i - index;
                    var added = new T[addedCount - k];

                    for (var j = k; j < addedCount; j++)
                    {
                        var newItem = list[j];
                        added[j - k] = newItem;
                    }

                    items.InsertRange(i, added);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added, i));
                }

                OnEssentialPropertiesChanged();
            }
            else if (changesMade)
                OnIndexerPropertyChanged();
        }

        return count - oldCount;

        #region Local Function

        void OnRangeReplaced(int followingItemIndex, ICollection<T>? newCluster, ICollection<T>? oldCluster)
        {
            if (oldCluster is null || oldCluster.Count == 0)
            {
                Debug.Assert(newCluster is null || newCluster.Count == 0);
                return;
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new List<T>(newCluster!), new List<T>(oldCluster), followingItemIndex - oldCluster.Count));

            oldCluster.Clear();
            newCluster?.Clear();
        }

        #endregion
    }

    #endregion

    #region Protected Methods

    protected override void ClearItems()
    {
        if (Count == 0)
            return;

        base.ClearItems();
    }

    protected virtual IDisposable DeferEvents() => new DeferredEventsCollection(this);

    protected override void InsertItem(int index, T item)
    {
        if (!AllowDuplicates && Items.Contains(item))
            return;

        base.InsertItem(index, item);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_deferredEvents is not null)
        {
            _deferredEvents.Add(e);
            return;
        }

        base.OnCollectionChanged(e);
    }

    protected override void SetItem(int index, T item)
    {
        if (AllowDuplicates)
        {
            if (Comparer.Equals(this[index], item))
                return;
        }
        else if (Items.Contains(item, Comparer))
            return;

        base.SetItem(index, item);
    }

    #endregion

    #region Private Methods

    private void OnCollectionReset() => OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

    private void OnEssentialPropertiesChanged()
    {
        OnPropertyChanged(EventArgsCache.CountPropertyChanged);
        OnIndexerPropertyChanged();
    }

    private void OnIndexerPropertyChanged() => OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

    #endregion

    #region Private Types

    private sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
    {
        private readonly ObservableRangeCollection<T> _collection;

        public DeferredEventsCollection(ObservableRangeCollection<T> collection)
        {
            Debug.Assert(collection is not null);
            Debug.Assert(collection._deferredEvents is null);

            _collection = collection;
            _collection._deferredEvents = this;
        }

        public void Dispose()
        {
            _collection._deferredEvents = null;

            foreach(var args in this)
                _collection.OnCollectionChanged(args);
        }
    }

    #endregion
}

file static class EventArgsCache
{
    public static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");

    public static readonly PropertyChangedEventArgs IndexerPropertyChanged = new("Item[]");
    
    public static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);
}