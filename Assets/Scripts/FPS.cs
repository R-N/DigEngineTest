using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {
	public Text text = null;
	int i = 0;
	float[] dts = new float[10];
	// Use this for initialization
	void Start () {
		if (text == null) {
			text = GetComponent<Text> ();
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		for (int i = 1; i < 10; ++i) {
			dts [i - 1] = dts [i];
		}
		dts [9] = Time.deltaTime;

		if (dts [0] == 0) {
			return;
		}
		float fps = 0;
		for (int i = 0; i < 10; ++i) {
			fps += dts [i];
		}
		fps = 10 / fps;
		text.text = "FPS: " + fps;
	}
}
