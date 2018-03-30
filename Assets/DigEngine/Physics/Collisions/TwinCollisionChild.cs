using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;
using System;

namespace DigEngine{
	public class TwinCollisionChild:ICloneable  {
		public Collider self;
		public Collider other;
		public Vector2 contactPoint;
		public Vector2 selfDepenetration;
		public Vector2 otherDepenetration;
		public Vector2 selfOtherNormal;
		public float t;
		public Vector2 selfMovement;
		public Vector2 otherMovement;
		public Vector2 selfRequirement;
		public Vector2 otherRequirement;
		public Vector2 similarDepenetration;
		public Vector2 thinSelf;
		public Vector2 thinOther;
		public bool toDelete = false;

		public Vector2 selfOriginalMovement;
		public Vector2 otherOriginalMovement;

		public static bool operator==(TwinCollisionChild lhs, TwinCollisionChild rhs){
			return lhs.self == rhs.self
			&& lhs.other == rhs.other
			&& lhs.t == rhs.t
			&& lhs.contactPoint == rhs.contactPoint
			&& lhs.selfDepenetration == rhs.selfDepenetration
			&& lhs.otherDepenetration == rhs.otherDepenetration
			&& lhs.selfOtherNormal == rhs.selfOtherNormal
			&& lhs.selfMovement == rhs.selfMovement
			&& lhs.otherMovement == rhs.otherMovement
			&& lhs.selfRequirement == rhs.selfRequirement
			&& lhs.otherRequirement == rhs.otherRequirement
			&& lhs.similarDepenetration == rhs.similarDepenetration
			&& lhs.thinSelf == rhs.thinSelf
			&& lhs.thinOther == rhs.thinOther;
		}
		public static bool operator!=(TwinCollisionChild lhs, TwinCollisionChild rhs){
			return lhs.self != rhs.self
			|| lhs.other != rhs.other
			|| lhs.t != rhs.t
			|| lhs.contactPoint != rhs.contactPoint
			|| lhs.selfDepenetration != rhs.selfDepenetration
			|| lhs.otherDepenetration != rhs.otherDepenetration
			|| lhs.selfOtherNormal != rhs.selfOtherNormal
			|| lhs.selfMovement != rhs.selfMovement
			|| lhs.otherMovement != rhs.otherMovement
			|| lhs.selfRequirement != rhs.selfRequirement
			|| lhs.otherRequirement != rhs.otherRequirement
			|| lhs.similarDepenetration != rhs.similarDepenetration
			|| lhs.thinSelf != rhs.thinSelf
			|| lhs.thinOther != rhs.thinOther;
		}

		Vector2 selfDepenetration0;
		Vector2 otherDepenetration0;
		Vector2 similarDepenetration0;
		Vector2 selfMovement0;
		Vector2 otherMovement0;
		Vector2 selfRequirement0;
		Vector2 otherRequirement0;

		public TwinCollisionChild(Collider self, Collider other, DotToLineCollisionResult source){
			this.self = self;
			this.other = other;
			this.contactPoint = source.collisionPoint;
			this.selfDepenetration = source.dotDepenetration;
			this.otherDepenetration = source.lineDepenetration;
			this.selfOtherNormal = source.lineNormal;
			this.t = source.t;
			this.selfMovement = source.dotMovement;
			this.otherMovement = source.lineMovement;
			this.selfRequirement = source.dotRequirement;
			this.otherRequirement = source.lineRequirement;
			this.thinSelf = source.thinDot;
			this.thinOther = source.thinLine;
			this.similarDepenetration = source.similarDepenetration;

			this.selfOriginalMovement = source.dotOriginalMovement;
			this.otherOriginalMovement = source.lineOriginalMovement;

			selfDepenetration0 = selfDepenetration;
			otherDepenetration0 = otherDepenetration;
			similarDepenetration0 = similarDepenetration;
			selfMovement0 = selfMovement;
			otherMovement0 = otherMovement;
			selfRequirement0 = selfRequirement;
			otherRequirement0 = otherRequirement;
		}

		public object Clone(){
			return MemberwiseClone ();
		}


		public CollisionChild selfCollisionChild{
			get{
				return new CollisionChild () {
					self = self,
					other = other,
					contactPoint = contactPoint,
					depenetration = selfDepenetration,
					normal = selfOtherNormal,
					t = t,
					movement=selfMovement,
					requirement = selfRequirement,
					similarDepenetration = similarDepenetration
				};
			}
		}

		public bool CheckAndSetSelfMovement(Vector2 movement){
			if (!CheckSelfRequirement (movement)) {
				return false;
			}
			//selfMovement += similarDepenetration - thinSelf;
			Vector2 selfDepenetration = Util.Project(this.selfRequirement - movement, selfOtherNormal);
			Vector2 otherDepenetration = this.otherDepenetration + similarDepenetration - thinOther;
			Vector2 otherMovement = otherRequirement;
			Vector2 selfMovement = selfRequirement;
			/*if (selfRequirement == Vector2.zero && otherRequirement == Vector2.zero && selfDepenetration == Vector2.zero && otherDepenetration == Vector2.zero) {
				UnityEngine.Debug.Log ("CHECK SELF ZERO");
				return false;
			}*/
			if (selfDepenetration.x == -0.25f) {
				bool z = true;
				string selfName = self.gameObject.name;
				string otherName = other.gameObject.name;
				int hash = GetHashCode ();
				bool x = true;
			}
			this.selfDepenetration = selfDepenetration;
			this.otherDepenetration = otherDepenetration;
			this.selfMovement = selfMovement;
			this.otherMovement = otherMovement;

			SetMovementPt2 ();
			return true;
		}
		public bool CheckAndSetOtherMovement(Vector2 movement){
			if (!CheckOtherRequirement (movement)) {
				return false;
			}
			//otherMovement += similarDepenetration - thinOther;
			Vector2 otherDepenetration = Util.Project(this.otherRequirement - movement, selfOtherNormal);
			Vector2 selfDepenetration = this.selfDepenetration + similarDepenetration - thinSelf;
			Vector2 otherMovement = otherRequirement;
			Vector2 selfMovement = selfRequirement;
			/*if (selfRequirement == Vector2.zero && otherRequirement == Vector2.zero && selfDepenetration == Vector2.zero && otherDepenetration == Vector2.zero) {
				UnityEngine.Debug.Log ("CHECK OTHER ZERO");
				return false;
			}*/
			if (selfDepenetration.x == -0.25f) {
				string selfName = self.gameObject.name;
				string otherName = other.gameObject.name;
				int hash = GetHashCode ();
				bool x = true;
			}
			this.selfDepenetration = selfDepenetration;
			this.otherDepenetration = otherDepenetration;
			this.selfMovement = selfMovement;
			this.otherMovement = otherMovement;

			SetMovementPt2 ();
			return true;
		}

		private bool SetMovementPt2(){
			similarDepenetration = Physics.SimilarDepenetration (selfDepenetration, otherDepenetration, selfOtherNormal);
			if (similarDepenetration != Vector2.zero) {
				selfDepenetration -= similarDepenetration;
				otherDepenetration -= similarDepenetration;
				selfMovement -= similarDepenetration;
				otherMovement -= similarDepenetration;
			}

			if (similarDepenetration.x == -0.25f) {
				string selfName = self.gameObject.name;
				string otherName = other.gameObject.name;
				int hash = GetHashCode ();
				bool x = true;
			}

			/*Physics.Thin (selfMovement, selfDepenetration, otherMovement, otherDepenetration, selfOtherNormal, out thinSelf, out thinOther);
			selfMovement += thinSelf;
			otherMovement += thinOther;
			selfDepenetration += thinSelf;
			otherMovement += thinOther;*/
			return true;
		}

		public bool CheckSelfRequirement(Vector2 movement){
			bool x = Util.CheckMovementRequirement (selfRequirement, movement, otherMovement-otherDepenetration, selfOtherNormal);
			if (!x) {
				UnityEngine.Debug.Log ("CHECK SELF FALSE");
			}
			return x;
		}

		public bool CheckOtherRequirement(Vector2 movement){
			bool x = Util.CheckMovementRequirement (otherRequirement, movement, selfMovement-selfDepenetration, -selfOtherNormal);
			if (!x) {
				UnityEngine.Debug.Log ("CHECK OTHER FALSE");
			}
			return x;
		}
		public bool CheckSelfRequirement2(Vector2 movement){
			return Util.CheckMovementRequirement (selfMovement-selfDepenetration, movement, otherMovement-otherDepenetration, selfOtherNormal);
		}

		public bool CheckOtherRequirement2(Vector2 movement){
			return Util.CheckMovementRequirement (otherMovement-otherDepenetration, movement, selfMovement-selfDepenetration, -selfOtherNormal);
		}

		public CollisionChild otherCollisionChild{
			get{
				return new CollisionChild () {
					self = other,
					other = self,
					contactPoint = contactPoint,
					depenetration = otherDepenetration,
					normal = -selfOtherNormal,
					t = t,
					movement=otherMovement,
					requirement = otherRequirement,
					similarDepenetration = similarDepenetration
				};
			}
		}
	}
}
