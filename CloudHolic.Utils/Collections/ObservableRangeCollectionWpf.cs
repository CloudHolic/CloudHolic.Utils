using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Data;

namespace CloudHolic.Utils.Collections;

public class ObservableRangeCollectionWpf<T> : ObservableRangeCollection<T>
{
    #region Private Fields

    private DeferredEventsCollection? _deferredEvents;

    #endregion

    #region Constructors

    public ObservableRangeCollectionWpf(bool allowDuplicates = true, EqualityComparer<T>? comparer = null) 
        : base(allowDuplicates, comparer)
    {

    }

    public ObservableRangeCollectionWpf(IEnumerable<T> collection, bool allowDuplicates = true, EqualityComparer<T>? comparer = null)
        : base(collection, allowDuplicates, comparer)
    {

    }

    public ObservableRangeCollectionWpf(List<T> list, bool allowDuplicates = true, EqualityComparer<T>? comparer = null)
        : base(list, allowDuplicates, comparer)
    {

    }

    #endregion

    #region Protected Methods

    protected override IDisposable DeferEvents() => new DeferredEventsCollection(this);

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (typeof(ObservableRangeCollection<T>)
                .GetField(nameof(_deferredEvents), BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(this) is ICollection<NotifyCollectionChangedEventArgs> deferredEvents)
        {
            deferredEvents.Add(e);
            return;
        }

        foreach (var handler in GetHandlers())
        {
            if (IsRange(e) && handler.Target is CollectionView cv)
                cv.Refresh();
            else
                handler(this, e);
        }
    }

    #endregion

    #region Private Methods

    private static bool IsRange(NotifyCollectionChangedEventArgs e) => e.NewItems?.Count > 1 || e.OldItems?.Count > 1;

    private IEnumerable<NotifyCollectionChangedEventHandler> GetHandlers()
    {
        var info = typeof(ObservableCollection<T>).GetField(nameof(CollectionChanged), BindingFlags.Instance | BindingFlags.NonPublic);
        var @event = info?.GetValue(this) as MulticastDelegate;

        return @event?.GetInvocationList()
                   .Cast<NotifyCollectionChangedEventHandler>()
                   .Distinct()
               ?? Enumerable.Empty<NotifyCollectionChangedEventHandler>();
    }

    #endregion

    #region Private Types

    private class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
    {
        private readonly ObservableRangeCollectionWpf<T> _collection;

        public DeferredEventsCollection(ObservableRangeCollectionWpf<T> collection)
        {
            Debug.Assert(collection is not null);
            Debug.Assert(collection._deferredEvents is null);

            _collection = collection;
            _collection._deferredEvents = this;
        }
        
        public void Dispose()
        {
            _collection._deferredEvents = null;

            var handlers = _collection.GetHandlers()
                .ToLookup(x => x.Target is CollectionView);

            foreach(var handler in handlers[false])
            foreach (var e in this)
                handler(_collection, e);

            foreach(var cv in handlers[true].Select(x => x.Target).Cast<CollectionView>().Distinct())
                cv.Refresh();
        }
    }

    #endregion
}
