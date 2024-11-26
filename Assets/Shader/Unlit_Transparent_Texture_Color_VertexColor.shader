Shader "Unlit/Transparent Color VertexColor" {
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
          fixed4 color : COLOR;
        };

        fixed4 _MainTex_ST;
        fixed4 _Color;

        v2f vert(appdata_full v) {
          v2f o;

          o.vertex = UnityObjectToClipPos(v.vertex);
          o.texcoord = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
          o.color = v.color * _Color;

          return o;
        }

        sampler2D _MainTex;

        half4 frag(v2f i) : SV_TARGET {
          return tex2D(_MainTex, i.texcoord) * i.color;
        }

        ENDCG
 }
}
Fallback "Diffuse"
}