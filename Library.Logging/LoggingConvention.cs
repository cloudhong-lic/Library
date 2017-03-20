using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Library.Logging
{
	public static class LoggingConvention
	{
		/// <summary>
		///     Put the reins on how much information is logged via logger. If you are logging more levels of object hierarchy than
		///     this then you should be thinking about what you are trying to achieve with your logging!
		///     Also, makes sure we don't end up in h.e.l.l from circular references (where child object has an explicit reference
		///     to its parent object).
		/// </summary>
		public const int MaxDepth = 3;

		/// <summary>
		///     Takes a dictionary and converts the values to the logging format.
		/// </summary>
		/// <param name="message">The message that will be formatted with the </param>
		/// <param name="pairs">A dictionary with property-value pairs.</param>
		/// <param name="lowerKey">Should the key string be lower cased.</param>
		/// <returns>
		///     e.g.
		///     ForLogging("Bob", new {FullName = "Jim", IsCow = false});
		///     Bob fullname="Jim", iscow="false"
		/// </returns>
		/// <see>VTGWEB-577</see>
		public static string LogDictionary(string message, IDictionary<string, object> pairs, bool lowerKey = true)
		{
			if (pairs == null || !pairs.Any()) return message;
			return string.Format("{0} {1}", message, LogDictionary(pairs, lowerKey));
		}

		/// <summary>
		///     Takes an anonymous object and converts the properties to the logging format.
		/// </summary>
		/// <param name="pairs">An object with property-value pairs.</param>
		/// <param name="lowerKey">Should the key string be lower cased.</param>
		/// <returns>
		///     e.g.
		///     ForLogging(new {FullName = "Jim", IsCow = false});
		///     fullname="Jim", iscow="false"
		/// </returns>
		/// <see>VTGWEB-577</see>
		public static string LogDictionary(IDictionary<string, object> pairs, bool lowerKey = true)
		{
			if (pairs == null)
				return string.Empty;

			var strings = new List<string>();

			foreach (var item in pairs)
			{
				var key = item.Key;
				if (lowerKey)
					key = key.ToLower();

				var value = SplunkLogFormatter.FormatValue(item.Value);
				var pair = string.Format("{0}={1}", key, value);
				strings.Add(pair);
			}

			return string.Join(", ", strings);
		}

		/// <summary>
		///     Takes an anonymous object and converts the properties to the logging format.
		/// </summary>
		/// <param name="message">The message that will be formatted with the </param>
		/// <param name="pairs">An object with property-value pairs.</param>
		/// <param name="lowerKey">Should the key string be lower cased.</param>
		/// <param name="depth">
		///     The object traversal depth for this call, since this is a recursive function. Ensures we only log up to MaxDepth
		///     levels before stopping reflection goodness.
		/// </param>
		/// <returns>
		///     e.g.
		///     ForLogging("Bob", new {FullName = "Jim", IsCow = false});
		///     Bob fullname="Jim", iscow="false"
		/// </returns>
		/// <see>VTGWEB-577</see>
		public static string ForLogging(string message, object pairs, bool lowerKey = true, int depth = 0)
		{
			if (pairs == null) return message;
			return string.Format("{0} {1}", message, ForLogging(pairs, lowerKey, depth));
		}

		/// <summary>
		///     Takes an anonymous object and converts the properties to the logging format.
		/// </summary>
		/// <param name="pairs">An object with property-value pairs.</param>
		/// <param name="lowerKey">Should the key string be lower cased.</param>
		/// <param name="depth">
		///     The object traversal depth for this call, since this is a recursive function. Ensures we only log up to MaxDepth
		///     levels before stopping reflection goodness.
		/// </param>
		/// <returns>
		///     e.g.
		///     ForLogging(new {FullName = "Jim", IsCow = false});
		///     fullname="Jim", iscow="false"
		/// </returns>
		/// <see>VTGWEB-577</see>
		public static string ForLogging(object pairs, bool lowerKey = true, int depth = 0)
		{
			// bail if we have reached the max recursion depth
			if (depth >= MaxDepth)
				return
					"log_error=The object being logged contains too much complexity. Please rethink the purpose of this log message and simplify what is being logged.";
			if (pairs == null)
				return string.Empty;
			if (pairs is string)
				return (string) pairs;
			if (IsPrimitive(pairs))
				return SplunkLogFormatter.FormatValue(pairs).ToString();

			var strings = new List<string>();

			var type = pairs.GetType();

			foreach (var property in type.GetProperties())
			{
				var key = property.Name;

				if (lowerKey)
					key = key.ToLower();

				var value = ExtractValue(pairs, property, lowerKey, depth);
				var pair = string.Format("{0}={1}", key, value);

				strings.Add(pair);
			}

			return string.Join(", ", strings);
		}

		private static object ExtractValue(object pairs, PropertyInfo property, bool lowerKey, int depth)
		{
			var value = property.GetValue(pairs, null);
			if (value == null || IsPrimitive(value))
				return SplunkLogFormatter.FormatValue(value);

			if (value is IEnumerable)
				return ExtractArray((IEnumerable) value, lowerKey, depth + 1);

			var buf = new StringBuilder("{");
			return buf.Append(ForLogging(value, lowerKey, depth + 1)).Append('}').ToString();
		}

		private static bool IsPrimitive(object obj)
		{
			return obj != null &&
			       (obj is string || obj is decimal || obj.GetType().IsPrimitive || obj.GetType().IsEnum || obj is DateTime ||
			        obj is DateTimeOffset || obj is Guid);
		}

		private static object ExtractArray(IEnumerable collection, bool lowerKey, int depth)
		{
			var buf = new StringBuilder("[");
			var isFirst = true;
			foreach (var item in collection)
			{
				if (!isFirst)
					buf.Append(',');
				else
					isFirst = false;
				if (!IsPrimitive(item) && item.GetType().GetProperties().Any())
					buf.Append('{').Append(ForLogging(item, lowerKey, depth + 1)).Append('}');
				else
					buf.Append(ForLogging(item, lowerKey));
			}
			return buf.Append(']').ToString();
		}
	}
}