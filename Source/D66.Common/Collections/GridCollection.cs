using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Collections
{

	/// <summary>
	/// A generic matrix that uses types for indexing
	/// </summary>
	/// <remarks>Since a dictionary is used to store column and row indices, the column and row types should offer an efficient implementation of GetHashCode()</remarks>
	/// <typeparam name="TRow"></typeparam>
	/// <typeparam name="TColumn"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[Serializable()]
	public class GridCollection<TRow, TColumn, TValue>
	{
		public GridCollection(IEnumerable<TColumn> columns, Func<TValue> initialValue = null)
		{
			if (initialValue == null)
			{
				this.initialValue = () => default(TValue);
			}
			else
			{
				this.initialValue = initialValue;
			}
			this.columns =
				columns
					.Select((c, i) => new { c, i })
					.ToDictionary(e => e.c, e => e.i);
			rows = new Dictionary<TRow, int>();
			rowData = new List<TValue[]>();

		}

		public GridCollection(IEnumerable<TRow> rows, IEnumerable<TColumn> columns, Func<TValue> initialValue = null)
		{
			if(initialValue == null)
			{
				this.initialValue = () => default(TValue);
			}
			else
			{
				this.initialValue = initialValue;
			}
			this.rows =
				rows
					.Select((r, i) => new { r, i })
					.ToDictionary(e => e.r, e => e.i);
			this.columns =
				columns
					.Select((c, i) => new { c, i })
					.ToDictionary(e => e.c, e => e.i);
			rowData = new List<TValue[]>();
			foreach(var r in this.rows)
			{
				AddNewRow();
			}
		}

		private readonly Dictionary<TRow, int> rows = new Dictionary<TRow, int>();
		private readonly Dictionary<TColumn, int> columns = new Dictionary<TColumn, int>();
		private readonly List<TValue[]> rowData;
		private readonly Func<TValue> initialValue;

		public IEnumerable<TColumn> Columns()
		{
			return columns.Keys;
		}

		public IEnumerable<TRow> Rows()
		{
			return rows.Keys;
		}

		public bool HasRow(TRow name)
		{
			int tmp;
			if (rows.TryGetValue(name, out tmp))
			{
				return true;
			}
			return false;
		}

		public void AddRow(TRow row)
		{
			rows.Add(row, rows.Count);
			AddNewRow();
		}

		private void AddNewRow()
		{
			var row = new TValue[this.columns.Count];
			for(var i=0; i<this.columns.Count; i++)
			{
				row[i] = initialValue();
			}
			rowData.Add(row);
		}

		public TValue this[TRow row, TColumn column]
		{
			get
			{
				var rowIndex = rows[row];
				var columnIndex = columns[column];
				return rowData[rowIndex][columnIndex];
			}
			set { rowData[rows[row]][columns[column]] = value; }
		}




		public IEnumerable<TValue> Row(TRow row)
		{
			var r = rows[row];
			return columns.Select((t, c) => rowData[r][c]);
		}


		public IEnumerable<TValue> Column(TColumn column)
		{
			var c = columns[column];
			return rows.Select((t, r) => rowData[r][c]);
		}
	}


	
}
