using UnityEngine;
using System.Collections;

public class DestructableItem : MonoBehaviour {

	public int Points = 0;
	public int Weight = 0;
	public GameObject NextGameObject = null;
	public AudioClip HitSound = null;
	public float AudioVolume = 0.8f;

	protected GameManager MyGameManager = null;
	protected Transform MyTransform = null;
	protected GameObject MyGameObject = null; 

	protected bool Dying = false; 

	protected virtual void Start () {
		MyGameManager = GameManager.Instance;
		MyTransform = transform;
		MyGameObject = gameObject;
		if (MyGameManager != null)
			MyGameManager.RegisterTimer ("Invincible");
	}
	
	protected virtual void FixedUpdate () {
		if (MyGameManager != null && MyTransform != null && MyGameManager.CurrentState == GameState.Playing && MyGameManager.GetTime("Wall Stop Timer") == 0) {
			MyTransform.position -= Vector3.up * Time.deltaTime * MyGameManager.WallSpeed;
		}
		if (MyGameManager.GetTime ("Invincible") > 0)
			GetComponent<Collider> ().isTrigger = true;
		else
			GetComponent<Collider> ().isTrigger = false;

	}

	protected virtual void Update () {
		if (MyTransform.position.y < -0.3f)
			MyGameManager.EndGame ();
	}

	protected virtual void OnCollisionEnter (Collision ball) {
		if (MyTransform.position.y > 12)
			return;
		if (Dying)
			return;
		if (MyGameManager.CurrentState == GameState.Playing) {
			if (NextGameObject != null) {
				Instantiate (NextGameObject, MyTransform.position, MyTransform.rotation);
			}
			KillBrick ();
		}
	}

	protected virtual void OnTriggerEnter (Collider ball) {
		if (MyTransform.position.y > 12)
			return;
		if (Dying)
			return;
		KillBrick ();

	}

	public virtual void KillBrick (){
		if (MyGameManager.CurrentState == GameState.Playing) {
			if (Dying)
				return;
			Dying = true;
			if (MyGameManager != null)
				MyGameManager.AddPoints (Points);
			if (HitSound)
				AudioSource.PlayClipAtPoint (HitSound, MyTransform.position, AudioVolume);
			DestroyObject (MyGameObject);
		}
	}
}
