using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.IO
{
	public abstract class TableBase
	{

		/// <summary>
		/// Header
		/// </summary>
		public Line Header { get; set; }




		public class Line
		{
			public Line(TableBase sv, string[] values, string original)
			{
				this.values = values.ToList();
				this.sv = sv;
				this.Original = original;
			}

			public int Count
			{
				get
				{
					return values.Count;
				}
			}

			public void SetValue(int index, string value)
			{
				values[index] = value;
			}

			public string GetValue(int index)
			{
				return values[index];
			}

			public string GetValue(string varName, Line header)
			{
				for (int i = 0; i < header.Count; i++)
				{
					if (header.values[i] == varName)
					{
						if (i >= values.Count)
						{
							return null;
						}
						return values[i];
					}
				}
				return null;
			}

			public string GetValue(string varName)
			{
				if (sv.Header == null)
				{
					throw new InvalidOperationException("Can't get variables by name unless ReadHeader() is called first on the reader");
				}
				return GetValue(varName, sv.Header);
			}


			public int GetInt32OrDefault(string key, int defaultValue = 0)
			{
				var str = GetValue(key);
				int result;
				if(int.TryParse(str, out result))
				{
					return result;
				}
				return defaultValue;
			}

			public void SetValue(string varName, string value, Line header)
			{
				for (int i = 0; i < header.Count; i++)
				{
					if (header.values[i] == varName)
					{
						if (i < values.Count)
						{
							values[i] = value;
							return;
						}
					}
				}
			}

			public void SetValue(string varName, string value)
			{
				if (sv.Header == null)
				{
					throw new InvalidOperationException("Can't get variables by name unless ReadHeader() is called first on the reader");
				}
				SetValue(varName, value, sv.Header);
			}

			public string this[int index]
			{
				get
				{
					return GetValue(index);
				}
			}

			public string this[string name]
			{
				get
				{
					return GetValue(name);
				}
			}

			public override string ToString()
			{
				return Original;
			}

			private readonly List<string> values;
			private readonly TableBase sv;

			/// <summary>
			/// Original
			/// </summary>
			public string Original
			{
				get { return _Original; }
				private set { _Original = value; }
			}
			private string _Original;

			public void InsertValue(string value, int index)
			{
				values.Insert(index, value);
			}

			public bool ContainsValue(string value)
			{
				return IndexOf(value) != -1;
			}

			public int IndexOf(string value)
			{
				for (var i = 0; i < values.Count; i++)
				{
					if (values[i] == value)
					{
						return i;
					}
				}
				return -1;
			}
		}

	}

	public interface ITableReader : IDisposable
	{

		TableBase.Line Header { get; }

		TableBase.Line ReadHeader();

		TableBase.Line ReadLine();

		IEnumerable<TableBase.Line> ReadLines();

	}
}
