using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace D66.Common.DB
{
	public static partial class Extensions
	{

		public static string GetString(this IDataReader reader, string columnName)
		{
			return reader.GetString(reader.GetOrdinal(columnName));
		}

		public static int GetInt32(this IDataReader reader, string columnName)
		{
			return reader.GetInt32(reader.GetOrdinal(columnName));
		}

	}
}
