using UnityEngine;
using System.Collections;

public class Screen_Manager_Base : MonoBehaviour {
	protected float ScreenFade = 1.0f;

	protected static Screen_Manager_Base _Instance = null;
	public static Screen_Manager_Base Instance {
		get{ 
			if (_Instance == null) 
				_Instance = (Screen_Manager_Base)FindObjectOfType(typeof(Screen_Manager_Base));
			return _Instance;
		}
	}
	public float GetScreenFade(){
		return ScreenFade;
	}
	public void SetScreenFade(float fade) {
		ScreenFade = fade;
	}	
}
