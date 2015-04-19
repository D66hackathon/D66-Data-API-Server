using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace D66.Common.Security
{
	/// <summary>
	/// Generates unpredictable tokens
	/// </summary>
	public interface ITokenGenerator
	{

		/// <summary>
		/// Generates a base-64 encoded
		/// </summary>
		/// <param name="byteCount">length in bytes.</param>
		/// <returns></returns>
		byte[] Generate(int byteCount);

	}

	public class TokenGenerator : ITokenGenerator
	{
		private readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

		public byte[] Generate(int byteCount)
		{
			var buffer = new byte[byteCount];
			rngCsp.GetBytes(buffer);
			return buffer;
		}
	}
}
