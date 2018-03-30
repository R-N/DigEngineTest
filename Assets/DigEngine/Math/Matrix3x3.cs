using System.Collections;
using System.Collections.Generic;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System;


namespace DigEngine{
	public struct Matrix3x3 {

		public static Matrix3x3 identity = new Matrix3x3(	1, 0, 0, 
															0, 1, 0,
															0, 0, 1	);
		
		public static Matrix3x3 zero = new Matrix3x3 (	0, 0, 0, 
			                               				0, 0, 0,
			                               				0, 0, 0);
		public float m11, m12, m13; //row x column
		public float m21, m22, m23;
		public float m31, m32, m33;

		public float determinant
		{
			get
			{
				return m11 * m22 * m33 + m12 * m23 * m31 + m13 * m21 * m32 - (m31 * m22 * m13 + m32 * m23 * m11 + m33 * m21 * m12);
			}
		}
		public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33){
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m31 = m31;
			this.m32 = m32;
			this.m33 = m33;
		}

		public Matrix2x2 minor11 {
			get {
				return new Matrix2x2 (m22, m23, m32, m33);
			}
		}
		public Matrix2x2 minor12 {
			get {
				return new Matrix2x2 (m21, m23, m31, m33);
			}
		}
		public Matrix2x2 minor13 {
			get {
				return new Matrix2x2 (m21, m22, m31, m32);
			}
		}
		public Matrix2x2 minor21 {
			get {
				return new Matrix2x2 (m12, m13, m32, m33);
			}
		}
		public Matrix2x2 minor22 {
			get {
				return new Matrix2x2 (m11, m13, m31, m33);
			}
		}
		public Matrix2x2 minor23 {
			get {
				return new Matrix2x2 (m11, m12, m31, m32);
			}
		}
		public Matrix2x2 minor31 {
			get {
				return new Matrix2x2 (m12, m13, m22, m23);
			}
		}
		public Matrix2x2 minor32 {
			get {
				return new Matrix2x2 (m11, m13, m21, m23);
			}
		}
		public Matrix2x2 minor33 {
			get {
				return new Matrix2x2 (m11, m12, m21, m22);
			}
		}

		public Matrix3x3 transpose{
			get{
				return new Matrix3x3 (m11, m21, m31,
									m12, m22, m32,
									m13, m23, m33);
			}
		}
		public Matrix3x3 cofactor {
			get {
				return new Matrix3x3 (minor11.determinant, minor12.determinant, minor13.determinant,
									minor21.determinant, minor22.determinant, minor23.determinant,
									minor31.determinant, minor32.determinant, minor33.determinant);
			}
		}
		public Matrix3x3 adjoint
		{
			get
			{
				Matrix3x3 co = cofactor;
				return new Matrix3x3(co.m11, -co.m12, co.m13,
									-co.m21, co.m22, -co.m23,
									co.m31, -co.m32, co.m33);
			}
		}

		public Matrix3x3 inverse
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


		public static Matrix3x3 operator*(Matrix3x3 m, int x)
		{
			return new Matrix3x3(m.m11 * x, m.m12 * x, m.m13 * x,
				m.m21 * x, m.m22 * x, m.m23 * x,
				m.m31 * x, m.m32 * x, m.m33 * x);
		}

		public static Matrix3x3 operator*(Matrix3x3 m, long x)
		{
			return new Matrix3x3(m.m11 * x, m.m12 * x, m.m13 * x,
				m.m21 * x, m.m22 * x, m.m23 * x,
				m.m31 * x, m.m32 * x, m.m33 * x);
		}

		public static Matrix3x3 operator*(Matrix3x3 m, float x)
		{
			return new Matrix3x3(m.m11 * x, m.m12 * x, m.m13 * x,
				m.m21 * x, m.m22 * x, m.m23 * x,
				m.m31 * x, m.m32 * x, m.m33 * x);
		}
		public static Matrix3x3 operator*(int x, Matrix3x3 m)
		{
			return new Matrix3x3(m.m11 * x, m.m12 * x, m.m13 * x,
				m.m21 * x, m.m22 * x, m.m23 * x,
				m.m31 * x, m.m32 * x, m.m33 * x);
		}

		public static Matrix3x3 operator*(long x, Matrix3x3 m)
		{
			return new Matrix3x3(m.m11 * x, m.m12 * x, m.m13 * x,
				m.m21 * x, m.m22 * x, m.m23 * x,
				m.m31 * x, m.m32 * x, m.m33 * x);
		}

		public static Matrix3x3 operator*(float x, Matrix3x3 m)
		{
			return new Matrix3x3(m.m11 * x, m.m12 * x, m.m13 * x,
				m.m21 * x, m.m22 * x, m.m23 * x,
				m.m31 * x, m.m32 * x, m.m33 * x);
		}

		public static Matrix3x3 operator/(Matrix3x3 m, int x)
		{
			return new Matrix3x3(m.m11 / x, m.m12 / x, m.m13 / x,
				m.m21 / x, m.m22 / x, m.m23 / x,
				m.m31 / x, m.m32 / x, m.m33 / x);
		}

		public static Matrix3x3 operator/(Matrix3x3 m, long x)
		{
			return new Matrix3x3(m.m11 / x, m.m12 / x, m.m13 / x,
				m.m21 / x, m.m22 / x, m.m23 / x,
				m.m31 / x, m.m32 / x, m.m33 / x);
		}

		public static Matrix3x3 operator/(Matrix3x3 m, float x)
		{
			return new Matrix3x3(m.m11 / x, m.m12 / x, m.m13 / x,
				m.m21 / x, m.m22 / x, m.m23 / x,
				m.m31 / x, m.m32 / x, m.m33 / x);
		}


		public static Matrix3x3 operator +(Matrix3x3 lhs, Matrix3x3 rhs)
		{
			return new Matrix3x3(lhs.m11 + rhs.m11, lhs.m12 + rhs.m12, lhs.m13 + rhs.m13,
				lhs.m21 + rhs.m21, lhs.m22 + rhs.m22, lhs.m23 + rhs.m23,
				lhs.m31 + rhs.m31, lhs.m32 + rhs.m32, lhs.m33 + rhs.m33);
		}

		public static Matrix3x3 operator -(Matrix3x3 lhs, Matrix3x3 rhs)
		{
			return new Matrix3x3(lhs.m11 - rhs.m11, lhs.m12 - rhs.m12, lhs.m13 - rhs.m13,
				lhs.m21 - rhs.m21, lhs.m22 - rhs.m22, lhs.m23 - rhs.m23,
				lhs.m31 - rhs.m31, lhs.m32 - rhs.m32, lhs.m33 - rhs.m33);
		}

		public static Matrix3x3 operator *(Matrix3x3 lhs, Matrix3x3 rhs)
		{
			return new Matrix3x3(	lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31, 			lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32,		lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33, 

									lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31, 			lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32,		lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33,

									lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31, 			lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32,		lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33);
		}

		public static Vector3 operator *(Matrix3x3 m, Vector3 v)
		{
			return new Vector3(	m.m11 * v.x + m.m12 * v.y + m.m13 * v.z,
				m.m21 * v.x + m.m22 * v.y + m.m23 * v.z,
				m.m31 * v.x + m.m32 * v.y + m.m33 * v.z);
		}

		public override string ToString(){
			return String.Format ("( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} )", m11, m12, m13, m21, m22, m23, m31, m32, m33);
		}
	}
}
