using System;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public struct Collision : ICloneable
	{
		public Collider self;
		public Collider other;
		public Vector2[] contactPoints;
		public float[] ts;
		public Vector2[] depenetrations;
		public Vector2[] normals;
		public Vector2 sumDepenetration;
		public Collision(Collider self, Collider other, Vector2[] contactPoints, Vector2[] depenetrations, Vector2[] normals, float[] ts)
		{
			this.self = self;
			this.other = other;
			int l = depenetrations.Length;
			if (contactPoints.Length != l || l != normals.Length || l != ts.Length) {
				throw new Exception ("Contactpoints, depenetrations, othernormals, and ts must have the same length");
			}
			this.ts = ts;
			this.contactPoints = contactPoints;
			this.depenetrations = depenetrations;
			this.normals = normals;
			this.sumDepenetration = Vector2.zero;
			for (int i = 0; i < l; ++i) {
				this.sumDepenetration += depenetrations [i];
			}
		}
		public object Clone()
		{
			return GetClone();
		}

		public Collision GetClone()
		{

			return new Collision(
				(Collider)self.GetClone(),
				(Collider)other.GetClone(),
				(Vector2[])contactPoints.Clone(),
				(Vector2[])depenetrations.Clone(),
				(Vector2[])normals.Clone(),
				(float[])ts.Clone()
				);
		}
		public Collision GetClone(long timeStamp)
		{

			return new Collision(
				(Collider)self.GetClone(timeStamp),
				(Collider)other.GetClone(timeStamp),
				(Vector2[])contactPoints.Clone(),
				(Vector2[])depenetrations.Clone(),
				(Vector2[])normals.Clone(),
				(float[])ts.Clone()
				);
		}

		public static bool operator== (Collision lhs, Collision rhs){
			if(!(lhs.self == rhs.self && lhs.other == rhs.other && lhs.sumDepenetration == rhs.sumDepenetration && lhs.depenetrations.Length == rhs.depenetrations.Length && lhs.contactPoints.Length == rhs.contactPoints.Length && lhs.normals.Length == rhs.normals.Length && lhs.ts.Length == rhs.ts.Length)) {
				return false;
			}
			float[] lhsTs = lhs.ts;
			float[] rhsTs = rhs.ts;
			int l = lhsTs.Length;
			for (int i = 0; i < l; ++i) {
				if (lhsTs [i] != rhsTs [i]) {
					return false;
				}
			}
			Vector2[] lhsPoints = lhs.contactPoints;
			Vector2[] rhsPoints = rhs.contactPoints;
			for (int i = 0; i < l; ++i) {
				if (lhsPoints [i] != rhsPoints [i]) {
					return false;
				}
			}
			Vector2[] lhsDepenetrations = lhs.depenetrations;
			Vector2[] rhsDepenetrations = rhs.depenetrations;
			for (int i = 0; i < l; ++i) {
				if (lhsDepenetrations [i] != rhsDepenetrations [i]) {
					return false;
				}
			}
			Vector2[] lhsNormals = lhs.normals;
			Vector2[] rhsNormals = rhs.normals;
			for (int i = 0; i < l; ++i) {
				if (lhsNormals [i] != rhsNormals [i]) {
					return false;
				}
			}
			return true;
		}

		public static bool operator!= (Collision lhs, Collision rhs){
			return !(lhs == rhs);
		}

	}
}
