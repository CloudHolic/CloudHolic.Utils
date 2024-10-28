using System.Diagnostics;

namespace CloudHolic.Utils;

public class DictionaryDebugView<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
{
    private readonly IDictionary<TKey, TValue> _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<TKey, TValue>[] Items
    {
        get
        {
            var array = new KeyValuePair<TKey, TValue>[_dictionary.Count];
            _dictionary.CopyTo(array, 0);
            return array;
        }
    }
}
