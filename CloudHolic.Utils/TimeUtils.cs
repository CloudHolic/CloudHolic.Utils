namespace CloudHolic.Utils;

public static class TimeUtils
{
    public static long GetTimestamp() =>
        (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
    {
        var diff = (7 + dateTime.DayOfWeek - startOfWeek) % 7;
        return dateTime.AddDays(-1 * diff);
    }
}
