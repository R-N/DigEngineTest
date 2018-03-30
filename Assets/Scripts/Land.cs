using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour {
	DigBox digObject = null;
	public Vector2 velocity = Vector2.zero;
	public Vector2 preCollisionMovement = Vector2.zero;
	public Vector2 postCollisionMovement = Vector2.zero;
	// Use this for initialization
	void Start () {
		digObject = new DigBox (gameObject);
		digObject.rigidbody.useGravity = false;
		digObject.postPhysicsUpdate1Callback = PostPhysicsUpdate1;
	}

	void PostPhysicsUpdate1(float dt){
		velocity = digObject.rigidbody.velocity;
		preCollisionMovement = digObject.rigidbody.preCollisionMovement;
		postCollisionMovement = digObject.rigidbody.postCollisionMovement;
	}

	void Update(){
		//digObject.Draw ();
	}
}
