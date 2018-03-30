using System;
using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public class PointCollider : Collider
	{
		public PointCollider(GameObject gameObject) : base(gameObject)
		{
		}
		
		public override bool CheckCollision(Collider other, out TwinCollisionChild result)
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
		public override bool CheckGraze(Collider other, out TwinCollisionChild result)
		{
			throw new NotImplementedException();
		}
	}
}
