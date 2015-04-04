using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D66.Common.IO
{
	public class SVWriter : SVBase, IDisposable
	{

		public SVWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
		}

		private readonly TextWriter writer;


		public void Write(Line line)
		{
			for(var i=0; i<line.Count; i++)
			{
				if(i > 0)
				{
					writer.Write(Separator);
				}
				if(QuoteCharacter.HasValue)
				{
					writer.Write("{0}{1}{2}", QuoteCharacter.Value, line[i], QuoteCharacter.Value);
				}
				else
				{
					writer.Write(line[i]);
				}
			}
			writer.WriteLine();
		}


		public void Dispose()
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				writer.Dispose();
			}
		}

		private bool IsDisposed = false;

	}
}
