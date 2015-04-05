using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common
{
	/// <summary>
	/// Allows mocking DateTime.Now
	/// </summary>
	public class NowService : INowService
	{

		public NowService()
		{
			Offset = TimeSpan.Zero;
		}

		public DateTime UtcNow
		{
			get { return DateTime.UtcNow.Add(Offset); }
		}
		public DateTime Now
		{
			get { return DateTime.Now.Add(Offset); }
		}

		/// <summary>
		/// Allows offsetting for testing purposes
		/// </summary>
		public TimeSpan Offset { get; set; }

	}

	public interface INowService
	{
		DateTime UtcNow { get; }
		DateTime Now { get; }

	}
}
