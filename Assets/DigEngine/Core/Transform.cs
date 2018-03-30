using System;
using System.Collections.Generic;
using System.Collections;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;

namespace DigEngine
{
	public class Transform : Component
	{
		public Vector2 position{
			get{
				if (_parent == null) {
					return localPosition;
				} else {
					return LocalToWorldPoint (localPosition);
				}
			}
			set{
				if (gameObject.name == "GreenBox" && value.x > 0) {
					bool x = true;
				}
				if (_parent == null) {
					localPosition = value;
				} else {
					localPosition = WorldToLocalPoint (value);
				}
			}
		}
		public Vector2 localPosition = Vector2.zero;
		public Vector2 scale {
			get {
				if (_parent == null) {
					return localScale;
				} else {
					return Util.OneToOneMultiplication(_parent.scale, localScale);
				}
			}
			set{
				if (_parent == null) {
					localScale = value;
				} else {
					localScale = Util.OneToOneDivision (value, _parent.scale);
				}
			}
		}
		public Vector2 localScale = Vector2.one;
		public float localRotation = 0;
		public float rotation {
			get {
				if (_parent == null) {
					return localRotation;
				} else {
					return _parent.rotation + localRotation;
				}
			}
			set{
				if (_parent == null) {
					localRotation = value;
				} else {
					localRotation = value - _parent.rotation;
				}
			}
		}
		private Transform _parent = null;
		public Transform parent
		{
			get
			{
				return _parent;
			}
			set
			{
				if (value == null)
				{
					_parent._RemoveChild(this);
				}
				else
				{
					_parent._AddChild(this);
				}
				_parent = value;
			}
		}
		private List<Transform> childs = new List<Transform>();
		protected override void ValidateDispose()
		{
			if (_parent != null)
			{
				_parent.Dispose();
				_parent = null;
			}
			Util.Dispose(childs);
			childs = null;
			base.ValidateDispose();
		}
		protected override void ValidateClone()
		{
			base.ValidateClone();
			if (_parent != null) _parent = (Transform)_parent.GetClone(timeStamp);
			childs = Util.Clone<Transform>(childs, timeStamp);
		}
		public int childCount
		{
			get
			{
				return childs.Count;
			}
		}
		public Transform GetChild(int i = 0)
		{
			return childs[i];
		}
		public IEnumerator<Transform> GetChildEnumerator()
		{
			return childs.GetEnumerator();
		}
		public void _AddChild(Transform child)
		{
			childs.Add(child);
		}
		public void AddChild(Transform child)
		{
			child.parent = this;
		}
		public bool _RemoveChild(Transform child)
		{
			return childs.Remove(child);
		}
		public bool RemoveChild(Transform child)
		{
			if (child.parent != this)
			{
				return false;
			}
			child.parent = null;
			return true;
		}
		public bool RemoveChild(int i)
		{
			if (i < 0 || i >= childs.Count)
			{
				return false;
			}
			childs.RemoveAt(i);
			return true;
		}

		public Transform(GameObject gameObject, Vector2 position) : base(gameObject){
			this.localPosition = position;
		}

		public Transform(GameObject gameObject, Vector2 position, Vector2 scale) : this(gameObject, position)
		{
			this.localScale = scale;
		}

		public Vector2 LocalToWorldPoint(Vector2 point, float time)
		{
			Vector2 scale = LerpedLocalScale (time);
			point = Util.Rotate(Util.OneToOneMultiplication(point, scale), LerpedLocalRotation(time)) + LerpedLocalPosition(time);
			if (parent != null)
			{
				point = parent.LocalToWorldPoint(point, time);
			}
			return point;
		}
		public Vector2[] LocalToWorldPoints(Vector2[] points, float time)
		{
			int l = points.Length;
			Vector2[] ret = new Vector2[l];
			for (int i = 0; i < l; ++i) {
				ret [i] = LocalToWorldPoint (points [i], time);
			}
			return ret;
		}

		public Vector2 WorldToLocalPoint(Vector2 point, float time)
		{
		
			if (parent != null)
			{
				point = parent.WorldToLocalPoint(point, time);
			}
			point = point - LerpedLocalPosition(time);
			Vector2 scale = LerpedLocalScale (time);
			return Util.Rotate(Util.OneToOneDivision(point, scale), -LerpedLocalRotation(time));
		}
		public Vector2[] WorldToLocalPoints(Vector2[] points, float time)
		{
			int l = points.Length;
			Vector2[] ret = new Vector2[l];
			for (int i = 0; i < l; ++i) {
				ret [i] = WorldToLocalPoint (points [i], time);
			}
			return ret;
		}

		public Vector2 LocalToWorldPoint(Vector2 point)
		{
			point = Util.Rotate(Util.OneToOneMultiplication(point, localScale), localRotation) + localPosition;
			if (parent != null)
			{
				point = parent.LocalToWorldPoint(point);
			}
			return point;
		}
		public Vector2[] LocalToWorldPoints(Vector2[] points)
		{
			int l = points.Length;
			Vector2[] ret = new Vector2[l];
			for (int i = 0; i < l; ++i) {
				ret [i] = LocalToWorldPoint (points [i]);
			}
			return ret;
		}

		public Vector2 WorldToLocalPoint(Vector2 point)
		{

			if (parent != null)
			{
				point = parent.WorldToLocalPoint(point);
			}
			point = point - localPosition;
			return Util.Rotate(Util.OneToOneDivision(point, localScale), -localRotation);
		}
		public Vector2[] WorldToLocalPoints(Vector2[] points)
		{
			int l = points.Length;
			Vector2[] ret = new Vector2[l];
			for (int i = 0; i < l; ++i) {
				ret [i] = WorldToLocalPoint (points [i]);
			}
			return ret;
		}

		public Vector2 LocalToWorldDirection(Vector2 dir){
			dir = Util.Rotate (dir, localRotation);
			if (parent != null) {
				return parent.LocalToWorldDirection (dir);
			} else {
				return dir;
			}
		}

		public Vector2 WorldToLocalDirection(Vector2 dir){
			if (parent != null) {
				dir = parent.WorldToLocalDirection (dir);
			} 
			return Util.Rotate (dir, -localRotation);
		}

		public Vector2 LocalToWorldVector(Vector2 v){
			v = Util.OneToOneMultiplication (Util.Rotate (v, localRotation), localScale);
			if (parent != null) {
				return parent.LocalToWorldVector (v);
			} else {
				return v;
			}
		}

		public Vector2 WorldToLocalVector(Vector2 v){
			if (parent != null) {
				v = parent.WorldToLocalVector (v);
			} 
			return Util.OneToOneDivision(Util.Rotate (v, -localRotation), localScale);
		}

		public Vector2 LerpedPosition(float time){
			if (previous == null) {
				return position;
			}
			float t = (this.time.time - time) / this.time.deltaTime;
			if (t < 0 || t > 1) {
				UnityEngine.Debug.Log ("t " + t);
			}
			return Vector2.Lerp (((Transform)previous).position, position, t);
		}

		public float LerpedRotation(float time){
			if (previous == null) {
				return rotation;
			}
			float t = (this.time.time - time) / this.time.deltaTime;
			if (t < 0 || t > 1) {
				UnityEngine.Debug.Log ("t " + t);
			}
			return Mathf.Lerp (((Transform)previous).rotation, rotation, t);
		}
		public Vector2 LerpedScale(float time){
			if (previous == null) {
				return scale;
			}
			float t = (this.time.time - time) / this.time.deltaTime;
			if (t < 0 || t > 1) {
				UnityEngine.Debug.Log ("t " + t);
			}
			return Vector2.Lerp (((Transform)previous).scale, scale, t);
		}
		public Vector2 LerpedLocalPosition(float time){
			if (previous == null) {
				return localPosition;
			}
			float t = (this.time.time - time) / this.time.deltaTime;
			if (t < 0 || t > 1) {
				UnityEngine.Debug.Log ("t " + t);
			}
			return Vector2.Lerp (((Transform)previous).localPosition, localPosition, t);
		}

		public float LerpedLocalRotation(float time){
			if (previous == null) {
				return localRotation;
			}
			float t = (this.time.time - time) / this.time.deltaTime;
			if (t < 0 || t > 1) {
				UnityEngine.Debug.Log ("t " + t);
			}
			return Mathf.Lerp (((Transform)previous).localRotation, localRotation, t);
		}
		public Vector2 LerpedLocalScale(float time){
			if (previous == null) {
				return localScale;
			}
			float t = (this.time.time - time) / this.time.deltaTime;
			if (t < 0 || t > 1) {
				UnityEngine.Debug.Log ("t " + t);
			}
			return Vector2.Lerp (((Transform)previous).localScale, localScale, t);
		}

		public Vector2 up{
			get{
				return LocalToWorldDirection (Vector2.up);
			}
		}
		public Vector2 down{
			get{
				return LocalToWorldDirection (Vector2.down);
			}
		}
		public Vector2 left{
			get{
				return LocalToWorldDirection (Vector2.left);
			}
		}
		public Vector2 right{
			get{
				return LocalToWorldDirection (Vector2.right);
			}
		}

		public override bool GetActive()
		{
			return gameObject.active;
		}
		
		public override bool active
		{
			get
			{
				return gameObject.active;
			}
		}
	}
}

