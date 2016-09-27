using UnityEngine;
using System.Collections;

public class Timer {

	private float timer = 0.0f;
	public Timer () {
		timer = 0.0f;
	}

	public void Tick (float tick) {
		if (timer > 0.0f)
			timer -= tick;
		else
			timer = 0.0f;
	}
	public void AddTime (float addTime) {
		timer += addTime;
	}
	public float GetTime (){
		return timer;
	}
}
