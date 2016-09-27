using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	public Color PaddleColor = Color.black;
	public float MouseSensitivity = 0.6f;
	private GameManager MyGameManager = null;
	private Transform MyTransform = null;

	void Awake () {
		MyGameManager = GameManager.Instance;
		MyTransform = transform;
		GetComponent<Renderer>().material.color = PaddleColor; 
	}
	
	// Update is called once per frame
	void Update () {
		if (MyGameManager != null && MyGameManager.CurrentState != GameState.Paused) {
			float delta = Input.GetAxis ("Mouse X") * MouseSensitivity;
			MyTransform.position += new Vector3(delta,0,0);

			if (Input.GetMouseButtonDown (0))
				MyGameManager.ReleaseBall ();
			if (MyTransform.position.x < -9.97f)
				MyTransform.position = new Vector3(-9.97f,MyTransform.position.y, MyTransform.position.z);
			else if (MyTransform.position.x > 9.97f)
				MyTransform.position = new Vector3(9.97f,MyTransform.position.y, MyTransform.position.z);
		}

	}
}
