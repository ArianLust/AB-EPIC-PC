Shader "Custom/Color" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  Color [_Color]
  Material {
   Ambient [_Color]
   Diffuse [_Color]
  }
  ZWrite Off
 }
}
}