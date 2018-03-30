using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;

namespace DigEngine{
	public class CollisionChild {
		public Collider self;
		public Collider other;
		public Vector2 contactPoint;
		public Vector2 depenetration;
		public Vector2 normal;
		public Vector2 requirement;
		public float t;
		public Vector2 movement;
		public CollisionApplication application = null;
		public CollisionChild next;
		public CollisionChild prev;
		public Vector2 similarDepenetration;

		bool _applied =false;

		public bool applied {
			get {
				if (application == null) {
					return _applied;
				} else {
					return application.applied;
				}
			}
		}

		public void ApplyAndBroadcast(){
			if (applied) {
				return;
			}
			string selfName = self.gameObject.name;
			string otherName = other.gameObject.name;
			int hash = GetHashCode ();
			if (self.rigidbody != null) {
				if (application == null) {
					application = new CollisionApplication (this);
					application.Apply ();
				} else if (application.applicable){
					bool x = true;
				}
			} 
				_applied = true;

			self.ImmediateCollisionBroadcast (this);
		}
	}
}
