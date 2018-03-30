using System;
using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;

namespace DigEngine
{
	public class GameInstance : DigObject
	{
		public static Type tGameObject = typeof(GameObject);
		public static Type tBehaviour = typeof(Behaviour);
		public static Type tRigidbody = typeof(Rigidbody);
		public static Type tCollider = typeof(Collider);

		public List<DigObject> digObjects = new List<DigObject>();
		public List<GameObject> gameObjects = new List<GameObject>();
		public List<Behaviour> behaviours = new List<Behaviour>();
		public List<Rigidbody> rigidbodies = new List<Rigidbody>();
		public List<Collider> colliders = new List<Collider>();

		public long lastId = 0;
		private Physics _physics = null;
		public override Physics physics
		{
			get
			{
				return _physics;
			}
		}
		private Time _time = null;
		public override Time time
		{
			get
			{
				return _time;
			}
		}
		
		protected override void ValidateDispose()
		{
			Util.Dispose(behaviours);
			behaviours = null;
			Util.Dispose(rigidbodies);
			rigidbodies = null;
			Util.Dispose(colliders);
			colliders = null;
			Util.Dispose(gameObjects);
			gameObjects = null;
			
			_physics.Dispose();
			_physics = null;
			_time.Dispose();
			_time = null;
			base.ValidateDispose();
		}
		protected override void ValidateClone()
		{
			base.ValidateClone();
			_time = _time.GetClone<Time>(timeStamp);
			_physics = _physics.GetClone<Physics>(timeStamp);
			gameObjects = Util.Clone(gameObjects, timeStamp);
			behaviours = Util.Clone(behaviours, timeStamp);
			rigidbodies = Util.Clone(rigidbodies, timeStamp);
			colliders = Util.Clone(colliders, timeStamp);
		}

		public GameInstance()
		{
			this._gameInstance = this;
			this._id = 0;
			digObjects.Add(this);
			this._time = new Time(this);
			this._physics = new Physics(this);
		}
		public long Init(DigObject x)
		{
			long id = x.id;
			if (id == 0L)
			{
				id = ++lastId;
				x.id = id;
				digObjects.Add(x);
			}
			
			Type t = x.GetType();
			if (t == tGameObject)
			{
				gameObjects.Add((GameObject)x);
			}
			else if (t == tRigidbody)
			{
				rigidbodies.Add((Rigidbody)x);
			}
			else if (t.IsSubclassOf(tBehaviour))
			{
				behaviours.Add((Behaviour)x);
			}
			else if (t.IsSubclassOf(tCollider))
			{
				colliders.Add((Collider)x);
			}
			return id;
		}

		public GameObject CreateGameObject(Vector2 position)
		{
			return new GameObject(this, position);
		}
		public GameObject CreateGameObject(Vector2 position, Vector2 scale)
		{
			return new GameObject(this, position, scale);
		}



		public void Update()
		{
			DisposeOld();
			time.PreUpdate();
			IEnumerator<DigObject> all = digObjects.GetEnumerator();
			while (all.MoveNext())
			{
				all.Current.UpdateTimeStamp();
			}
			float dt = time.deltaTime;
			
			
			
			IEnumerator<GameObject> en = gameObjects.GetEnumerator();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				go.TryAwake1();
			}
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				go.TryAwake2();
			}
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				if (go.active)
				{
					go.TryStart1();
				}
			}
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				if (go.active)
				{
					go.TryStart2();
				}
			}
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				if (go.active)
				{
					go.PrePhysicsUpdate1(dt);
				}
			}
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				if (go.active)
				{
					go.PrePhysicsUpdate2(dt);
				}
			}
			
			physics.Update(dt);
			
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				if (go.active)
				{
					go.PostPhysicsUpdate1(dt);
				}
			}
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				if (go.active)
				{
					go.PostPhysicsUpdate2(dt);
				}
			}
			en.Reset();
			while (en.MoveNext())
			{
				GameObject go = en.Current;
				if (go.active)
				{
					go.PostUpdate();
				}
			}
			time.PostUpdate();
			GetClone(timeStamp);
		}
	}
}
