using System;
using System.Collections.Generic;
using System.Collections;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;

namespace DigEngine
{
	public class GameObject : DigObject{
		private long _id;
		public long id
		{
			get
			{
				return _id;
			}
		}
		private Transform _transform = null;
		public Transform transform {
			get {
				return _transform;
			}
		}
		private Collider _collider = null;
		public Collider collider {
			get {
				return _collider;
			}
		}
		private Rigidbody _rigidbody = null;
		public Rigidbody rigidbody {
			get {
				return _rigidbody;
			}
		}
		
		private List<Component> components = new List<Component>();
		private List<Behaviour> behaviours = new List<Behaviour>();
		internal List<Collider> __colliders = new List<Collider>();
		private Collider[] _colliders = new Collider[0];
		private bool __destroyed = false;
		private bool _destroyed = false;

		public String name = "";
		
		protected override void ValidateDispose()
		{
			_transform.Dispose();
			_transform = null;
			if (_collider != null)
			{
				_collider.Dispose();
				_collider = null;
			}
			if (_rigidbody != null)
			{
				_rigidbody.Dispose();
				_rigidbody = null;
			}
			Util.Dispose<Component>(components);
			components = null;
			Util.Dispose<Behaviour>(behaviours);
			behaviours = null;
			Util.Dispose<Collider>(__colliders);
			__colliders = null;
			Util.Dispose<Collider>(_colliders);
			_collider = null;
			name = null;
			base.ValidateDispose();
		}
		
		protected override void ValidateClone()
		{
			base.ValidateClone();
			_transform = (Transform)_transform.GetClone(timeStamp);
			if (_collider != null) _collider = (Collider)_collider.GetClone(timeStamp);
			if (_rigidbody != null) _rigidbody = (Rigidbody)_rigidbody.GetClone(timeStamp);
			components = Util.Clone<Component>(components, timeStamp);
			behaviours = Util.Clone<Behaviour>(behaviours, timeStamp);
			__colliders = Util.Clone<Collider>(__colliders, timeStamp);
			_colliders = Util.Clone<Collider>(_colliders, timeStamp);
			name = (string)name.Clone ();
		}
		public bool destroyed
		{
			get
			{
				return _destroyed;
			}
		}
		
		public Collider[] colliders
		{
			get
			{
				return _colliders;
			}
		}



		public GameObject(GameInstance gameInstance, Vector2 position, string name = "") : this(gameInstance, position, Vector2.one, name){

		}

		public GameObject(GameInstance gameInstance, Vector2 position, Vector2 scale, string name="") : base(gameInstance)
		{
			this.name = name;
			_transform = new Transform(this, position, scale);
		}

		public void AddComponent(Component c) 
		{	
			components.Add (c);
			Util.TrySort(components);
			Type t = c.GetType();
			if (t == GameInstance.tRigidbody)
			{
				this._rigidbody = (Rigidbody)c;
				((Rigidbody)c).GetColliders();
			}else if (t.IsSubclassOf(GameInstance.tCollider)){
				this._collider = (Collider)c;
				((Collider)c).GetRigidbody();
				this.__colliders.Add((Collider)c);
				Util.TrySort(__colliders);
				this._colliders = __colliders.ToArray();
			}
			else if (t.IsSubclassOf(GameInstance.tBehaviour))
			{
				behaviours.Add((Behaviour)c);
				Util.TrySort(behaviours);
			}
				
		}


		public T GetComponent<T>(int i = 0) where T : Component
		{
			return GetComponent<T>(i, out i);
		}
		public T GetComponent<T>(int i, out int o) where T:Component{
			IEnumerator<Component> en = components.GetEnumerator ();
			Type tType = typeof(T);
			while (en.MoveNext ()) {
				Component cur = en.Current;
				Type tCur = cur.GetType ();
				if(tCur == tType || tCur.IsSubclassOf(tType)){
					if (i <= 0) {
						o = i;
						return (T)cur;
					} else {
						--i;
					}
				}
			}
			o = i;
			return null;
		}


		public List<T> GetAllComponents<T>() where T:Component{
			IEnumerator<Component> en = components.GetEnumerator ();
			Type tType = typeof(T);
			List<T> ret = new List<T> ();
			while (en.MoveNext ()) {
				Component cur = en.Current;
				Type tCur = cur.GetType ();
				if(tCur == tType || tCur.IsSubclassOf(tType)){
					ret.Add ((T)cur);
				}
			}
			return ret;
		}
		
		public T GetComponentInParent<T>(int i = 0) where T : Component
		{
			return GetComponentInParent<T>(i, out i);
		}

		public T GetComponentInParent<T>(int i, out int o) where T : Component
		{
			if (transform.parent == null)
			{
				o = i;
				return null;
			}
			else
			{
				T ret = transform.parent.gameObject.GetComponent<T>(i, out i);
				if (ret == null)
				{
					ret = transform.parent.gameObject.GetComponentInParent<T>(i, out i);
				}
				o = i;
				return ret;
			}
		}
		public T GetComponentInChild<T>(int i = 0) where T : Component
		{
			return GetComponentInChild<T>(i, out i);
		}

		public T GetComponentInChild<T>(int i, out int o) where T : Component
		{
			if (transform.childCount == 0)
			{
				o = i;
				return null;
			}
			else
			{
				IEnumerator<Transform> childs = transform.GetChildEnumerator();
				T ret = null;
				while (childs.MoveNext())
				{
					ret = childs.Current.gameObject.GetComponent<T>(i, out i);
					if (ret != null)
					{
						o = i;
						return ret;
					}
				}
				childs.Reset();
				while (childs.MoveNext())
				{
					ret = childs.Current.gameObject.GetComponentInChild<T>(i, out i);
					if (ret != null)
					{
						o = i;
						return ret;
					}
				}
				o = i;
				return ret;
			}
		}
		
		public List<T> GetAllComponentsInChild<T>() where T : Component
		{
			List<T> rets = new List<T>();
			if (transform.childCount == 0)
			{
				return rets;
			}
			else
			{
				IEnumerator<Transform> childs = transform.GetChildEnumerator();
				while (childs.MoveNext())
				{
					List<T> rets2 = childs.Current.gameObject.GetAllComponents<T>();
					for (int i = 0; i < rets.Count; ++i)
					{
						rets.Add(rets2[i]);
					}
				}
				childs.Reset();
				while (childs.MoveNext())
				{
					List<T> rets2 = childs.Current.gameObject.GetAllComponentsInChild<T>();
					for (int i = 0; i < rets.Count; ++i)
					{
						rets.Add(rets2[i]);
					}
				}
				return rets;
			}
		}

		public bool RemoveComponent(Component x){
			return components.Remove (x);
		}
		public bool RemoveComponent(Behaviour x){
			return components.Remove(x) || behaviours.Remove(x);
		}
		public bool RemoveComponent(Rigidbody x){
			Collider[] cols = x.colliders;
			int l = cols.Length;
			for (int i = 0; i < l; ++i)
			{
				if (cols[i].rigidbody == x)
				{
					cols[i].GetRigidbody();
				}
			}
			bool ret = _rigidbody == x;
			if (ret)
			{
				_rigidbody = null;
			}
			ret = ret || components.Remove(x);
			return ret;
		}
		public bool RemoveComponent(Collider x){
		
			bool ret = __colliders.Remove(x) || components.Remove(x);
			if (_collider == x)
			{
				_collider = null;
				ret = true;
			}
			_colliders = __colliders.ToArray();
			if (x.rigidbody != null)
			{
				x.rigidbody.GetColliders();
			}
			return ret;
		}

		public bool RemoveComponent<T>(int i = 0) where T:Component{
			IEnumerator<Component> en = components.GetEnumerator ();
			Type tType = typeof(T);
			int j = 0;
			while (en.MoveNext ()) {
				Component cur = en.Current;
				Type tCur = cur.GetType ();
				if(tCur == tType || tCur.IsSubclassOf(tType)){
					if (i <= 0) {
						if (tCur == GameInstance.tRigidbody)
						{
							RemoveComponent((Rigidbody)cur);
						}
						else if (tCur.IsSubclassOf(GameInstance.tCollider))
						{
							RemoveComponent((Collider)cur);
						}else
						if (tCur.IsSubclassOf (GameInstance.tBehaviour)) {
							RemoveComponent ((Behaviour)cur);
						}
							
						return true;
					} else {
						--i;
					}
				}
				++j;
			}
			return false;
		}
		public bool RemoveAllComponents<T>() where T:Component{
			Type tType = typeof(T);
			
			bool removed = false;
			int count = components.Count;
			for (int j = 0; j < count; ++j){
				Component cur = components[j];
				Type tCur = cur.GetType ();
				if(tCur == tType || tCur.IsSubclassOf(tType)){
					if (tCur == GameInstance.tRigidbody)
					{
						RemoveComponent((Rigidbody)cur);
					}
					else if (tCur.IsSubclassOf(GameInstance.tCollider))
					{
						RemoveComponent((Collider)cur);
					}
					else
					if (tCur.IsSubclassOf(GameInstance.tBehaviour))
					{
						RemoveComponent((Behaviour)cur);
					}
					else
					{
						components.RemoveAt(j);
					}
					--j;
					removed = true;
				}
			}
			return removed;
		}

		public void PostUpdate()
		{
			if (this.__destroyed != this._destroyed)
			{
				this._destroyed = this.__destroyed;
				if (this._destroyed)
				{
					IEnumerator<Component> en = components.GetEnumerator();
					while (en.MoveNext())
					{
						en.Current.OnDestroy();
					}
				}
				else
				{
					IEnumerator<Component> en = components.GetEnumerator();
					while (en.MoveNext())
					{
						en.Current.RevertDestroy();
					}
				}
			}
		}


		public void TryAwake1(){
			IEnumerator<Component> en = components.GetEnumerator ();
			while (en.MoveNext ()) {
				Component cur = en.Current;
				cur.TryAwake1 ();
			}
		}
		public void TryAwake2(){
			IEnumerator<Component> en = components.GetEnumerator ();
			while (en.MoveNext ()) {
				Component cur = en.Current;
				cur.TryAwake2 ();
			}
		}

		public void TryStart1(){
			IEnumerator<Component> en = components.GetEnumerator ();
			while (en.MoveNext ()) {
				Component cur = en.Current;
				if (cur.active) {
					cur.TryStart1();
				}
			}
		}

		public void TryStart2(){
			IEnumerator<Component> en = components.GetEnumerator ();
			while (en.MoveNext ()) {
				Component cur = en.Current;
				if (cur.active) {
					cur.TryStart2();
				}
			}
		}

		public void PrePhysicsUpdate1(float dt){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.PrePhysicsUpdate1 (dt);
				}
			}
		}

		public void PrePhysicsUpdate2(float dt){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.PrePhysicsUpdate2 (dt);
				}
			}
		}

		public void PostPhysicsUpdate1(float dt){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.PostPhysicsUpdate1 (dt);
				}
			}
		}

		public void PostPhysicsUpdate2(float dt){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.PostPhysicsUpdate2 (dt);
				}
			}
		}
		public override void Awake1(){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				cur.Awake1 ();
			}
		}
		public override void Awake2(){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				cur.Awake2 ();
			}
		}
		public override void Start1(){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.Start1 ();
				}
			}
		}
		public override void Start2(){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.Start2 ();
				}
			}
		}
		
		public override bool GetActive()
		{
			return base.GetActive() && !destroyed;
		}

		public void Destroy()
		{
			this.__destroyed = true;
		}

		
		public void OnCollisionEnter(Collision col){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnCollisionEnter (col);
				}
			}
		}
		public void OnCollisionStay(Collision col){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnCollisionStay (col);
				}
			}
		}
		public void OnCollisionExit(Collision col){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnCollisionExit (col);
				}
			}
		}
		
		public void OnTriggerEnter(Trigger trig){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnTriggerEnter (trig);
				}
			}
		}
		public void OnTriggerStay(Trigger trig){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnTriggerEnter (trig);
				}
			}
		}
		public void OnTriggerExit(Trigger trig){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnTriggerEnter (trig);
				}
			}
		}
		public void OnImmediateCollisionEnter(CollisionChild col){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnImmediateCollisionEnter (col);
				}
			}
		}
		public void OnImmediateCollisionStay(CollisionChild col){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnImmediateCollisionStay (col);
				}
			}
		}
		
		public void OnImmediateTriggerEnter(Trigger trig){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnImmediateTriggerEnter (trig);
				}
			}
		}
		public void OnImmediateTriggerStay(Trigger trig){
			IEnumerator<Behaviour> en = behaviours.GetEnumerator ();
			while (en.MoveNext ()) {
				Behaviour cur = en.Current;
				if (cur.active) {
					cur.OnImmediateTriggerEnter (trig);
				}
			}
		}

	}
}

