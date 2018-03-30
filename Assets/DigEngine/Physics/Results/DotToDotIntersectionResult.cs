using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;


namespace DigEngine{
	public struct DotToDotIntersectionResult {


		public Vector2 dot1Depenetration;
		public Vector2 dot2Depenetration;
		public Vector2 intersectionPoint;
		public float t;
	}
}
