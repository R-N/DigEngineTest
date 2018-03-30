using System;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public class RectangleCollider : QuadCollider
	{

		float _width;
		float _height;



		public float width{
			get{
				return _width;
			}
			set{
				_width = value;
				Recalculate();
			}
		}


		public float height{
			get{
				return _height;
			}
			set{
				_height = value;
				Recalculate();
			}
		}
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
		private void Recalculate()
		{
			float hw = _width / 2;
			float hh = _height / 2;
			_points[0] = new Vector2(-hw, hh);
			_points[1] = new Vector2(hw, hh);
			_points[2] = new Vector2(hw, -hh);
			_points[3] = new Vector2(-hw, -hh);
			_bounds = new BoundingBox(botLeft, topRight);
		}

		public RectangleCollider(GameObject gameObject, float width, float height) : base(gameObject){
			_points = new Vector2[4];
			this._width = width;
			this._height = height;
			Recalculate ();
		}

	}
}
