using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using D66.Common.Environment;

namespace D66.Common.Pdf
{
	public class PdfCreator
	{

		public PdfCreator()
		{
			Cookies = new Dictionary<string, string>();
			UseStandardOutput = true;
		}

		public bool UseStandardOutput { get; set; }

		public string WkHtmlToPdfPath { get; set; }

		public string Url { get; set; }

		public TimeSpan	Timeout { get; set; }

		public Dictionary<string, string> Cookies { get; set; }

		/// <summary>
		/// Top margin (mm)
		/// </summary>
		public int MarginTop { get; set; }

		/// <summary>
		/// Left margin (mm)
		/// </summary>
		public int MarginLeft { get; set; }

		/// <summary>
		/// Bottom margin (mm)
		/// </summary>
		public int MarginBottom { get; set; }

		/// <summary>
		/// Right margin (mm)
		/// </summary>
		public int MarginRight { get; set; }

		public string HeaderUri { get; set; }

		/// <summary>
		/// Header size (mm)
		/// </summary>
		public int HeaderSize { get; set; }

		public string FooterUri { get; set; }

		public int FooterSize { get; set; }

		public byte[] Create()
		{
			var tempFileName = Path.Combine(Path.GetTempPath(), string.Format("{0}.pdf", Guid.NewGuid()));
			try
			{
				var runner = new ProcessRunner()
				             {
				             	Path = WkHtmlToPdfPath,
				             	Parameters = CreateParameters(tempFileName),
				             	Timeout = Timeout
				             };
				if(runner.Run())
				{
					if(runner.ExitCode == 0 || runner.ExitCode == 2)
					{
						byte[] result;
						if(UseStandardOutput)
						{
							result = runner.StandardOutputBytes;
						}
						else
						{
							result = File.ReadAllBytes(tempFileName);
						}
						return result;
					}
					else if(runner.ExitCode == 2)
					{
						throw new InvalidOperationException(string.Format("Could not find one or more of the follow URLS: {0}, Header: {1}, Footer: {2}, Standard error: {3}", Url, HeaderUri, FooterUri, runner.StandardError));
					}
					throw new ProcessRunException(string.Format("Process exited with code {0}; StandardError: {1}; Output: {2}", runner.ExitCode, runner.StandardError, runner.StandardOutput))
					      {
					      	StandardError = runner.StandardError,
							Output = runner.StandardOutput
					      };
				}
				throw new ProcessRunException("Timeout")
				      {
				      	StandardError = runner.StandardError,
				      	Output = runner.StandardOutput
				      };
			}
			finally
			{
				if (File.Exists(tempFileName))
				{
					File.Delete(tempFileName);
				}
			}
		}

		private string CreateParameters(string outputFile)
		{
			var result = new StringBuilder();

			// Disable 'smart' shrinking that fucks up the layout
			result.AppendFormat(" --disable-smart-shrinking");
			result.AppendFormat(
				" -T {0} -R {1} -L {2} -B {3}",
				HeaderSize,
				0,
				0,
				FooterSize
			);
			//result.AppendFormat(" --ignore-load-errors");

			// Header stuff
			if (!string.IsNullOrEmpty(HeaderUri))
			{
				result.AppendFormat(" --header-html \"{0}\"", HeaderUri);
			}
			if (!string.IsNullOrEmpty(FooterUri))
			{
				result.AppendFormat(" --footer-html \"{0}\"", FooterUri);
			}

			// Add cookies
			foreach (var kv in Cookies)
			{
				result.AppendFormat(" --cookie \"{0}\" \"{1}\"", kv.Key, kv.Value);
			}
			
			// ... and finally: the URL and output file
			result.AppendFormat(" \"{0}\"", Url);
			if(UseStandardOutput)
			{
				result.Append(" -");
			}
			else
			{
				result.AppendFormat(" \"{0}\"", outputFile);
			}


			return result.ToString();
		}


	}
}
