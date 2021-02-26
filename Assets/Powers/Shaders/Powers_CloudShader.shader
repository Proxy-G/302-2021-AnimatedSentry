// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Proxy/CloudShader"
{
	Properties
	{
		_CloudTexture("CloudTexture", 2D) = "black" {}
		_CloudSoftness("CloudSoftness", Range( 0 , 8)) = 0
		_CloudCutoff("CloudCutoff", Range( 0 , 1)) = 0.25
		_CloudSpeed("CloudSpeed", Float) = 1
		_NoiseMainScale("NoiseMainScale", Float) = 1
		_NoiseScale1("NoiseScale1", Float) = 0.2
		_NoiseScale2("NoiseScale2", Float) = 0.2
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
		};

		uniform sampler2D _CloudTexture;
		uniform float _NoiseScale1;
		uniform float _CloudSpeed;
		uniform float _NoiseMainScale;
		uniform float _NoiseScale2;
		uniform float _CloudCutoff;
		uniform float _CloudSoftness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 color20 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			o.Emission = color20.rgb;
			float3 ase_worldPos = i.worldPos;
			float2 appendResult8 = (float2(ase_worldPos.x , ase_worldPos.z));
			float mulTime10 = _Time.y * ( _CloudSpeed * 10.0 );
			float2 appendResult11 = (float2(mulTime10 , mulTime10));
			float temp_output_33_0 = ( _NoiseMainScale * 0.01 );
			o.Alpha = pow( saturate( (0.0 + (( tex2D( _CloudTexture, ( _NoiseScale1 * ( appendResult8 - appendResult11 ) * temp_output_33_0 ) ).r * tex2D( _CloudTexture, ( ( appendResult8 + appendResult11 ) * _NoiseScale2 * temp_output_33_0 ) ).r ) - _CloudCutoff) * (1.0 - 0.0) / (1.0 - _CloudCutoff)) ) , _CloudSoftness );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows nofog 

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
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
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
Version=16301
171;230;962;843;1329.465;265.7567;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;32;-3443.775,31.38086;Float;False;Property;_CloudSpeed;CloudSpeed;3;0;Create;True;0;0;False;0;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-3439.087,137.6131;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-3212.774,71.14172;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-3044.022,80.56815;Float;False;1;0;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;7;-3049.511,-131.472;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;31;-2386.313,-49.83579;Float;False;Property;_NoiseMainScale;NoiseMainScale;4;0;Create;True;0;0;False;0;1;0.425;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-2813.311,56.99176;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;8;-2816.888,-104.7638;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-2220.822,228.1832;Float;False;Property;_NoiseScale2;NoiseScale2;6;0;Create;True;0;0;False;0;0.2;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-2571.597,17.75145;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2279.132,-249.643;Float;False;Property;_NoiseScale1;NoiseScale1;5;0;Create;True;0;0;False;0;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-2571.175,-111.0222;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-2124.679,-45.85934;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1871.119,-220.4752;Float;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;13;-2268.88,-508.6241;Float;True;Property;_CloudTexture;CloudTexture;0;0;Create;True;0;0;False;0;266fb577f27f2f24db5dbccdf58ce22c;266fb577f27f2f24db5dbccdf58ce22c;False;black;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-1849.875,45.88199;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;14;-1633.53,-251.2197;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-1622.445,90.18375;Float;True;Property;_NoiseTexture;NoiseTexture;1;0;Create;True;0;0;False;0;266fb577f27f2f24db5dbccdf58ce22c;266fb577f27f2f24db5dbccdf58ce22c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-1240.076,163.9262;Float;False;Property;_CloudCutoff;CloudCutoff;2;0;Create;True;0;0;False;0;0.25;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1272.273,-62.45784;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;24;-803.2062,132.4235;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;23;-575.7114,132.8039;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-697.048,40.79031;Float;False;Property;_CloudSoftness;CloudSoftness;1;0;Create;True;0;0;False;0;0;0.84;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;-514.3785,-166.2025;Float;False;Constant;_Color0;Color 0;2;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;21;-327.0702,83.67425;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;55.93405,-62.64614;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Proxy/CloudShader;False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;34;0;32;0
WireConnection;34;1;35;0
WireConnection;10;0;34;0
WireConnection;11;0;10;0
WireConnection;11;1;10;0
WireConnection;8;0;7;1
WireConnection;8;1;7;3
WireConnection;12;0;8;0
WireConnection;12;1;11;0
WireConnection;17;0;8;0
WireConnection;17;1;11;0
WireConnection;33;0;31;0
WireConnection;15;0;16;0
WireConnection;15;1;17;0
WireConnection;15;2;33;0
WireConnection;29;0;12;0
WireConnection;29;1;30;0
WireConnection;29;2;33;0
WireConnection;14;0;13;0
WireConnection;14;1;15;0
WireConnection;6;0;13;0
WireConnection;6;1;29;0
WireConnection;18;0;14;1
WireConnection;18;1;6;1
WireConnection;24;0;18;0
WireConnection;24;1;25;0
WireConnection;23;0;24;0
WireConnection;21;0;23;0
WireConnection;21;1;22;0
WireConnection;0;2;20;0
WireConnection;0;9;21;0
ASEEND*/
//CHKSM=C5D547CE798039652A6D6913BB61751CE91813C3