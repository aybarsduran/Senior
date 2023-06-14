// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IdenticalStudios/Burning"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		[Normal]_NormalMap("Normal Map", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_Albedo_Burned("Albedo_Burned", 2D) = "white" {}
		_Emmision_Burned("Emmision_Burned", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_BurnedAmount("BurnedAmount", Range( 0 , 1)) = 1
		_EmberIntensity("Ember Intensity", Range( 0 , 3)) = 1
		_NormalMap_Burned("Normal Map_Burned", 2D) = "white" {}
		_BurnedTint("BurnedTint", Color) = (1,1,1,0)
		_Noise("Noise", 2D) = "white" {}
		_AlbedoBurnAmount("AlbedoBurnAmount", Range( 0 , 10)) = 5
		_EmberFrequency("EmberFrequency", Range( 1 , 5)) = 2.387236
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _NormalMap_Burned;
		uniform float4 _NormalMap_Burned_ST;
		uniform float _BurnedAmount;
		uniform sampler2D _Noise;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Albedo_Burned;
		uniform float4 _Albedo_Burned_ST;
		uniform float4 _BurnedTint;
		uniform float _AlbedoBurnAmount;
		uniform sampler2D _Emmision_Burned;
		uniform float4 _Emmision_Burned_ST;
		uniform float _EmberFrequency;
		uniform float _EmberIntensity;
		uniform float _Metallic;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float2 uv_NormalMap_Burned = i.uv_texcoord * _NormalMap_Burned_ST.xy + _NormalMap_Burned_ST.zw;
			float3 NormalMap_Burned17 = UnpackNormal( tex2D( _NormalMap_Burned, uv_NormalMap_Burned ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 uv_TexCoord88 = i.uv_texcoord * float2( 3,3 );
			float4 Noise138 = tex2D( _Noise, uv_TexCoord88 );
			float lerpResult86 = lerp( ( ase_vertex3Pos.y * _BurnedAmount ) , 1.0 , ( _BurnedAmount * Noise138 ).r);
			float Mask30 = saturate( lerpResult86 );
			float3 lerpResult37 = lerp( UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) ) , NormalMap_Burned17 , Mask30);
			o.Normal = lerpResult37;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float2 uv_Albedo_Burned = i.uv_texcoord * _Albedo_Burned_ST.xy + _Albedo_Burned_ST.zw;
			float4 Albedo_Burned16 = tex2D( _Albedo_Burned, uv_Albedo_Burned );
			float BurnedAmount43 = _BurnedAmount;
			float clampResult149 = clamp( ( Mask30 * 1.0 * ( BurnedAmount43 * _AlbedoBurnAmount ) ) , 0.0 , 1.4 );
			float4 lerpResult8 = lerp( tex2D( _Albedo, uv_Albedo ) , ( Albedo_Burned16 * _BurnedTint ) , clampResult149);
			float4 break23 = lerpResult8;
			float4 appendResult24 = (float4(break23.r , break23.g , break23.b , 0.0));
			o.Albedo = appendResult24.xyz;
			float2 uv_Emmision_Burned = i.uv_texcoord * _Emmision_Burned_ST.xy + _Emmision_Burned_ST.zw;
			float4 Emmision_Burned19 = tex2D( _Emmision_Burned, uv_Emmision_Burned );
			float4 lerpResult50 = lerp( Emmision_Burned19 , float4( 0,0,0,0 ) , (float4( 0.7735849,0.7735849,0.7735849,0 ) + (( Noise138 * sin( ( _Time.y * _EmberFrequency ) ) ) - float4( 0,0,0,0 )) * (float4( 1,1,1,0 ) - float4( 0.7735849,0.7735849,0.7735849,0 )) / (float4( 0.3490566,0.3490566,0.3490566,0 ) - float4( 0,0,0,0 ))));
			float4 lerpResult42 = lerp( ( lerpResult50 * _EmberIntensity ) , float4( 0,0,0,0 ) , ( 1.0 - BurnedAmount43 ));
			float4 temp_cast_2 = 0;
			float4 ifLocalVar81 = 0;
			if( BurnedAmount43 > 0.5 )
				ifLocalVar81 = lerpResult42;
			else if( BurnedAmount43 < 0.5 )
				ifLocalVar81 = temp_cast_2;
			o.Emission = ifLocalVar81.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = break23.a;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=15800
210;342;1798;632;2354.145;286.3209;1.464389;True;True
Node;AmplifyShaderEditor.CommentaryNode;12;-2944.258,-316.3577;Float;False;809.5626;828.1309;;9;88;62;17;29;19;18;16;13;138;Burned Textures;0.8490566,0.2938689,0.1722143,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;88;-2916.809,375.425;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;3,3;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;21;-3141.232,610.6473;Float;False;1210.691;489.4006;;10;145;139;86;43;146;85;30;25;9;26;Burn Mask;0.5566038,0.5566038,0.5566038,1;0;0
Node;AmplifyShaderEditor.SamplerNode;62;-2652.878,314.9554;Float;True;Property;_Noise;Noise;10;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-2350.806,360.6328;Float;False;Noise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-3116.967,864.0262;Float;False;Property;_BurnedAmount;BurnedAmount;6;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;-2834.113,1019.717;Float;False;138;Noise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;9;-3089.147,681.5396;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;146;-2583.434,937.8726;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-2749.816,691.6827;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-2559.192,999.2043;Float;False;2;2;0;FLOAT;0;False;1;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;86;-2498.731,729.1245;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;48;-1939.149,0.3934326;Float;False;1671.1;456.7012;;17;136;44;47;45;50;46;141;140;81;83;42;82;35;126;103;104;98;Ember;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;85;-2316.361,684.8826;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;40;-1552.355,-823.8578;Float;False;1273.149;769.9629;;15;74;58;72;73;8;24;23;1;55;57;31;20;142;143;149;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-2859.634,938.0577;Float;False;BurnedAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-2927.7,-269.8271;Float;True;Property;_Albedo_Burned;Albedo_Burned;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;104;-1923.625,361.0496;Float;False;Property;_EmberFrequency;EmberFrequency;12;0;Create;True;0;0;False;0;2.387236;0;1;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-1523.292,-131.1609;Float;False;Property;_AlbedoBurnAmount;AlbedoBurnAmount;11;0;Create;True;0;0;False;0;5;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;-1519.578,-205.7069;Float;False;43;BurnedAmount;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;98;-1929.404,172.5097;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-2142.433,675.2854;Float;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-1143.662,-308.3803;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-1531.139,-286.9166;Float;False;30;Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-2620.289,-197.8548;Float;False;Albedo_Burned;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-1703.111,208.1043;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-1517.567,-564.5325;Float;False;16;Albedo_Burned;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1082.873,-455.5765;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;57;-1518.194,-484.8228;Float;False;Property;_BurnedTint;BurnedTint;9;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;140;-1556.997,141.6866;Float;False;138;Noise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;18;-2923.332,131.003;Float;True;Property;_Emmision_Burned;Emmision_Burned;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;126;-1554.501,307.4656;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1500.305,-763.6035;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;149;-934.0974,-433.327;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-1427.998,230.2867;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-1262.558,-549.3102;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-2625.072,205.5462;Float;False;Emmision_Burned;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;8;-748.4539,-584.1844;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;136;-1271.122,130.7397;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.3490566,0.3490566,0.3490566,0;False;3;COLOR;0.7735849,0.7735849,0.7735849,0;False;4;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-1915.149,63.1465;Float;False;19;Emmision_Burned;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1092.819,267.5795;Float;False;Property;_EmberIntensity;Ember Intensity;7;0;Create;True;0;0;False;0;1;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;44;-1068.242,353.8999;Float;False;43;BurnedAmount;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;72;-465.0306,-334.4101;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;29;-2927.599,-71.51691;Float;True;Property;_NormalMap_Burned;Normal Map_Burned;8;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;50;-1083.465,45.71978;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;39;-729.9374,489.1223;Float;False;483.7578;409.4822;;4;37;38;32;2;Normal Map;0.526893,0.4607067,0.8207547,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;45;-840.8525,357.2203;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-823.323,134.8748;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;73;-808.7004,-288.4187;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-2624.203,-2.461427;Float;False;NormalMap_Burned;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;42;-666.833,172.2292;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;83;-664.9095,323.7573;Float;False;Constant;_Black;Black;12;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;23;-775.515,-248.8237;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;82;-684.5564,51.59444;Float;False;43;BurnedAmount;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-712.1463,555.5996;Float;True;Property;_NormalMap;Normal Map;1;1;[Normal];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;32;-682.127,730.0565;Float;False;17;NormalMap_Burned;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-639.7718,803.5182;Float;False;30;Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;81;-424.3644,107.2193;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0.5;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-430.262,-256.2019;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;37;-392.5637,630.7493;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-83.35557,615.0198;Float;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-92.41906,705.2099;Float;True;Property;_AO;AO;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;161.1403,-13.36869;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SurvivalTemplatePro/Burning;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;62;1;88;0
WireConnection;138;0;62;0
WireConnection;146;0;26;0
WireConnection;25;0;9;2
WireConnection;25;1;26;0
WireConnection;145;0;146;0
WireConnection;145;1;139;0
WireConnection;86;0;25;0
WireConnection;86;2;145;0
WireConnection;85;0;86;0
WireConnection;43;0;26;0
WireConnection;30;0;85;0
WireConnection;143;0;142;0
WireConnection;143;1;74;0
WireConnection;16;0;13;0
WireConnection;103;0;98;2
WireConnection;103;1;104;0
WireConnection;58;0;31;0
WireConnection;58;2;143;0
WireConnection;126;0;103;0
WireConnection;149;0;58;0
WireConnection;141;0;140;0
WireConnection;141;1;126;0
WireConnection;55;0;20;0
WireConnection;55;1;57;0
WireConnection;19;0;18;0
WireConnection;8;0;1;0
WireConnection;8;1;55;0
WireConnection;8;2;149;0
WireConnection;136;0;141;0
WireConnection;72;0;8;0
WireConnection;50;0;35;0
WireConnection;50;2;136;0
WireConnection;45;0;44;0
WireConnection;46;0;50;0
WireConnection;46;1;47;0
WireConnection;73;0;72;0
WireConnection;17;0;29;0
WireConnection;42;0;46;0
WireConnection;42;2;45;0
WireConnection;23;0;73;0
WireConnection;81;0;82;0
WireConnection;81;2;42;0
WireConnection;81;4;83;0
WireConnection;24;0;23;0
WireConnection;24;1;23;1
WireConnection;24;2;23;2
WireConnection;37;0;2;0
WireConnection;37;1;32;0
WireConnection;37;2;38;0
WireConnection;0;0;24;0
WireConnection;0;1;37;0
WireConnection;0;2;81;0
WireConnection;0;3;4;0
WireConnection;0;4;23;3
WireConnection;0;5;5;1
ASEEND*/
//CHKSM=ABEB0EF8061B3DE076F173E52AD6DF15900EB773