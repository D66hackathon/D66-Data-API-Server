using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common
{

	/// <summary>
	/// Use this exception in defensive programming scenario's
	/// </summary>
	public class ThisShouldNotHappenException : ApplicationException
	{

		public ThisShouldNotHappenException(string message) : base(message)
		{
		}

	}
}
