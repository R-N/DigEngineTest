using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;

namespace DigEngine{
	public class CollisionComparer : IComparer<TwinCollisionChild> {


		public CollisionComparer(){
		}
		public int Compare(TwinCollisionChild a, TwinCollisionChild b){
			if (a.t != b.t) {
				return a.t.CompareTo (b.t);
			}
			//NOTE
			return Util.Min(a.selfMovement.sqrMagnitude, a.otherMovement.sqrMagnitude).CompareTo (
				Util.Min(b.selfMovement.sqrMagnitude, b.otherMovement.sqrMagnitude));
			/*return (a.selfMovement.sqrMagnitude + a.otherMovement.sqrMagnitude).CompareTo (
				b.selfMovement.sqrMagnitude + b.otherMovement.sqrMagnitude);*/
			/*return (a.selfRequirement.sqrMagnitude + a.otherRequirement.sqrMagnitude).CompareTo (
			b.selfRequirement.sqrMagnitude + b.otherRequirement.sqrMagnitude);*/
		}
	}
}
