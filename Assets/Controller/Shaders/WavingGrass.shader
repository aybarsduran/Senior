// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/TerrainEngine/Details/WavingDoublePass" {
Properties {
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
	_Cutoff ("Cutoff", float) = 0.5

}

SubShader {
	Tags {
		"Queue" = "Geometry+200"
		"IgnoreProjector"="True"
		"RenderType"="Grass"
		"DisableBatching"="True"
	}
	Cull Off
	LOD 200
		
CGPROGRAM
#pragma surface surf BlinnPhong vertex:WavingGrassVert addshadow
//exclude_path:deferred
#include "TerrainEngine.cginc"

sampler2D _MainTex;
fixed _Cutoff;

fixed _GrassGloss;
fixed _GrassShininess;


struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);// * IN.color;
	o.Albedo = c.rgb * IN.color.rgb;
	clip (c.a * IN.color.a - _Cutoff);
	o.Alpha = 1;

	//o.Gloss = IN.uv_MainTex.y * _GrassGloss;
	o.Specular = IN.uv_MainTex.y * _GrassShininess;
	//o.Specular = 0.05;
	//o.Specular = _GrassSpecularColor;
}
ENDCG
}
	Fallback Off
}
