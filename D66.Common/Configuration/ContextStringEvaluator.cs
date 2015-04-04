using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace D66.Common.Configuration
{
	public class ContextStringEvaluator<T>
	{

		public string Evaluate(string str, T context)
		{
			if(str == null)
			{
				throw new ArgumentNullException("str");
			}
			var result = new StringBuilder();
			foreach (var c in EvaluateRoot(str, context))
			{
				result.Append(c);
			}
			return result.ToString();
		}

		private IEnumerable<object> EvaluateRoot(string str, T context)
		{
			int index = 0;
			while(index < str.Length)
			{
				var c = str[index++];
				if (c == '}')
				{
					throw new ArgumentException("Unexpected character '}' while parsing expression " + str);
				}
				if (c == '{')
				{
					foreach(var c1 in EvaluateParameter(str, ref index, context))
					{
						yield return c1;
					}
				}
				else
				{
					yield return c;
				}
			}
		}

		private string EvaluateParameter(string str, ref int index, T context)
		{
			var parameter = new StringBuilder();
			while(true)
			{
				if(index >= str.Length)
				{
					throw new ArgumentException("Unterminated parameter in expression " + str);
				}
				char c = str[index++];
				if(c == '}')
				{
					return EvaluateParameter(parameter.ToString(), context);
				}
				parameter.Append(c);
			}
		}

		private string EvaluateParameter(string p, T context)
		{
			var parts = p.Split(':');
			if(parts.Length > 2)
			{
				throw new ArgumentException("Unexpected character ':' in parameter specification");
			}
			var parameterName = parts.First();
			var format = parts.Skip(1).FirstOrDefault();
			var value = GetParameterValue(parameterName, context);
			if(value == null)
			{
				return "";
			}
			if(!string.IsNullOrEmpty(format))
			{
				var formattable = value as IFormattable;
				if (formattable != null)
				{
					return formattable.ToString(format, null);
				}
			}
			return value.ToString();
		}

		private object GetParameterValue(string parameterName, object context)
		{
			if(string.IsNullOrEmpty(parameterName) || parameterName == ".")
			{
				return context;
			}
			if(context == null)
			{
				return null;
			}
			var parts = parameterName.Split(new[] {'.'}, 2);
			var type = context.GetType();
			var result = type.GetProperty(parts[0]).GetValue(context, null);
			if(parts.Length > 1)
			{
				return GetParameterValue(parts[1], result);
			}
			return result;
		}

	}
}
