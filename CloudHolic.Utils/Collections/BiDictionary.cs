using CloudHolic.Utils.Extensions;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace CloudHolic.Utils.Collections;

[Serializable]
[DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
public class BiDictionary<TFirst, TSecond> : IDictionary<TFirst, TSecond>, IReadOnlyDictionary<TFirst, TSecond>, IDictionary where TFirst : notnull where TSecond : notnull
{
    #region Private Fields

    private readonly IDictionary<TFirst, TSecond> _firstToSecond = new Dictionary<TFirst, TSecond>();

    [NonSerialized]
    private readonly IDictionary<TSecond, TFirst> _secondToFirst = new Dictionary<TSecond, TFirst>();

    [NonSerialized]
    private readonly ReverseDictionary _reverseDictionary;

    #endregion

    #region Properties

    #region Explicit Implementations

    public IDictionary<TSecond, TFirst> Reverse => _reverseDictionary;

    public int Count => _firstToSecond.Count;

    object ICollection.SyncRoot => ((ICollection)_firstToSecond).SyncRoot;

    bool ICollection.IsSynchronized => ((ICollection)_firstToSecond).IsSynchronized;

    bool IDictionary.IsFixedSize => ((IDictionary)_firstToSecond).IsFixedSize;

    object? IDictionary.this[object? key]
    {
        get => key != null ? ((IDictionary)_firstToSecond)[key] : null;
        set
        {
            if (key == null || value == null)
                return;

            ((IDictionary)_firstToSecond)[key] = value;
            ((IDictionary)_secondToFirst)[value] = key;
        }
    }

    ICollection IDictionary.Keys => ((IDictionary)_firstToSecond).Keys;

    IEnumerable<TFirst> IReadOnlyDictionary<TFirst, TSecond>.Keys => ((IReadOnlyDictionary<TFirst, TSecond>)_firstToSecond).Keys;

    ICollection IDictionary.Values => ((IDictionary)_firstToSecond).Values;

    IEnumerable<TSecond> IReadOnlyDictionary<TFirst, TSecond>.Values => ((IReadOnlyDictionary<TFirst, TSecond>)_firstToSecond).Values;

    IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)_firstToSecond).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Public Properties

    public bool IsReadOnly => _firstToSecond.IsReadOnly || _secondToFirst.IsReadOnly;

    public TSecond this[TFirst key]
    {
        get => _firstToSecond[key];
        set
        {
            _firstToSecond[key] = value;
            _secondToFirst[value] = key;
        }
    }

    public ICollection<TFirst> Keys => _firstToSecond.Keys;

    public ICollection<TSecond> Values => _firstToSecond.Values;

    #endregion

    #endregion

    public BiDictionary() => _reverseDictionary = new ReverseDictionary(this);

    #region Methods

    #region Explicit Implementations

    void IDictionary.Add(object key, object? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        ((IDictionary)_firstToSecond).Add(key, value);
        ((IDictionary)_secondToFirst).Add(value, key);
    }

    void ICollection<KeyValuePair<TFirst, TSecond>>.Add(KeyValuePair<TFirst, TSecond> item)
    {
        _firstToSecond.Add(item);
        _secondToFirst.Add(item.Reverse());
    }

    void IDictionary.Remove(object key)
    {
        var firstToSecond = (IDictionary)_firstToSecond;
        if (!firstToSecond.Contains(key))
            return;

        var value = firstToSecond[key];
        firstToSecond.Remove(key);
        if (value != null)
            ((IDictionary)_secondToFirst).Remove(value);
    }

    bool ICollection<KeyValuePair<TFirst, TSecond>>.Remove(KeyValuePair<TFirst, TSecond> item) =>
        _firstToSecond.Remove(item);

    bool IDictionary.Contains(object key) => ((IDictionary)_firstToSecond).Contains(key);

    bool ICollection<KeyValuePair<TFirst, TSecond>>.Contains(KeyValuePair<TFirst, TSecond> item) =>
        _firstToSecond.Contains(item);

    void ICollection.CopyTo(Array array, int index) => ((IDictionary)_firstToSecond).CopyTo(array, index);

    void ICollection<KeyValuePair<TFirst, TSecond>>.CopyTo(KeyValuePair<TFirst, TSecond>[] array, int arrayIndex) =>
        _firstToSecond.CopyTo(array, arrayIndex);

    #endregion

    #region Public Methods

    public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator() => _firstToSecond.GetEnumerator();

    public void Add(TFirst key, TSecond value)
    {
        _firstToSecond.Add(key, value);
        _secondToFirst.Add(value, key);
    }

    public bool Remove(TFirst key)
    {
        if (!_firstToSecond.Remove(key, out var value))
            return false;

        _secondToFirst.Remove(value);
        return true;
    }

    public bool ContainsKey(TFirst key) => _firstToSecond.ContainsKey(key);

    public bool TryGetValue(TFirst key, out TSecond value) => _firstToSecond.TryGetValue(key, out value!);

    public void Clear()
    {
        _firstToSecond.Clear();
        _secondToFirst.Clear();
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        _secondToFirst.Clear();
        foreach (var item in _firstToSecond)
            _secondToFirst.Add(item.Value, item.Key);
    }

    #endregion

    #endregion

    #region Private Type

    private class ReverseDictionary : IDictionary<TSecond, TFirst>, IReadOnlyDictionary<TSecond, TFirst>, IDictionary
    {
        #region Private Fields

        private readonly BiDictionary<TFirst, TSecond> _owner;

        #endregion

        #region Properties

        #region Explicit Implementation

        object ICollection.SyncRoot => ((ICollection)_owner._secondToFirst).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)_owner._secondToFirst).IsSynchronized;

        bool IDictionary.IsFixedSize => ((IDictionary)_owner._secondToFirst).IsFixedSize;

        object? IDictionary.this[object key]
        {
            get => ((IDictionary)_owner._secondToFirst)[key];
            set
            {
                if (value == null)
                    return;

                ((IDictionary)_owner._secondToFirst)[key] = value;
                ((IDictionary)_owner._firstToSecond)[value] = key;
            }
        }

        ICollection IDictionary.Keys => ((IDictionary)_owner._secondToFirst).Keys;

        IEnumerable<TSecond> IReadOnlyDictionary<TSecond, TFirst>.Keys => ((IReadOnlyDictionary<TSecond, TFirst>)_owner._secondToFirst).Keys;

        ICollection IDictionary.Values => ((IDictionary)_owner._secondToFirst).Values;

        IEnumerable<TFirst> IReadOnlyDictionary<TSecond, TFirst>.Values => ((IReadOnlyDictionary<TSecond, TFirst>)_owner._secondToFirst).Values;

        #endregion

        #region Public Properties

        public int Count => _owner._secondToFirst.Count;

        public bool IsReadOnly => _owner._secondToFirst.IsReadOnly || _owner._firstToSecond.IsReadOnly;

        public TFirst this[TSecond key]
        {
            get => _owner._secondToFirst[key];
            set
            {
                _owner._secondToFirst[key] = value;
                _owner._firstToSecond[value] = key;
            }
        }

        public ICollection<TSecond> Keys => _owner._secondToFirst.Keys;

        public ICollection<TFirst> Values => _owner._secondToFirst.Values;

        #endregion

        #endregion

        public ReverseDictionary(BiDictionary<TFirst, TSecond> owner) => _owner = owner;

        #region Methods

        #region Explicit Implementations

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)_owner._secondToFirst).GetEnumerator();

        void IDictionary.Add(object key, object? value)
        {
            if (value == null)
                return;

            ((IDictionary)_owner._secondToFirst).Add(key, value);
            ((IDictionary)_owner._firstToSecond).Add(value, key);
        }

        void ICollection<KeyValuePair<TSecond, TFirst>>.Add(KeyValuePair<TSecond, TFirst> item)
        {
            _owner._secondToFirst.Add(item);
            _owner._firstToSecond.Add(item.Reverse());
        }

        void IDictionary.Remove(object key)
        {
            var firstToSecond = (IDictionary)_owner._secondToFirst;
            if (!firstToSecond.Contains(key))
                return;

            var value = firstToSecond[key];
            firstToSecond.Remove(key);
            if (value != null)
                ((IDictionary)_owner._firstToSecond).Remove(value);
        }

        bool ICollection<KeyValuePair<TSecond, TFirst>>.Remove(KeyValuePair<TSecond, TFirst> item) => _owner._secondToFirst.Remove(item);

        bool IDictionary.Contains(object key) => ((IDictionary)_owner._secondToFirst).Contains(key);

        bool ICollection<KeyValuePair<TSecond, TFirst>>.Contains(KeyValuePair<TSecond, TFirst> item) => _owner._secondToFirst.Contains(item);

        void ICollection<KeyValuePair<TSecond, TFirst>>.CopyTo(KeyValuePair<TSecond, TFirst>[] array, int arrayIndex) => _owner._secondToFirst.CopyTo(array, arrayIndex);

        void ICollection.CopyTo(Array array, int index) => ((IDictionary)_owner._secondToFirst).CopyTo(array, index);

        #endregion

        #region Public Methods

        public IEnumerator<KeyValuePair<TSecond, TFirst>> GetEnumerator() => _owner._secondToFirst.GetEnumerator();

        public void Add(TSecond key, TFirst value)
        {
            _owner._secondToFirst.Add(key, value);
            _owner._firstToSecond.Add(value, key);
        }

        public bool Remove(TSecond key)
        {
            if (!_owner._secondToFirst.Remove(key, out var value))
                return false;

            _owner._firstToSecond.Remove(value);
            return true;
        }

        public bool ContainsKey(TSecond key) => _owner._secondToFirst.ContainsKey(key);

        public bool TryGetValue(TSecond key, out TFirst value) => _owner._secondToFirst.TryGetValue(key, out value!);

        public void Clear()
        {
            _owner._secondToFirst.Clear();
            _owner._firstToSecond.Clear();
        }

        #endregion

        #endregion
    }

    #endregion
}
