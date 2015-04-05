using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace D66.Common.IO
{
	/// <summary>
	/// Allows reading of separated files
	/// </summary>
	public class SVReader : SVBase, ITableReader, IDisposable
	{

		public SVReader(TextReader reader)
		{
			if(reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.reader = reader;
		}

		private readonly TextReader reader;


		public static List<Line> ReadTSVFromAssembly(Assembly assembly, string path)
		{
			using (var sv = new SVReader(new StreamReader(assembly.GetManifestResourceStream(path))) { Separator = '\t' })
			{
				sv.ReadHeader();
				return sv.ReadLines().ToList();
			}
		} 

		/// <summary>
		/// AutoQuote
		/// </summary>
		public bool AutoQuote
		{
			get { return _AutoQuote; }
			set { _AutoQuote = value; }
		}

		private bool _AutoQuote;


		/// <summary>
		/// Reads a line and saves it as a header
		/// </summary>
		/// <returns></returns>
		public Line ReadHeader()
		{
			return (Header = ReadLine());
		}

		private bool firstCharProcessed = false;

		/// <summary>
		/// Reads a single line
		/// </summary>
		/// <returns></returns>
		public Line ReadLine()
		{
			if (reader.Peek() == -1)
			{
				return null;
			}
			List<char> original = new List<char>();
			List<char> chars = new List<char>();
			List<string> fields = new List<string>();
			bool inQuotes = false;
			bool escape = false;
			while (reader.Peek() != -1)
			{
				int i = reader.Read();
				// Handle UTF-8 Byte order mark correctly
				if (!firstCharProcessed)
				{
					firstCharProcessed = true;
					if (i == 65279)
					{
						if (reader.Peek() == -1)
						{
							return null;
						}
						i = reader.Read();
					}
				}
				char c = (char)i;
				original.Add(c);
				if (escape)
				{
					chars.Add(c);
					escape = false;
				}
				else
				{
					if (EscapeCharacter.Equals(c))
					{
						escape = true;
					}
					else
					{
						if (QuoteCharacter.Equals(c))
						{
							inQuotes = !inQuotes;
						}
						else
						{
							if (inQuotes)
							{
								chars.Add(c);
							}
							else
							{
								if (c == Separator)
								{
									fields.Add(new string(chars.ToArray()));
									chars = new List<char>();
								}
								else if (c == '\r')
								{
									if (reader.Peek() != -1 && (char)reader.Peek() == '\n')
									{
										reader.Read(); // skip \n
									}
									break;
								}
								else if (c == '\n')
								{
									break;
								}
								else
								{
									chars.Add(c);
								}
							}
						}
					}
				}
			}
			fields.Add(new string(chars.ToArray()));
			return new Line(
				this,
				fields.ToArray(), new string(original.ToArray()));
		}



		/// <summary>
		/// Reads all remaining lines
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Line> ReadLines()
		{
			while (reader.Peek() != -1)
			{
				var line = ReadLine();
				if (line != null)
				{
					yield return line;
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				reader.Dispose();
			}
		}

		private bool IsDisposed = false;

		#endregion

		#region Serialization

		public IEnumerable<T> Deserialize<T>() where T : new()
		{
			var type = typeof(T);
			ReadHeader();
			PropertyInfo[] props = new PropertyInfo[Header.Count];
			for (int i = 0; i < Header.Count; i++)
			{

				props[i] = type.GetProperty(Header[i]);
			}
			foreach (var line in ReadLines())
			{
				T result = new T();
				for (int i = 0; i < Header.Count; i++)
				{
					if (i < line.Count)
					{
						props[i].SetValue(result, line[i], null);
					}
				}
				yield return result;
			}
		}


		#endregion

	}

	public abstract class SVBase : TableBase
	{

		/// <summary>
		/// EscapeCharacter. Default is '\' (backslash)
		/// </summary>
		public char? EscapeCharacter
		{
			get { return _EscapeCharacter; }
			set { _EscapeCharacter = value; }
		}
		private char? _EscapeCharacter = '\\';

		/// <summary>
		/// QuoteCharacter. Default is '"' (double quotes)
		/// </summary>
		public char? QuoteCharacter
		{
			get { return _QuoteCharacter; }
			set { _QuoteCharacter = value; }
		}
		private char? _QuoteCharacter = '\"';





		/// <summary>
		/// Separator. Must be set. Default is ','
		/// </summary>
		public char Separator
		{
			get { return _Separator; }
			set { _Separator = value; }
		}
		private char _Separator = ',';

		
		public string FormatLine(Line line)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < line.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(Separator);
				}
				if (QuoteCharacter.HasValue)
				{
					builder.Append(QuoteCharacter.Value);
				}
				builder.Append(line.GetValue(i));
				if (QuoteCharacter.HasValue)
				{
					builder.Append(QuoteCharacter.Value);
				}
			}
			return builder.ToString();
		}

	}
}
