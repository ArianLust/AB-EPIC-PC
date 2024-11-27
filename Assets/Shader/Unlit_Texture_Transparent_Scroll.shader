Shader "Chimera/Unlit_Texture_Transparent_Scroll" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _ScrollSpeed ("Scroll Speed", Vector) = (0.000000,0.000000,0.000000,0.000000)
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