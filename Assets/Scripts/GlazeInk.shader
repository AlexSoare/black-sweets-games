Shader "Custom/Colored Glaze Ink" {
	Properties{
		_InkColor("Ink Color", Color) = (1,1,1,1)
		_GlazeColor("Glaze Color", Color) = (0.2,0.2,0.2,0.2)
		_InkAmount("Ink Amount", Range(0.0, 1.0)) = 0.5
		_GlazeAmount("Glaze Amount", Range(0.0, 1.0)) = 0.9
		_EmptyPixelColor("Empty Pixel Color", Color) = (1,1,1,0)
		_FlowSpeed("Flow Speed", Range(0.1, 10.0)) = 1.0
		_FlowDirection("Flow Direction", Range(0.0, 360.0)) = 0.0
		_FlowStrength("Flow Strength", Range(0.0, 1.0)) = 0.5
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseAmount("Noise Amount", Range(0.0, 1.0)) = 0.1
		_NoiseScale("Noise Scale", Range(1.0, 10.0)) = 1.0
		_WaveFrequency("Wave Frequency", Range(0.0, 100.0)) = 1
		_WaveAmplitude("Wave Amplitude", Range(0.0, 1)) = 0.1
	}

	SubShader{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Texture
			uniform sampler2D _MainTex;
			// Colors
			uniform float4 _InkColor;
			uniform float4 _GlazeColor;
			uniform float4 _EmptyPixelColor;
			// Amounts
			uniform float _InkAmount;
			uniform float _GlazeAmount;
			  // Flow
            uniform float _FlowSpeed;
            uniform float _FlowDirection;
            uniform float _FlowStrength;
            // Noise
            uniform sampler2D _NoiseTex;
            uniform float _NoiseAmount;
            uniform float _NoiseScale;
			// Wave
			uniform float _WaveFrequency;
			uniform float _WaveAmplitude;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target {
				// Sample the texture
				float4 tex = tex2D(_MainTex, i.uv);
				float4 color = _EmptyPixelColor;

				// Check if the pixel is empty
				if (tex.a > 0.01) {
					// Ink
					float4 ink = _InkColor * tex.r * _InkAmount;

					// Glaze
					float flowTime = _Time.y * _FlowSpeed;
					float2 uvOffset = float2(0, 1 - i.uv.y) * flowTime * _FlowStrength;
					float2 waveOffset = sin(uvOffset * _WaveFrequency + flowTime) * _WaveAmplitude;
					float2 offset = float2(0, uvOffset.y) + waveOffset;
					float4 glaze = _GlazeColor * _GlazeAmount * tex.a;
					float2 noiseUV = (i.uv * _NoiseScale) + (offset * 0.5);
					float4 noise = tex2D(_NoiseTex, noiseUV) * _NoiseAmount;

					// Apply glaze
					color = ink + glaze + noise;

					// Clamp values
					color.rgb = saturate(color.rgb);

					// Mix with the original texture color
					//color = lerp(tex, color, color.a);
				}

				return color;
			}
			ENDCG
		}	
	}
	FallBack "Diffuse"
}