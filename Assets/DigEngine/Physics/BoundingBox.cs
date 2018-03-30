using System;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public struct BoundingBox
	{
		public Vector2 min;
		public Vector2 max;

		public BoundingBox(Vector2 min, Vector2 max)
		{
			this.min = min;
			this.max = max;
		}

		public BoundingBox(float minX, float minY, float maxX, float maxY)
		{
			this.min = new Vector2 (minX, minY);
			this.max = new Vector2 (maxX, maxY);
		}

		public bool Intersect(BoundingBox other){
			return ((this.min.x <= other.min.x && other.min.x <= this.max.x) || (other.min.x <= this.min.x && this.min.x <= other.max.x))
				&& ((this.min.y <= other.min.y && other.min.y <= this.max.y) || (other.min.y <= this.min.y && this.min.y <= other.max.y));
		}
	}
}
