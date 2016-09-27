using UnityEngine;
using System.Collections;

public class WallLowering : DestructableItem {
	public float StopTime = 15.0f;

	public override void KillBrick ()
	{
		if (Dying)
			return;
		if(MyGameManager != null)
			MyGameManager.UpdateTimer ("Wall Stop Timer", StopTime);
		base.KillBrick ();
	}
}
