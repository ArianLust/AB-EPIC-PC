Shader "RageSpline/Textured (alpha)" {
Properties {
 _MainTex ("Texture1 (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture lerp(texture) primary }
 }
}
}