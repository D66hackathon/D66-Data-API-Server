using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;

namespace D66.Common.UI
{
	public struct Color
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hue">[0, 360)</param>
		/// <param name="saturation">[0, 1)</param>
		/// <param name="value">[0, 1)</param>
		/// <returns></returns>
		public static Color FromHSV(double hue, double saturation, double value)
		{
			if (hue < 0 || hue >= 360) { throw new ArgumentOutOfRangeException("hue"); }
			if (saturation < 0 || saturation > 1) { throw new ArgumentOutOfRangeException("saturation"); }
			if (value < 0 || value > 1) { throw new ArgumentOutOfRangeException("value"); }

			int hi = (int)System.Math.Floor(hue / 60.0) % 6;
			double f = (hue / 60.0) - System.Math.Floor(hue / 60.0);

			double p = value * (1.0 - saturation);
			double q = value * (1.0 - (f * saturation));
			double t = value * (1.0 - ((1.0 - f) * saturation));
			switch (hi)
			{
				case 0: return new Color(value, t, p);
				case 1: return new Color(q, value, p);
				case 2: return new Color(p, value, t);
				case 3: return new Color(p, q, value);
				case 4: return new Color(q, p, value);
				case 5: return new Color(value, p, q);
				default: throw new InvalidOperationException();
			}
		}

		public Color(double r, double g, double b)
			: this(r, g, b, 1)
		{
		}

		public Color(byte r, byte g, byte b)
			: this(r, g, b, 255)
		{
		}

		public Color(byte r, byte g, byte b, byte a) : this(r / 255.0, g / 255.0, b / 255.0, a / 255.0)
		{
		}

		public Color(double r, double g, double b, double a)
		{
			_r = Math.Min(Math.Max(r, 0), 1);
			_g = Math.Min(Math.Max(g, 0), 1);
			_b = Math.Min(Math.Max(b, 0), 1);
			_a = Math.Min(Math.Max(a, 0), 1);
		}

		private readonly double _a, _r, _g, _b;

		public double R { get { return _r; } }
		public double G { get { return _g; } }
		public double B { get { return _b; } }
		/// <summary>
		/// Alpha (0 = transparent, 1 = opaque)
		/// </summary>
		public double A { get { return _a; } }


		public byte RByte { get { return (byte)(R * 255); } }
		public byte GByte { get { return (byte)(G * 255); } }
		public byte BByte { get { return (byte)(B * 255); } }
		public byte AByte { get { return (byte)(A * 255); } }



		public static Color Heatmap(double value)
		{
			value = System.Math.Max(0, System.Math.Min(1, value));
			var c0 = new Color(0, 0, 0);
			var c2 = new Color(0, 0, 1);
			var c4 = new Color(0, 1, 0);
			var c6 = new Color(1, 1, 0);
			var c8 = new Color(1, 0.5, 0);
			var c10 = new Color(1, 0, 0);
			double stratification = 0.3;
			if (value < 0.2)
			{
				return Lerp(c0.Lighten(stratification), c2.Darken(stratification), value / 0.2);
			}
			if (value < 0.4)
			{
				return Lerp(c2.Lighten(stratification), c4.Darken(stratification), (value - 0.2) / 0.2);
			}
			if (value < 0.6)
			{
				return Lerp(c4.Lighten(stratification), c6.Darken(stratification), (value - 0.4) / 0.2);
			}
			if (value < 0.8)
			{
				return Lerp(c6.Lighten(stratification), c8.Darken(stratification), (value - 0.6) / 0.2);
			}
			return Lerp(c8.Lighten(stratification), c10.Darken(stratification), (value - 0.8) / 0.2);
		}

		public Color Darken(double amount = 1)
		{
			var alpha = Math.Pow(0.7, amount);
			return new Color(R*alpha, G*alpha, B*alpha, A);
		}

		public Color Lighten(double amount)
		{
			var alpha = Math.Pow(0.7, -amount);
			return new Color(Math.Max(R * alpha, 0.117), Math.Max(G * alpha, 0.117), Math.Max(B * alpha, 0.117), A);
		}

		[Pure()]
		public Color SetAlpha(double alpha)
		{
			return new Color(R, G, B, alpha);
		}

		public static Color Lerp(Color c1, Color c2, double alpha)
		{
			if (alpha < 0)
			{
				return c1;
			}
			if (alpha > 1)
			{
				return c2;
			}
			double dr = (c2.R - c1.R) * alpha;
			double dg = (c2.G - c1.G) * alpha;
			double db = (c2.B - c1.B) * alpha;
			double da = (c2.A - c1.A) * alpha;
			return new Color(c1.R + dr, c1.G + dg, c1.B + db, c1.A + da);
		}

		/// <summary>
		/// Distance along the color cube. Maximum distance (between black and white) is Sqrt(3)
		/// </summary>
		/// <param name="c2"></param>
		/// <returns></returns>
		public double Distance(Color c2)
		{
			var dr = c2.R - this.R;
			var dg = c2.G - this.G;
			var db = c2.B - this.B;
			return System.Math.Sqrt(dr * dr + dg * dg + db * db);
		}

		public string ToHtml()
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", RByte, GByte, BByte);
		}

		public static Color FromHtml(string p)
		{
			if(p.StartsWith("#"))
			{
				p = p.Substring(1);
			}
			if(p.Length == 3)
			{
				return new Color(
					byte.Parse(p.Substring(0, 1), NumberStyles.HexNumber),
					byte.Parse(p.Substring(1, 1), NumberStyles.HexNumber),
					byte.Parse(p.Substring(2, 1), NumberStyles.HexNumber)
				);
			}
			if (p.Length == 6)
			{
				return new Color(
					byte.Parse(p.Substring(0, 2), NumberStyles.HexNumber),
					byte.Parse(p.Substring(2, 2), NumberStyles.HexNumber),
					byte.Parse(p.Substring(4, 2), NumberStyles.HexNumber)
				);
			}
			throw new ArgumentException("Invalid HTML color value: " + p);
		}
	}
}
