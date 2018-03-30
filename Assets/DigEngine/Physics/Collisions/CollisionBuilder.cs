using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;

namespace DigEngine{
	public class CollisionBuilder  {
		Dictionary<ColliderPair, List<CollisionChild>> childs = new Dictionary<ColliderPair, List<CollisionChild>>();
		public CollisionBuilder(){
		}

		public void Add(TwinCollisionChild tcc){
			Add (tcc.selfCollisionChild);
			Add (tcc.otherCollisionChild);
		}

		public void Add(CollisionChild cc){
			ColliderPair cp = new ColliderPair (cc.self, cc.other);
			if (childs.ContainsKey (cp)) {
				childs [cp].Add (cc);
			} else {
				List<CollisionChild> ccs = new List<CollisionChild> ();
				childs [cp] = ccs;
				ccs.Add (cc);
			}
			cc.t = cc.self.t + (1 - cc.self.t) * cc.t;
		}

		public void ApplyNotApplied(){
			int i = 0;
			int j = 0;
			int hash = GetHashCode ();
			foreach (List<CollisionChild> ccs in childs.Values) {
				foreach (CollisionChild cc in ccs) {
					if (!cc.applied) {
						cc.ApplyAndBroadcast ();
					}
					++j;
				}
				++i;
			}
		}

		public void BuildAndBroadcast(){
			foreach (KeyValuePair<ColliderPair, List<CollisionChild>> kvp in childs) {
				ColliderPair key = kvp.Key;
				List<CollisionChild> ccs = kvp.Value;
				//ccs.Sort ((a, b) => a.t.CompareTo (b.t));
				int l = ccs.Count;
				Collision col = new Collision (key.self, key.other,
					                new Vector2[l], new Vector2[l], new Vector2[l], new float[l]);
				int i = 0;
				IEnumerator<CollisionChild> en = ccs.GetEnumerator ();
				while (en.MoveNext ()) {
					CollisionChild cc = en.Current;
					col.contactPoints [i] = cc.contactPoint;
					col.depenetrations [i] = cc.depenetration;
					col.normals [i] = cc.normal;
					col.ts [i] = cc.t;
					++i;
				}
				key.self.AddCollision (col);
			}
		}
	}
}
