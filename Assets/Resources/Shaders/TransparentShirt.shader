Shader "ShirtColor/Transparent" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ShirtColor ("Shirt Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 300
	
CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
fixed4 _ShirtColor;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
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
	float3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	n.z = -n.z;
	o.Normal = n;
}
ENDCG
}

FallBack "Transparent/Cutout/Diffuse"
}
