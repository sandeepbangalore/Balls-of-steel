using UnityEngine;
using System.Collections;

public class Bomb : DestructableItem {

	public GameObject ExplosionPrefab = null;
	public float ExplosionRadius = 4.0f;

	private int BrickLayer = 0;
	private GameObject Explosion = null;
	protected override void Start ()
	{
		base.Start ();
		BrickLayer = LayerMask.NameToLayer ("Brick");
	}
	public override void KillBrick ()
	{
		if (Dying)
			return;
		base.KillBrick ();
		if (ExplosionPrefab != null && Explosion == null) {
			Explosion = Instantiate (ExplosionPrefab, MyTransform.position, Quaternion.identity) as GameObject;
		}
		Collider[] colliders = Physics.OverlapSphere (MyTransform.position, ExplosionRadius, 1 << BrickLayer);

		for (int i = 0; i < colliders.Length; i++) {
			DestructableItem script = colliders [i].GetComponent<DestructableItem> ();
			if (script != null) {
				script.KillBrick ();
			}
		}
	}
}