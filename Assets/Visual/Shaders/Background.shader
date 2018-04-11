// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Background"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Top("Top", Color) = (0,0,0.428,1)
		_Bottom("Bottom", Color) = (0.9275863,0,0.3541172,1)
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
			#include "UnityShaderVariables.cginc"
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
				float4 ase_texcoord1 : TEXCOORD1;
			};

			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			UNITY_INSTANCING_CBUFFER_START(Background)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Bottom)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Top)
			UNITY_INSTANCING_CBUFFER_END
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.texcoord.xy = v.texcoord.xy;
				o.texcoord.zw = v.texcoord1.xy;
				
				// ase common template code
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord1.xyz = worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 myColorVar;
				// ase common template code
				float4 _Bottom_Instance = UNITY_ACCESS_INSTANCED_PROP(_Bottom);
				float4 _Top_Instance = UNITY_ACCESS_INSTANCED_PROP(_Top);
				float3 worldPos = i.ase_texcoord1.xyz;
				float2 temp_output_47_0 = (worldPos).xy;
				float2 panner32 = ( temp_output_47_0 + _SinTime.x * float2( 0.2,0 ));
				float2 uv12 = i.texcoord.xy * float2( 5,1 ) + panner32;
				float simplePerlin2D9 = snoise( uv12 );
				float2 uv2 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner37 = ( temp_output_47_0 + _Time.y * float2( 0,0.2 ));
				float2 uv39 = i.texcoord.xy * float2( 2,2 ) + panner37;
				float simplePerlin2D40 = snoise( uv39 );
				float4 lerpResult6 = lerp( _Bottom_Instance , _Top_Instance , ( (-0.3 + (simplePerlin2D9 - -1.0) * (0.0 - -0.3) / (1.0 - -1.0)) + uv2.y + (-0.3 + (simplePerlin2D40 - -1.0) * (0.0 - -0.3) / (1.0 - -1.0)) ));
				
				
				myColorVar = lerpResult6;
				return myColorVar;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
6;354;1906;599;2746.207;483.4471;1.810671;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;43;-1935.954,-480.8821;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;47;-1592.509,-289.5599;Float;False;True;True;False;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinTimeNode;42;-1612.896,-149.9888;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;36;-1558.262,290.6252;Float;True;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;13;-1287.818,-443.2254;Float;False;Constant;_Vector0;Vector 0;2;0;Create;True;5,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;32;-1324.289,-288.5981;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;37;-1327.262,323.0903;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.2;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;38;-1290.791,174.0676;Float;False;Constant;_Vector1;Vector 1;2;0;Create;True;2,2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-1082.435,92.40208;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1079.462,-440.8212;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;40;-819.5123,85.98386;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;9;-816.5381,-447.2394;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;41;-439.354,268.9626;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.3;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-389.0107,-208.3622;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;22;-451.3256,-424.9273;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.3;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-124.9951,-209.9649;Float;True;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-127.6489,-387.7591;Float;False;InstancedProperty;_Top;Top;0;0;Create;True;0,0,0.428,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-133.7088,-556.4921;Float;False;InstancedProperty;_Bottom;Bottom;1;0;Create;True;0.9275863,0,0.3541172,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-1561.262,99.62516;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;6;129.6226,-352.1797;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-1558.289,-433.5981;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMasterNode;8;387.9793,-353.1669;Float;False;True;2;Float;ASEMaterialInspector;0;2;Background;6e114a916ca3e4b4bb51972669d463bf;ASETemplateShaders/DefaultUnlit;Off;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;47;0;43;0
WireConnection;32;0;47;0
WireConnection;32;1;42;0
WireConnection;37;0;47;0
WireConnection;37;1;36;0
WireConnection;39;0;38;0
WireConnection;39;1;37;0
WireConnection;12;0;13;0
WireConnection;12;1;32;0
WireConnection;40;0;39;0
WireConnection;9;0;12;0
WireConnection;41;0;40;0
WireConnection;22;0;9;0
WireConnection;19;0;22;0
WireConnection;19;1;2;2
WireConnection;19;2;41;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;6;2;19;0
WireConnection;8;0;6;0
ASEEND*/
//CHKSM=B2C3645C5A40651390172EB5EC1C4AAC118EB86C