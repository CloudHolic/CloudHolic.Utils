using System.ComponentModel;

namespace CloudHolic.Utils.Extensions;

public static class EnumExtensions
{
    public class ValueDescription
    {
        public Enum? Value { get; set; }

        public string? Description { get; set; }
    }

    public static string GetDescription(this Enum value) =>
        value.GetType()
            .GetField(value.ToString())
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() is DescriptionAttribute description
            ? description.Description
            : value.ToString();

    public static IEnumerable<ValueDescription> GetValuesAndDescriptions(Type? type, Func<Enum, bool>? filter)
    {
        if (type is not { IsEnum: true })
            return new List<ValueDescription>();

        return Enum.GetValues(type).Cast<Enum>()
            .Where(filter ?? (_ => true))
            .Select(x => new ValueDescription {
                Value = x,
                Description = x.GetDescription()
            }).ToList();
    }
}
