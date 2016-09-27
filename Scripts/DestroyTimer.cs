using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {
	private float timer = 0.0f;
	public float DestroyTime = 2.0f;
	private GameObject MyGameObject = null;
	// Use this for initialization
	void Start () {
		MyGameObject = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= DestroyTime)
			Destroy (MyGameObject);
	}
}
