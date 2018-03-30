using System.Collections;
using System.Collections.Generic;
using DigEngine;
using Vector2 = UnityEngine.Vector2;
using System;

public class DigBox : DigEngine.Behaviour {
	public DigEngine.RectangleCollider collider = null;
	public DigEngine.Rigidbody rigidbody = null;
	public UnityEngine.GameObject unityObject = null;
	public Action<float> prePhysicsUpdate1Callback = null;
	public Action<float> postPhysicsUpdate1Callback = null;
	public DigBox other = null;

	public DigBox(Vector2 position, Vector2 scale, float width, float height, String name="") : base(
		new DigEngine.GameObject (Main.gameInstance, position, scale, name)
	){
		collider = new DigEngine.RectangleCollider (gameObject, width, height);
		rigidbody = new DigEngine.Rigidbody (gameObject);
	}
	public DigBox(UnityEngine.GameObject unityObject, float width, float height):this(
		unityObject.transform.position,
		unityObject.transform.localScale,
		width,
		height,
		unityObject.name
	){
		this.unityObject = unityObject;
	}
	public DigBox(UnityEngine.GameObject unityObject):this(
		unityObject,
		1,
		1
	){
		UnityEngine.Debug.Log ("Added GameObject " + _id +  ": '" + unityObject.name + "'");
		this.unityObject = unityObject;
	}

	public override void PrePhysicsUpdate1(float dt){
		if (prePhysicsUpdate1Callback != null) {
			prePhysicsUpdate1Callback (dt);
		}
	}
	public override void PostPhysicsUpdate1(float dt){
		unityObject.transform.position = transform.LerpedPosition(UnityEngine.Time.time);
		if (postPhysicsUpdate1Callback != null) {
			postPhysicsUpdate1Callback (dt);
		}
	}

	public void Draw(){
		UnityEngine.Vector2[] p = transform.LocalToWorldPoints(collider.points, UnityEngine.Time.time);
		UnityEngine.Debug.DrawLine (p[3], p[0]);
		for (int i = 1; i < 4; ++i) {
			UnityEngine.Debug.DrawLine (p[i-1], p[i]);
		}
	}

	public override void OnCollisionEnter(Collision col){
	}

	public override void OnCollisionStay(Collision col){
	}

	public override void OnCollisionExit(Collision col){
	}
}
