using System;
using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public class CircleCollider : Collider
	{
		
		private float _radius = 0;
		public float radius
		{
			get
			{
				return _radius;
			}
			set
			{
				this._radius = value;
				float hr = _radius / 2;
				_bounds = new BoundingBox(new Vector2(-hr, -hr), new Vector2(hr, hr));
			}
		}

		public CircleCollider(GameObject gameObject, float radius) : base(gameObject)
		{
			this.radius = radius;
		}
		

		public override bool CheckCollision(Collider other, out TwinCollisionChild result)
		{
			throw new NotImplementedException();
		}
		public override bool CheckGraze(Collider other, out TwinCollisionChild result)
		{
			throw new NotImplementedException();
		}
			
		public override bool CheckTrigger(Collider other)
		{
			throw new NotImplementedException();
		}
		
		public override bool IsPointInCollider(Vector2 point)
		{
			throw new NotImplementedException();
		}

		public override Vector2 Distance (Vector2 point)
		{
			throw new NotImplementedException ();
		}

		public override Vector2 Distance (Collider other)
		{
			throw new NotImplementedException ();
		}
		public override void RecalculateBounds ()
		{
			throw new NotImplementedException ();
		}
	}
}
