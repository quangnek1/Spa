using System.Globalization;

namespace Spa.Infrastructure.Common;

public static class Helper
{
    public static int? ToNullableInt(object value)
    {
        var valueStr = value?.ToString()?.Trim();
        if (string.IsNullOrEmpty(valueStr)) return null;

        if (int.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return null;
    }

    public static double? ToNullableDouble(object value)
    {
        var valueStr = value?.ToString()?.Trim();
        if (string.IsNullOrEmpty(valueStr)) return null;

        if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return null;
    }

    public static DateTime? ToNullableDate(object value)
    {
        var valueStr = value?.ToString()?.Trim();
        if (string.IsNullOrEmpty(valueStr)) return null;

        // Excel lưu ngày tháng dạng OADate (double)
        if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var oaDate))
            try
            {
                return DateTime.FromOADate(oaDate);
            }
            catch
            {
                return null;
            }

        if (DateTime.TryParse(valueStr, out var dt))
            return dt;

        return null;
    }
}