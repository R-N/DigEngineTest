using System;
using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;

namespace DigEngine
{
	public abstract class Collider : Component
	{
		protected bool _isTrigger = false;
		public virtual bool isTrigger{
			get{
				return _isTrigger;
			}
			set{
				_isTrigger = value;
			}
		}
		public Dictionary<Collider, Collision> collisions = new Dictionary<Collider, Collision>();
		public Dictionary<Collider, Collision> prevCollisions = new Dictionary<Collider, Collision>();
		public HashSet<Collider> triggers = new HashSet<Collider>();
		public HashSet<Collider> prevTriggers = new HashSet<Collider>();
		internal Rigidbody rigidbody;
		protected BoundingBox _bounds;
		internal float _t = 0;
		internal float t{
			get{
				if (rigidbody != null) {
					return Mathf.Max(rigidbody.t, _t);
				} else {
					return _t;
				}
			}
			set{
				if (rigidbody != null) {
					if (rigidbody.t >= _t) {
						rigidbody.t = Mathf.Max (value, rigidbody.t);
						_t = rigidbody.t;
					} else {
						_t = Mathf.Max (_t, value);
						rigidbody.t = _t;
					}
				} else {
					_t = Mathf.Max (_t, value);
				}
			}
		}
		protected override void ValidateDispose()
		{
			if (rigidbody != null)
			{
				rigidbody.Dispose();
				rigidbody = null;
			}
			Util.DisposeK(collisions);
			Util.DisposeK(prevCollisions);
			Util.Dispose(triggers);
			Util.Dispose(prevTriggers);
			base.ValidateDispose();
		}
		protected override void ValidateClone()
		{
			base.ValidateClone();
			if (rigidbody != null) rigidbody = (Rigidbody)rigidbody.GetClone(timeStamp);
			collisions = Util.CloneKDVC(collisions, timeStamp);
			prevCollisions = Util.CloneKDVC(prevCollisions, timeStamp);
			triggers = Util.Clone(triggers, timeStamp);
			prevTriggers = Util.Clone(prevTriggers, timeStamp);
		}
		public BoundingBox bounds
		{
			get
			{
				return _bounds;
			}
		}
		
		public Collider(GameObject gameObject) : base(gameObject){
			GetRigidbody();
		}

		public Rigidbody GetRigidbody()
		{
			this.rigidbody = gameObject.rigidbody;
			if (rigidbody == null)
			{
				this.rigidbody = gameObject.GetComponentInParent<Rigidbody>();
			}
			return this.rigidbody;
		}
		
		public Vector2 movement
		{
			get
			{
				return rigidbody == null ? Vector2.zero : rigidbody.movement;
			}
		}
		
		public override void SetActive(bool active)
		{
			base.SetActive(active);
			if (rigidbody != null)
			{
				if (active)
				{
					rigidbody.AddCollider(this);
				}
				else
				{
					rigidbody.RemoveCollider(this);
				}
			}
		}
		public virtual void PrePhysicsUpdate()
		{
			_t = 0;
			prevCollisions.Clear();
			prevCollisions = collisions;
			collisions = new Dictionary<Collider, Collision>();
			prevTriggers.Clear();
			prevTriggers = triggers;
			triggers = new HashSet<Collider>();
		}
		public abstract bool CheckGraze(Collider other, out TwinCollisionChild result);

		public abstract bool CheckCollision(Collider other, out TwinCollisionChild result);

		public abstract bool CheckTrigger(Collider other);

		public abstract bool IsPointInCollider(Vector2 point);

		public void CollisionBroadcast()
		{
			IEnumerator<Collision> cols = collisions.Values.GetEnumerator();
			while (cols.MoveNext())
			{
				Collision col = cols.Current;
				if (!prevCollisions.ContainsKey(col.other))
				{
					Transform t;
					for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
					{
						t.gameObject.OnCollisionEnter(col);
					}
					t.gameObject.OnCollisionEnter(col);
				}
			}
			cols.Reset();
			while (cols.MoveNext())
			{
				Collision col = cols.Current;
				if (prevCollisions.ContainsKey(col.other))
				{
					Transform t;
					for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
					{
						t.gameObject.OnCollisionStay(col);
					}
					t.gameObject.OnCollisionStay(col);
				}
			}
			IEnumerator<Collision> prevCols = prevCollisions.Values.GetEnumerator();
			while (prevCols.MoveNext())
			{
				Collision prevCol = prevCols.Current;
				if (!collisions.ContainsKey(prevCol.other))
				{
					Transform t;
					for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
					{
						t.gameObject.OnCollisionExit(prevCol);
					}
					t.gameObject.OnCollisionExit(prevCol);
				}
			}
		}


		public Trigger CreateTrigger(Collider other)
		{

			return new Trigger
			(
				this,
				 other
			);
		}

		public bool isGroundedGravity {
			get {
				if (rigidbody == null) {
					return false;
				} else {
					return rigidbody.isGroundedGravity;
				}
			}
		}
		public bool isGroundedUp {
			get {
				if (rigidbody == null) {
					return false;
				} else {
					return rigidbody.isGroundedUp;
				}
			}
		}
		public bool isGroundedDown {
			get {
				if (rigidbody == null) {
					return false;
				} else {
					return rigidbody.isGroundedDown;
				}
			}
		}
		public bool isGroundedLeft {
			get {
				if (rigidbody == null) {
					return false;
				} else {
					return rigidbody.isGroundedLeft;
				}
			}
		}

		public bool isGroundedRight {
			get {
				if (rigidbody == null) {
					return false;
				} else {
					return rigidbody.isGroundedRight;
				}
			}
		}

		public void TriggerBroadcast()
		{
			IEnumerator<Collider> others = triggers.GetEnumerator();
			while (others.MoveNext())
			{
				Collider other = others.Current;
				if (!prevTriggers.Contains(other))
				{

					Trigger trig = CreateTrigger(other);
					Transform t;
					for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
					{
						t.gameObject.OnTriggerEnter(trig);
					}
					t.gameObject.OnTriggerEnter(trig);
				}
			}
			others.Reset();
			while (others.MoveNext())
			{
				Collider other = others.Current;
				if (prevTriggers.Contains(other))
				{
					Trigger trig = CreateTrigger(other);
					Transform t;
					for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
					{
						t.gameObject.OnTriggerStay(trig);
					}
					t.gameObject.OnTriggerStay(trig);
				}
			}
			IEnumerator<Collider> prevOthers = prevTriggers.GetEnumerator();
			while (prevOthers.MoveNext())
			{
				Collider prevOther = prevOthers.Current;
				if (!triggers.Contains(prevOther))
				{
					Trigger prevTrig = CreateTrigger(prevOther);
					Transform t;
					for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
					{
						t.gameObject.OnTriggerExit(prevTrig);
					}
					t.gameObject.OnTriggerExit(prevTrig);
				}
			}
		}


		public void ImmediateCollisionBroadcast(CollisionChild col)
		{
			if (prevCollisions.ContainsKey(col.other))
			{
				Transform t;
				for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
				{
					t.gameObject.OnImmediateCollisionStay(col);
				}
				t.gameObject.OnImmediateCollisionStay(col);
				
			}
			else
			{
				Transform t;
				for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
				{
					t.gameObject.OnImmediateCollisionEnter(col);
				}
				t.gameObject.OnImmediateCollisionEnter(col);
			}
		}

		public void ImmediateTriggerBroadcast(Collider other)
		{
			Trigger trig = CreateTrigger(other);

			if (prevTriggers.Contains(other))
			{
				Transform t;
				for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
				{
					t.gameObject.OnImmediateTriggerStay(trig);
				}
				t.gameObject.OnImmediateTriggerStay(trig);
				
			}
			else
			{
				Transform t;
				for (t = transform; t.gameObject.rigidbody != this.rigidbody; t = t.parent)
				{
					t.gameObject.OnImmediateTriggerEnter(trig);
				}
				t.gameObject.OnImmediateTriggerEnter(trig);
			}
		}

		public bool AddCollision(Collision col)
		{
			/*if (collisions.ContainsKey(col.other))
			{
				Collision oldCol = collisions [col.other];
				List<Vector2> depenetrations = new List<Vector2> ();
				List<Vector2> contactPoints = new List<Vector2> ();
				List<Vector2> normals = new List<Vector2> ();
				List<float> ts = new List<float> ();
				int ol = oldCol.depenetrations.Length;
				int ol1 = ol - 1;
				for (int i = 0; i < ol; ++i) {
					Vector2 vd = oldCol.depenetrations [i];
					depenetrations.Add (vd);
					Vector2 vcp = oldCol.contactPoints [i];
					contactPoints.Add (vcp);
					Vector2 vn = oldCol.normals [i];
					normals.Add (vn);
					float t = oldCol.ts [i];
					ts.Add (t);
				}
				int l = col.depenetrations.Length;
				bool pass = false;
				for (int i = 0; i < l; ++i) {
					if (!pass && col.contactPoints[i] == oldCol.contactPoints[ol1]) {
						continue;
					}
					pass = true;
					Vector2 vd = col.depenetrations [i];
					depenetrations.Add (vd);
					Vector2 vcp = col.contactPoints [i];
					contactPoints.Add (vcp);
					Vector2 vn = col.normals [i];
					normals.Add (vn);
					float t = col.ts [i];
					ts.Add (t);
				}
				Collision newCol = new Collision(
					col.self,
					col.other,
					contactPoints.ToArray(),
					depenetrations.ToArray(),
					normals.ToArray(),
					ts.ToArray()
					);
				if (oldCol == newCol) {
					return false;
				} else {
					collisions [col.other] = newCol;
					//ImmediateCollisionBroadcast (newCol);
					return true;
				}
			}
			else
			{*/
			collisions [col.other] = col;

			if (rigidbody != null) {
				int l = col.ts.Length;
				Collision source = col;
				for (int k = 0; k < l; ++k) {
					Vector2 selfDepNorm = source.normals [k];
					if (Vector2.Dot (selfDepNorm, source.self.physics.negativeNormalizedGravity) >= Rigidbody.groundMinimumCos) {
						rigidbody.isGroundedGravity = true;
					}
					if (Vector2.Dot (selfDepNorm, source.self.transform.down) >= Rigidbody.groundMinimumCos) {
						rigidbody.isGroundedUp = true;
					}
					if (Vector2.Dot (selfDepNorm, source.self.transform.up) >= Rigidbody.groundMinimumCos) {
						rigidbody.isGroundedDown = true;
					}
					if (Vector2.Dot (selfDepNorm, source.self.transform.right) >= Rigidbody.groundMinimumCos) {
						rigidbody.isGroundedLeft = true;
					}
					if (Vector2.Dot (selfDepNorm, source.self.transform.left) >= Rigidbody.groundMinimumCos) {
						rigidbody.isGroundedRight = true;
					}
				}
			}
			return true;
		}

		public Vector2 velocity{
			get{
				if (rigidbody == null){
					return Vector2.zero;
				}else{
					return rigidbody.velocity;
				}
			}
		}

		public bool AddTrigger(Collider col)
		{
			if (triggers.Add (col)) {
				ImmediateTriggerBroadcast (col);
				return true;
			} else {
				return false;
			}
		}

		public abstract Vector2 Distance(Vector2 point);
		public Vector2 _Distance(Vector2 point){
			Vector2 ret = Distance (point);
				return ret;

		}

		public abstract Vector2 Distance(Collider other);

		public Vector2 _Distance(Collider other){
			Vector2 ret = Distance (other);

				return ret;

		}

		public abstract void RecalculateBounds();

		public void CollisionCheck(){
			IEnumerator<Collider> prevCols = prevCollisions.Keys.GetEnumerator();
			while (prevCols.MoveNext ()) {
				Collider c = prevCols.Current;
				if (collisions.ContainsKey (c)) {
					continue;
				}
				if (Mathf.Abs((transform.position - c.transform.position).sqrMagnitude - (
					((Transform)transform.previous).position - ((Transform)c.transform.previous).position).sqrMagnitude) <= Util.thin ){
					Collision selfOldCollision = this.prevCollisions [c];

					Collision selfCollision = new Collision (
						this, 
						c, 
						new Vector2[]{selfOldCollision.contactPoints[selfOldCollision.contactPoints.Length-1]}, 
						new Vector2[]{Vector2.zero}, 
						new Vector2[]{selfOldCollision.normals[selfOldCollision.normals.Length-1]},
						new float[]{0}
					);
					Collision otherOldCollision = c.prevCollisions [this];
					Collision otherCollision = new Collision (
						c,
						this,
						new Vector2[]{otherOldCollision.contactPoints[otherOldCollision.contactPoints.Length-1]}, 
						new Vector2[]{Vector2.zero}, 
						new Vector2[]{otherOldCollision.normals[otherOldCollision.normals.Length-1]},
						new float[]{0}
					);
					this.AddCollision (selfCollision);
					c.AddCollision (otherCollision);
				}
			}
		}

		public BoundingBox worldBounds {
			get {
				return new BoundingBox (transform.LocalToWorldPoint (_bounds.min), transform.LocalToWorldPoint (_bounds.max));
			}
		}

		private long _lastMovementBoundsTimeStamp = 0;
		private BoundingBox _movementBounds;

		internal BoundingBox __movementBounds{
			get{
				if (_lastMovementBoundsTimeStamp < timeStamp){
					_lastMovementBoundsTimeStamp = timeStamp;
					_movementBounds = movementBounds;
				}
				return _movementBounds;
			}
		}


		internal HashSet<Collider> possiblyCollidingColliders = new HashSet<Collider> ();
		public HashSet<Collider> GetPossiblyCollidingColliders(){
			possiblyCollidingColliders.Clear ();
			if (!active || _isTrigger) {
				return possiblyCollidingColliders;
			}
			BoundingBox movBounds = __movementBounds;
			IEnumerator<Collider> cols0 = gameInstance.colliders.GetEnumerator();
			while (cols0.MoveNext ()) {
				Collider c = cols0.Current;
				if (!c.active || c.isTrigger || c.rigidbody == rigidbody){// || possiblyCollidingColliders.Contains(c)) {
					continue;
				}
				if (movBounds.Intersect (c.__movementBounds)) {
					possiblyCollidingColliders.Add (c);
					//c.possiblyCollidingColliders.Add (this);
				}
			}
			return possiblyCollidingColliders;
		}
		internal HashSet<Collider> possibleIntersectingColliders = new HashSet<Collider> ();
		public HashSet<Collider> GetPossiblyIntersectingColliders(){
			possibleIntersectingColliders.Clear ();
			if (!active) {
				return possibleIntersectingColliders;
			}
				
			BoundingBox movBounds = __movementBounds;
			IEnumerator<Collider> cols0 = gameInstance.colliders.GetEnumerator();
			while (cols0.MoveNext ()) {
				Collider c = cols0.Current;
				if (!c.active || (_isTrigger && !c.isTrigger) || c.rigidbody == rigidbody) {
					continue;
				}
				if (movBounds.Intersect (c.__movementBounds)) {
					possibleIntersectingColliders.Add (c);
				}
			}
			return possibleIntersectingColliders;
		}

		public BoundingBox movementBounds {
			get {
				Vector2 mov = movement;
				if (mov == Vector2.zero) {
					return worldBounds;
				}
				BoundingBox boundsPreMovement = worldBounds;

				float minX = boundsPreMovement.min.x;
				float minY = boundsPreMovement.min.y;
				float maxX = boundsPreMovement.max.x;
				float maxY = boundsPreMovement.max.y;

				if (mov.x > 0) {
					maxX += mov.x;
				} else if (mov.x < 0) {
					minX += mov.x;
				}
				if (mov.y > 0) {
					maxY += mov.y;
				} else if (mov.y < 0) {
					minY += mov.y;
				}
				return new BoundingBox (minX, minY, maxX, maxY);
			}
		}

		public void TriggerCheck(){
			
			if (this.rigidbody != null) {
				return;
			}

			IEnumerator<Collider> cols = gameInstance.colliders.GetEnumerator();
			while (cols.MoveNext ()) {
				Collider c = cols.Current;
				if (c.rigidbody != null) {
					continue;
				}
				if (CheckTrigger (c)) {
					AddTrigger (c);
					c.AddTrigger (this);
				}
			}
		}
	}
}

