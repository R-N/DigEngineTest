using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;


namespace DigEngine{
	public struct DotToLineIntersectionResult {

		public Vector2 dotDepenetration;
		public Vector2 lineDepenetration;
		public Vector2 intersectionPoint;
		public float t;
		public float r;
	}
}
