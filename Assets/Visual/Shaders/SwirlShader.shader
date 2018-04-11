// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpiralSwirl"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Color1("Color 1", Color) = (0.891,0.2620589,0.5672804,1)
		_Color0("Color 0", Color) = (0.07974432,0.03864098,0.437931,1)
		_Swirl("Swirl", 2D) = "black" {}
		_RotationalSpeed("RotationalSpeed", Vector) = (0.9,-1,1.1,0)
		_Intensity("Intensity", Range( 1 , 10)) = 3.224502
		_Power("Power", Range( 0.5 , 2)) = 1.066949
		_MyTime("MyTime", Float) = 0
	}
	
	SubShader
	{
		Tags { "Queue"="Transparent" "LightMode" = "ForwardBase" }
		LOD 100
		Cull Off Blend SrcAlpha OneMinusSrcAlpha


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
			uniform sampler2D _Swirl;
			UNITY_INSTANCING_CBUFFER_START(SpiralSwirl)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Color0)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Color1)
				UNITY_DEFINE_INSTANCED_PROP(float3, _RotationalSpeed)
				UNITY_DEFINE_INSTANCED_PROP(float, _MyTime)
				UNITY_DEFINE_INSTANCED_PROP(float, _Intensity)
				UNITY_DEFINE_INSTANCED_PROP(float, _Power)
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
				float4 _Color0_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color0);
				float4 _Color1_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color1);
				float2 uv32 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float3 _RotationalSpeed_Instance = UNITY_ACCESS_INSTANCED_PROP(_RotationalSpeed);
				float _MyTime_Instance = UNITY_ACCESS_INSTANCED_PROP(_MyTime);
				float cos24 = cos( ( _RotationalSpeed_Instance.x * _MyTime_Instance ) );
				float sin24 = sin( ( _RotationalSpeed_Instance.x * _MyTime_Instance ) );
				float2 rotator24 = mul( uv32 - float2( 0.5,0.5 ) , float2x2( cos24 , -sin24 , sin24 , cos24 )) + float2( 0.5,0.5 );
				float cos57 = cos( ( _MyTime_Instance * _RotationalSpeed_Instance.y ) );
				float sin57 = sin( ( _MyTime_Instance * _RotationalSpeed_Instance.y ) );
				float2 rotator57 = mul( uv32 - float2( 0.5,0.5 ) , float2x2( cos57 , -sin57 , sin57 , cos57 )) + float2( 0.5,0.5 );
				float cos58 = cos( ( _MyTime_Instance * _RotationalSpeed_Instance.z ) );
				float sin58 = sin( ( _MyTime_Instance * _RotationalSpeed_Instance.z ) );
				float2 rotator58 = mul( uv32 - float2( 0.5,0.5 ) , float2x2( cos58 , -sin58 , sin58 , cos58 )) + float2( 0.5,0.5 );
				float2 uv46 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float _Intensity_Instance = UNITY_ACCESS_INSTANCED_PROP(_Intensity);
				float clampResult65 = clamp( pow( ( 1.0 - distance( uv46 , float2( 0.5,0.5 ) ) ) , _Intensity_Instance ) , 0.0 , 1.0 );
				float temp_output_47_0 = ( (0.0 + (( tex2D( _Swirl, rotator24 ).r + tex2D( _Swirl, rotator57 ).g + tex2D( _Swirl, rotator58 ).b ) - 0.0) * (1.0 - 0.0) / (3.0 - 0.0)) * clampResult65 );
				float _Power_Instance = UNITY_ACCESS_INSTANCED_PROP(_Power);
				float3 lerpResult56 = lerp( (_Color0_Instance).rgb , (_Color1_Instance).rgb , pow( temp_output_47_0 , _Power_Instance ));
				float clampResult88 = clamp( (-0.16 + (temp_output_47_0 - 0.0) * (1.59 - -0.16) / (1.0 - 0.0)) , 0.0 , 1.0 );
				float4 appendResult74 = (float4(lerpResult56 , ( 1.0 - pow( ( 1.0 - clampResult88 ) , 21.91 ) )));
				
				
				myColorVar = appendResult74;
				return myColorVar;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
7;434;1906;599;970.5693;-121.0019;1.3;True;False
Node;AmplifyShaderEditor.RangedFloatNode;96;-1348.872,137.9017;Float;False;InstancedProperty;_MyTime;MyTime;6;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;30;-1398.999,248.8935;Float;False;InstancedProperty;_RotationalSpeed;RotationalSpeed;3;0;Create;True;0.9,-1,1.1;0.3,0.2,-0.4;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1077.17,219.8017;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-1044.491,88.85587;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-1086.271,410.9019;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-1083.67,317.3018;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-541.3386,647.4224;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;57;-725.3647,276.517;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;24;-724.122,157.7627;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;58;-726.4623,395.5764;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;62;-305.5412,647.9306;Float;False;2;0;FLOAT2;0.0,0,0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;40;-758.4465,-89.09083;Float;True;Property;_Swirl;Swirl;2;0;Create;True;7d7e548c11159864e9d58ae4f17c6d63;7d7e548c11159864e9d58ae4f17c6d63;False;black;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-269.1082,754.2407;Float;False;InstancedProperty;_Intensity;Intensity;4;0;Create;True;3.224502;3.224502;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;41;-459.5934,257.1117;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;5bf2ba0a806774c47b64e3822cc8d6a4;5bf2ba0a806774c47b64e3822cc8d6a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;66;-150.843,647.5998;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-460.8934,444.3171;Float;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;5bf2ba0a806774c47b64e3822cc8d6a4;5bf2ba0a806774c47b64e3822cc8d6a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;42;-461.8669,71.03087;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;5bf2ba0a806774c47b64e3822cc8d6a4;5bf2ba0a806774c47b64e3822cc8d6a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-141.5582,285.7461;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;64;16.69788,644.9433;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;63;-18.59692,285.6406;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;3.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;65;166.6073,640.7668;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;358.8915,412.5356;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;87;643.4481,640.1451;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.16;False;4;FLOAT;1.59;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;88;904.7476,642.746;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;333.9096,632.2392;Float;False;InstancedProperty;_Power;Power;5;0;Create;True;1.066949;0.755;0.5;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;52;697.7956,67.21116;Float;False;InstancedProperty;_Color0;Color 0;1;0;Create;True;0.07974432,0.03864098,0.437931,1;0.9344827,0.1855223,0.8794122,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;90;1046.447,644.0458;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;53;646.438,234.9156;Float;False;InstancedProperty;_Color1;Color 1;0;0;Create;True;0.891,0.2620589,0.5672804,1;0.02865112,0.05519547,0.3896552,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;73;889.7437,235.4742;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;92;658.9086,400.8393;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;72;939.0266,67.40403;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;89;1202.449,642.746;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;21.91;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;56;1311.316,412.6488;Float;True;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;91;1368.847,649.2458;Float;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;74;1617.143,427.6031;Float;True;COLOR;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMasterNode;94;1882.015,429.3701;Float;False;True;2;Float;ASEMaterialInspector;0;2;SpiralSwirl;6e114a916ca3e4b4bb51972669d463bf;ASETemplateShaders/DefaultUnlit;Off;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;95;0;30;1
WireConnection;95;1;96;0
WireConnection;98;0;96;0
WireConnection;98;1;30;3
WireConnection;97;0;96;0
WireConnection;97;1;30;2
WireConnection;57;0;32;0
WireConnection;57;2;97;0
WireConnection;24;0;32;0
WireConnection;24;2;95;0
WireConnection;58;0;32;0
WireConnection;58;2;98;0
WireConnection;62;0;46;0
WireConnection;41;0;40;0
WireConnection;41;1;57;0
WireConnection;66;0;62;0
WireConnection;43;0;40;0
WireConnection;43;1;58;0
WireConnection;42;0;40;0
WireConnection;42;1;24;0
WireConnection;45;0;42;1
WireConnection;45;1;41;2
WireConnection;45;2;43;3
WireConnection;64;0;66;0
WireConnection;64;1;44;0
WireConnection;63;0;45;0
WireConnection;65;0;64;0
WireConnection;47;0;63;0
WireConnection;47;1;65;0
WireConnection;87;0;47;0
WireConnection;88;0;87;0
WireConnection;90;0;88;0
WireConnection;73;0;53;0
WireConnection;92;0;47;0
WireConnection;92;1;93;0
WireConnection;72;0;52;0
WireConnection;89;0;90;0
WireConnection;56;0;72;0
WireConnection;56;1;73;0
WireConnection;56;2;92;0
WireConnection;91;0;89;0
WireConnection;74;0;56;0
WireConnection;74;3;91;0
WireConnection;94;0;74;0
ASEEND*/
//CHKSM=0BB88CDAE005A4674A3D7DF6BAD3A46AD1AB68E3