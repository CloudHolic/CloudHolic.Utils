namespace CloudHolic.Utils.Extensions;

public static class ArrayExtensions
{
    public static TOutput[] Convert<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
    {
        if (array.Length < 1)
            return Array.Empty<TOutput>();

        var result = new TOutput[array.Length];
        for (var i = 0; i < array.Length; i++)
            result[i] = converter(array[i]);

        return result;
    }
}
