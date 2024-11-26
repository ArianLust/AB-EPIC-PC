Shader "RageSpline/3D Basic Fill" {
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
  }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
 }
}
}