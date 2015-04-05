using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace D66.Common.IO
{
	public class TempFile : IDisposable
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="extension">for instance: txt</param>
		public TempFile(string extension)
		{
			this.Name = Path.Combine(Path.GetTempPath(), string.Format("{0}.{1}", Guid.NewGuid().ToString(), extension));
		}

		public string Name { get; private set; }

		public void Dispose()
		{
			if(File.Exists(Name))
			{
				File.Delete(Name);
			}
		}
	}
}
