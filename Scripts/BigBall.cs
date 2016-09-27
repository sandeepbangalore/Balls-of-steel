using UnityEngine;
using System.Collections;

public class BigBall : DestructableItem {

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
			MyGameManager.UpdateTimer ("Big Ball Time", extraTime);
		}
		base.KillBrick ();
	}
}
