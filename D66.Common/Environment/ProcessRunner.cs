using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace D66.Common.Environment
{

	/// <summary>
	/// Helper class to run command line tools
	/// </summary>
	public class ProcessRunner
	{

		

		/// <summary>
		/// Path to the executable
		/// </summary>
		public string Path { get; set; }

		public string Parameters { get; set; }

		/// <summary>
		/// Timeout. Timespan.Zero means no timeout
		/// </summary>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns>true if program terminated, false on timeout</returns>
		public bool Run()
		{
			using (var process = new Process()
			{
				StartInfo = new ProcessStartInfo(Path, Parameters)
				{
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput = true
				},

			})
			{
				var milliseconds = (int)Math.Round(Timeout.TotalMilliseconds);
				if (milliseconds == 0)
				{
					milliseconds = -1;
				}


				//var output = new StringBuilder();
				var error = new StringBuilder();

				//using (var outputWaitHandle = new AutoResetEvent(false))
				using (var errorWaitHandle = new AutoResetEvent(false))
				{
					//process.OutputDataReceived += (sender, e) =>
					//{
					//    if (e.Data == null)
					//    {
					//        outputWaitHandle.Set();
					//    }
					//    else
					//    {
					//        output.AppendLine(e.Data);
					//    }
					//};
					process.ErrorDataReceived += (sender, e) =>
					{
						if (e.Data == null)
						{
							errorWaitHandle.Set();
						}
						else
						{
							error.AppendLine(e.Data);
						}
					};

					process.Start();

					//process.BeginOutputReadLine();
					process.BeginErrorReadLine();

					var encoding = process.StandardOutput.CurrentEncoding;
					StandardOutput = process.StandardOutput.ReadToEnd();
					StandardOutputBytes = encoding.GetBytes(StandardOutput);
					var timeout = false;
					if (process.WaitForExit(milliseconds) &&
						//outputWaitHandle.WaitOne(milliseconds) &&
						errorWaitHandle.WaitOne(milliseconds))
					{
						this.ExitCode = process.ExitCode;
					}
					else
					{
						timeout = true;
					}
					this.StandardError = error.ToString();
					//this.StandardOutput = output.ToString();
					return !timeout;
				}

			}
		}

		public int ExitCode { get; private set; }

		public string StandardError { get; private set; }

		public string StandardOutput { get; private set; }

		public byte[] StandardOutputBytes { get; private set; }
	}

	public class ProcessRunException : ApplicationException
	{
		public ProcessRunException(string message) : base(message) {}

		public string Output { get; set; }
		public string StandardError { get; set; }

	}
}
