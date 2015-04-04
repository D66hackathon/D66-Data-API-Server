using System;
using System.Collections.Generic;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class Matrix
	{

		///<summary>
		/// Columns
		///</summary>
		public int Columns
		{
			get { return _columns; }
		}
		private int _columns;

		///<summary>
		/// Rows
		///</summary>
		public int Rows
		{
			get { return _rows; }
		}
		private int _rows;

		///<summary>
		/// IsSquare
		///</summary>
		public bool IsSquare
		{
			get { return Rows == Columns; }
		}



		public Matrix(int rows, int columns)
		{
			_columns = columns;
			_rows = rows;
			data = new double[rows, columns];
		}


		public Matrix(double[,] d)
		{
			_columns = d.GetLength(1);
			_rows = d.GetLength(0);
			data = d;
		}

		/// <summary>
		/// Default indexer
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public double this[int row, int column]
		{
			get
			{
				return data[row, column];
			}
			set
			{
				data[row, column] = value;
			}
		}
		private double[,] data;


		#region Basic operators


		public static Matrix operator +(Matrix a, Matrix b)
		{
			if (a.Columns != b.Columns || a.Rows != b.Rows)
			{
				throw new InvalidOperationException("Matrices should be of the same size");
			}
			Matrix result = new Matrix(a.Rows, a.Columns);
			for (int r = 0; r < a.Rows; r++)
			{
				for (int c = 0; c < a.Columns; c++)
				{
					result[r, c] = a[r, c] + b[r, c];
				}
			}
			return result;
		}


		public static Matrix operator -(Matrix a, Matrix b)
		{
			if (a.Columns != b.Columns || a.Rows != b.Rows)
			{
				throw new InvalidOperationException("Matrices should be of the same size");
			}
			Matrix result = new Matrix(a.Rows, a.Columns);
			for (int r = 0; r < a.Rows; r++)
			{
				for (int c = 0; c < a.Columns; c++)
				{
					result[r, c] = a[r, c] - b[r, c];
				}
			}
			return result;
		}



		public static Matrix operator *(Matrix a, Matrix b)
		{
			if (a.Columns != b.Rows)
			{
				throw new InvalidOperationException("Can't multiply matrices - the number of columns of the first matrix should be equal to the number of columns of the second matrix");
			}
			Matrix result = new Matrix(a.Rows, b.Columns);
			if (result.Columns == 1 && result.Rows == 1)
			{
				for (int i = 0; i < a.Columns; i++)
				{
					result[0, 0] += a[0, i] * b[i, 0];
				}
			}
			else
			{
				for (int r = 0; r < a.Rows; r++)
				{
					for (int c = 0; c < b.Columns; c++)
					{
						result[r, c] = (a.Part(r, 0, 1, a.Columns) * b.Part(0, c, b.Rows, 1))[0, 0];
					}
				}
			}
			return result;
		}


		public static Matrix operator *(Matrix a, double b)
		{
			Matrix result = new Matrix(a.Rows, a.Columns);
			for (int r = 0; r < a.Rows; r++)
			{
				for (int c = 0; c < a.Columns; c++)
				{
					result[r, c] = a[r, c] * b;
				}
			}
			return result;
		}


		public static Matrix operator *(double a, Matrix b)
		{
			return b * a;
		}

		public static Matrix operator /(Matrix a, double b)
		{
			Matrix result = new Matrix(a.Rows, a.Columns);
			for (int r = 0; r < a.Rows; r++)
			{
				for (int c = 0; c < a.Columns; c++)
				{
					result[r, c] = a[r, c] / b;
				}
			}
			return result;
		}


		#endregion

		#region Additional operations


		public Matrix Part(int r, int c, int rows, int columns)
		{
			if (r < 0 || (r + rows) > this.Rows)
			{
				throw new InvalidOperationException("Too many rows");
			}
			if (c < 0 || (c + columns) > this.Columns)
			{
				throw new InvalidOperationException("Too many rows");
			}

			Matrix result = new Matrix(rows, columns);
			for (int r2 = 0; r2 < rows; r2++)
			{
				for (int c2 = 0; c2 < columns; c2++)
				{
					result[r2, c2] = this[r + r2, c + c2];
				}
			}
			return result;
		}

		/// <summary>
		/// Returns a minor of this matrix
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public Matrix Minor(int row, int column)
		{
			if (row < 0 || row >= Rows || column < 0 || column >= Columns)
			{
				throw new InvalidOperationException("row or column out of range");
			}
			Matrix result = new Matrix(Rows - 1, Columns - 1);
			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					if (r != row && c != column)
					{
						int r2 = r;
						int c2 = c;
						if (r > row)
						{
							r2--;
						}
						if (c > column)
						{
							c2--;
						}
						result[r2, c2] = this[r, c];
					}
				}
			}
			return result;
		}

		public double Sign(int row, int column)
		{
			if (row + column % 2 == 0)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}

		public double Determinant()
		{
			if (!IsSquare)
			{
				throw new InvalidOperationException("Can't calculate determinant for non-square matrices");
			}

			switch (Columns)
			{
				case 1:
					// |[a]| = a
					return this[0, 0];

				case 2:
					// | [ a b ] |
					// | [     ] |  = ad - bc
					// | [ c d ] |
					return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];

				default:
					double result = 0;

					if (IsTriangular)
					{
						// Determinant is the product of diagonal elements
						return ProductOfDiagonals();
					}
					else
					{
						// Use laplace formula
						// TODO: This is horribly, horribly inefficient.
						// a n^3 algorithm exists:
						// LU-decompose the matrix in two
						// Diagonal matrices and multiply the (easily computed)
						// Determinants of these two matrices
						for (int c = 0; c < Columns; c++)
						{
							if (this[0, c] != 0)
							{
								double sign = Sign(0, c);
								Matrix minor = Minor(0, c);
								double minorDeterminant = minor.Determinant();
								result += sign * minorDeterminant * this[0, c];
							}
						}
					}
					return result;
			}
		}

		public double ProductOfDiagonals()
		{
			double result = 1;
			for (int i = 0; i < Columns; i++)
			{
				result *= this[i, i];
			}
			return result;
		}

		/// <summary>
		/// Inverts this matrix
		/// </summary>
		/// <returns>null if matrix is not invertible</returns>
		public Matrix Invert()
		{
			if (!IsSquare)
			{
				throw new InvalidOperationException("Can't invert nonsquare matrix");
			}
			Matrix augmented = Augment(Unit(Columns));
			if (augmented.GaussJordanInPlace())
			{
				return augmented.Part(0, Columns, Columns, Columns);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gaussian elimination
		/// 
		/// NOTE: This operation is in-place
		/// </summary>
		/// <remarks>Thank god for Wikipedia</remarks>
		/// <param name="inPlace"></param>
		public void GaussianEliminationInPlace()
		{
			int r = 0;
			int c = 0;
			while (r < Rows && c < Columns)
			{
				int maxr = r;
				double maxval = this[r, c];

				//  Find pivot in column j, starting in row i:
				//  maxi := i
				//  for k := i+1 to m do
				//    if abs(A[k,j]) > abs(A[maxi,j]) then
				//      maxi := k
				//    end if
				//  end for
				for (int r2 = r + 1; r2 < Rows; r2++)
				{
					if (Math.Abs(this[r2, c]) > Math.Abs(maxval))
					{
						maxval = this[r2, c];
						maxr = r2;
					}
				}

				//  if A[maxi,j] != 0 then
				if (maxval != 0)
				{
					//    swap rows i and maxi, but do not change the value of i
					//    Now A[i,j] will contain the old value of A[maxi,j].
					SwapRows(maxr, r);
					//    divide each entry in row i by A[i,j]
					for (int c2 = 0; c2 < Columns; c2++)
					{
						this[r, c2] /= maxval;
					}
					//    Now A[i,j] will have the value 1.
					//    for u := i+1 to m do
					for (int r2 = r + 1; r2 < Rows; r2++)
					{
						//      subtract A[u,j] * row i from row u
						AddRows(r2, r, -this[r2, c]);
					}
					//      Now A[u,j] will be 0, since A[u,j] - A[i,j] * A[u,j] = A[u,j] - 1 * A[u,j] = 0.
					//    end for

					//    i := i + 1
					r++;

					//  end if
				}

				//  j := j + 1
				c++;
			}
		}


		/// <summary>
		/// Swaps rows r1 and r2
		/// this is an in-place operation
		/// </summary>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		public void SwapRows(int r1, int r2)
		{
			if (r1 != r2)
			{
				for (int c = 0; c < Columns; c++)
				{
					double temp = this[r1, c];
					this[r1, c] = this[r2, c];
					this[r2, c] = temp;
				}
			}
		}

		/// <summary>
		/// r1 = r1 + r2 * factor
		/// </summary>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		/// <param name="factor"></param>
		public void AddRows(int r1, int r2, double factor)
		{
			for (int c = 0; c < Columns; c++)
			{
				this[r1, c] += this[r2, c] * factor;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public Matrix Augment(Matrix m2)
		{
			if (Rows != m2.Rows)
			{
				throw new InvalidOperationException("Matrices m1 and m2 should have the same number of rows");
			}
			Matrix result = new Matrix(Rows, Columns + m2.Columns);
			this.CopyTo(result, 0, 0);
			m2.CopyTo(result, 0, Columns);
			return result;
		}

		public void CopyTo(Matrix target, int r, int c)
		{
			for (int r2 = 0; r2 < Rows; r2++)
			{
				for (int c2 = 0; c2 < Columns; c2++)
				{
					target[r + r2, c + c2] = this[r2, c2];
				}
			}
		}



		#endregion

		#region Triangularity

		/// <summary>
		/// Is this matrix triangular under the diagonal
		/// </summary>
		public bool IsLowerTriangular
		{
			get
			{
				for (int r = 0; r < Rows - 1; r++)
				{
					for (int c = r + 1; c < Columns; c++)
					{
						if (this[r, c] != 0)
						{
							return false;
						}
					}
				}
				return true;
			}
		}


		/// <summary>
		/// Is this matrix triangular above the diagonal
		/// </summary>
		public bool IsUpperTriangular
		{
			get
			{
				for (int r = 1; r < Rows; r++)
				{
					for (int c = 0; c < r; c++)
					{
						if (this[r, c] != 0)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Is this a triangular matrix?
		/// </summary>
		public bool IsTriangular
		{
			get
			{
				return IsUpperTriangular || IsLowerTriangular;
			}
		}



		#endregion

		#region Static helpers

		public static Matrix Unit(int size)
		{
			Matrix result = new Matrix(size, size);
			for (int i = 0; i < size; i++)
			{
				result[i, i] = 1;
			}
			return result;
		}

		#endregion


		public string ToString(int decimals)
		{
			StringBuilder[] lines = new StringBuilder[Rows];
			for (int r = 0; r < Rows; r++)
			{
				lines[r] = new StringBuilder();
			}

			for (int c = 0; c < Columns; c++)
			{
				string[] nums = new string[Rows];
				int maxlength = 0;
				for (int r = 0; r < Rows; r++)
				{
					string s = Math.Round(this[r, c] * Math.Pow(10, decimals)).ToString().PadLeft(decimals, '0');
					if (s == "0")
					{
						if (decimals == 0)
						{
							nums[r] = "0";
						}
						else
						{
							nums[r] = "0".PadRight(decimals + 2);
						}
					}
					else
					{
						if (s[0] == '-' && (s.Length == decimals + 1))
						{
							nums[r] = "-0";
						}
						else if (s.Length == decimals)
						{
							nums[r] = "0";
						}
						else
						{
							nums[r] = s.Substring(0, s.Length - decimals);
						}
						if (decimals > 0)
						{
							nums[r] += ".";
							nums[r] += s.Substring(s.Length - decimals);
						}
					}
					maxlength = Math.Max(maxlength, nums[r].Length);

				}

				for (int r = 0; r < Rows; r++)
				{
					if (c == 0)
					{
						lines[r].Append("| ");
					}
					else
					{
						lines[r].Append(", ");
					}
					lines[r].Append(nums[r].PadLeft(maxlength));
				}
			}
			StringBuilder builder = new StringBuilder();
			foreach (StringBuilder line in lines)
			{
				builder.AppendLine(line.ToString() + " |");
			}
			return builder.ToString();
		}

		public override string ToString()
		{
			return ToString(1);
		}


		/// <summary>
		/// 
		/// </summary>
		public bool GaussJordanInPlace()
		{
			GaussianEliminationInPlace();
			for (int i = 0; i < Rows; i++)
			{
				if (this[i, i] != 1)
				{
					var value = this[i, i];
					return false;
				}
			}
			for (int r = Rows - 1; r >= 0; r--)
			{
				for (int r2 = 0; r2 < r; r2++)
				{
					AddRows(r2, r, -this[r2, r]);
				}
			}
			return true;
		}

		/// <summary>
		/// Returns AT
		/// </summary>
		/// <returns></returns>
		public Matrix Transpose()
		{
			var m = new Matrix(Columns, Rows);
			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					m[c, r] = this[r, c];
				}
			}
			return m;
		}
	}
}
