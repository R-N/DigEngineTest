using System;
using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;
using Vector3=UnityEngine.Vector3;
using Mathf=UnityEngine.Mathf;

namespace DigEngine
{
	public class Physics : DigObject
	{
		private Vector2 _gravity = new Vector2(0, -9.81f);
		public Vector2 gravity {
			get {
				return _gravity;
			}
			set {
				_gravity = value;
				_normalizedGravity = _gravity.normalized;
				_negativeNormalizedGravity = -_normalizedGravity;
			}
		}
		private Vector2 _normalizedGravity = Vector2.down;
		private Vector2 _negativeNormalizedGravity = Vector2.up;
		public Vector2 normalizedGravity{
			get{
				return _normalizedGravity;
			}
				
		}
		public Vector2 negativeNormalizedGravity {
			get {
				return _negativeNormalizedGravity;
			}
		}
		public static bool MovingDotToMovingDotIntersection(
			Vector2 dot1, Vector2 dot1Movement,
			Vector2 dot2, Vector2 dot2Movement,
			out DotToDotIntersectionResult result){
			result = new DotToDotIntersectionResult ();
			if (dot1 == dot2) {
				result.dot1Depenetration = dot1Movement;
				result.dot2Depenetration = dot2Movement;
				result.intersectionPoint = dot1;
				result.t = 0;
				return true;
			}
			if (dot1Movement == Vector2.zero && dot2Movement == Vector2.zero) {
				return false;
			}
			Vector2 diff = dot1Movement - dot2Movement;
			if (diff == Vector2.zero) {
				return false;
			}
			/// dot1 + dot1Movement * t = dot2 + dot2Movement * t
			/// (dot1Movement-dot2Movement)*t = dot2-dot1
			/// (dot1Movement.x - dot2Movement.x) * t = dot2.x-dot1.x
			float t;
			if (dot1Movement.x - dot2Movement.x == 0) {
				t = (dot2.y - dot1.y) / diff.y;
			} else {
				t = (dot2.x - dot1.x) / diff.x;
			}
			if (t < 0 || t > 1) {
				return false;
			}
			result.t = t;
			result.intersectionPoint = dot1 + dot1Movement * t;
			float t1 = 1 - t;
			result.dot1Depenetration = dot1Movement * t1;
			result.dot2Depenetration = dot2Movement * t1;
			return true;


		}
		public static bool MovingDotToMovingDotCollision(
			Vector2 dot1, Vector2 dot1Movement,
			Vector2 dot2, Vector2 dot2Movemeent,
			out DotToDotCollisionResult result){
			result = new DotToDotCollisionResult ();
			return false;
		}
		public static bool MovingDotToMovingLineIntersection(
			Vector2 dot, Vector2 dotMovement, 
			Vector2 lineP1, Vector2 lineP2, Vector2 lineMovement,
			out DotToLineIntersectionResult result)
		{
			result = new DotToLineIntersectionResult ();
			float t = 1.1f;
			float r = 1.1f;
			Vector2 p = lineP1;
			Vector2 u = lineP2 - lineP1;
			Vector2 q = dot;
			Vector2 v = lineMovement;
			Vector2 w = dotMovement;
			Vector2 vw = w - v;
			//Vector2 pq = q - p;
			Vector2 qp = p-q;
			/// q + w * t = p + u * r + v * t
			/// q + t(w-v) = p + r*u
			/// q + t(vw) = p + r*u
			/// | vw.x   u.x | * | t | = | q.x - p.x |
			/// | vw.y   u.y |   | r |   | q.y - p.y |
			Matrix2x2 m = new Matrix2x2(vw.x, -u.x, vw.y, -u.y);
			if (m.determinant == 0)
			{
				return false;
			}
			Vector2 ret = m.inverse * qp;
			t = ret.x;
			r = ret.y;
			if (t < 0 || t > 1 || r < 0 || r > 1)
			{
				return false;
			}
			result.t = t;
			result.r = r;
			float t1 = t - 1;
			result.dotDepenetration = dotMovement * t1;
			result.lineDepenetration = lineMovement * t1;
			result.intersectionPoint = dot + dotMovement + result.dotDepenetration;

			return true;
		}

		public static bool MovingDotToMovingLineGraze(
			Vector2 dot, Vector2 dotMovement, 
			Vector2 lineP1, Vector2 lineP2, Vector2 lineMovement,
			out DotToLineCollisionResult result)
		{
			result = new DotToLineCollisionResult ();
			result.dotOriginalMovement = dotMovement;
			result.lineOriginalMovement = lineMovement;
			Vector2 lineNormal = Util.Normal (lineP1, lineP2);
			result.lineNormal = lineNormal;
			result.t = 1;
			Vector2 diff0 = (dotMovement - lineMovement);
			float movDot = Vector2.Dot (lineNormal, diff0.normalized);
			if (movDot < 0) {
				return false;
			}
			Vector2 u = lineP2 - lineP1;
			Vector2 v = dot - lineP1;
			Vector2 w = v + diff0;
			Vector2 proj = Util.Project (v, u);
			Vector2 rej = w - proj;
			if (rej.sqrMagnitude < 3 * Util.sqrThin) {
				if (Vector2.Dot (v, u) < 0) {
					return false;
				} else {
					float r0 = proj.sqrMagnitude / u.sqrMagnitude;
					float r = Util.Project(w, u).magnitude / u.magnitude;
					if (r0 > 1) {
						return false;
					} else {
						if (r > 1) {
							r = 1;
						}
						
						result.r = r;
						result.t = 1;
						result.dotDepenetration = Vector2.zero;
						result.collisionPoint = dot + dotMovement;
						result.lineDepenetration = Vector2.zero;
						result.dotMovement = dotMovement;
						result.dotRequirement = dotMovement;
						result.dotOriginalMovement = dotMovement;
						result.lineNormal = Util.Normal (u);
						result.similarDepenetration = Vector2.zero;
						result.thinDot = Vector2.zero;
						result.thinLine = Vector2.zero;
						result.lineOriginalMovement = lineMovement;
						result.lineRequirement = lineMovement;
						result.lineMovement = lineMovement;

						return true;
					}

				}
			}
			return false;
		}

		public static bool MovingDotToMovingLineCollision(
			Vector2 dot, Vector2 dotMovement, 
			Vector2 lineP1, Vector2 lineP2, Vector2 lineMovement,
			out DotToLineCollisionResult result)
		{
			result = new DotToLineCollisionResult ();
			result.dotOriginalMovement = dotMovement;
			result.lineOriginalMovement = lineMovement;
			Vector2 lineNormal = Util.Normal (lineP1, lineP2);
			result.lineNormal = lineNormal;
			result.t = 1;
			//Vector2 d = lineP2 - lineP1;
			if (dotMovement == Vector2.zero && lineMovement == Vector2.zero) {
				UnityEngine.Debug.Log ("ZERO MOVEMENT");
				return false;
			}
			Vector2 diff0 = (dotMovement - lineMovement);
			float movDot = Vector2.Dot (lineNormal, diff0.normalized);
			if (movDot > 0) {
				/*if (dotMovement.x != 0 && lineNormal.x != 0) {
					bool signDot = dotMovement.x > 0;
					bool signLine = lineMovement.x > 0;
					if (signDot == signLine) {
						UnityEngine.Debug.Log ("DOT ESCAPE");
					}
				}*/
				return false;
			} 

			if (movDot == 0){
				Vector2 u = lineP2 - lineP1;
				Vector2 v = dot - lineP1;
				Vector2 w = v + diff0;
				Vector2 proj = Util.Project (w, u);
				if (Vector2.Dot (v, u) < 0) {
					return false;
				} else{
					float r = proj.magnitude / u.magnitude;
					Vector2 rej = w-proj;
					if (r > 1) {
						return false;
					} else if (rej.sqrMagnitude > 3 * Util.sqrThin) {
						return false;
					}else{
						result.r = r;
						result.t = 1;
						result.dotDepenetration = Vector2.zero;
						result.collisionPoint = dot + dotMovement;
						result.lineDepenetration = Vector2.zero;
						result.dotMovement = dotMovement;
						result.dotRequirement = dotMovement;
						result.dotOriginalMovement = dotMovement;
						result.lineNormal = Util.Normal (u);
						result.similarDepenetration = Vector2.zero;
						result.thinDot = Vector2.zero;
						result.thinLine = Vector2.zero;
						result.lineOriginalMovement = lineMovement;
						result.lineRequirement = lineMovement;
						result.lineMovement = lineMovement;

						return true;
					}

				}
			}
			DotToLineIntersectionResult _r;
			if (!MovingDotToMovingLineIntersection (dot, dotMovement, lineP1, lineP2, lineMovement, out _r)) {
				return MovingDotToMovingLineGraze (dot, dotMovement, lineP1, lineP2, lineMovement, out result);
				return false;
			}
			bool needThinning = true;
			if (false) {
				float t = _r.t;
				Vector2 u = lineP2 - lineP1;
				Vector2 endPoint = _r.intersectionPoint;
				Vector2 postLineP1 = lineP1 + lineMovement + result.lineDepenetration;
				Vector2 diff = endPoint - postLineP1;
				float crossPost = Util.Cross (diff, u);
				if (crossPost != 0) {
					//UnityEngine.Debug.Log ("CROSS IS NOT ZERO");
					float crossPre = Util.Cross (dot - lineP1, u);
					bool signPost = crossPost > 0;
					bool signPre = crossPre > 0;
					needThinning = true;
					if (signPre != signPost) {
						UnityEngine.Debug.Log ("SIGN IS DIFFERENT");
						if (t <= 0) {
							UnityEngine.Debug.Log ("T NEGATIVE");
							needThinning = true;
							t = 0;
						} else {
							int i = 0;
							do {
								if (t >= _r.t) {
									t *= (1 - Util.thin);
								}
								if (t >= _r.t) {
									t -= Util.sqrThin;
								}
								if (t >= _r.t) {
									t -= Util.halfThin;
								}
								if (t >= _r.t) {
									t -= Util.thin;
								}
								if (t < 0) {
									UnityEngine.Debug.Log ("T NEGATIVE");
									needThinning = true;
									t = 0;
									break;
								}
								_r.t = t;
								float t1 = t - 1;
								_r.dotDepenetration = dotMovement * t1;
								_r.lineDepenetration = lineMovement * t1;
								_r.intersectionPoint = dot + dotMovement + _r.dotDepenetration;

								endPoint = _r.intersectionPoint;
								postLineP1 = lineP1 + lineMovement + _r.lineDepenetration;
								diff = endPoint - postLineP1;
								crossPost = Util.Cross (diff, u);
								crossPre = Util.Cross (dot - lineP1, u);
								signPost = crossPost > 0;
								signPre = crossPre > 0;
								if (signPost == signPre) {
									needThinning=false;
									break;
								}else{
									UnityEngine.Debug.Log ("FAILED");
								}
								++i;
							} while(needThinning && i < 5 && t > 0);
						}
					}
				}
			}
			//needThinning = false;

			Vector2 _dotMovement = dotMovement + _r.dotDepenetration;
			Vector2 _lineMovement = lineMovement + _r.lineDepenetration;

			Vector2 _dotRequirement = _dotMovement;
			Vector2 _lineRequirement = _lineMovement;

			_r.dotDepenetration = Util.Project (_r.dotDepenetration, lineNormal);
			_r.lineDepenetration = Util.Project (_r.lineDepenetration, lineNormal);

			if (dotMovement == Vector2.zero && lineMovement == Vector2.zero && _r.dotDepenetration == Vector2.zero && _r.lineDepenetration == Vector2.zero) {

				UnityEngine.Debug.Log ("ZERO MOVEMENT AND DEPENETRATION");
				return false;
			}



			//Vector2 similarDepenetration = Vector2.zero;
			Vector2 similarDepenetration = SimilarDepenetration (_r.dotDepenetration, _r.lineDepenetration, lineNormal);
			if (similarDepenetration != Vector2.zero) {
				_r.dotDepenetration -= similarDepenetration;
				_r.lineDepenetration -= similarDepenetration;
				_dotMovement -= similarDepenetration;
				_lineMovement -= similarDepenetration;
			}

			Vector2 thinDot = Vector2.zero, thinLine = Vector2.zero;

			if (needThinning) {
				Thin (_dotMovement, _r.dotDepenetration, _lineMovement, _r.lineDepenetration, lineNormal, out thinDot, out thinLine);
				_r.dotDepenetration += thinDot;
				_r.lineDepenetration += thinLine;
				_dotMovement += thinDot;
				_lineMovement += thinLine;
			}

			result.similarDepenetration = similarDepenetration;
			result.lineNormal = lineNormal;
			result.t = _r.t;
			result.r = _r.r;
			result.dotDepenetration = _r.dotDepenetration;
			result.lineDepenetration = _r.lineDepenetration;
			result.collisionPoint = _r.intersectionPoint;
			result.dotMovement = _dotMovement;
			result.lineMovement = _lineMovement;
			result.dotRequirement = _dotRequirement;// - _r.dotDepenetration;
			result.lineRequirement = _lineRequirement;// - _r.lineDepenetration;
			result.thinDot = thinDot;
			result.thinLine = thinLine;

			return true;
		}

		public static Vector2 SimilarDepenetration(Vector2 selfDepenetration, Vector2 otherDepenetration, Vector2 normal){
			selfDepenetration = Util.Project (selfDepenetration, normal);
			otherDepenetration = Util.Project (otherDepenetration, normal);
			float depenetrationsDot = Vector2.Dot (selfDepenetration, otherDepenetration);

			Vector2 similarDepenetration = Vector2.zero;
			if (depenetrationsDot > 0) {
				if (selfDepenetration.sqrMagnitude > otherDepenetration.sqrMagnitude) {
					similarDepenetration = otherDepenetration;
				} else {
					similarDepenetration = selfDepenetration;
				}
			} 
			return similarDepenetration;
		}


		public static void Thin(Vector2 selfMovement, Vector2 selfDepenetration, Vector2 otherMovement, Vector2 otherDepenetration, Vector2 selfOtherNormal,
			out Vector2 thinSelf, out Vector2 thinOther){
			selfMovement = Util.Project (selfMovement, selfOtherNormal);
			selfDepenetration = Util.Project (selfDepenetration, selfOtherNormal);
			otherMovement = Util.Project (otherMovement, selfOtherNormal);
			otherDepenetration = Util.Project (otherDepenetration, selfOtherNormal);
			thinSelf = Vector2.zero;
			thinOther = Vector2.zero;
			Vector2 thina = Util.Thin (selfOtherNormal);
			Vector2 thinb = -thina;
			Vector2 diffDot = (selfMovement + selfDepenetration);
			Vector2 diffLine = (otherMovement + otherDepenetration);
			bool a = diffDot.sqrMagnitude >= Util.sqrThin;
			bool b = diffLine.sqrMagnitude >= Util.sqrThin;
			if (a) {
				thinSelf += thina;
			} else {
				thinSelf -= diffDot;
				if (b) {
					thinOther += diffDot - thina;
				} else {
					thinOther -= diffLine;
				}
			}
			diffDot = (selfMovement + selfDepenetration);
			diffLine = (otherMovement + otherDepenetration);
			a = diffDot.sqrMagnitude >= Util.sqrThin;
			b = diffLine.sqrMagnitude >= Util.sqrThin;
			if (b) {
				thinOther += thinb;
			} else {
				thinOther -= diffLine;
				if (a) { 
					thinSelf += diffLine - thinb;
				} else {
					thinSelf -= diffDot;
				}
			}
		}

		public static bool MovingLineToMovingLineIntersection(
			Vector2 line1P1, Vector2 line1P2, Vector2 line1Movement,
			Vector2 line2P1, Vector2 line2P2, Vector2 line2Movement,
			out LineToLineIntersectionResult result)
		{
			result = new LineToLineIntersectionResult ();
			float r = 1.1f;
			float y = 1.1f;
			float t = 1.1f;
			Vector2 p = line2P1;
			Vector2 u = line2P2 - line2P1;
			Vector2 q = line1P1;
			Vector2 x = line1P2 - line1P1;
			Vector2 v = line2Movement;
			Vector2 w = line1Movement;
			Vector2 vw = w - v;
			Vector2 pq = q - p;
			//Vector2 qp = p-q;
			Vector2 xu = x - u;
			/// q + x*y + w * t = p + u * r + v * t
			/// q + t(w-v) = p + r*u - y*x
			/// q-p = r*u - y*x - t*vw
			/// pq = r*u - y*x - t*vw
			/// 
			/// | pq.x | = | u.x   -x.x   -vw.x | * | r |
			/// | pq.y |   | u.y   -x.y   -vw.y |   | y |
			/// 									| t |
			/// 
			/// | pq.x |   | u.x   -x.x   -vw.x |   | r |
			/// | pq.y | = | u.y   -x.y   -vw.y | * | y |
			/// |   0  |   |  0      0       0	|   | t |
			/// 
			/// | u.x   -x.x   -vw.x |^-1      | pq.x |   | r |
			/// | u.y   -x.y   -vw.y | 	  *    | pq.y | = | y |
			/// |  0      0       0	 |         |   0  |   | t |
			/// 


			Matrix3x3 m = new Matrix3x3(u.x, -x.x, -vw.x, u.y, -x.y, -vw.y, 0,0,1);
			if (m.determinant == 0)
			{
				return false;
			}
			Vector3 ret = m.inverse * new Vector3(pq.x, pq.y, 0);
			r = ret.x;
			y = ret.y;
			t = ret.z;
			if (t < 0 || t > 1 || r < 0 || r > 1 || y < 0 || y > 1)
			{
				return false;
			}
			result.t = t;
			float t1 = t - 1;
			result.line1Depenetration = line1Movement * t1;
			result.line2Depenetration = line2Movement * t1;
			result.intersectionPoint = line1P1 + u * r + line1Movement * t;
			return true;
		}
		public static Vector2 PointToLineDistance(Vector2 point, Vector2 lineP1, Vector2 lineP2){
			Vector2 hit;
			return PointToLineDistance (point, lineP1, lineP2, out hit);
		}

		public static Vector2 PointToLineDistance(Vector2 point, Vector2 lineP1, Vector2 lineP2, out Vector2 hit){
			if (lineP1 == lineP2) {
				hit = lineP1;
				return point - lineP1;
			} else if (point == lineP1 || point == lineP2) {
				hit = lineP1;
				return Vector2.zero;
			}
			Vector2 dp = point - lineP1;
			Vector2 d0 = lineP2 - lineP1;
			float dot = Vector2.Dot (dp, d0);
			if (dot < 0) {
				hit = lineP1;
				return lineP1-point;
			} else if (dp.sqrMagnitude > d0.sqrMagnitude) { //check this
				hit=lineP2;
				return lineP2-point;
			} else {
				Vector2 proj0 = Util.Project (dp, d0);
				hit = lineP1 + proj0;
				return (dp - proj0);
			}
		}



		public Physics(GameInstance gameInstance) : base(gameInstance)
		{
		}

		public void CollisionCheck(){

			IEnumerator<Collider> cols0 = gameInstance.colliders.GetEnumerator();
			List<Collider> cols1 = new List<Collider> ();
			while (cols0.MoveNext ()) {
				Collider c = cols0.Current;
				if (c.active && !c.isTrigger) {
					cols1.Add (c);
				}
			}
			Util.Sort<Collider> (cols1);

			Dictionary<Collider, Vector2[]> movs = new Dictionary<Collider, Vector2[]> ();
			IEnumerator<Collider> selves = cols1.GetEnumerator ();
			while (selves.MoveNext ()) {
				Collider c = selves.Current;
				movs [c] = new Vector2[]{c.movement, c.movement};
			}

			int l = movs.Count;
			int i = 0;
			long timeStamp = this.timeStamp;
			CollisionBuilder builder = new CollisionBuilder ();
			List<TwinCollisionChild> allTCC = new List<TwinCollisionChild> ();
			int graze = 0;
			bool collidedWhile = false;
			do {
				collidedWhile = false;
				List<TwinCollisionChild> possibleCollisions = new List<TwinCollisionChild> ();
				selves.Reset();
				while (selves.MoveNext()){
					Collider self = selves.Current;
					IEnumerator<Collider> others = self.possiblyCollidingColliders.GetEnumerator();
					while(others.MoveNext()){
						TwinCollisionChild rIJ;
						if(self.CheckCollision(others.Current, out rIJ)){
							TwinCollisionChild tc = rIJ;
							String selfName = tc.self.gameObject.name;
							String otherName = tc.other.gameObject.name;
							/*if (selfName == "GreenBox" && otherName != "Land" && tc.otherDepenetration == Vector2.zero){
								UnityEngine.Debug.Log("2 GreenBox collided with " + otherName + ". tc.otherDepenetration=" + Util.ToString(tc.otherDepenetration)
									+ ". tc.similarDepenetration=" + Util.ToString(tc.similarDepenetration)
									+ ". tc.otherMovement=" + Util.ToString(tc.otherMovement));
							}
							if (selfName != "Land" && otherName == "GreenBox" && tc.selfDepenetration == Vector2.zero){
								UnityEngine.Debug.Log("2 " + selfName + " collided with GreenBox. tc.selfDepenetration=" + Util.ToString(tc.selfDepenetration)
									+ ". tc.similarDepenetration=" + Util.ToString(tc.similarDepenetration)
									+ ". tc.selfMovement=" + Util.ToString(tc.selfMovement));
							}*/
							possibleCollisions.Add(rIJ);
							if (rIJ.selfDepenetration != Vector2.zero  || rIJ.otherDepenetration != Vector2.zero || rIJ.similarDepenetration != Vector2.zero){
								collidedWhile = true;
							}
						}
					}
				}
				Util.Sort(possibleCollisions);


				if (collidedWhile){
					IEnumerator<TwinCollisionChild> en = possibleCollisions.GetEnumerator();
					IEnumerator<TwinCollisionChild> en2 = possibleCollisions.GetEnumerator();
					while(en.MoveNext()){
						TwinCollisionChild tc = en.Current;
						if(tc.toDelete){
							continue;
						}

						String selfName = tc.self.gameObject.name;
						String otherName = tc.other.gameObject.name;
						/*if (selfName == "GreenBox" && otherName != "Land" && tc.otherDepenetration == Vector2.zero){
						UnityEngine.Debug.Log("3 GreenBox collided with " + otherName + ". tc.otherDepenetration=" + Util.ToString(tc.otherDepenetration)
							+ ". tc.similarDepenetration=" + Util.ToString(tc.similarDepenetration)
							+ ". tc.otherMovement=" + Util.ToString(tc.otherMovement));
					}
					if (selfName != "Land" && otherName == "GreenBox" && tc.selfDepenetration == Vector2.zero){
						UnityEngine.Debug.Log("3 " + selfName + " collided with GreenBox. tc.selfDepenetration=" + Util.ToString(tc.selfDepenetration)
							+ ". tc.similarDepenetration=" + Util.ToString(tc.similarDepenetration)
							+ ". tc.selfMovement=" + Util.ToString(tc.selfMovement));
					}*/
						if (tc.selfDepenetration == Vector2.zero && tc.otherDepenetration == Vector2.zero){
							continue;
						}
						movs[tc.self][1] += tc.selfDepenetration;
						movs[tc.other][1] += tc.otherDepenetration;


						en2.Reset();
						while(en2.MoveNext()){
							TwinCollisionChild tci = en2.Current;
							if(tci.toDelete || tci == tc){
								continue;
							}
							if (tci.self != tc.self 
								&& tci.self != tc.other
								&& tci.other != tc.self
								&& tci.other != tc.other
								&& !tc.self.possiblyCollidingColliders.Contains(tci.self)
								&& !tc.self.possiblyCollidingColliders.Contains(tci.other)
								&& !tc.other.possiblyCollidingColliders.Contains(tci.self)
								&& !tc.other.possiblyCollidingColliders.Contains(tci.other)){
								continue;
							}
							if (tci.self == tc.other && tci.other == tc.self){
								tci.toDelete = true;
							}
							if (tci.t > tc.t){
								tci.toDelete = true;
							}else if (tci.t == tc.t 
								&& (tci.self == tc.self
									|| tci.self == tc.other
									|| tci.other == tc.self
									|| tci.other == tc.other
									|| tci.self.possiblyCollidingColliders.Contains(tc.self)
									|| tci.self.possiblyCollidingColliders.Contains(tc.other)
									|| tci.other.possiblyCollidingColliders.Contains(tc.self)
									|| tci.other.possiblyCollidingColliders.Contains(tc.other))){
								/*if(!tci.CheckSelfRequirement2(movs[tci.self][1])
								|| !tci.CheckOtherRequirement2(movs[tci.other][1]
								)
							){
								tci.toDelete = true;
							}*/
								tci.toDelete = true;
							}
						}
					}

					possibleCollisions.RemoveAll((x) => x.toDelete);
				}
				foreach(TwinCollisionChild tc in possibleCollisions){
					if(tc.t > 1){
						tc.t = 1;
					}else if (tc.t < 0){
						tc.t = 0;
					}
					builder.Add(tc);
					allTCC.Add(tc);
					String selfName = tc.self.gameObject.name;
					String otherName = tc.other.gameObject.name;
					if (selfName == "GreenBox" && otherName != "Land" && tc.otherDepenetration == Vector2.zero){
						UnityEngine.Debug.Log("GreenBox collided with " + otherName + ". tc.otherDepenetration=" + Util.ToString(tc.otherDepenetration)
							+ ". tc.similarDepenetration=" + Util.ToString(tc.similarDepenetration)
							+ ". tc.otherMovement=" + Util.ToString(tc.otherMovement));
					}
					if (selfName != "Land" && otherName == "GreenBox" && tc.selfDepenetration == Vector2.zero){
						UnityEngine.Debug.Log(selfName + " collided with GreenBox. tc.selfDepenetration=" + Util.ToString(tc.selfDepenetration)
							+ ". tc.similarDepenetration=" + Util.ToString(tc.similarDepenetration)
							+ ". tc.selfMovement=" + Util.ToString(tc.selfMovement));
					}
				}
				builder.ApplyNotApplied();
				foreach(KeyValuePair<Collider, Vector2[]> kvp in movs){
					kvp.Value[0] = kvp.Key.movement;
					kvp.Value[1] = kvp.Value[0];
				}
				++i;
				if(i > 100){
					bool x = true;
					if (i > 200){
					UnityEngine.Debug.Log("STUCK");
					break;
					}
				}
			} while(collidedWhile);

			builder.BuildAndBroadcast ();
		}

		public void Update(float dt)
		{
			
			IEnumerator<Collider> cols = gameInstance.colliders.GetEnumerator();
			while (cols.MoveNext())
			{
				Collider col = cols.Current;
				if (!col.active)
				{
					continue;
				}
				col.PrePhysicsUpdate();
			}
			IEnumerator<Rigidbody> rbs = gameInstance.rigidbodies.GetEnumerator();
			while (rbs.MoveNext())
			{
				Rigidbody rb = rbs.Current;
				if (!rb.active)
				{
					continue;
				}
				rb.PreCollisionUpdate(dt);
			}
			cols.Reset();
			while (cols.MoveNext())
			{
				Collider col = cols.Current;
				if (!col.active)
				{
					continue;
				}
				col.GetPossiblyCollidingColliders();
			}
			CollisionCheck ();
			/*rbs.Reset();
			while (rbs.MoveNext())
			{
				Rigidbody rb = rbs.Current;
				if (!rb.active)
				{
					continue;
				}
				rb.CollisionCheck ();
			}*/
			cols.Reset();
			while (cols.MoveNext())
			{
				Collider col = cols.Current;
				if (!col.active)
				{
					continue;
				}
				col.GetPossiblyIntersectingColliders();
			}
			rbs.Reset();
			while (rbs.MoveNext())
			{
				Rigidbody rb = rbs.Current;
				if (!rb.active)
				{
					continue;
				}
				rb.TriggerCheck();
			}
			rbs.Reset();
			while (rbs.MoveNext())
			{
				Rigidbody rb = rbs.Current;
				if (!rb.active)
				{
					continue;
				}
				rb.PostCollisionUpdate(dt);
			}
			/*cols.Reset();
			while (cols.MoveNext())
			{
				Collider col = cols.Current;
				if (!col.active)
				{
					continue;
				}
				col.CollisionCheck();
			}*/
			cols.Reset();
			while (cols.MoveNext())
			{
				Collider col = cols.Current;
				if (!col.active)
				{
					continue;
				}
				col.CollisionBroadcast();
			}
			cols.Reset();
			while (cols.MoveNext())
			{
				Collider col = cols.Current;
				if (!col.active)
				{
					continue;
				}
				col.TriggerCheck();
			}
			cols.Reset();
			while (cols.MoveNext())
			{
				Collider col = cols.Current;
				if (!col.active)
				{
					continue;
				}
				col.TriggerBroadcast();
			}
			rbs.Reset();
			while (rbs.MoveNext())
			{
				Rigidbody rb = rbs.Current;
				if (!rb.active)
				{
					continue;
				}
				rb.PostPhysicsUpdate(dt);
			}
				
		}
	}
}

