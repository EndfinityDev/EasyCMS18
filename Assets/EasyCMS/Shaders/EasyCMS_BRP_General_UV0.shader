Shader "EasyCMS/EasyCMS_BRP_General_UV0" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_BackfaceColor ("Backface Color", Color) = (0,0,0,0)
		_MainTex ("MainTex", 2D) = "white" {}
		_DiffuseMap ("Diffuse Map", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_NormalMapStrength ("NormalMapStrength", float) = 1.0
		_Roughness ("Roughness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_AlphaClipThreshold ("Alpha Clip Threshold", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="AlphaTest" }
		//Tags { "RenderType"="Transparent" "Queue"="Transparent" } for glass
		LOD 200
		//Cull Off
		ZWrite On

		Cull Back
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alphatest:_AlphaClipThreshold
		//#pragma surface surf Standard alpha:premul // for glass

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DiffuseMap;
		sampler2D _NormalMap;
		float _NormalMapStrength;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DiffuseMap;
			float2 uv_NormalMap;
			fixed facing : VFACE;
			UNITY_VERTEX_INPUT_INSTANCE_ID
			INTERNAL_DATA
		};

		half _Roughness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_DiffuseMap, IN.uv_DiffuseMap) * _Color;
			fixed4 a = tex2D (_MainTex, IN.uv_MainTex).r;
			fixed4 normal = tex2D(_NormalMap, IN.uv_NormalMap);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = 1 - _Roughness;
			o.Alpha = a * c.a;
			o.Normal = UnpackScaleNormal(normal, _NormalMapStrength);
			if (IN.facing < 0) // VFACE is typically 1 for front, -1 for back.
			{
			    o.Normal *= -1;
			}
		}
		ENDCG

		
		Cull Front
		ZWrite On

		CGPROGRAM
		#pragma surface surf Standard alphatest:_AlphaClipThreshold
		#pragma target 3.0
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			fixed facing : VFACE;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		fixed4 _BackfaceColor;

		void surf (Input IN, inout SurfaceOutputStandard o) {
               // Apply the backface color
			fixed4 a = tex2D (_MainTex, IN.uv_MainTex).r;
               o.Albedo = _BackfaceColor.rgb;
               o.Alpha = a;
			   //if (IN.facing < 0) // VFACE is typically 1 for front, -1 for back.
				//{
					//o.Normal *= -1;
				//}
           }

		ENDCG
		
	}
	FallBack "Diffuse"
}
