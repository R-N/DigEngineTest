using System;
using System.Collections;
using System.Collections.Generic;
using Vector2=UnityEngine.Vector2;
using Mathf=UnityEngine.Mathf;
namespace DigEngine
{
	public static class Util
	{
		public static Type tDigObject = typeof(DigObject);
		public static T[] Clone<T>(T[] arr, long timeStamp) where T:DigObject
		{
			int l = arr.Length;
			T[] ret = new T[l];
			for (int i = 0; i < l; ++i)
			{
				ret[i] = (T)arr[i].GetClone(timeStamp);
			}
			return ret;
		}
		public static T[] Clone<T>(T[] arr) where T:ICloneable
		{
			int l = arr.Length;
			T[] ret = new T[l];
			for (int i = 0; i < l; ++i)
			{
				ret[i] = (T)arr[i].Clone();
			}
			return ret;
		}

		public static void Dispose<T>(T[] arr) where T : DigObject
		{
			int l = arr.Length;
			for (int i = 0; i < l; ++i)
			{
				arr[i].Dispose();
				arr[i] = null;
			}
		}
			

		public static List<T> Clone<T>(IEnumerable<T> en, long timeStamp) where T:DigObject
		{
			return Clone(en.GetEnumerator(), timeStamp);
		}

		public static List<T> Clone<T>(IEnumerator<T> en, long timeStamp) where T:DigObject
		{
			en.Reset();
			List<T> ret = new List<T>();
			while (en.MoveNext())
			{
				ret.Add((T)en.Current.GetClone(timeStamp));
			}
			return ret;
		}
		public static List<T> Clone<T>(IEnumerable<T> en) where T:ICloneable
		{
			return Clone(en.GetEnumerator());
		}

		public static List<T> Clone<T>(IEnumerator<T> en) where T:ICloneable
		{
			en.Reset();
			List<T> ret = new List<T>();
			while (en.MoveNext())
			{
				ret.Add((T)en.Current.Clone());
			}
			return ret;
		}
		
		public static void Dispose<T>(IList<T> en) where T:DigObject
		{
			Dispose(en.GetEnumerator());
			en.Clear();
		}
		
		public static void Dispose<T>(IEnumerable<T> en) where T:DigObject
		{
			Dispose(en.GetEnumerator());
		}

		public static void Dispose<T>(IEnumerator<T> en) where T:DigObject
		{
			en.Reset();
			while (en.MoveNext())
			{
				en.Current.Dispose();
			}
		}

		public static Dictionary<K, V> CloneV<K, V>(IDictionary<K, V> dict, long timeStamp) where V : DigObject
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[kv.Key] = (V)kv.Value.GetClone(timeStamp);
			}
			return ret;
		}

		public static Dictionary<K, V> CloneK<K, V>(IDictionary<K, V> dict, long timeStamp) where K:DigObject
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[(K)kv.Key.GetClone(timeStamp)] = kv.Value;
			}
			return ret;
		}

		public static Dictionary<K, V> CloneKDVD<K, V>(IDictionary<K, V> dict, long timeStamp) where K:DigObject where V:DigObject
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[(K)kv.Key.GetClone(timeStamp)] = (V)kv.Value.GetClone(timeStamp);
			}
			return ret;
		}
		
		public static Dictionary<K, V> CloneV<K, V>(IDictionary<K, V> dict) where V : ICloneable
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[kv.Key] = (V)kv.Value.Clone();
			}
			return ret;
		}

		public static Dictionary<K, V> CloneK<K, V>(IDictionary<K, V> dict) where K:ICloneable
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[(K)kv.Key.Clone()] = kv.Value;
			}
			return ret;
		}

		public static Dictionary<K, V> CloneKCVC<K, V>(IDictionary<K, V> dict) where K:ICloneable where V:ICloneable
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[(K)kv.Key.Clone()] = (V)kv.Value.Clone();
			}
			return ret;
		}
		
		public static Dictionary<K, V> CloneKDVC<K, V>(IDictionary<K, V> dict, long timeStamp) where K:DigObject where V:ICloneable
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[(K)kv.Key.GetClone(timeStamp)] = (V)kv.Value.Clone();
			}
			return ret;
		}
		public static Dictionary<K, V> CloneKCVD<K, V>(IDictionary<K, V> dict, long timeStamp) where K:ICloneable where V:DigObject
		{
			Dictionary<K, V> ret = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> kv in dict)
			{
				ret[(K)kv.Key.Clone()] = (V)kv.Value.GetClone(timeStamp);
			}
			return ret;
		}
		
		
		public static void DisposeV<K, V>(IDictionary<K, V> dict) where V : DigObject
		{
			foreach (KeyValuePair<K, V> kv in dict)
			{
				kv.Value.Dispose();
			}
			dict.Clear();
		}

		public static void DisposeK<K, V>(IDictionary<K, V> dict) where K:DigObject
		{
			foreach (KeyValuePair<K, V> kv in dict)
			{
				kv.Key.Dispose();
			}
			dict.Clear();
		}

		public static void DisposeKV<K, V>(IDictionary<K, V> dict) where K:DigObject where V:DigObject
		{
			foreach (KeyValuePair<K, V> kv in dict)
			{
				kv.Key.Dispose();
				kv.Value.Dispose();
			}
			dict.Clear();
		}

		public static HashSet<T> Clone<T>(HashSet<T> set, long timeStamp) where T:DigObject
		{
			HashSet<T> ret = new HashSet<T>();
			foreach (T t in set)
			{
				ret.Add((T)t.GetClone(timeStamp));
			}
			return ret;
		}

		public static void Dispose<T>(HashSet<T> set) where T:DigObject
		{
			foreach (T t in set)
			{
				t.Dispose();
			}
			set.Clear();
		}
		public static bool CheckMovementRequirement(Vector2 requirement, Vector2 movement, Vector2 otherMovement, Vector2 normal){
			if (movement == Vector2.zero) {
				return false;
			}
			if (requirement != Vector2.zero) {
				float dot2 = Vector2.Dot (movement, requirement);
				if (dot2 <= 0) {
					return false;
				}
			}
			float dot = Vector2.Dot (movement-otherMovement, normal);
			if (dot >= 0) {
				return false;
			}
			return Util.Project (movement, requirement).sqrMagnitude >= requirement.sqrMagnitude;
		}
		public static Vector2 MaxRequirement(Vector2 a, Vector2 b){
			float x = Mathf.Abs (a.x) > Mathf.Abs (b.x) ? a.x : b.x;
			float y = Mathf.Abs (a.y) > Mathf.Abs (b.y) ? a.y : b.y;
			return new Vector2 (x, y);
		}
		public static void SortByDistance(List<Collider> cols, Collider col)
		{
			cols.Sort (new ColliderDistanceComparer (col));
			//cols.Sort((a, b) => col._Distance(a).CompareTo(col._Distance(b)));
		}
		public static void Sort(List<TwinCollisionChild> cols){
			//cols.Sort ((a, b) => a.t.CompareTo (b.t));
			cols.Sort (new CollisionComparer ());
			//cols.Sort ((a, b) => a.ts [0].CompareTo (b.ts [0]));
		}
		public static void Sort<T>(List<T> list) where T:DigObject
		{
			list.Sort((a, b) => a.id.CompareTo(b.id));
		}
		public static bool TrySort<T>(List<T> list) where T:DigObject
		{
			int c = list.Count;
			if (c > 1 && list[c - 2].id > list[c - 1].id)
			{
				Sort(list);
				return true;
			}
			return false;
		}

		public static float Cross(Vector2 lhs, Vector2 rhs){
			return lhs.x * rhs.y - rhs.x * lhs.y;
		}
		public static Vector2 Project(Vector2 a, Vector2 b){
			if (a == Vector2.zero || b == Vector2.zero) {
				return Vector2.zero;
			}
			return (Vector2.Dot (a, b) / b.sqrMagnitude) * b;
		}
		public static Vector2 ProjectIfNotZero(Vector2 a, Vector2 b){
			if (a == Vector2.zero) {
				return Vector2.zero;
			}else if (b == Vector2.zero) {
				return a;
			}
			return (Vector2.Dot (a, b) / b.sqrMagnitude) * b;
		}

		public static Vector2 Reject(Vector2 a, Vector2 b){
			if (a == Vector2.zero) {
				return Vector2.zero;
			}
			return a - Project(a, b);
		}
		private static float _thin = 0.0001f;
		private static float _thin2 = _thin * 2;
		private static float _halfThin = _thin /2;
		private static float _sqrThin = _thin*_thin;
		public static float thin{
			get{
				return _thin;
			}
			set{
				value = Mathf.Abs (value);
				_thin = value;
				_thin2 = value*2;
				_halfThin = _thin / 2;
				_sqrThin = value * value;
			}
		}

		public static float thin2{
			get{
				return _thin2;
			}
			set{
				value = Mathf.Abs (value);
				_thin2 = value;
				_thin = value/2;
				_sqrThin = _thin * _thin;
				_halfThin = _thin / 2;
			}
		}

		public static float sqrThin{
			get{
				return _sqrThin;
			}
			set{
				value = Mathf.Abs (value);
				_sqrThin = value;
				_thin = Mathf.Sqrt (_sqrThin);
				_thin2 = _thin / 2;
				_halfThin = _thin / 2;
			}
		}

		public static float halfThin{
			get{
				return _halfThin;
			}
			set{
				value = Mathf.Abs (value);
				_halfThin = value;
				_thin = value * 2;
				_thin2 = value * 4;
				_sqrThin = _thin * _thin;
			}
		}
		public static Vector2 Thin(Vector2 a){
			if (a == Vector2.zero) {
				return a;
			}
			return a.normalized * _thin;
		}

		public static Vector2 Rotate(Vector2 v, float rad){
			float cos = Mathf.Cos (rad);
			float sin = Mathf.Sin (rad);
			return new Vector2 (cos * v.x - sin * v.y, sin * v.x + cos * v.y);
		}
		public static float PI2 = Mathf.PI/2;
		public static Vector2 Normal(Vector2 p1, Vector2 p2){
			return Normal (p2 - p1);
		}
		public static Vector2 Normal(Vector2 v){
			return new Vector2(-v.y, v.x).normalized;
		}
		public static int RotaryClamp(int x, int min, int max){
			int diff = max - min + 1;
			while (x > max) {
				x -= diff;
			}
			while (x < min) {
				x += diff;
			}
			return x;
		}

		public static float Clamp(float x, float limit1, float limit2){
			float max = Mathf.Max (limit1, limit2);
			float min = Mathf.Min (limit1, limit2);
			if (x < min) {
				return min;
			}
			if (x > max) {
				return max;
			}
			return x;
		}

		public static Vector2 Clamp(Vector2 v, Vector2 limit1, Vector2 limit2){
			return new Vector2 (Clamp (v.x, limit1.x, limit2.x), Clamp (v.y, limit1.y, limit2.y));

		}

		public static string ToString(Vector2 v){
			return String.Format("({0}, {1})", v.x, v.y);
		}

		public static Vector2 OneToOneMultiplication(Vector2 lhs, Vector2 rhs){
			return new Vector2(lhs.x * rhs.x, lhs.y * rhs.y);
		}
		public static Vector2 OneToOneDivision(Vector2 lhs, Vector2 rhs){
			return new Vector2(lhs.x / rhs.x, lhs.y / rhs.y);
		}
		public static float Min(float a, float b){
			if (a < b) {
				return a;
			} else {
				return b;
			}
		}

		public static void Extend<T>(List<T> main, List<T> tail){
			IEnumerator<T> en = tail.GetEnumerator ();
			while (en.MoveNext ()) {
				main.Add (en.Current);
			}
		}
	}
}
