using UnityEngine;
using System.Collections;

public class GlowBall : DestructableItem {
	public float extraTime = 15.0f;
	protected override void Start ()
	{
		base.Start ();
		MyGameManager.RegisterTimer ("Big Ball Time");
	}
	public override void KillBrick ()
	{
		if (Dying)
			return;
		if (MyGameManager != null) {
			MyGameManager.UpdateTimer ("Invincible", extraTime);
			MyGameManager.UpdateTimer ("Big Ball Time", extraTime);
		}
		base.KillBrick ();
	}
}
