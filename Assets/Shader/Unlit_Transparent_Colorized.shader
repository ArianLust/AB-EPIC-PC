Shader "Unlit/Transparent Colorized" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM

        #include "UnityCG.cginc"
  			#pragma vertex vert
  			#pragma fragment frag
  			#pragma target 3.0

        struct v2f {
          fixed4 vertex : POSITION;
          fixed2 texcoord : TEXCOORD0;
        };

        fixed4 _MainTex_ST;

        v2f vert(appdata_full v) {
          v2f o;

          o.vertex = UnityObjectToClipPos(v.vertex);
          o.texcoord = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

          return o;
        }

        sampler2D _MainTex;
        fixed4 _Color;

        half4 frag(v2f i) : SV_TARGET {
          // what the hell
          fixed4 c_1;
          half4 tmpvar_2;
          tmpvar_2 = tex2D (_MainTex, i.texcoord);
          c_1 = tmpvar_2;
          fixed3 c_3;
          c_3 = _Color.xyz;
          fixed4 tmpvar_4;
          tmpvar_4.xy = c_3.zy;
          tmpvar_4.zw = fixed2(-1.0, 0.666667);
          fixed4 tmpvar_5;
          tmpvar_5.xy = c_3.yz;
          tmpvar_5.zw = fixed2(0.0, -0.333333);
          fixed4 tmpvar_6;
          tmpvar_6 = lerp (tmpvar_4, tmpvar_5, fixed(_Color.y >= _Color.z));
          fixed4 tmpvar_7;
          tmpvar_7.xyz = tmpvar_6.xyw;
          tmpvar_7.w = c_3.x;
          fixed4 tmpvar_8;
          tmpvar_8.x = c_3.x;
          tmpvar_8.yzw = tmpvar_6.yzx;
          fixed4 tmpvar_9;
          tmpvar_9 = lerp (tmpvar_7, tmpvar_8, fixed(_Color.x >= tmpvar_6.x));
          fixed tmpvar_10;
          tmpvar_10 = (tmpvar_9.x - min (tmpvar_9.w, tmpvar_9.y));
          fixed3 tmpvar_11;
          tmpvar_11.x = abs((tmpvar_9.z + ((tmpvar_9.w - tmpvar_9.y) / ((6.0 * tmpvar_10) + 0.0000000001))));
          tmpvar_11.y = (tmpvar_10 / (tmpvar_9.x + 1e-10));
          tmpvar_11.z = tmpvar_9.x;
          c_1.xyz = (dot (c_1.xyz, fixed3(0.3, 0.59, 0.11)) * lerp (fixed3(1.0, 1.0, 1.0), clamp ((abs(((frac((tmpvar_11.xxx + fixed3(1.0, 0.666667, 0.333333))) * 6.0) - fixed3(3.0, 3.0, 3.0))) - fixed3(1.0, 1.0, 1.0)), 0.0, 1.0), tmpvar_11.yyy));
          return c_1;
        }

        ENDCG
  
  /*
  "!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform fixed4 _MainTex_ST;
varying fixed2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform fixed4 _Color;
varying fixed2 xlv_TEXCOORD0;
void main ()
{
  fixed4 c_1;
  half4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  c_1 = tmpvar_2;
  fixed3 c_3;
  c_3 = _Color.xyz;
  fixed4 tmpvar_4;
  tmpvar_4.xy = c_3.zy;
  tmpvar_4.zw = vec2(-1.0, 0.666667);
  fixed4 tmpvar_5;
  tmpvar_5.xy = c_3.yz;
  tmpvar_5.zw = vec2(0.0, -0.333333);
  fixed4 tmpvar_6;
  tmpvar_6 = mix (tmpvar_4, tmpvar_5, vec4(float((_Color.y >= _Color.z))));
  fixed4 tmpvar_7;
  tmpvar_7.xyz = tmpvar_6.xyw;
  tmpvar_7.w = c_3.x;
  fixed4 tmpvar_8;
  tmpvar_8.x = c_3.x;
  tmpvar_8.yzw = tmpvar_6.yzx;
  fixed4 tmpvar_9;
  tmpvar_9 = mix (tmpvar_7, tmpvar_8, vec4(float((_Color.x >= tmpvar_6.x))));
  highp float tmpvar_10;
  tmpvar_10 = (tmpvar_9.x - min (tmpvar_9.w, tmpvar_9.y));
  fixed3 tmpvar_11;
  tmpvar_11.x = abs((tmpvar_9.z + ((tmpvar_9.w - tmpvar_9.y) / ((6.0 * tmpvar_10) + 1e-10))));
  tmpvar_11.y = (tmpvar_10 / (tmpvar_9.x + 1e-10));
  tmpvar_11.z = tmpvar_9.x;
  c_1.xyz = (dot (c_1.xyz, vec3(0.3, 0.59, 0.11)) * mix (vec3(1.0, 1.0, 1.0), clamp ((abs(((fract((tmpvar_11.xxx + vec3(1.0, 0.666667, 0.333333))) * 6.0) - vec3(3.0, 3.0, 3.0))) - vec3(1.0, 1.0, 1.0)), 0.0, 1.0), tmpvar_11.yyy));
  gl_FragData[0] = c_1;
}



#endif"
  */
 }
}
Fallback "Diffuse"
}