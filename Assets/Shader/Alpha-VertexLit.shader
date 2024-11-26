Shader "Legacy Shaders/Transparent/VertexLit" {
Properties {
 _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _SpecColor ("Spec Color", Color) = (1.000000,1.000000,1.000000,0.000000)
 _Emission ("Emissive Color", Color) = (0.000000,0.000000,0.000000,0.000000)
 _Shininess ("Shininess", Range(0.100000,1.000000)) = 0.700000
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}