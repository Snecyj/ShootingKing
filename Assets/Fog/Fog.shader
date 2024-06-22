Shader "Fog/Fog Color"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FogColor("Fog Color", Color) = (1,1,1,1)
		_FogDensity("Fog Density", Float) = 1
		_FogPulse("Fog Pulse", Float) = 0
		_FogTimePulse("Fog Time Pulse", Float) = 1
	}
		SubShader
		{
			// No culling or depth
			Cull Back ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float4 scrPos : TEXCOORD1;
					half2 texcoord  : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _CameraDepthTexture;
				fixed4 _FogColor;
				float _FogDensity;
				float _FogPulse;
				float _FogTimePulse;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.scrPos = ComputeScreenPos(o.vertex);

					o.texcoord = v.texcoord;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 colorTex = tex2D(_MainTex, TRANSFORM_TEX(i.texcoord, _MainTex));
				
					float depth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
					float linearDepth = clamp(((Linear01Depth(depth) - Linear01Depth(i.vertex.z)) * (_ProjectionParams.z )) / (_FogDensity + sin(_Time.y/_FogTimePulse)* _FogPulse),0,1);
					//return fixed4(linearDepth, 0, 0, 1);
					fixed4 color = fixed4(colorTex.rgb*_FogColor.rgb, colorTex.a*_FogColor.a * linearDepth);
					return color;
				}
				ENDCG
			}
		}
}