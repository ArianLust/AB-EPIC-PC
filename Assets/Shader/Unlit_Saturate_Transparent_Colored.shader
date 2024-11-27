Shader "Custom/Unlit_Saturate_Transparent_Colored" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Saturation ("Saturation", Float) = 1
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
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
        half4 _Color;
        fixed _Saturation;

        half4 frag(v2f i) : SV_TARGET {
          half4 orig;
          half4 texcol;
          orig = texcol = tex2D(_MainTex, i.texcoord);
          half temp = dot(texcol.xyz, half3(0.3, 0.59, 0.11));
          texcol.xyz = half3(temp, temp, temp);
          texcol.xyz = lerp(texcol.xyz, orig.xyz, fixed3(_Saturation, _Saturation, _Saturation)) * _Color.xyz;
          return texcol;
        }

        ENDCG
 }
}
Fallback "Unlit/Transparent"
}