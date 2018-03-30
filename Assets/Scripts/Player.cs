using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	Collider2D asd;
	DigBox digObject = null;
	public bool jump = false;
	public bool left = false;
	public bool right = false;
	public bool isGroundedGravity = false;
	public bool isGroundedRight = false;
	public Vector2 velocity = Vector2.zero;
	public Vector2 stepMovement = Vector2.zero;
	public float moveSpeed = 10;
	public bool flipControl = false;
	public Player other = null;
	public int control = 0;
	public KeyCode jumpButton;
	public Dropdown controlDropdown = null;
	public Toggle controlFlipToggle = null;
	public bool useGravity = true;
	public InputField speedField = null;
	KeyCode[] jumpKey = new KeyCode[]{
		KeyCode.W,
		KeyCode.T,
		KeyCode.I
	};
	KeyCode[] leftKey = new KeyCode[]{
		KeyCode.A,
		KeyCode.F,
		KeyCode.J
	};
	KeyCode[] rightKey = new KeyCode[]{
		KeyCode.D,
		KeyCode.H,
		KeyCode.L
	};
	// Use this for initialization
	void Start () {
		digObject = new DigBox (gameObject);
		digObject.rigidbody.useGravity = useGravity;
		digObject.prePhysicsUpdate1Callback = PrePhysicsUpdate1Callback;
		digObject.postPhysicsUpdate1Callback = PostPhysicsUpdate1Callback;
		if (controlDropdown != null) {
			SetControl ();
		}
		if (controlFlipToggle != null) {
			SetControlFlip ();
		}
		if (speedField != null) {
			SetSpeed ();
		}
	}
	public void SetControl(){
		control = controlDropdown.value;
	}

	public void SetControlFlip(){
		flipControl = controlFlipToggle.isOn;
	}

	public void SetSpeed(){
		string spd = speedField.text;
		float spd2;
		if (string.IsNullOrEmpty (spd)) {
			speedField.text = moveSpeed.ToString ();
		} else if (float.TryParse (spd, out spd2)) {
			moveSpeed = spd2;
		} else {
			speedField.text = moveSpeed.ToString ();
		}
	}
	public void FixedUpdate(){
		if (other != null) {
			digObject.other = other.digObject;
		}
	}
	public void PrePhysicsUpdate1Callback(float dt){
		if (jump) {
			jump = false;
			if (digObject.rigidbody.isGroundedGravity) {
				digObject.rigidbody.velocity += digObject.physics.gravity.normalized * -moveSpeed;
			} 
		}
		//if (digObject.rigidbody.isGroundedGravity){
			stepMovement = Vector2.zero;
		//}
		if (left) {
			left = false;
			//if (digObject.rigidbody.isGroundedGravity) {
				if (flipControl) {
					stepMovement += Vector2.right * moveSpeed * dt;
				} else {
					stepMovement += Vector2.left * moveSpeed * dt;
				}
			//}
		}
		if (right) {
			right = false;
			//if (digObject.rigidbody.isGroundedGravity) {
				if (flipControl) {
					stepMovement += Vector2.left * moveSpeed * dt;
				} else {
					stepMovement += Vector2.right * moveSpeed * dt;
				}
			//}
		}
		digObject.rigidbody.stepMovement = stepMovement;
		//this.velocity = digObject.rigidbody.velocity;
	}
	public void PostPhysicsUpdate1Callback(float dt){
		this.velocity = digObject.rigidbody.velocity;
		this.isGroundedGravity = digObject.rigidbody.isGroundedGravity;
		this.isGroundedRight = digObject.rigidbody.isGroundedRight;
	}
	void Update(){
		if (Input.GetKeyDown (jumpKey[control])) {
			jump = true;
		}
		if (Input.GetKey (leftKey[control])) {
			left = true;
		}
		if (Input.GetKey (rightKey[control])) {
			right = true;
		}
	}
	void LateUpdate(){
		//digObject.Draw ();
	}
}
