// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASETemplateShaders/DefaultUnlit"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_TopColor("TopColor", Color) = (0.891,0.2620589,0.5672804,1)
		_BottomColor("BottomColor", Color) = (0.07974432,0.03864098,0.437931,1)
		_BlendMap("Blend Map", 2D) = "white" {}
		_CurvePower("CurvePower", Float) = 0.64
		_VerticalSpeeds("VerticalSpeeds", Vector) = (-0.05,0.1,0.05,0)
		_HorizontalSpeeds("HorizontalSpeeds", Vector) = (-0.05,0.1,0.05,0)
		_Tiling("Tiling", Vector) = (5,0.01,0,0)
		_MyTime("MyTime", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }
		LOD 100
		Cull Off


		Pass
		{
			CGPROGRAM
			#pragma target 3.0 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_instancing


			struct appdata
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform sampler2D _BlendMap;
			UNITY_INSTANCING_CBUFFER_START(ASETemplateShadersDefaultUnlit)
				UNITY_DEFINE_INSTANCED_PROP(float4, _BottomColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _TopColor)
				UNITY_DEFINE_INSTANCED_PROP(float, _MyTime)
				UNITY_DEFINE_INSTANCED_PROP(float3, _HorizontalSpeeds)
				UNITY_DEFINE_INSTANCED_PROP(float3, _VerticalSpeeds)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Tiling)
				UNITY_DEFINE_INSTANCED_PROP(float, _CurvePower)
			UNITY_INSTANCING_CBUFFER_END
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.texcoord.xy = v.texcoord.xy;
				o.texcoord.zw = v.texcoord1.xy;
				
				// ase common template code
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 myColorVar;
				// ase common template code
				float4 _BottomColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_BottomColor);
				float4 _TopColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_TopColor);
				float _MyTime_Instance = UNITY_ACCESS_INSTANCED_PROP(_MyTime);
				float3 _HorizontalSpeeds_Instance = UNITY_ACCESS_INSTANCED_PROP(_HorizontalSpeeds);
				float3 _VerticalSpeeds_Instance = UNITY_ACCESS_INSTANCED_PROP(_VerticalSpeeds);
				float2 appendResult8 = (float2(_HorizontalSpeeds_Instance.x , _VerticalSpeeds_Instance.x));
				float2 _Tiling_Instance = UNITY_ACCESS_INSTANCED_PROP(_Tiling);
				float2 uv13 = i.texcoord.xy * _Tiling_Instance + float2( 0,0 );
				float2 panner3 = ( uv13 + _MyTime_Instance * appendResult8);
				float2 appendResult9 = (float2(_HorizontalSpeeds_Instance.y , _VerticalSpeeds_Instance.y));
				float2 panner4 = ( uv13 + _MyTime_Instance * appendResult9);
				float2 appendResult10 = (float2(_HorizontalSpeeds_Instance.z , _VerticalSpeeds_Instance.z));
				float2 panner5 = ( uv13 + _MyTime_Instance * appendResult10);
				float temp_output_21_0 = ( tex2D( _BlendMap, panner3 ).r + tex2D( _BlendMap, panner4 ).g + tex2D( _BlendMap, panner5 ).b );
				float2 uv37 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float _CurvePower_Instance = UNITY_ACCESS_INSTANCED_PROP(_CurvePower);
				float clampResult35 = clamp( ( 1.0 - pow( ( 1.0 - ( temp_output_21_0 * uv37.y * 0.3 ) ) , _CurvePower_Instance ) ) , 0.0 , 1.0 );
				float4 lerpResult28 = lerp( _BottomColor_Instance , _TopColor_Instance , clampResult35);
				
				
				myColorVar = lerpResult28;
				return myColorVar;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
7;589;1906;444;1124.428;207.7428;1;True;False
Node;AmplifyShaderEditor.Vector3Node;39;-840.5197,210.6272;Float;False;InstancedProperty;_VerticalSpeeds;VerticalSpeeds;4;0;Create;True;-0.05,0.1,0.05;0.02,0.01,0.1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;6;-840.8027,40.2721;Float;False;InstancedProperty;_HorizontalSpeeds;HorizontalSpeeds;5;0;Create;True;-0.05,0.1,0.05;-0.05,0.01,0.03;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;41;-837.912,-163.0123;Float;False;InstancedProperty;_Tiling;Tiling;6;0;Create;True;5,0.01;5.09,0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-651.7708,-164.6463;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;8;-560.1827,19.55814;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.06;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;-557.577,108.6541;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;-0.15;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;-555.3773,203.8525;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;-0.06;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-563.4956,-54.1646;Float;False;InstancedProperty;_MyTime;MyTime;7;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;34;-275.3047,-373.4863;Float;True;Property;_BlendMap;Blend Map;2;0;Create;True;33e58a2199badbf41bb46f7b104d88f7;33e58a2199badbf41bb46f7b104d88f7;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;5;-252.2992,179.9985;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;4;-251.1993,7.097901;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;3;-251.8992,-166.3005;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0.27;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;17;-0.5970733,-25.22034;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;5bf2ba0a806774c47b64e3822cc8d6a4;5bf2ba0a806774c47b64e3822cc8d6a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;18;-1.89708,161.985;Float;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;5bf2ba0a806774c47b64e3822cc8d6a4;5bf2ba0a806774c47b64e3822cc8d6a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2.870594,-211.3012;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;5bf2ba0a806774c47b64e3822cc8d6a4;5bf2ba0a806774c47b64e3822cc8d6a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;21;308.9138,-12.2139;Float;True;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;37;91.5353,368.2233;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;327.4502,502.1211;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;531.5573,216.6112;Float;True;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;704.0277,74.88271;Float;False;InstancedProperty;_CurvePower;CurvePower;3;0;Create;True;0.64;6.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;23;713.1287,-5.719733;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;25;865.2328,-0.5195968;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;3.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;27;1099.24,0.7804402;Float;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;973.6544,-169.5003;Float;False;InstancedProperty;_TopColor;TopColor;0;0;Create;True;0.891,0.2620589,0.5672804,1;0.02745497,0.01645538,0.2034483,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;35;1323.532,-3.899494;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;977.5118,-337.2043;Float;False;InstancedProperty;_BottomColor;BottomColor;1;0;Create;True;0.07974432,0.03864098,0.437931,1;0.1388946,0.8586207,0.7316102,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;22;410.7736,-173.9601;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;3.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;28;1642.464,-122.3802;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMasterNode;0;1878.367,-120.5611;Float;False;True;2;Float;ASEMaterialInspector;0;2;ASETemplateShaders/DefaultUnlit;6e114a916ca3e4b4bb51972669d463bf;ASETemplateShaders/DefaultUnlit;Off;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;13;0;41;0
WireConnection;8;0;6;1
WireConnection;8;1;39;1
WireConnection;9;0;6;2
WireConnection;9;1;39;2
WireConnection;10;0;6;3
WireConnection;10;1;39;3
WireConnection;5;0;13;0
WireConnection;5;2;10;0
WireConnection;5;1;42;0
WireConnection;4;0;13;0
WireConnection;4;2;9;0
WireConnection;4;1;42;0
WireConnection;3;0;13;0
WireConnection;3;2;8;0
WireConnection;3;1;42;0
WireConnection;17;0;34;0
WireConnection;17;1;4;0
WireConnection;18;0;34;0
WireConnection;18;1;5;0
WireConnection;1;0;34;0
WireConnection;1;1;3;0
WireConnection;21;0;1;1
WireConnection;21;1;17;2
WireConnection;21;2;18;3
WireConnection;36;0;21;0
WireConnection;36;1;37;2
WireConnection;36;2;38;0
WireConnection;23;0;36;0
WireConnection;25;0;23;0
WireConnection;25;1;26;0
WireConnection;27;0;25;0
WireConnection;35;0;27;0
WireConnection;22;0;21;0
WireConnection;28;0;29;0
WireConnection;28;1;30;0
WireConnection;28;2;35;0
WireConnection;0;0;28;0
ASEEND*/
//CHKSM=8B3D0609D8BC61644016E90C87F58172C7781B92