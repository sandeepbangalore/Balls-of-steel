Shader "Unlit/CameraFade"
{
	Properties { _Color ("Main Color", Color) = (1,1,1,0) 
	}
	SubShader {
	Pass { 
		   ZTest Always Cull Off Zwrite Off
		   Blend SrcAlpha OneMinusSrcAlpha
           Color[_Color]
		 }
	}
}
