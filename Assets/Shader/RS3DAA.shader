Shader "RageSpline/3D AA" {
SubShader { 
 Tags { "QUEUE"="Transparent+1" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+1" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
  }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
 }
}
}