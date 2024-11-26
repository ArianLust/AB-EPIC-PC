// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/VertexColor_Texture" {
	Properties{
	 _MainTex("Base (RGB)", 2D) = "white" { }
	}
		SubShader{
		 Tags { "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		 Pass {
		  Name "FORWARD"
		  Tags { "LIGHTMODE" = "ForwardBase" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		  ZWrite Off
		  Blend SrcAlpha OneMinusSrcAlpha
		  ColorMask RGB
		  GpuProgramID 16804
		CGPROGRAM
		//#pragma target 4.0

		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
		#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
		#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
		#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


		#define CODE_BLOCK_VERTEX
		//uniform float4 unity_SHAr;
		//uniform float4 unity_SHAg;
		//uniform float4 unity_SHAb;
		//uniform float4 unity_SHBr;
		//uniform float4 unity_SHBg;
		//uniform float4 unity_SHBb;
		//uniform float4 unity_SHC;
		//uniform float4x4 UNITY_MATRIX_MVP;
		//uniform float4x4 unity_ObjectToWorld;
		//uniform float4x4 unity_WorldToObject;
		uniform float4 _MainTex_ST;
		uniform sampler2D _MainTex;
		struct appdata_t
		{
			float4 vertex :POSITION;
			float4 color :COLOR;
			float3 normal :NORMAL;
			float4 texcoord :TEXCOORD0;
		};

		struct OUT_Data_Vert
		{
			float2 xlv_TEXCOORD0 :TEXCOORD0;
			float3 xlv_TEXCOORD1 :TEXCOORD1;
			float3 xlv_TEXCOORD2 :TEXCOORD2;
			float4 xlv_COLOR0 :COLOR0;
			float3 xlv_TEXCOORD3 :TEXCOORD3;
			float4 vertex :SV_POSITION;
		};

		struct v2f
		{
			float2 xlv_TEXCOORD0 :TEXCOORD0;
			float4 xlv_COLOR0 :COLOR0;
		};

		struct OUT_Data_Frag
		{
			float4 color :SV_Target0;
		};

		OUT_Data_Vert vert(appdata_t in_v)
		{
			OUT_Data_Vert out_v;
			float3 worldNormal_1;
			float3 tmpvar_2;
			float4 tmpvar_3;
			tmpvar_3.w = 1;
			tmpvar_3.xyz = in_v.vertex.xyz;
			float4 v_4;
			v_4.x = conv_mxt4x4_0(unity_WorldToObject).x;
			v_4.y = conv_mxt4x4_1(unity_WorldToObject).x;
			v_4.z = conv_mxt4x4_2(unity_WorldToObject).x;
			v_4.w = conv_mxt4x4_3(unity_WorldToObject).x;
			float4 v_5;
			v_5.x = conv_mxt4x4_0(unity_WorldToObject).y;
			v_5.y = conv_mxt4x4_1(unity_WorldToObject).y;
			v_5.z = conv_mxt4x4_2(unity_WorldToObject).y;
			v_5.w = conv_mxt4x4_3(unity_WorldToObject).y;
			float4 v_6;
			v_6.x = conv_mxt4x4_0(unity_WorldToObject).z;
			v_6.y = conv_mxt4x4_1(unity_WorldToObject).z;
			v_6.z = conv_mxt4x4_2(unity_WorldToObject).z;
			v_6.w = conv_mxt4x4_3(unity_WorldToObject).z;
			float3 tmpvar_7;
			tmpvar_7 = normalize((((v_4.xyz * in_v.normal.x) + (v_5.xyz * in_v.normal.y)) + (v_6.xyz * in_v.normal.z)));
			worldNormal_1 = tmpvar_7;
			tmpvar_2 = worldNormal_1;
			float3 normal_8;
			normal_8 = worldNormal_1;
			float4 tmpvar_9;
			tmpvar_9.w = 1;
			tmpvar_9.xyz = float3(normal_8);
			float3 res_10;
			float3 x_11;
			x_11.x = dot(unity_SHAr, tmpvar_9);
			x_11.y = dot(unity_SHAg, tmpvar_9);
			x_11.z = dot(unity_SHAb, tmpvar_9);
			float3 x1_12;
			float4 tmpvar_13;
			tmpvar_13 = (normal_8.xyzz * normal_8.yzzx);
			x1_12.x = dot(unity_SHBr, tmpvar_13);
			x1_12.y = dot(unity_SHBg, tmpvar_13);
			x1_12.z = dot(unity_SHBb, tmpvar_13);
			res_10 = (x_11 + (x1_12 + (unity_SHC.xyz * ((normal_8.x * normal_8.x) - (normal_8.y * normal_8.y)))));
			float _tmp_dvx_11 = max(((1.055 * pow(max(res_10, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
			res_10 = float3(_tmp_dvx_11, _tmp_dvx_11, _tmp_dvx_11);
			out_v.vertex = UnityObjectToClipPos(tmpvar_3);
			out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
			out_v.xlv_TEXCOORD1 = tmpvar_2;
			out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
			out_v.xlv_COLOR0 = in_v.color;
			out_v.xlv_TEXCOORD3 = max(float3(0, 0, 0), res_10);
			return out_v;
		}

		#define CODE_BLOCK_FRAGMENT
		OUT_Data_Frag frag(v2f in_f)
		{
			OUT_Data_Frag out_f;
			float4 c_1;
			float4 tmpvar_2;
			tmpvar_2 = in_f.xlv_COLOR0;
			float tmpvar_3;
			tmpvar_3 = tmpvar_2.w;
			float4 c_4;
			float4 c_5;
			c_5.xyz = float3(0, 0, 0);
			c_5.w = tmpvar_3;
			c_4.w = c_5.w;
			c_4.xyz = c_5.xyz;
			c_1.w = c_4.w;
			c_1.xyz = tex2D(_MainTex, in_f.xlv_TEXCOORD0).xyz;
			out_f.color = c_1;
			return out_f;
		}


		ENDCG

		}
		 Pass {
		  Name "FORWARD"
		  Tags { "LIGHTMODE" = "ForwardAdd" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		  ZWrite Off
		  Blend SrcAlpha One
		  ColorMask RGB
		  GpuProgramID 121866
		CGPROGRAM
			//#pragma target 4.0

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
			#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
			#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
			#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


			#define CODE_BLOCK_VERTEX
			//uniform float4x4 UNITY_MATRIX_MVP;
			//uniform float4x4 unity_ObjectToWorld;
			//uniform float4x4 unity_WorldToObject;
			struct appdata_t
			{
				float4 vertex :POSITION;
				float4 color :COLOR;
				float3 normal :NORMAL;
			};

			struct OUT_Data_Vert
			{
				float3 xlv_TEXCOORD0 :TEXCOORD0;
				float3 xlv_TEXCOORD1 :TEXCOORD1;
				float4 xlv_COLOR0 :COLOR0;
				float4 vertex :SV_POSITION;
			};

			struct v2f
			{
				float4 xlv_COLOR0 :COLOR0;
			};

			struct OUT_Data_Frag
			{
				float4 color :SV_Target0;
			};

			OUT_Data_Vert vert(appdata_t in_v)
			{
				OUT_Data_Vert out_v;
				float3 worldNormal_1;
				float3 tmpvar_2;
				float4 tmpvar_3;
				tmpvar_3.w = 1;
				tmpvar_3.xyz = in_v.vertex.xyz;
				float4 v_4;
				v_4.x = conv_mxt4x4_0(unity_WorldToObject).x;
				v_4.y = conv_mxt4x4_1(unity_WorldToObject).x;
				v_4.z = conv_mxt4x4_2(unity_WorldToObject).x;
				v_4.w = conv_mxt4x4_3(unity_WorldToObject).x;
				float4 v_5;
				v_5.x = conv_mxt4x4_0(unity_WorldToObject).y;
				v_5.y = conv_mxt4x4_1(unity_WorldToObject).y;
				v_5.z = conv_mxt4x4_2(unity_WorldToObject).y;
				v_5.w = conv_mxt4x4_3(unity_WorldToObject).y;
				float4 v_6;
				v_6.x = conv_mxt4x4_0(unity_WorldToObject).z;
				v_6.y = conv_mxt4x4_1(unity_WorldToObject).z;
				v_6.z = conv_mxt4x4_2(unity_WorldToObject).z;
				v_6.w = conv_mxt4x4_3(unity_WorldToObject).z;
				float3 tmpvar_7;
				tmpvar_7 = normalize((((v_4.xyz * in_v.normal.x) + (v_5.xyz * in_v.normal.y)) + (v_6.xyz * in_v.normal.z)));
				worldNormal_1 = tmpvar_7;
				tmpvar_2 = worldNormal_1;
				out_v.vertex = UnityObjectToClipPos(tmpvar_3);
				out_v.xlv_TEXCOORD0 = tmpvar_2;
				out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
				out_v.xlv_COLOR0 = in_v.color;
				return out_v;
			}

			#define CODE_BLOCK_FRAGMENT
			OUT_Data_Frag frag(v2f in_f)
			{
				OUT_Data_Frag out_f;
				float4 tmpvar_1;
				tmpvar_1 = in_f.xlv_COLOR0;
				float tmpvar_2;
				tmpvar_2 = tmpvar_1.w;
				float4 c_3;
				float4 c_4;
				c_4.xyz = float3(0, 0, 0);
				c_4.w = tmpvar_2;
				c_3.w = c_4.w;
				c_3.xyz = c_4.xyz;
				out_f.color = c_3;
				return out_f;
			}


			ENDCG

			}
	}
		Fallback "Unlit/Texture"
}