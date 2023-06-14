// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IdenticalStudios/HealthIndicator"
{
	Properties
	{
		[HDR]_HealthColor("Health Color", Color) = (0.3410663,0.754717,0.1459594,1)
		[HDR]_EmptyColor("Empty Color", Color) = (0.08627451,0.154902,0.05098039,0.3921569)
		_HealthAmount("Health Amount", Range( 0 , 1)) = 1
		_BorderOffset("Border Offset", Range( 0 , 3)) = 0
		_Direction("Direction", Range( -1 , 1)) = 0
		[HDR]_EdgeColor("EdgeColor", Color) = (1,0.4631137,0,0)
		_EdgeSize("EdgeSize", Range( -0.05 , 1)) = 0.02
		_LineAlbedo("Line Albedo", 2D) = "black" {}
		_LineIntensity("Line Intensity", Range( 0 , 3)) = 1
		_LineSize("Line Size", Range( 0.1 , 2)) = 1
		[HideInInspector]_Alpha("Alpha", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform float4 _EdgeColor;
		uniform float _Direction;
		uniform float _BorderOffset;
		uniform float _HealthAmount;
		uniform float _EdgeSize;
		uniform sampler2D _LineAlbedo;
		uniform float _LineSize;
		uniform float _LineIntensity;
		uniform float4 _EmptyColor;
		uniform float4 _HealthColor;
		uniform float _Alpha;


		inline float3 ASESafeNormalize(float3 inVec)
		{
			float dp3 = max( 0.001f , dot( inVec , inVec ) );
			return inVec* rsqrt( dp3);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 normalizeResult100 = ASESafeNormalize( ase_vertex3Pos );
			float temp_output_105_0 = ((0.0 + (( normalizeResult100.x * ( _Direction >= 0.0 ? 1.0 : -1.0 ) ) - 0.0) * (1.0 - 0.0) / (_BorderOffset - 0.0))*-1.0 + ( _HealthAmount + -0.5 ));
			float smoothstepResult135 = smoothstep( 0.0 , 0.02 , (( _EdgeSize * -1.0 ) + (temp_output_105_0 - 0.0) * (1.0 - ( _EdgeSize * -1.0 )) / (1.0 - 0.0)));
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 temp_cast_0 = (( ase_screenPosNorm.x * _LineSize )).xx;
			float2 panner28 = ( _Time.y * float2( 0,1 ) + temp_cast_0);
			float temp_output_116_0 = ( ( tex2D( _LineAlbedo, panner28 ).r * _LineIntensity ) + step( 0.0 , temp_output_105_0 ) );
			float4 lerpResult130 = lerp( _EdgeColor , float4( 0,0,0,0 ) , ( saturate( smoothstepResult135 ) + ( 1.0 - temp_output_116_0 ) ));
			float smoothstepResult154 = smoothstep( 0.95 , 1.0 , _HealthAmount);
			float4 lerpResult5 = lerp( _EmptyColor , _HealthColor , temp_output_116_0);
			o.Emission = ( ( lerpResult130 * ( 1.0 - smoothstepResult154 ) ) + lerpResult5 ).rgb;
			float lerpResult13 = lerp( _EmptyColor.a , _HealthColor.a , temp_output_116_0);
			o.Alpha = ( lerpResult13 * _Alpha );
		}

		ENDCG
		CGPROGRAM
		#pragma exclude_renderers xboxseries playstation switch nomrt 
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

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
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.screenPos = IN.screenPos;
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
83;389;1335;855;3008.859;1726.293;3.369735;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;8;-1727.262,-705.0712;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;120;-1539.935,-445.5324;Inherit;False;Property;_Direction;Direction;4;0;Create;True;0;0;0;False;0;False;0;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;100;-1459.843,-647.7161;Inherit;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-1445.198,-959.4685;Float;False;Property;_LineSize;Line Size;9;0;Create;True;0;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;118;-1416.123,-1140.58;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;98;-1297.288,-642.0736;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Compare;122;-1239.024,-490.1701;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-1104.217,-1016.84;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;26;-1137.508,-777.5245;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1124.576,-220.9691;Float;False;Property;_HealthAmount;Health Amount;2;0;Create;True;0;0;0;False;0;False;1;0.361;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-1023.935,-569.5324;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-1125.53,-314.9332;Inherit;False;Property;_BorderOffset;Border Offset;3;0;Create;True;0;0;0;False;0;False;0;0.64;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;28;-927.4498,-948.2995;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-402.6475,-1050.815;Inherit;False;Property;_EdgeSize;EdgeSize;6;0;Create;True;0;0;0;False;0;False;0.02;0;-0.05;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-741.6954,-266.7949;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;110;-764.8381,-488.7486;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.65;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;-697.0969,-981.7558;Inherit;True;Property;_LineAlbedo;Line Albedo;7;0;Create;True;0;0;0;False;0;False;-1;None;73029b4cc5226b74dbabe07650f0ed0d;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;119;-648.9418,-731.2772;Float;False;Property;_LineIntensity;Line Intensity;8;0;Create;True;0;0;0;False;0;False;1;0.64;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;105;-539.4791,-453.6241;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-228.1394,-911.3131;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;126;-83.4681,-731.1191;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.05;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-300.5443,-756.9919;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;102;-286.8891,-362.536;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;135;86.13242,-741.6962;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;116;-18.05366,-437.2628;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;159;-239.1401,527.0999;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;131;229.817,-745.1737;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;141;240.231,-552.396;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;154;628.2867,403.6563;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.95;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;158;854.2166,353.6994;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;396.955,-666.1841;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;124;262.8854,-1081.761;Inherit;False;Property;_EdgeColor;EdgeColor;5;1;[HDR];Create;True;0;0;0;False;0;False;1,0.4631137,0,0;1,0.4631137,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;130;598.5463,-732.5316;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-388.5377,-126.324;Float;False;Property;_EmptyColor;Empty Color;1;1;[HDR];Create;True;0;0;0;False;0;False;0.08627451,0.154902,0.05098039,0.3921569;0,0,0,0.1176471;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;160;1061.697,-291.8089;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-381.9474,78.32343;Float;False;Property;_HealthColor;Health Color;0;1;[HDR];Create;True;0;0;0;False;0;False;0.3410663,0.754717,0.1459594,1;1.612565,0.03190975,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;13;176.5673,32.06332;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;722.285,-529.4268;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;5;280.5997,-268.7008;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;74;86.11809,216.659;Float;False;Property;_Alpha;Alpha;10;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;468.0078,-16.82862;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;129;521.7734,-299.6589;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;748.5219,-313.1156;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SurvivalTemplatePro/HealthIndicator;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;100;0;8;0
WireConnection;98;0;100;0
WireConnection;122;0;120;0
WireConnection;111;0;118;1
WireConnection;111;1;112;0
WireConnection;121;0;98;0
WireConnection;121;1;122;0
WireConnection;28;0;111;0
WireConnection;28;1;26;0
WireConnection;104;0;4;0
WireConnection;110;0;121;0
WireConnection;110;2;90;0
WireConnection;21;1;28;0
WireConnection;105;0;110;0
WireConnection;105;2;104;0
WireConnection;138;0;137;0
WireConnection;126;0;105;0
WireConnection;126;3;138;0
WireConnection;115;0;21;1
WireConnection;115;1;119;0
WireConnection;102;1;105;0
WireConnection;135;0;126;0
WireConnection;116;0;115;0
WireConnection;116;1;102;0
WireConnection;159;0;4;0
WireConnection;131;0;135;0
WireConnection;141;0;116;0
WireConnection;154;0;159;0
WireConnection;158;0;154;0
WireConnection;143;0;131;0
WireConnection;143;1;141;0
WireConnection;130;0;124;0
WireConnection;130;2;143;0
WireConnection;160;0;158;0
WireConnection;13;0;3;4
WireConnection;13;1;1;4
WireConnection;13;2;116;0
WireConnection;144;0;130;0
WireConnection;144;1;160;0
WireConnection;5;0;3;0
WireConnection;5;1;1;0
WireConnection;5;2;116;0
WireConnection;77;0;13;0
WireConnection;77;1;74;0
WireConnection;129;0;144;0
WireConnection;129;1;5;0
WireConnection;0;2;129;0
WireConnection;0;9;77;0
ASEEND*/
//CHKSM=A6EAC86B23AEB5D299DFC48699D1C683CF4FA002