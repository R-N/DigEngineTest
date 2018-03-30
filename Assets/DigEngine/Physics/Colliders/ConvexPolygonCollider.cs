using System;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public class ConvexPolygonCollider : PolygonCollider
	{
		public override bool isTrigger {
			get {
				return _isTrigger;
			}
			set {
				_isTrigger = value;
			}
		}

		protected ConvexPolygonCollider(GameObject gameObject) : base(gameObject){
		}

		public ConvexPolygonCollider(GameObject gameObject, Vector2[] points) : base(gameObject, points){
		}



		public bool CheckTrigger(PolygonCollider other)
		{
			Vector2[] otherPoints = other.worldPoints;
			int opl = otherPoints.Length;
			for (int i = 0; i < opl; ++i)
			{
				if (this.IsPointInCollider(otherPoints[i]))
				{
					return true;
				}
			}
			Vector2[] myPoints = worldPoints;
			int mpl = myPoints.Length;
			for (int i = 0; i < mpl; ++i)
			{
				if (other.IsPointInCollider(myPoints[i]))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsPointInCollider(Vector2 point)
		{
			int l1 = points.Length - 1;

			for (int i = 0; i < l1; ++i)
			{
				if (Util.Cross(point-points[i],points[i+1]-points[i]) <= 0){
					return false;
				}
			}
			return !(Util.Cross(point - points[3],points[0] - points[3]) <= 0);
		}
		public override bool CheckTrigger(Collider other)
		{
			if (!this.isTrigger && !other.isTrigger)
			{
				return false;
			}
			bool ret = false;
			if (other.GetType() == Rigidbody.tConvexPolygonCollider)
			{
				return CheckTrigger((ConvexPolygonCollider)other);
			}
			return base.CheckTrigger(other);
		}


	}
}
