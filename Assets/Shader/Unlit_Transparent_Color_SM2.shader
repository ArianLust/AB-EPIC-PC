Shader "Unlit/Transparent Color SM2" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _OffsetX ("OffsetX", Float) = 0
 _OffsetY ("OffsetY", Float) = 0
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
        fixed _OffsetX;
        fixed _OffsetY;

        v2f vert(appdata_full v) {
          v2f o;

          o.vertex = UnityObjectToClipPos(v.vertex);
          o.texcoord = v.texcoord.xy + fixed2(_OffsetX, _OffsetY) * _MainTex_ST.xy + _MainTex_ST.zw;
          // o.texcoord = (v.texcoord.xy + fixed2(_OffsetX, _OffsetY)) * _MainTex_ST.xy + _MainTex_ST.zw;
          // it should be `o.texcoord = (v.texcoord.xy + fixed2(_OffsetX, _OffsetY)) * _MainTex_ST.xy + _MainTex_ST.zw;` i think

          return o;
        }

        sampler2D _MainTex;
        fixed4 _Color;

        half4 frag(v2f i) : SV_TARGET {
          return tex2D(_MainTex, i.texcoord);
        }

        ENDCG

 }
}
Fallback "Unlit/Transparent"
}