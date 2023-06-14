// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IdenticalStudios/Skin"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		[Normal]_NormalMap("Normal Map", 2D) = "white" {}
		_Metallic_Smoothness("Metallic_Smoothness", 2D) = "black" {}
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_Cutout_Multiplier("Cutout_Multiplier", Range( 0 , 1)) = 0.5
		[HideInInspector]_OpacityMask0("OpacityMask0", 2D) = "black" {}
		[HideInInspector]_OpacityMask1("OpacityMask1", 2D) = "black" {}
		[HideInInspector]_OpacityMask2("OpacityMask2", 2D) = "black" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.25
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Metallic_Smoothness;
		uniform float4 _Metallic_Smoothness_ST;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;
		uniform sampler2D _OpacityMask0;
		uniform float4 _OpacityMask0_ST;
		uniform sampler2D _OpacityMask1;
		uniform float4 _OpacityMask1_ST;
		uniform sampler2D _OpacityMask2;
		uniform float4 _OpacityMask2_ST;
		uniform float _Cutout_Multiplier;
		uniform float _Cutoff = 0.25;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = tex2DNode1.rgb;
			float2 uv_Metallic_Smoothness = i.uv_texcoord * _Metallic_Smoothness_ST.xy + _Metallic_Smoothness_ST.zw;
			float4 tex2DNode3 = tex2D( _Metallic_Smoothness, uv_Metallic_Smoothness );
			o.Metallic = tex2DNode3.r;
			o.Smoothness = tex2DNode3.a;
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			o.Occlusion = saturate( tex2D( _AmbientOcclusion, uv_AmbientOcclusion ) ).r;
			o.Alpha = 1;
			float2 uv_OpacityMask0 = i.uv_texcoord * _OpacityMask0_ST.xy + _OpacityMask0_ST.zw;
			float2 uv_OpacityMask1 = i.uv_texcoord * _OpacityMask1_ST.xy + _OpacityMask1_ST.zw;
			float2 uv_OpacityMask2 = i.uv_texcoord * _OpacityMask2_ST.xy + _OpacityMask2_ST.zw;
			float Albedo_Opacity31 = tex2DNode1.a;
			clip( ( ( 1.0 - ( tex2D( _OpacityMask0, uv_OpacityMask0 ) + tex2D( _OpacityMask1, uv_OpacityMask1 ) + tex2D( _OpacityMask2, uv_OpacityMask2 ) ) ) * pow( Albedo_Opacity31 , ( _Cutout_Multiplier * 2.0 ) ) ).r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=15800
8;329;1798;626;1666.121;-214.2722;1.45967;True;True
Node;AmplifyShaderEditor.SamplerNode;1;-641.8552,-46.66462;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-1101.695,683.9058;Float;True;Property;_OpacityMask0;OpacityMask0;5;1;[HideInInspector];Create;True;0;0;False;0;None;37e6f91f3efb0954cbdce254638862ea;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;40;-1093.384,871.1317;Float;True;Property;_OpacityMask1;OpacityMask1;6;1;[HideInInspector];Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;41;-1090.68,1087.004;Float;True;Property;_OpacityMask2;OpacityMask2;7;1;[HideInInspector];Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-335.7705,107.8291;Float;False;Albedo_Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-618.9546,1105.058;Float;False;Property;_Cutout_Multiplier;Cutout_Multiplier;4;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-510.2796,746.0623;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-461.2247,1017.919;Float;False;31;Albedo_Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-312.2705,1094.058;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;39;-170.976,983.0022;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-644.9861,522.1271;Float;True;Property;_AmbientOcclusion;Ambient Occlusion;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;62;-229.7854,753.3553;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-643.8911,334.264;Float;True;Property;_Metallic_Smoothness;Metallic_Smoothness;2;0;Create;True;0;0;False;0;None;7a170cdb7cc88024cb628cfcdbb6705c;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;27.38028,736.001;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;17;-3.321655,502.2245;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-644.8304,147.5851;Float;True;Property;_NormalMap;Normal Map;1;1;[Normal];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;268.308,272.6537;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SurvivalTemplatePro/Skin;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.25;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;8;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;1;4
WireConnection;61;0;5;0
WireConnection;61;1;40;0
WireConnection;61;2;41;0
WireConnection;35;0;34;0
WireConnection;39;0;32;0
WireConnection;39;1;35;0
WireConnection;62;0;61;0
WireConnection;33;0;62;0
WireConnection;33;1;39;0
WireConnection;17;0;4;0
WireConnection;0;0;1;0
WireConnection;0;1;2;0
WireConnection;0;3;3;0
WireConnection;0;4;3;4
WireConnection;0;5;17;0
WireConnection;0;10;33;0
ASEEND*/
//CHKSM=7AE6FBD1AC1EC1567786DC00DAE6F3B7B4CCCB18