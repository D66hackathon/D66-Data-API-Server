using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D66.Common.Environment
{
	public abstract class ConsoleApplicationContext
	{
		public ConsoleApplicationContext()
		{
			if(AvailableOptions().SingleOrDefault(o => o.Switch == "help") == null)
			{
				throw new InvalidOperationException("At least help switch should be supported");
			}
			Output = Console.Out;
		}

		public int Run(string[] args)
		{
			try
			{
				if (!Parse(args))
				{
					return ExitCode;
				}
				return RunCore();
			}
			finally
			{
				#if DEBUG
				Console.ReadLine();
				#endif
			}

		}

		public bool Parse(string[] args)
		{
			Output.WriteLine(Title);
			Output.WriteLine();
			string currentOption = null;
			List<string> currentArgs = null;
			SelectedOptions = new List<KeyValuePair<string, string[]>>();
			foreach (var arg in args)
			{
				if (arg.StartsWith("--"))
				{
					if(currentOption != null)
					{
						SelectedOptions.Add(new KeyValuePair<string, string[]>(currentOption, currentArgs.ToArray()));
					}
					currentOption = arg.Substring(2).ToLower();
					if(!AvailableOptions().Any(o => o.Switch == currentOption))
					{
						Output.WriteLine("Error: Unknown switch: --{0}", currentOption);
						Help();
						ExitCode = 1;
						return false;
					}
					currentArgs = new List<string>();
				}
				else
				{
					if(currentOption == null)
					{
						if (string.IsNullOrEmpty(DefaultArgument))
						{
							DefaultArgument = arg;
						}
						else
						{
							Output.WriteLine("Error: Can only supply one default argument");
							Output.WriteLine();
							Output.WriteLine("Use --help to display all options");
							ExitCode = 1;
							return false;
						}
					}
					else
					{
						currentArgs.Add(arg);
					}
				}
			}
			if (currentOption != null)
			{
				SelectedOptions.Add(new KeyValuePair<string, string[]>(currentOption, currentArgs.ToArray()));
			}
			if (IsSwitchEnabled("help"))
			{
				Help();
				return false;
			}
			return true;
		}

		protected bool IsSwitchEnabled(string name)
		{
			return SelectedOptions.Any(o => o.Key == name);
		}

		protected abstract int RunCore();

		protected void Help()
		{
			Output.WriteLine("This program supports the following switches: ");
			var maxSwitch = AvailableOptions().OrderByDescending(o => o.Switch.Length).First().Switch;
			var maxArgumentName = AvailableOptions().OrderByDescending(o => o.ArgumentName.Length).First().ArgumentName;
			var switchFormat = "--{0} {1}  ";
			var indent = string.Format(switchFormat, maxSwitch, maxArgumentName).Length;
			foreach(var option in AvailableOptions())
			{
				Output.WriteLine();
				var lines = SplitLines(option.Description, 78 - indent);
				for(var i=0; i<lines.Length; i++)
				{
					if(i == 0)
					{
						Output.Write(string.Format(switchFormat, option.Switch, option.ArgumentName).PadRight(indent));
					}
					else
					{
						Output.Write("".PadRight(indent));
					}
					Output.WriteLine(lines[i]);
				}
			}
		}

		private string[] SplitLines(string str, int width)
		{
			var result = new List<string>();
			var line = new StringBuilder();
			foreach(var word in str.Split(' '))
			{
				if(line.Length + word.Length + 1 >= width)
				{
					if(line.Length > 0)
					{
						result.Add(line.ToString().Substring(1));
						line = new StringBuilder();
					}
				}
				line.Append(" ");
				line.Append(word);
			}
			if (line.Length > 0)
			{
				result.Add(line.ToString().Substring(1));
			}
			return result.ToArray();
		}

		protected string GetSingleArgument(string name)
		{
			return 
					SelectedOptions
					.Where(o => o.Key == name)
					.Select(kv => kv.Value.FirstOrDefault())
					.FirstOrDefault();
		}

		public int ExitCode { get; private set; }

		public string DefaultArgument { get; private set; }

		public List<KeyValuePair<string, string[]>> SelectedOptions { get; private set; }

		public TextWriter Output { get; set; }

		protected abstract IEnumerable<Option> AvailableOptions();

		protected abstract string Title { get; }

	}

	public class Option
	{
		public Option()
		{
			ArgumentName = "";
			Description = "";
			Switch = "";
		}
		public string Switch { get; set; }
		public string ArgumentName { get; set; }
		public string Description { get; set; }
	}

}
