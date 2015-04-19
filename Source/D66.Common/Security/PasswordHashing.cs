using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace D66.Common.Security
{
	public static class PasswordHashing
	{

		public static void CreatePasswordHash(string password, out string passwordHash, out string salt)
		{
			var saltBytes = new byte[8];
			using (var rngCsp = new RNGCryptoServiceProvider())
			{
				// Fill the array with a random value.
				rngCsp.GetBytes(saltBytes);
			}
			salt = Convert.ToBase64String(saltBytes);
			passwordHash = GetPasswordHash(password, salt);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="password">The password, in plaintext</param>
		/// <param name="salt">The salt, BASE64 encoded</param>
		/// <returns>The password hash, BASE64 encoded</returns>
		public static string GetPasswordHash(string password, string salt)
		{
			var passwordBytes = Encoding.UTF8.GetBytes(password);
			var hashFunction = new Rfc2898DeriveBytes(passwordBytes, Convert.FromBase64String(salt), hashIterations);
			var key = hashFunction.GetBytes(hashIterations);
			return Convert.ToBase64String(key);
		}

		private const int hashIterations = 1001;

	}
}
