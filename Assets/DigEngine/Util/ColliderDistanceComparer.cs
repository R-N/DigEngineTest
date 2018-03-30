using System.Collections;
using System.Collections.Generic;
namespace DigEngine{

	public class ColliderDistanceComparer : IComparer<Collider> {
		Collider distanceFrom = null;
		Collider prevDistanceFrom = null;

		public ColliderDistanceComparer(Collider distanceFrom){
			this.distanceFrom = distanceFrom;
			this.prevDistanceFrom = (Collider)distanceFrom.previous;
		}
		public int Compare(Collider a, Collider b){
			float distA = distanceFrom._Distance (a).sqrMagnitude;
			float distB = distanceFrom._Distance (b).sqrMagnitude;

			if (prevDistanceFrom != null && distA == distB) {
				Collider prevA = (Collider)a.previous;
				Collider prevB = (Collider)b.previous;
				if (prevA != null) {
					if (prevB != null) {
						return prevDistanceFrom._Distance (prevA).sqrMagnitude.CompareTo (prevDistanceFrom._Distance (prevB).sqrMagnitude);
					} else {
						return -1;
					}
				} else {
					if (prevB != null) {
						return 1;
					}
				}
			} 
			return distA.CompareTo (distB);

		}

	}
}
