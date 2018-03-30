using System;
using System.Collections;
using System.Collections.Generic;

using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;


namespace DigEngine
{
	public class Rigidbody : Component
	{
		public static Type tCircleCollider = typeof(CircleCollider);
		public static Type tPointCollider = typeof(PointCollider);
		public static Type tPolygonCollider = typeof(PolygonCollider);
		public static Type tConvexPolygonCollider = typeof(ConvexPolygonCollider);
		public static Type tQuadCollider = typeof(QuadCollider);
		public static Type tRectangleCollider = typeof(RectangleCollider);
		internal Vector2 _velocity = Vector2.zero;
		private Vector2 _pendingVelocity = Vector2.zero;
		public Vector2 position {
			get {
				return transform.position;
			}
			set {
				if (gameObject.name == "GreenBox" && (value.x - transform.position.x) > 0) {
					bool x = true;
				}
				transform.position = value;
			}
		}
		internal Vector2 _movement = Vector2.zero;
		internal Vector2 movement {
			get {
				return _movement;
			}
			set {
				Vector2 mov = _movement;
				long timeStamp = this.timeStamp;
				if (gameObject.name == "GreenBox" && value.x > 0 && stepMovement.x < 0) {
					bool x = true;
				}
				if (gameObject.name == "GreenBox" && _movement != Vector2.zero && value == Vector2.zero && stepMovement.x < 0) {
					bool x = true;
				}
				_movement = value;
			}
		}
		public Vector2 velocity{
			get{
				return _velocity;
			}
			set{
				_pendingVelocity += value - _velocity;
			}
		}
		public Vector2 stepMovement = Vector2.zero;
		internal float t;

		public bool isGroundedGravity = false;
		public bool isGroundedUp = false;
		public bool isGroundedLeft = false;
		public bool isGroundedRight = false;
		public bool isGroundedDown = false;

		public static float groundMinimumCos = 0.5f;

		public bool useGravity = false;
		public Vector2 prevVel = Vector2.zero;


		public Rigidbody(GameObject gameObject) : base(gameObject)
		{
			GetColliders();
		}

		private List<Collider> _colliders = new List<Collider>();
		private List<Collider> _nonTriggerColliders = new List<Collider>();
		private List<Collider> _triggerColliders = new List<Collider>();
		protected override void ValidateDispose()
		{
			Util.Dispose(_colliders);
			_colliders = null;
			Util.Dispose(_nonTriggerColliders);
			_nonTriggerColliders = null;
			Util.Dispose(_triggerColliders);
			_triggerColliders = null;
			base.ValidateDispose();
		}
		protected override void ValidateClone()
		{
			base.ValidateClone();
			_colliders = Util.Clone(_colliders, timeStamp);
			_nonTriggerColliders = Util.Clone(_nonTriggerColliders, timeStamp);
			_triggerColliders = Util.Clone(_triggerColliders, timeStamp);
		}
		public Collider[] colliders
		{
			get
			{
				return _colliders.ToArray();
			}
		}
		public Collider[] triggerColliders
		{
			get
			{
				return _triggerColliders.ToArray();
			}
		}
		public Collider[] nonTriggerColliders
		{
			get
			{
				return _nonTriggerColliders.ToArray();
			}
		}
		public void AddCollider(Collider col)
		{
			col.rigidbody = this;
			if (!col.active)
			{
				return;
			}
			_colliders.Add(col);
			
			Util.TrySort(_colliders);
			if (col.isTrigger)
			{
				_triggerColliders.Add(col);
				Util.TrySort(_triggerColliders);
			}
			else
			{
				_nonTriggerColliders.Add(col);
				Util.TrySort(_nonTriggerColliders);
			}

		}
		public bool RemoveCollider(Collider col)
		{
			if (col.isTrigger)
			{
				return _colliders.Remove(col) || _triggerColliders.Remove(col);
			}
			else
			{
				return _colliders.Remove(col) || _nonTriggerColliders.Remove(col);
			}
		}

		public void SetColliderTrigger(Collider col)
		{
			if (col.isTrigger)
			{
				_nonTriggerColliders.Remove(col);
				_triggerColliders.Add(col);
				Util.TrySort(_triggerColliders);
			}
			else
			{
				_triggerColliders.Remove(col);
				_nonTriggerColliders.Add(col);
				Util.TrySort(_nonTriggerColliders);
			}
		}

		public List<Collider> GetColliders()
		{
			_colliders = new List<Collider>();
			_nonTriggerColliders = new List<Collider>();
			_triggerColliders = new List<Collider>();

			Collider[] myCols = gameObject.colliders;
			int count = myCols.Length;
			for (int i = 0; i < count; ++i)
			{
				Collider c = myCols[i];
				c.rigidbody = this;
				if (c.active)
				{
					AddCollider(c);
				}

			}
			List<Collider> childCols = gameObject.GetAllComponentsInChild<Collider>();
			int cCount = childCols.Count;
			for (int i = 0; i < cCount; ++i)
			{
				Collider c = childCols[i];
				if (c.active)
				{
					if (c.rigidbody == this)
					{
						AddCollider(c);
					}
					else if (c.rigidbody == null)
					{
						c.rigidbody = this;
						AddCollider(c);
					}
				}
			}

			return _colliders;
		}


		public void TriggerCheck()
		{

			Collider[] myCols = colliders;
			IEnumerator<Collider> cols = gameInstance.colliders.GetEnumerator();
			while (cols.MoveNext())
			{
				Collider c = cols.Current;
				if (c.rigidbody == this || c.gameObject.GetComponentInParent<Rigidbody>() == this)
				{
					continue;

				}

				foreach (Collider mc in myCols)
				{
					if ((!c.isTrigger && !mc.isTrigger) || mc.triggers.Contains(c)) {
						continue;
					}
					TwinCollisionChild r;
					bool result = mc.CheckCollision(c, out r);
					if (result)
					{
						c.AddTrigger (mc);
						mc.AddTrigger (c);
					}
				}
			}

		}
		public Vector2 preCollisionMovement = Vector2.zero;
		public void PreCollisionUpdate(float dt)
		{
			t = 0;
			isGroundedGravity = false;
			isGroundedUp = false;
			isGroundedLeft = false;
			isGroundedRight = false;
			isGroundedDown = false;
			if (useGravity)
			{
				_velocity += physics.gravity * dt;
			}
			_velocity += _pendingVelocity;
			movement = (_velocity+prevVel) * dt / 2 + stepMovement;
			preCollisionMovement = movement;
			prevVel = _velocity;
		}

		public Vector2 postCollisionMovement = Vector2.zero;
		public void PostCollisionUpdate(float dt)
		{
			postCollisionMovement = movement;
			transform.position += movement;
			_pendingVelocity = Vector2.zero;
			/*float x = _velocity.x;
			float y = _velocity.y;
			if (x * velocityChangeDueToCollision.x < 0) {
				if (Mathf.Abs (velocityChangeDueToCollision.x) > Mathf.Abs (x)) {
					x = 0;
				} else {
					x += velocityChangeDueToCollision.x;
				}
			}
			if (y * velocityChangeDueToCollision.y < 0) {
				if (Mathf.Abs (velocityChangeDueToCollision.y) > Mathf.Abs (y)) {
					y = 0;
				} else {
					y += velocityChangeDueToCollision.y;
				}
			}*/
			//_velocity = Util.Clamp (_velocity + velocityChangeDueToCollision, _velocity, Vector2.zero);
			//velocityChangeDueToCollision = Vector2.zero;
			//movement = Vector2.zero;
			prevVel = _velocity;
			_movement = Vector2.zero;
			stepMovement = Vector2.zero;
		}

		public void PostPhysicsUpdate(float dt)
		{

		}
	}
}

