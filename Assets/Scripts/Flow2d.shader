Shader "Custom/Flow2D" {
	Properties{
		_FlowTex("Flow Texture", 2D) = "white" {}
		_Speed("Speed", Range(0, 10)) = 1
	}
		SubShader{
			Tags { "RenderType" = "Transparent" }
			  LOD 100

			Pass {

				Blend SrcAlpha OneMinusSrcAlpha // Set the blending mode

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _FlowTex;
				float _Speed;

				v2f vert(appdata v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					float4 mainTex = tex2D(_MainTex, i.uv);
					if (mainTex.a == 0) {
						return mainTex;
					}

					float2 center = float2(0.5, 0.5);

					float2 flowOffset = center + 0.5 * sin(_Time.y * _Speed) * normalize(i.uv - center);
					float flowRadius = length(flowOffset - center);
					float flowAngle = atan2(flowOffset.y - center.y, flowOffset.x - center.x);

					float radius = length(i.uv - center);
					float angle = atan2(i.uv.y - center.y, i.uv.x - center.x);

					if (flowRadius > radius) {
						float2 newOffset = center + 0.5 * sin(_Time.y * _Speed + 0.5 * 3.14159265359f) * normalize(i.uv - center);
						flowRadius = length(newOffset - center);
						flowAngle = atan2(newOffset.y - center.y, newOffset.x - center.x);
					}

					float4 flowTex = tex2D(_FlowTex, center + flowRadius * float2(cos(flowAngle), sin(flowAngle)));
					return flowTex;
				}
				ENDCG
			}
		}
			FallBack "Diffuse"
}