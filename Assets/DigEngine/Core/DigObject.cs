using System;
using System.Collections.Generic;
namespace DigEngine
{
	public abstract class DigObject : ICloneable
	{
		private bool disposed = false;
		protected long _id = 0;
		public long id
		{
			get
			{
				return _id;
			}
			set
			{
				if (_id == 0)
				{
					_id = value;
				}
			}
		}
		
		

		protected GameInstance _gameInstance = null;
		public GameInstance gameInstance
		{
			get
			{
				return _gameInstance;
			}
			
		}

		internal long timeStamp;
		internal Dictionary<long, DigObject> saveStates = new Dictionary<long, DigObject>();

		private Holder<DigObject> _main = null; //it's a holder object for ease on SetMain
		public DigObject main
		{
			get
			{
				return _main.value;
			}
		}
		
		public DigObject previous
		{
			get
			{
				if (saveStates.ContainsKey(timeStamp - 1))
				{
					return saveStates[timeStamp - 1];
				}
				else if (saveStates.ContainsKey(timeStamp))
				{
					DigObject ret = saveStates[timeStamp];
					if (ret != this)
					{
						return ret;
					}
					else
					{
						return null;
					}
				}
				else
				{
					return null;
				}
				//return _previous;
			}
		}

		public DigObject next
		{
			get
			{
				if (saveStates.ContainsKey(timeStamp + 1))
				{
					return saveStates[timeStamp + 1];
				}
				else if (main != this)
				{
					return main;
				}
				else
				{
					return null;
				}
				//return _next;
			}
		}

		public object Clone()
		{
			return GetClone();
		}

		public DigObject GetClone()
		{
			return GetClone(timeStamp);
		}

		public DigObject GetClone(long timeStamp) //this prefers the savestates, and creates one if doesn't exist and has the same timestamp
		{
			if (saveStates.ContainsKey(timeStamp))
			{
				return saveStates[timeStamp];
			}
			if (timeStamp == this.timeStamp)
			{
				DisposeOld();
				DigObject clone = (DigObject)this.MemberwiseClone();
				saveStates[timeStamp] = clone; //this must be done before ValidateClone to avoid infinite recursion
				clone.ValidateClone();
				return clone;
			}
			else
			{
				throw new Exception("Timestamp is too old. Timestamp: " + timeStamp + ". This timestamp: " + this.timeStamp);
			}
		}
		public T GetClone<T>() where T : DigObject
		{
			return (T)GetClone();
		}

		public T GetClone<T>(long timeStamp) where T : DigObject
		{
			return (T)GetClone(timeStamp);
		}

		public void SetAsMain()
		{
			if (main == this)
			{
				return;
			}
			_main.value = this;
			long i = timeStamp; //we also remove this from the savestates because savestates are for past objects
			while (saveStates.ContainsKey(i))
			{
				DigObject doi = saveStates[i];
				if (doi != this) doi.Dispose();
				saveStates.Remove(i);
			}
			
		}

		public static int saveStatesLimit = 3;

		public void DisposeOld()
		{
			if (saveStates.ContainsKey(timeStamp - saveStatesLimit))
			{
				saveStates[timeStamp - saveStatesLimit].Dispose();
			}
		}

		public void Dispose()
		{
			//it's a chain dispose, just like GetClone, 
			//the rules are the same but instead of cloning you're disposing
			//also, you call base.Dispose() at the end not at the beginning
			
			//this is for recursion
			if (disposed)
			{
				return;
			}
			disposed = true;

			ValidateDispose();
			
			//we also remove this from saveStates if exists
			if (saveStates.ContainsKey(this.timeStamp))
			{
				saveStates.Remove(this.timeStamp);
			}
		}

		protected virtual void ValidateDispose()
		{
			//it's a chain dispose, just like ValidateClone, 
			//the rules are the same but instead of cloning you're disposing
			//also, you call base.ValidateDispose() at the end not at the beginning
			
			//we dispose all references and dereference all references
			if (this._gameInstance != null)
			{
				this._gameInstance.Dispose();
				this._gameInstance = null;
			}
			//just to be safe
			this._main = null;
		}

		public DigObject GetInstance(long timeStamp) //this prefers the main object
		{
			if (timeStamp == this.timeStamp)
			{
				return this;
			}
			else
			{
				return saveStates[timeStamp];
			}
		}

		protected virtual void ValidateClone()
		{
			//this must be overridden if the subclass has new reference fields that it needs to clone
			//String and arrays are also refrence types
			//Structs are value types, don't need to be handled specially unless you want to do something with em
			//the overriding method must still call base.ValidateClone() first

			//_main is the reference to the 'present' object, the 'main' object
			//_main already and still refers to the 'main' object due to MemberwiseClone's shallow copy 
			//so we're not gonna touch _main

			//here we clone the _gameInstance, though it's not really necessary, but just to be sure, right?
			this._gameInstance = (GameInstance)this._gameInstance.GetClone(timeStamp);

			//saveStates isn't cloned so all saveStates will share the same dictionary
			
		}

		internal void RemoveFromTimeline() //removal of obsolete objects that will not be touched again
		{
			saveStates.Remove(this.timeStamp);
		}
		protected internal DigObject() {
			this._main = new Holder<DigObject>(this);
		}

		public DigObject(GameInstance gameInstance) : this()
		{
			this._gameInstance = gameInstance;
			this._id = gameInstance.Init(this);
		}

		public void UpdateTimeStamp()
		{
			this.timeStamp = gameInstance.time.timeStamp;
		}
		
		public virtual Time time
		{
			get
			{
				return gameInstance.time;
			}
		}
		
		public virtual Physics physics
		{
			get
			{
				return gameInstance.physics;
			}
		}

		protected bool _active = true;
		public virtual bool active
		{
			get
			{
				return GetActive();
			}
			set{
				SetActive(value);
				if (value)
				{
					TryStart2();
				}
			}
		}
		public virtual bool GetActive()
		{
			return _active;
		}
		public virtual void SetActive(bool active)
		{
			this._active = active;
			if (active)
			{
				TryStart2();
			}
		}

		private bool awoken1 = false;
		public void TryAwake1()
		{
			if (!awoken1)
			{
				awoken1 = true;
				Awake1();
			}
		}
		private bool awoken2 = false;
		public void TryAwake2(){
			if (!awoken2)
			{
				awoken2 = true;
				TryAwake1();
				Awake2();
			}

		}

		public virtual void Awake1()
		{

		}

		public virtual void Awake2()
		{

		}

		private bool started1 = false;
		public void TryStart1()
		{
			if (!started1)
			{
				started1 = true;
				TryAwake2();
				Start1();
			}
		}
		private bool started2 = false;
		public void TryStart2(){
			if (!started2)
			{
				started2 = true;
				TryStart1();
				Start2();
			}

		}

		public virtual void Start1()
		{

		}

		public virtual void Start2()
		{

		}

		public virtual void OnDestroy()
		{
			
		}
		public virtual void RevertDestroy()
		{
			
		}
		
	}


}
