using System;

namespace Library.Logging
{
	public static class SplunkLogFormatter
	{
		public static object FormatValue(object value)
		{
			if (value is string)
				value = '"' + ((string) value).Replace('"', '\'') + '"';
			else if (value is DateTime)
				value = ((DateTime) value).ToString("O");
			else if (value is DateTimeOffset)
				value = ((DateTimeOffset) value).ToString("O");
			else if (value is Guid)
				value = '"' + value.ToString() + '"';
			return value;
		}
	}
}