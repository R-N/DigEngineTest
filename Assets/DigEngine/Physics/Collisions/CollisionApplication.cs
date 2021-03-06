using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;

namespace DigEngine{
	public class CollisionApplication {
		public CollisionChild source;
		public bool applicable;
		public Vector2 positionChange;
		public Vector2 movementChange;
		public Vector2 velocityChange;
		public bool isGroundedGravity;
		public bool isGroundedUp;
		public bool isGroundedDown;
		public bool isGroundedLeft;
		public bool isGroundedRight;

		public bool applied = false;

		private bool previousGroundedGravity;
		private bool previousGroundedUp;
		private bool previousGroundedDown;
		private bool previousGroundedLeft;
		private bool previousGroundedRight;

		private Vector2 _velocityChange;

		public void Apply(){
			applied = true;
			if (!applicable) {
				return;
			}
			int hash = GetHashCode ();
			Rigidbody rb = source.self.rigidbody;
			rb.t = source.t;
			rb.position += positionChange;
			Vector2 prevMov = rb.movement;
			rb.movement += movementChange;

			_velocityChange = Util.Clamp (rb._velocity + velocityChange, rb._velocity, Vector2.zero) - rb._velocity;
			rb._velocity += _velocityChange;
			previousGroundedGravity = rb.isGroundedGravity;
			if (isGroundedGravity) {
				rb.isGroundedGravity = true;
			}
			previousGroundedUp = rb.isGroundedUp;
			if (isGroundedUp) {
				rb.isGroundedUp = true;
			}
			previousGroundedDown = rb.isGroundedDown;
			if (isGroundedDown) {
				rb.isGroundedDown = true;
			}
			previousGroundedLeft = rb.isGroundedLeft;
			if (isGroundedLeft) {
				rb.isGroundedLeft = true;
			}
			previousGroundedRight = rb.isGroundedRight;
			if (isGroundedRight) {
				rb.isGroundedRight = true;
			}
		
		}

		public void Undo(){
			applied = false;
			if (!applicable) {
				return;
			}
			Rigidbody rb = source.self.rigidbody;
			rb.t = source.t;
			rb.position -= positionChange;
			rb.movement -= movementChange;
			rb._velocity -= _velocityChange;
			if (isGroundedGravity) {
				rb.isGroundedGravity = previousGroundedGravity;
			}
			if (isGroundedUp) {
				rb.isGroundedUp = previousGroundedUp;
			}
			if (isGroundedDown) {
				rb.isGroundedDown = previousGroundedDown;
			}
			if (isGroundedLeft) {
				rb.isGroundedLeft = previousGroundedLeft;
			}
			if (isGroundedRight) {
				rb.isGroundedRight = previousGroundedRight;
			}
		}

		public CollisionApplication(CollisionChild c){
			source = c;
			Vector2 velocity = c.self.velocity;
			movementChange = Vector2.zero;
			velocityChange = Vector2.zero;
			isGroundedGravity = false;
			isGroundedUp = false;
			isGroundedDown = false;
			isGroundedLeft = false;
			isGroundedRight = false;
			applicable = c.self.rigidbody != null;// && c.depenetration != Vector2.zero;
			if (!applicable) {
				return;
			}
			movementChange = c.depenetration - c.movement;
			positionChange = c.movement;

			/*int l = source.normals.Length;
			for(int k = 0; k < l; ++k){*/
			Vector2 norm = source.normal.normalized;
			float selfVelocityNormalDot = Vector2.Dot(velocity.normalized, norm);
			if (selfVelocityNormalDot < 0){
				Vector2 a = Util.Project(velocity, norm);
				Vector2 b = Util.Project(source.other.velocity, norm);
				if (Vector2.Dot(a, b) > 0){
					if (a.sqrMagnitude > b.sqrMagnitude){
						velocityChange -= a - b;
					}
				}else{
					velocityChange -= a;
				}
			}
			//}
			Rigidbody rb = source.self.rigidbody;
			velocityChange = Util.Clamp (rb._velocity + velocityChange, rb._velocity, Vector2.zero) - rb._velocity;
			//}
			//for(int k = 0; k < l; ++k){
			if (source.self.gameObject.name == "GreenBox" && source.other.gameObject.name == "WhiteBox") {
				//if (norm.y > 0) {
					UnityEngine.Debug.Log ("NORM " + Util.ToString (norm));
				//}
			}
			Vector2 selfDepNorm = norm;
			if(Vector2.Dot(selfDepNorm, source.self.physics.negativeNormalizedGravity) >= Rigidbody.groundMinimumCos){
				isGroundedGravity = true;
			}
			if (Vector2.Dot(selfDepNorm, source.self.transform.down) >= Rigidbody.groundMinimumCos){
				isGroundedUp = true;
			}
			if (Vector2.Dot(selfDepNorm, source.self.transform.up) >= Rigidbody.groundMinimumCos){
				isGroundedDown = true;
			}
			if (Vector2.Dot(selfDepNorm, source.self.transform.right) >= Rigidbody.groundMinimumCos){
				isGroundedLeft = true;
			}
			if(Vector2.Dot(selfDepNorm, source.self.transform.left) >= Rigidbody.groundMinimumCos){
				isGroundedRight = true;
			}
			//}

		}
	}
}
