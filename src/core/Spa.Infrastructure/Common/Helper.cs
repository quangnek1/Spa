using System.Globalization;

namespace Spa.Infrastructure.Common
{
	public static class Helper
	{
		public static int? ToNullableInt(object value)
		{
			var valueStr = value?.ToString()?.Trim();
			if (string.IsNullOrEmpty(valueStr)) return null;

			if (int.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
				return result;

			return null;
		}

		public static double? ToNullableDouble(object value)
		{
			var valueStr = value?.ToString()?.Trim();
			if (string.IsNullOrEmpty(valueStr)) return null;

			if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
				return result;

			return null;
		}

		public static DateTime? ToNullableDate(object value)
		{
			var valueStr = value?.ToString()?.Trim();
			if (string.IsNullOrEmpty(valueStr)) return null;

			// Excel lưu ngày tháng dạng OADate (double)
			if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double oaDate))
			{
				try
				{
					return DateTime.FromOADate(oaDate);
				}
				catch
				{
					return null;
				}
			}

			if (DateTime.TryParse(valueStr, out DateTime dt))
				return dt;

			return null;
		}
	}
}
