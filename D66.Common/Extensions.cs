using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common
{
	public static partial class Extensions
	{

		/// <summary>
		/// For some reason, StringBuilder.AppendLine() does not work like TextWriter.WriteLine(), which is pretty annoying.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public static void AppendLine(this StringBuilder builder, string format, params object[] args)
		{
			builder.AppendFormat(format, args);
			builder.AppendLine();
		}


		public static string CreateNiceExceptionMessage(this Exception exception)
		{
			var builder = new StringBuilder();
			builder.AppendLine("EXCEPTION TYPE: " + exception.GetType().FullName);
			builder.AppendLine(exception.Message);
			builder.AppendLine();
			builder.AppendLine("STACK TRACE");
			builder.AppendLine(exception.StackTrace);
			if (exception.InnerException != null)
			{
				builder.AppendLine();
				builder.AppendLine();
				builder.Append("INNER ");
				builder.Append(CreateNiceExceptionMessage(exception.InnerException));
			}
			return builder.ToString();
		}

		public static void WriteLine(this StringBuilder builder, string format, params object[] args)
		{
			builder.AppendFormat(format, args);
			builder.AppendLine();
		}

		public static V TryGet<K, V>(this IDictionary<K, V> dict, K key)
		{
			V result;
			if(dict.TryGetValue(key, out result))
			{
				return result;
			}
			return default(V);
		}

		public static T TryGet<T>(this T[] array, int index)
		{
			if (index < 0 || index >= array.Length)
			{
				return default(T);
			}
			return array[index];
		}

		public static string ToShortRelativeTime(this DateTime? date)
		{
			if (date.HasValue)
			{
				return ToShortRelativeTime(date.Value);
			}
			return "-";
		}

		public static string ToShortRelativeTime(this DateTime date)
		{
			if (date.Date == DateTime.Now.Date)
			{
				return date.TimeOfDay.ToString("h\\:mm");
			}
			if (date.Year == DateTime.Now.Year)
			{
				return date.Date.ToString("MMM d");
			}
			return date.Date.ToShortDateString();

		}

		/// <summary>
		/// Gets the specified value from the IDictionary. Throws a KeyNotFoundException if the specified key 
		/// is not found which mentions the key that was not found (the indexer for IDictionary&lt;K, V&gt; 
		/// does not do this).
		/// </summary>
		/// <remarks>Only use this method if key.ToString() will not throw an exception</remarks>
		/// <typeparam name="K"></typeparam>
		/// <typeparam name="V"></typeparam>
		/// <param name="dict"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static V Get<K, V>(this IDictionary<K, V> dict, K key)
		{
			V result;
			if (dict.TryGetValue(key, out result))
			{
				return result;
			}
			throw new KeyNotFoundException("Key not found: " + key);
		}

		
	}
}
