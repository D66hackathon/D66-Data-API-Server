using System;
using System.Diagnostics.Contracts;
#if SINGLE_PRECISION
using Scalar = System.Single;
#else

#endif

namespace D66.Common.Mathmatics
{
	[Serializable()]
	public struct Vector2
	{

		public Vector2(Double x, Double y)
			: this()
		{
			this.x = x;
			this.y = y;
		}

		public Double X { get { return x; } }

		private readonly Double x, y;

		public Double Y { get { return y; } }

		[Pure()]
		public Double Length()
		{
			return (Double)System.Math.Sqrt(x*x + y*y);
		}

		public Vector2 Normalize()
		{
			if(X == 0 && Y == 0)
			{
				throw new InvalidOperationException();
			}
			return this/Length();
		}

		public override string ToString()
		{
			return string.Format("({0:0.00}, {1:0.00})", X, Y);
		}


		[Pure()]
		public Double DistanceTo(Vector2 location)
		{
			Double dx = location.x - this.x;
			Double dy = location.y - this.y;
			return (Double)System.Math.Sqrt(dx*dx + dy*dy);
		}

		/// <summary>
		/// Projects vector v onto this vector
		/// </summary>
		/// <param name="v"></param>
		/// <returns>0 if v is orthogonal to this vector, 1 if it is equal.</returns>
		[Pure()]
		public double ProjectT(Vector2 v)
		{
			var dot = Dot(this, v);
			var projection = dot/this.Length();
			return projection/this.Length();
		}

		public static Vector2 operator +(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
		}

		public static Vector2 operator -(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
		}

		public static Vector2 operator /(Vector2 v1, Double v)
		{
			return new Vector2(v1.X / v, v1.Y / v);
		}

		public static Vector2 operator *(Vector2 v1, Double v)
		{
			return new Vector2(v1.X * v, v1.Y * v);
		}

		public static Vector2 operator *(Double v, Vector2 v1)
		{
			return new Vector2(v1.X * v, v1.Y * v);
		}

		public static Vector2 operator -(Vector2 v)
		{
			return new Vector2(-v.X, -v.Y);
		}

		public static bool operator ==(Vector2 v1, Vector2 v2)
		{
			return v1.X == v2.X && v1.Y == v2.Y;
		}

		public static bool operator !=(Vector2 v1, Vector2 v2)
		{
			return v1.X != v2.X || v1.Y != v2.Y;
		}

		public static double Dot(Vector2 v1, Vector2 v2)
		{
			return v1.X*v2.X + v1.Y*v2.Y;
		}

		public static double CosAngle(Vector2 v1, Vector2 v2)
		{
			return Dot(v1, v2)/(v1.Length()*v2.Length());
		}

		/// <summary>
		/// Rotates this vector around the origin.
		/// </summary>
		/// <param name="deltaAngle"></param>
		/// <returns></returns>
		public Vector2 Rotate(Double deltaAngle)
		{
			Double length = Length();
			if(length == 0)
			{
				return this;
			}
			var currentAngle = (Double)System.Math.Atan2(Y, X);
			currentAngle += deltaAngle;
			return new Vector2((Double)System.Math.Cos(currentAngle) * length, (Double)System.Math.Sin(currentAngle) * length);
		}


		public static double AngleBetween(Vector2 v1, Vector2 v2)
		{
			return System.Math.Atan2(v2.Y, v2.X) - System.Math.Atan2(v1.Y, v1.X);
		}

		public static Vector2 Min(Vector2 v1, Vector2 v2)
		{
			return new Vector2(
				System.Math.Min(v1.X, v2.X),
				System.Math.Min(v1.Y, v2.Y)
			);
		}

		public static Vector2 Max(Vector2 v1, Vector2 v2)
		{
			return new Vector2(
				System.Math.Max(v1.X, v2.X),
				System.Math.Max(v1.Y, v2.Y)
			);
		}

		public override bool Equals(object obj)
		{
			var other = (Vector2) obj;
			return this.X == other.X && this.Y == other.Y;
		}

		public bool Equals(Vector2 other, double epsilon)
		{
			return other.DistanceTo(this) < epsilon;
		}

		public override int GetHashCode()
		{
			int result = 0;
			result ^= X.GetHashCode();
			result ^= Y.GetHashCode();
			return result;
		}


		public Vector2 Lerp(Vector2 to, Double alpha)
		{
			var delta = to - this;
			return this + alpha*delta;
		}
	}
}
