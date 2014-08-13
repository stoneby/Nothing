Shader "Unlit/Transparent Colored"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
					fixed4 color : COLOR;
				};
	
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord1 = v.texcoord1;
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
					float saturation = i.texcoord1.x;
					if (saturation != 1) {
						float rr1 = 0.7 * saturation + 0.3;
						float rr2 = 0.3 * (1 - saturation);
						float gg1 = 0.41 * saturation + 0.59;
						float gg2 = 0.59 * (1 - saturation);
						float bb1 = 0.89 * saturation + 0.11;
						float bb2 = 0.11 * (1 - saturation);
						fixed r = col.r * rr1 + col.g * gg2 + col.b * bb2;
						fixed g = col.r * rr2 + col.g * gg1 + col.b * bb2;
						fixed b = col.r * rr2 + col.g * gg2 + col.b * bb1;
						col.r = r;
						col.g = g;
						col.b = b;
					}
					col.r = col.r + (1 - col.r) * i.texcoord1.y;
					col.g = col.g + (1 - col.g) * i.texcoord1.y;
					col.b = col.b + (1 - col.b) * i.texcoord1.y;
					return col;
				}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
