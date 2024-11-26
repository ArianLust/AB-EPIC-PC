// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

Shader "Unlit/Transparent Colored Shiny"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
		_ShinyTex ("Shiny Texture", 2D) = "black" { }
		_ShinyScroll ("Scrollspeed(x) Tiling (y)", Vector) = (0.000000,0.000000,0.000000,0.000000)
		_ShinyTransparency ("Transparency", Range(0.000000,1.000000)) = 1.000000
	}

	SubShader
	{ 
		LOD 100

		Tags
		{
			"QUEUE" = "Transparent"
			"IGNOREPROJECTOR" = "true"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Tags
			{
				"QUEUE" = "Transparent"
				"IGNOREPROJECTOR" = "true"
				"RenderType" = "Transparent"
			}

			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			//ShaderGLESExporter

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
		
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					return o;
				}
				
				uniform sampler2D _ShinyTex;
				uniform float4 _ShinyScroll;
				uniform float _ShinyTransparency;

				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col_1;

					fixed4 tmpvar_2 = tex2D(_MainTex, i.texcoord);
					col_1.w = tmpvar_2.w;
					col_1.xyz = (tmpvar_2.xyz * (1.0 - (0.5 * _ShinyTransparency)));

					float2 tmpvar_3;
					tmpvar_3.x = 1.0;
					tmpvar_3.y = (((i.texcoord.y + 
						(_ShinyScroll.x * _Time.y)
					) - (0.5 * i.texcoord.x)) * _ShinyScroll.y);

					col_1.xyz = (col_1.xyz + (tex2D (_ShinyTex, tmpvar_3).xyz * _ShinyTransparency));
					col_1 = (col_1 * i.color);

					return col_1;
				}
			ENDCG
		}
	}
}