using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;


namespace DigEngine{
	public struct DotToDotCollisionResult {


		public Vector2 dot1Depenetration;
		public Vector2 dot2Depenetration;
		public Vector2 intersectionPoint;
		public float t;



		public Vector2 dot1Movement;
		public Vector2 dot2Movement;

		public Vector2 normal;

		public Vector2 dot1Requirement;
		public Vector2 dot2Requirement;

		public Vector2 similarDepenetration;

		public Vector2 thinDot1;
		public Vector2 thinDot2;

		public bool CheckDot1MovementRequirement(Vector2 movement){
			return Util.CheckMovementRequirement (dot1Requirement, movement, dot2Movement-dot2Depenetration, normal);
		}

		public bool CheckDot2MovementRequirement(Vector2 movement){
			return Util.CheckMovementRequirement (dot2Requirement, movement, dot1Movement-dot1Depenetration, -normal);
		}
		public bool CheckDot1MovementRequirement2(Vector2 movement){
			return Util.CheckMovementRequirement (dot1Movement-dot1Depenetration, movement, dot2Movement-dot2Depenetration, normal);
		}

		public bool CheckDot2MovementRequirement2(Vector2 movement){
			return Util.CheckMovementRequirement (dot2Movement-dot2Depenetration, movement, dot1Movement-dot1Depenetration, -normal);
		}
	}
}
