using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using DigEngine;
using Vector2=UnityEngine.Vector2;
public class Main : MonoBehaviour {
	public static GameInstance gameInstance = null;
	void Awake(){
		gameInstance = new GameInstance ();
		gameInstance.time.actualDeltaTime = UnityEngine.Time.fixedDeltaTime;
		gameInstance.physics.gravity = UnityEngine.Physics.gravity;
	}
	// Use this for initialization

	public bool movingDotLineCollision = false;
	public UnityEngine.Vector2 collisionPoint = UnityEngine.Vector2.zero;
	public UnityEngine.Vector2 p = new UnityEngine.Vector2 (0, 2);
	public UnityEngine.Vector2 pDir = new UnityEngine.Vector2(5, 0);
	public UnityEngine.Vector2 lineP1 = new UnityEngine.Vector2 (2, 0);
	public UnityEngine.Vector2 lineP2 = new UnityEngine.Vector2(2, 4);
	public UnityEngine.Vector2 lineDir = new UnityEngine.Vector2(0, 0);
	public UnityEngine.Vector2 dotDepenetration = Vector2.zero;
	public UnityEngine.Vector2 lineDepenetration = Vector2.zero;

	public UnityEngine.Vector2 postPDir = new UnityEngine.Vector2(5, 0);
	public UnityEngine.Vector2 postLineDir = new UnityEngine.Vector2(0, 0);

	public UnityEngine.Vector2 pRequirement = Vector2.zero;
	public UnityEngine.Vector2 lineRequirement = Vector2.zero;

	public UnityEngine.Vector2 postP = new UnityEngine.Vector2 (0, 2);
	public UnityEngine.Vector2 postLineP1 = new UnityEngine.Vector2 (0, 2);
	public UnityEngine.Vector2 postLineP2 = new UnityEngine.Vector2 (0, 2);

	public float t = 1.1f;

	public Camera cam = null;
	void Start () {
		DotToLineCollisionTestCalc ();
		if (cam == null) {
			cam = GetComponent<Camera> ();
		}
	}


	void LateUpdate(){
		DotToLineCollisionTestDraw ();
	}

	void DotToLineCollisionTestCalc(){
		Vector2 thin = Util.Rotate (Vector2.right, Mathf.PI / 3) * Util.thin;
		Debug.Log ("Diving");
		postPDir = pDir;
		postLineDir = lineDir;
		postP = p;
		postLineP1 = lineP1;
		postLineP2 = lineP2;

		bool curMovingDotLineCollision = false;
		Vector2 curP = p;
		Vector2 curPDir = postPDir;
		Vector2 curLineP1 = lineP1;
		Vector2 curLineP2 = lineP2;
		Vector2 curLineDir = postLineDir;
		DotToLineCollisionResult result;
		do {
			curMovingDotLineCollision = DigEngine.Physics.MovingDotToMovingLineCollision (
				curP,
				curPDir,
				curLineP1,
				curLineP2,
				curLineDir,
				out result
			);
			curP += result.dotMovement;
			curPDir += result.dotDepenetration - result.dotMovement;
			curLineP1 += result.lineMovement;
			curLineP2 += result.lineMovement;
			curLineDir += result.lineDepenetration - result.lineMovement;
			postLineP1 = curLineP1;
			postLineP2 = curLineP2;
			postP = curP;
			postPDir = curPDir;
			postLineDir = curLineDir;
			dotDepenetration = result.dotDepenetration;
			lineDepenetration = result.lineDepenetration;
			collisionPoint = result.collisionPoint;
			movingDotLineCollision = curMovingDotLineCollision;
			t = result.t;
			pRequirement = result.dotRequirement;
			lineRequirement = result.lineRequirement;
		} while(curMovingDotLineCollision);
		Debug.Log ("Yay");
	}

	void DotToLineCollisionTestDraw(){
		Debug.DrawRay (p, pDir, Color.cyan);
		Debug.DrawLine (p, postP, Color.red);
		Debug.DrawLine (lineP1, lineP2, Color.blue);
		Debug.DrawRay (lineP1, lineDir, Color.magenta);
		Debug.DrawRay (lineP2, lineDir, Color.magenta);
		Debug.DrawLine (lineP1 + lineDir, lineP2 + lineDir, Color.magenta);
		Debug.DrawLine (lineP1, postLineP1, Color.yellow);
		Debug.DrawLine (lineP2, postLineP2, Color.yellow);
		Debug.DrawLine (postLineP1, postLineP2, Color.yellow);
	}

	bool firstUpdate = true;
	// Update is called once per frame
	void FixedUpdate () {
		gameInstance.Update ();
		if (firstUpdate) {
			firstUpdate = false;
			if (gameInstance.time.time < UnityEngine.Time.time) {
				UnityEngine.Debug.Log ("MAKE AHEAD");
				gameInstance.Update ();
			}
		}
	}
	public float moveSpeed = 3;
	public float sizeSpeed = 40;
	void Update(){
		while (gameInstance.time.time < UnityEngine.Time.time) {
			gameInstance.Update ();
		}
		Vector3 dir = Vector3.zero;
		if (Input.GetKey (KeyCode.LeftArrow)) {
			dir += Vector3.left;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			dir += Vector3.right;
		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			dir += Vector3.up;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			dir += Vector3.down;
		}

		transform.position = transform.position + dir * moveSpeed * cam.orthographicSize * UnityEngine.Time.deltaTime;


		if (Input.GetKey (KeyCode.Z)) {
			cam.orthographicSize -= sizeSpeed * UnityEngine.Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.X)) {
			cam.orthographicSize += sizeSpeed * UnityEngine.Time.deltaTime;
		}

	}

}
