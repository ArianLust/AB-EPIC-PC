Shader "Hidden/CubeBlur" {
Properties {
 _MainTex ("Main", CUBE) = "" { }
 _Texel ("Texel", Float) = 0.007812
 _Level ("Level", Float) = 0.000000
 _Scale ("Scale", Float) = 1.000000
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