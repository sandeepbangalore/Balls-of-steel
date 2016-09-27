using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	public float BallSpeed = 10;
	public float BigBallMultiplier = 2.5f;
	public float BallSpeedMultiplier =1.6f; 
	public GameObject GlowBallEffect = null;
	public Material NormalBallMaterial = null;
	public Material GlowBallMaterial = null;

	private GameManager MyGameManager = null;
	private Transform MyTransform = null;
	private Rigidbody MyRigidBody = null;

	private Vector3 OriginalBallScale = Vector3.zero;
	private Vector3 BigBallScale = Vector3.zero;

	private float OriginalSpeed = 0.0f;
	private float FastSpeed = 0.0f;

	// Use this for initialization
	void Awake () {
		MyGameManager = GameManager.Instance;
		MyTransform = transform;
		MyRigidBody = GetComponent<Rigidbody> ();
		OriginalBallScale = MyTransform.localScale;
		BigBallScale = MyTransform.localScale * BigBallMultiplier;
		OriginalSpeed = BallSpeed;
		FastSpeed = OriginalSpeed * BallSpeedMultiplier;

		if (MyGameManager.CurrentState == GameState.Playing) {
			MyGameManager.RegisterBall ();
		}

		gameObject.SetActiveRecursively(false);
		gameObject.active = true;
		CheckBallState ();
	}
	

	void FixedUpdate () {
		Vector3 vel = MyRigidBody.velocity;
		if (MyGameManager.CurrentState == GameState.Playing) {			
			if (Mathf.Abs (vel.y) < 3.0f) {
				if (vel.y < 0)
					vel.y = -3.0f;
				else
					vel.y = 3.0f;
				MyRigidBody.velocity = vel;
			}
		}
		if (MyTransform.position.y < -4.5f) {
			MyGameManager.UnregisterBall ();
			Destroy (gameObject);
		}
	}
	void Update () {
		if (MyGameManager.CurrentState == GameState.Playing) 
			Time.timeScale = 1;
		else if (MyGameManager.CurrentState == GameState.Paused) 
			Time.timeScale = 0;			
	}

	void CheckBallState () {
		if (MyGameManager != null) {
			float t = MyGameManager.GetTime ("Speed Ball");
			if (t > 0)
				BallSpeed = FastSpeed;
			else
				BallSpeed = OriginalSpeed;
			float invincibleTime = MyGameManager.GetTime ("Invincible");
			float bigBallTime = MyGameManager.GetTime ("Big Ball Time");

			if (invincibleTime > 0) {
				if (GlowBallEffect != null && GlowBallEffect.active == false) {
					GlowBallEffect.SetActiveRecursively (true);
					if (GetComponent<Renderer> ().material && GlowBallMaterial)
						GetComponent<Renderer> ().material = GlowBallMaterial;
				}
			} else {
				if (GlowBallEffect != null && GlowBallEffect.active == true) {
					GlowBallEffect.SetActiveRecursively (false);
					if (GetComponent<Renderer> ().material && NormalBallMaterial)
						GetComponent<Renderer> ().material = NormalBallMaterial;
				}
			}
			if (bigBallTime > 0) {
				MyTransform.localScale = BigBallScale;
			} else {
				MyTransform.localScale = OriginalBallScale;
			}
		}
	}

	void LateUpdate() {
		CheckBallState ();
		if(MyGameManager != null)
			MyRigidBody.velocity = MyRigidBody.velocity.normalized * BallSpeed;
	}
}
