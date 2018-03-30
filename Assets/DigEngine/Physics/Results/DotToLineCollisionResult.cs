using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;


namespace DigEngine{
	public struct DotToLineCollisionResult {
		public Vector2 dotDepenetration;
		public Vector2 lineDepenetration;
		public Vector2 collisionPoint;
		public float t;
		public float r;

		public Vector2 dotMovement;
		public Vector2 lineMovement;

		public Vector2 lineNormal;

		public Vector2 dotRequirement;
		public Vector2 lineRequirement;

		public Vector2 similarDepenetration;

		public Vector2 thinDot;
		public Vector2 thinLine;

		public bool CheckDotMovementRequirement(Vector2 movement){
			return Util.CheckMovementRequirement (dotRequirement, movement, lineMovement-lineDepenetration, lineNormal);
		}

		public bool CheckLineMovementRequirement(Vector2 movement){
			return Util.CheckMovementRequirement (lineRequirement, movement, dotMovement-dotDepenetration, -lineNormal);
		}
		public bool CheckDotMovementRequirement2(Vector2 movement){
			return Util.CheckMovementRequirement (dotMovement-dotDepenetration, movement, lineMovement-lineDepenetration, lineNormal);
		}

		public bool CheckLineMovementRequirement2(Vector2 movement){
			return Util.CheckMovementRequirement (lineMovement-lineDepenetration, movement, dotMovement-dotDepenetration, -lineNormal);
		}

		public Vector2 dotOriginalMovement;
		public Vector2 lineOriginalMovement;
	}

}
