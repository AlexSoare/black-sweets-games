Shader "Custom/AdditiveTexture"
{
	Properties
	{
		_AddTex("Additional Texture", 2D) = "white" {}
		_BlendFactor("Blend Factor", Range(0, 1)) = 1
		_Speed("Speed", Range(0, 100)) = 1
	}

		SubShader
		{
			Tags { "RenderType" = "Transparent" }
			LOD 100

			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha // Set the blending mode

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				float _Speed;
				sampler2D _MainTex;
				sampler2D _AddTex;
				float _BlendFactor;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// Sample the main texture from the image component
					fixed4 mainColor = tex2D(_MainTex, i.uv);

				// Check if the pixel is not empty
				if (mainColor.a > 0)
				{
					/*float fase = _Time * _Speed;
					fixed2 offset = fixed2(cos(fase), sin(fase)) * 0.5;
					fixed2 noiseUV = i.uv + offset;
					fixed4 addColor = tex2D(_AddTex, noiseUV);

					fixed4 finalColor = addColor * _BlendFactor;*/

					// Sample the additional texture using a noise texture based on time and circular movement
					float fase = -_Time * _Speed;
					float2 offset = float2(0, sin(fase)); // downward movement
					float2 noiseUV = i.uv + offset;
					fixed4 addColor = tex2D(_AddTex, noiseUV);

					// Blend the colors
					fixed4 finalColor = addColor * _BlendFactor;

					return finalColor;
				}

				return mainColor;
			}
			ENDCG
		}
		}
}