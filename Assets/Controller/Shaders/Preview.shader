// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IdenticalStudios/Preview"
{
	Properties
	{
		_Color("Base Color", Color) = (1,1,1,0)
		_MainTex("Base Color Map", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 2)) = 0
		_Smoothness("Smoothness", Range( 0 , 2)) = 0
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		[HDR]_Fresnel("Fresnel", Color) = (1,1,1,1)
		_LineMap("Line Map", 2D) = "white" {}
		[HDR]_LineColor("Line Color", Color) = (1.991077,1.306363,0.3513666,1)
		_LineAlpha("Line Alpha", Range( 0 , 1)) = 1
		_LineSize("Line Size", Range( 0.001 , 2)) = 1
		_LineSpeed("Line Speed", Range( 0.1 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _LineAlpha;
		uniform sampler2D _LineMap;
		uniform float _LineSpeed;
		uniform float _LineSize;
		uniform float4 _LineColor;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;
		uniform float4 _Fresnel;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( _Color * tex2DNode1 ).rgb;
			float2 temp_cast_1 = (_LineSpeed).xx;
			float3 ase_worldPos = i.worldPos;
			float2 temp_cast_2 = (( ase_worldPos.x * _LineSize )).xx;
			float2 panner243 = ( _Time.y * temp_cast_1 + temp_cast_2);
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			float4 tex2DNode4 = tex2D( _AmbientOcclusion, uv_AmbientOcclusion );
			float4 lerpResult278 = lerp( float4( 0,0,0,0 ) , ( ( tex2D( _LineMap, panner243 ) * _LineColor ) * float4( 0.245283,0.245283,0.245283,0 ) ) , tex2DNode4);
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV305 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode305 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV305, 7.0 ) );
			float4 Line32 = ( ( _LineAlpha * lerpResult278 ) + ( fresnelNode305 * _Fresnel ) );
			o.Emission = Line32.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Occlusion = tex2DNode4.r;
			o.Alpha = ( _Color.a * tex2DNode1.a );
		}

		ENDCG
		CGPROGRAM
		#pragma exclude_renderers xboxseries playstation switch nomrt 
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
782;185;1513;1186;-1032.793;384.1775;1;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;307;425.5908,207.3772;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;155;422.7739,540.1837;Float;False;Property;_LineSize;Line Size;10;0;Create;True;0;0;0;False;0;False;1;1.502;0.001;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;724.7547,444.8126;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;245;487.231,698.9694;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;425.1113,620.2534;Float;False;Property;_LineSpeed;Line Speed;11;0;Create;True;0;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;243;880.8021,545.7383;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;116;1073.072,520.4131;Inherit;True;Property;_LineMap;Line Map;7;0;Create;True;0;0;0;False;0;False;-1;None;02f71419854c0d845a930c9e0a0bf775;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;1118.748,730.1531;Float;False;Property;_LineColor;Line Color;8;1;[HDR];Create;True;0;0;0;False;0;False;1.991077,1.306363,0.3513666,1;1.991077,1.306363,0.3513666,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;1416.484,677.4853;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;1565.876,380.0213;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;285;1634.361,666.8494;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.245283,0.245283,0.245283,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;278;1905.847,640.4869;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;290;1884.437,558.0808;Float;False;Property;_LineAlpha;Line Alpha;9;0;Create;True;0;0;0;False;0;False;1;0.675;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;305;1867.574,863.6296;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;306;1891.54,1056.519;Inherit;False;Property;_Fresnel;Fresnel;6;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;2176.111,863.4739;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;2191.187,625.9677;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;304;2411.193,617.2138;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;2574.632,615.9269;Float;False;Line;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;292;1311.934,-453.5423;Float;False;Property;_Color;Base Color;1;0;Create;False;0;0;0;False;0;False;1,1,1,0;1,0.4994154,0.0518868,0.5529412;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;1188.156,-264.0096;Inherit;True;Property;_MainTex;Base Color Map;2;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;295;1627.536,-110.6647;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;298;1549.887,146.8869;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;1561.492,218.1364;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;1607.918,-321.2689;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;156;432.968,376.5057;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;265;1509.938,21.05006;Inherit;False;32;Line;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;50;2017.088,-35.68193;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SurvivalTemplatePro/Preview;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Overlay;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.01;0.8962264,0.7633126,0.2832413,0;VertexOffset;False;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;246;0;307;1
WireConnection;246;1;155;0
WireConnection;243;0;246;0
WireConnection;243;2;120;0
WireConnection;243;1;245;0
WireConnection;116;1;243;0
WireConnection;119;0;116;0
WireConnection;119;1;11;0
WireConnection;285;0;119;0
WireConnection;278;1;285;0
WireConnection;278;2;4;0
WireConnection;301;0;305;0
WireConnection;301;1;306;0
WireConnection;291;0;290;0
WireConnection;291;1;278;0
WireConnection;304;0;291;0
WireConnection;304;1;301;0
WireConnection;32;0;304;0
WireConnection;295;0;292;4
WireConnection;295;1;1;4
WireConnection;293;0;292;0
WireConnection;293;1;1;0
WireConnection;50;0;293;0
WireConnection;50;2;265;0
WireConnection;50;3;298;0
WireConnection;50;4;297;0
WireConnection;50;5;4;0
WireConnection;50;9;295;0
ASEEND*/
//CHKSM=AFA833812D492AE1E355DB17DD9AF2938DD485FF