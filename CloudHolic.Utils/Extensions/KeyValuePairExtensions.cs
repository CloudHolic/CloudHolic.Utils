namespace CloudHolic.Utils.Extensions;

public static class KeyValuePairExtensions
{
    public static KeyValuePair<TValue, TKey> Reverse<TKey, TValue>(this KeyValuePair<TKey, TValue> pair) =>
        new KeyValuePair<TValue, TKey>(pair.Value, pair.Key);
}
