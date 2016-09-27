using UnityEngine;
using System.Collections;

public class ScreenCamera : MonoBehaviour {

	private Screen_Manager_Base MyScreenManager = null;
	private Material FadeMaterial = null;

	// Use this for initialization
	void Awake () {
		MyScreenManager = Screen_Manager_Base.Instance;
		if (!FadeMaterial) {
			Shader shader = Shader.Find ("Unlit/CameraFade");
			FadeMaterial = new Material (shader);
		}
	}
	
	void OnPostRender () {
		if (MyScreenManager != null && MyScreenManager.GetScreenFade () != 0.0f) {
			FadeMaterial.SetColor ("_Color", new Color (0.0f, 0.0f, 0.0f, MyScreenManager.GetScreenFade ()));
			GL.PushMatrix ();
			GL.LoadOrtho ();
			for (int i = 0; i < FadeMaterial.passCount; i++) {
				FadeMaterial.SetPass (i);
				GL.Begin (GL.QUADS);
				GL.Vertex3 (0, 0, 0.1f);
				GL.Vertex3 (1, 0, 0.1f);
				GL.Vertex3 (1, 1, 0.1f);
				GL.Vertex3 (0, 1, 0.1f);
				GL.End ();
			}
			GL.PopMatrix ();
		}
	}
}
