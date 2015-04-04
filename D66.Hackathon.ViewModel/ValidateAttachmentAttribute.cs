using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace D66.Hackathon.ViewModel
{
	/// <remarks>
	/// All code in this file is owned by Fokke Consult. Fokke Consult has
	/// granted Clocktimizer B.V. an irrevocable, non-transferable, non-exclusive, 
	/// perpetual, worldwide, free right to use (including, but not limited to, 
	/// the right to compile, modify, and run) the Non Platform Specific Code for 
	/// both commercial and non-commercial purposes
	/// 
	/// This license is governed by the SOURCE CODE LICENSE AGREEMENT between
	/// Fokke Consult and Clocktimizer B.V.
	/// dated September 5, 2014.
	/// 
	/// See License.txt in the Solution Files folder for further details.
	/// </remarks>
	public class ValidateAttachmentAttribute : ValidationAttribute
	{

		/// <summary>
		/// Maximum content size in bytes
		/// </summary>
		public int MaxSize { get; set; }

		/// <summary>
		/// Extensions, separated by comma's
		/// </summary>
		public string Extensions { get; set; }

		public override bool IsValid(object value)
		{
			var file = value as HttpPostedFileBase;
			if (file == null)
			{
				ErrorMessage = "Invalid type for attachment";
				return false;
			}
			if (string.IsNullOrEmpty(file.FileName) || string.IsNullOrWhiteSpace(file.FileName))
			{
				ErrorMessage = "No file specified";
				return false;
			}
			if (MaxSize > 0)
			{
				if (file.ContentLength > MaxSize)
				{
					ErrorMessage = "File too large";
					return false;
				}
			}

			if (!string.IsNullOrEmpty(Extensions))
			{
				var extension = "." + file.FileName.Split('.').Last().ToLower();

				var options =
					Extensions
						.Split(new[] { ',', ';' })
						.Select(e => e.Trim().ToLower())
						.Where(e => !string.IsNullOrEmpty(e));
				if (!options.Any(e => extension.EndsWith(e)))
				{
					ErrorMessage = "File does not have of one of the supported extensions: " + Extensions;
					return false;
				}
			}
			return true;
		}
	}
}
