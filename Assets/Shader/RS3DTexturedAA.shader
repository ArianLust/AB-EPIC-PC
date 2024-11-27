Shader "RageSpline/3D Textured AA" {
Properties {
 _MainTex ("Texture1 (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+1" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+1" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture * primary, primary alpha }
 }
}
}