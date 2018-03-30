using System;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public class QuadCollider : ConvexPolygonCollider
	{


		public Vector2 topLeft
		{
			get
			{
				return points[0];
			}
		}

		public Vector2 topRight
		{
			get
			{
				return points[1];
			}
		}

		public Vector2 botRight
		{
			get
			{
				return points[2];
			}
		}

		public Vector2 botLeft
		{
			get
			{
				return points[3];
			}
		} 

		protected QuadCollider(GameObject gameObject) : base(gameObject){
			this._points = new Vector2[4];
		}

		public QuadCollider(GameObject gameObject, Vector2[] points) : base(gameObject, points){
			if (points.Length != 4) {
				throw new Exception ("QuadCollider must have exactly 4 points, ordered from top left, clockwise");
			}
		}

	}
}
