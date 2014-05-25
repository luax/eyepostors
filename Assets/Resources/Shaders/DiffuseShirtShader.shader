Shader "ShirtColor/Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ShirtColor ("Shirt Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;
fixed4 _ShirtColor;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	if( c.r > .48f && c.r < .76f &&
		c.g > .49f && c.g < .80f &&
		c.b > .50f && c.b < .84f){
			c *= _ShirtColor;
		}
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "VertexLit"
}
