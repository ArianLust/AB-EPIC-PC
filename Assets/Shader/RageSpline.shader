Shader "RageSpline/Basic" {
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
  }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
 }
}
}