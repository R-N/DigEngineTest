using System;
using System.Collections.Generic;
using System.Collections;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public class PolygonCollider : Collider
	{

		protected Vector2[] _points = new Vector2[4];
		public Vector2[] points
		{
			get
			{
				return _points;
			}
		}

		public override bool isTrigger {
			get {
				return false;
			}
			set {
				throw new Exception ("Trigger PolygonCollider is not supported yet");
			}
		}

		float _width;
		float _height;

		protected override void ValidateDispose()
		{
			_points = null;
			base.ValidateDispose();
		}
		protected override void ValidateClone()
		{
			base.ValidateClone();
			_points = (Vector2[])_points.Clone();
		}
			
		protected PolygonCollider(GameObject gameObject) : base(gameObject){

		}

		public PolygonCollider(GameObject gameObject, Vector2[] points) : base(gameObject){
			if (points.Length < 3) {
				throw new Exception ("Polygon needs to have at least 3 points");
			}
			this._points = points;
			RecalculateBounds ();
		}

		public override void RecalculateBounds(){
			float minX = float.PositiveInfinity;
			float minY = float.PositiveInfinity;
			float maxX = float.NegativeInfinity;
			float maxY = float.NegativeInfinity;

			int l = _points.Length;
			for (int i = 0; i < l; ++i) {
				Vector2 p = _points [i];
				if (p.x < minX) {
					minX = p.x;
				}
				if (p.y < minY) {
					minY = p.y;
				}
				if (p.x > maxX) {
					maxX = p.x;
				}
				if (p.y > maxY) {
					maxY = p.y;
				}
			}
			_bounds = new BoundingBox (minX, minY, maxX, maxY);
			
		}
		public bool CheckGraze(PolygonCollider other, out TwinCollisionChild result)
		{

			bool ret0 = false;

			result = null;

			Vector2 mov = movement;
			Vector2 oMov = other.movement;
			Vector2[] myPoints = worldPoints;
			Vector2[] otherPoints = other.worldPoints;
			int mpl = myPoints.Length;
			int mpl1 = mpl - 1;
			int opl = otherPoints.Length;
			int opl1 = opl - 1;
			Vector2 prevContactPoint = Vector2.zero;

			DotToLineCollisionResult r = new DotToLineCollisionResult ();

			for (int i = 0; i < mpl; ++i) {
				Vector2 p = myPoints [i];

				for (int j = 0; j < opl; ++j) {
					int j1 = Util.RotaryClamp(j - 1, 0, opl1);
					Vector2 p1 = otherPoints [j1];
					Vector2 p2 = otherPoints [j];
					DotToLineCollisionResult rIJ;
					if (Physics.MovingDotToMovingLineGraze (
						p,
						movement,
						p1,
						p2,
						other.movement,
						out rIJ) 
						&& (!ret0 || prevContactPoint != rIJ.collisionPoint)) {

						if (!ret0 || rIJ.t < r.t 
							||(
								rIJ.t == r.t 
								&& rIJ.dotMovement.sqrMagnitude < r.dotMovement.sqrMagnitude
								/*&& (
								!r.CheckDotRequirement2(mov+rIJ.dotDepenetration - rIJ.thinDot) 
								|| !r.CheckLineRequirement2(oMov+rIJ.lineDepenetration - rIJ.thinLine)
							)*/
							)
						)
						{
							bool collidedIJ = false;
							if (rIJ.r == 0) {
								if (Vector2.Dot(
									Util.Normal(myPoints[Util.RotaryClamp(i-1, 0, mpl1)], p),
									rIJ.lineNormal
								)== -1){
									collidedIJ = true;
								}
							} else if (rIJ.r == 1) {
								if (Vector2.Dot(
									Util.Normal(p, myPoints[Util.RotaryClamp(i+1, 0, mpl1)]),
									rIJ.lineNormal
								)== -1){

									collidedIJ = true;
								}
							} else {
								collidedIJ = true;
							}
							if(collidedIJ){
								ret0 = true;
								r = rIJ;
								//result = new TwinCollisionChild (this, other, rIJ);
							}
						}
					}
				}
			}
			if (ret0) {
				result = new TwinCollisionChild (this, other, r);
			}
			if (ret0 && gameObject.name == "GreenBox" && (result.selfDepenetration.x == 0.3125f || result.selfDepenetration.x == 0.3124999f)) {
				bool x = true;
			}
			if (ret0 && result.other.gameObject.name == "GreenBox" && (result.otherDepenetration.x == 0.3125f || result.otherDepenetration.x == 0.3124999f)) {
				bool x = true;
			}
			return ret0;
		}

		public virtual bool CheckCollision(PolygonCollider other, out TwinCollisionChild result)
		{
			bool ret0 = false;

			result = null;

			Vector2 mov = movement;
			Vector2 oMov = other.movement;
			Vector2[] myPoints = worldPoints;
			Vector2[] otherPoints = other.worldPoints;
			int mpl = myPoints.Length;
			int mpl1 = mpl - 1;
			int opl = otherPoints.Length;
			int opl1 = opl - 1;
			Vector2 prevContactPoint = Vector2.zero;

			DotToLineCollisionResult r = new DotToLineCollisionResult ();

			for (int i = 0; i < mpl; ++i) {
				Vector2 p = myPoints [i];

				for (int j = 0; j < opl; ++j) {
					int j1 = Util.RotaryClamp(j - 1, 0, opl1);
					Vector2 p1 = otherPoints [j1];
					Vector2 p2 = otherPoints [j];
					DotToLineCollisionResult rIJ;
					if (Physics.MovingDotToMovingLineCollision (
						p,
						movement,
						p1,
						p2,
						other.movement,
						out rIJ) 
						&& (!ret0 || prevContactPoint != rIJ.collisionPoint)) {

						if (!ret0 || rIJ.t < r.t 
							||(
								rIJ.t == r.t 
								&& rIJ.dotMovement.sqrMagnitude < r.dotMovement.sqrMagnitude
								/*&& (
								!r.CheckDotRequirement2(mov+rIJ.dotDepenetration - rIJ.thinDot) 
								|| !r.CheckLineRequirement2(oMov+rIJ.lineDepenetration - rIJ.thinLine)
							)*/
							)
						)
						{
							bool collidedIJ = false;
							if (rIJ.r == 0) {
								if (Vector2.Dot(
									Util.Normal(myPoints[Util.RotaryClamp(i-1, 0, mpl1)], p),
									rIJ.lineNormal
								)== -1){
									collidedIJ = true;
								}
							} else if (rIJ.r == 1) {
								if (Vector2.Dot(
									Util.Normal(p, myPoints[Util.RotaryClamp(i+1, 0, mpl1)]),
									rIJ.lineNormal
								)== -1){

									collidedIJ = true;
								}
							} else {
								collidedIJ = true;
							}
							if(collidedIJ){
								ret0 = true;
								r = rIJ;
								//result = new TwinCollisionChild (this, other, rIJ);
							}
						}
					}
				}
			}
			if (ret0) {
				result = new TwinCollisionChild (this, other, r);
			}
			if (ret0 && gameObject.name == "GreenBox" && (result.selfDepenetration.x == 0.3125f || result.selfDepenetration.x == 0.3124999f)) {
				bool x = true;
			}
			if (ret0 && result.other.gameObject.name == "GreenBox" && (result.otherDepenetration.x == 0.3125f || result.otherDepenetration.x == 0.3124999f)) {
				bool x = true;
			}
			return ret0;
		}

		public bool CheckTrigger(PolygonCollider other)
		{
			Vector2[] otherPoints = other.worldPoints;
			int opl = otherPoints.Length;
			for (int i = 0; i < opl; ++i)
			{
				if (this.IsPointInCollider(otherPoints[i]))
				{
					return true;
				}
			}
			Vector2[] myPoints = worldPoints;
			int mpl = myPoints.Length;
			for (int i = 0; i < mpl; ++i)
			{
				if (other.IsPointInCollider(myPoints[i]))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsPointInCollider(Vector2 point)
		{
			throw new Exception ("Method not implemented");
		}

		public override bool CheckCollision(Collider other, out TwinCollisionChild result)
		{
			Vector2 mov = movement;
			Vector2 oMov = other.movement;
			result = null;
			if (this.isTrigger || other.isTrigger ||(mov == Vector2.zero && oMov == Vector2.zero)) 
			{
				return false;
			}
			bool ret = false;
			Type t = other.GetType ();
			if (t == Rigidbody.tPolygonCollider || t.IsSubclassOf(Rigidbody.tPolygonCollider))
			{
				return CheckCollision((PolygonCollider)other, out result);
			}
			return ret;
		}
		public override bool CheckGraze(Collider other, out TwinCollisionChild result)
		{
			Vector2 mov = movement;
			Vector2 oMov = other.movement;
			result = null;
			if (this.isTrigger || other.isTrigger ||(mov == Vector2.zero && oMov == Vector2.zero)) 
			{
				return false;
			}
			bool ret = false;
			Type t = other.GetType ();
			if (t == Rigidbody.tPolygonCollider || t.IsSubclassOf(Rigidbody.tPolygonCollider))
			{
				return CheckGraze((PolygonCollider)other, out result);
			}
			return ret;
		}
		public override bool CheckTrigger(Collider other)
		{
			if (!this.isTrigger && !other.isTrigger)
			{
				return false;
			}
			bool ret = false;
			if (other.GetType() == Rigidbody.tPolygonCollider)
			{
				return CheckTrigger((PolygonCollider)other);
			}
			return ret;
		}
		public Vector2[] worldPoints
		{
			get
			{
				return transform.LocalToWorldPoints (points);
			}
		}


		public override Vector2 Distance (Vector2 point)
		{
			float _ret = float.PositiveInfinity;
			Vector2 ret = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

			Vector2[] points = worldPoints;
			int l = points.Length;
			Vector2 p01 = points [l-1];
			Vector2 p02 = points [0];

			ret = Physics.PointToLineDistance (point, p01, p02);
			_ret = ret.sqrMagnitude;

			for (int i = 1; i < l; ++i) {
				Vector2 reti = Physics.PointToLineDistance (point, points [i - 1], points [1]);
				float _reti = reti.sqrMagnitude;
				if (_reti < _ret) {
					ret = reti;
					_ret = _reti;
				}
			}
			return ret;

		}

		public Vector2 Distance(PolygonCollider other){
			float _ret = float.PositiveInfinity;
			Vector2 ret = new Vector2 (_ret, _ret);
			Vector2[] points = other.worldPoints;
			int l = points.Length;
			for (int i = 0; i < l; ++i) {
				Vector2 reti = Distance (points [i]);
				float _reti = reti.sqrMagnitude;
				if (_reti < _ret) {
					ret = reti;
					_ret = _reti;
				}
			}
			return ret;
		}

		public override Vector2 Distance (Collider other)
		{

			float _ret = float.PositiveInfinity;
			Vector2 ret = new Vector2 (float.PositiveInfinity, float.PositiveInfinity);
			Type t = other.GetType ();
			if (t == Rigidbody.tPolygonCollider || t.IsSubclassOf(Rigidbody.tPolygonCollider)) {
				return Distance ((PolygonCollider)other);
			}
			return ret;
		}
	}
}
