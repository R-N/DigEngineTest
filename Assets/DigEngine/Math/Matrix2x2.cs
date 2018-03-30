using System;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public struct Matrix2x2
	{
		public static Matrix2x2 identity = new Matrix2x2(1, 0, 0, 1);
		public static Matrix2x2 zero = new Matrix2x2(0, 0, 0, 0);
		public float m11, m12; //row x column
		public float m21, m22;
		
		public float determinant
		{
			get
			{
				return m11 * m22 - m12 * m21;
			}
		}

		public Matrix2x2(float m11, float m12, float m21, float m22)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m21 = m21;
			this.m22 = m22;
		}

		public Matrix2x2 transpose {
			get {
				return new Matrix2x2 (m11, m21, m12, m22);
			}
		}

		public Matrix2x2 adjoint
		{
			get
			{
				return new Matrix2x2(m22, -m12, -m21, m11);
			}
		}
		
		public Matrix2x2 inverse
		{
			get
			{
				float det = this.determinant;
				if (det == 0)
				{
					throw new ArithmeticException("Matrix doesn't have inverse. determinant is zero");
				}
				else
				{
					return adjoint / det;
				}
			}
		}


		public static Matrix2x2 operator*(Matrix2x2 m, int x)
		{
			return new Matrix2x2(m.m11 * x, m.m12 * x, m.m21 * x, m.m22 * x);
		}

		public static Matrix2x2 operator*(Matrix2x2 m, long x)
		{
			return new Matrix2x2(m.m11 * x, m.m12 * x, m.m21 * x, m.m22 * x);
		}

		public static Matrix2x2 operator*(Matrix2x2 m, float x)
		{
			return new Matrix2x2(m.m11 * x, m.m12 * x, m.m21 * x, m.m22 * x);
		}
		public static Matrix2x2 operator*(int x, Matrix2x2 m)
		{
			return new Matrix2x2(m.m11 * x, m.m12 * x, m.m21 * x, m.m22 * x);
		}

		public static Matrix2x2 operator*(long x, Matrix2x2 m)
		{
			return new Matrix2x2(m.m11 * x, m.m12 * x, m.m21 * x, m.m22 * x);
		}

		public static Matrix2x2 operator*(float x, Matrix2x2 m)
		{
			return new Matrix2x2(m.m11 * x, m.m12 * x, m.m21 * x, m.m22 * x);
		}

		public static Matrix2x2 operator/(Matrix2x2 m, int x)
		{
			return new Matrix2x2(m.m11 / x, m.m12 / x, m.m21 / x, m.m22 / x);
		}

		public static Matrix2x2 operator/(Matrix2x2 m, long x)
		{
			return new Matrix2x2(m.m11 / x, m.m12 / x, m.m21 / x, m.m22 / x);
		}

		public static Matrix2x2 operator/(Matrix2x2 m, float x)
		{
			return new Matrix2x2(m.m11 / x, m.m12 / x, m.m21 / x, m.m22 / x);
		}


		public static Matrix2x2 operator +(Matrix2x2 lhs, Matrix2x2 rhs)
		{
			return new Matrix2x2(lhs.m11 + rhs.m11, lhs.m12 + rhs.m12, lhs.m21 + rhs.m21, lhs.m22 + rhs.m22);
		}

		public static Matrix2x2 operator -(Matrix2x2 lhs, Matrix2x2 rhs)
		{
			return new Matrix2x2(lhs.m11 - rhs.m11, lhs.m12 - rhs.m12, lhs.m21 - rhs.m21, lhs.m22 - rhs.m22);
		}

		public static Matrix2x2 operator *(Matrix2x2 lhs, Matrix2x2 rhs)
		{
			return new Matrix2x2(	lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21, 			lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22	,
			
									lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21, 			lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22	);
		}

		public static Vector2 operator *(Matrix2x2 m, Vector2 v)
		{
			return new Vector2(	m.m11 * v.x + m.m12 * v.y,
								m.m21 * v.x + m.m22 * v.y);
		}
		public override string ToString(){
			return String.Format ("( {0}, {1}, {2}, {3} )", m11, m12, m21, m22);
		}
	}
}
