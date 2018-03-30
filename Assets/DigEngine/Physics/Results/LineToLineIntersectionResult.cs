using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;


namespace DigEngine{
	public struct LineToLineIntersectionResult {


		public Vector2 line1Depenetration;
		public Vector2 line2Depenetration;
		public Vector2 intersectionPoint;
		public float t;

		public float r;
		public float y;

	}
}
