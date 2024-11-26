Shader "RageSpline/MultiTextured" {
Properties {
 _MainTex ("Texture1 (RGB)", 2D) = "white" {}
 _MainTex2 ("Texture2 (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
  }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture * primary, primary alpha }
  SetTexture [_MainTex2] { combine texture * previous double, texture alpha * primary alpha }
 }
}
}