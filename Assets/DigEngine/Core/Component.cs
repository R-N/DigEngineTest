using System;

namespace DigEngine
{
	public abstract class Component : DigObject
	{
		protected GameObject _gameObject = null;
		public bool destroyed
		{
			get
			{
				return _gameObject.destroyed;
			}
		}
		public GameObject gameObject
		{
			get
			{
				return _gameObject;
			}
		}
		public Transform transform
		{
			get
			{
				return _gameObject.transform;
			}
		}
		
		protected override void ValidateDispose()
		{
			_gameObject.Dispose();
			_gameObject = null;
			base.ValidateDispose();
		}
		
		protected override void ValidateClone()
		{
			base.ValidateClone();
			_gameObject = (GameObject)_gameObject.GetClone(timeStamp);
		}

		public Component(GameObject gameObject) : base(gameObject.gameInstance)
		{
			this._gameObject = gameObject;
			gameObject.AddComponent(this);
		}

		public override bool GetActive()
		{
			return this._active && gameObject.active;
		}
		
	}

	
}

