/*Shader "Custom/NewSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}*/

Shader "Custom/PartialAlphaShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_LeftLimit("Left Limit: World Pos X", Float) = -2.0
		_RightLimit("Right Limit: World Pos X", Float) = 2.0
		_TopLimit("Top Limit: World Pos Y", Float) = -2.0
		_BottomLimit("Bottom Limit: World Pos Y", Float) = 2.0
	}

	SubShader{
		Lighting Off
		AlphaTest Greater 0.5

		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			Blend One OneMinusSrcAlpha
			LOD 200

			CGPROGRAM
	#pragma surface surf NoLighting
	#include "UnityCG.cginc"

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		float _LeftLimit;
		float _RightLimit;
		float _TopLimit;
		float _BottomLimit;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			//clip((7.0f)); // IN.worldPos.y
			if (IN.worldPos.x - _LeftLimit < 0 || _RightLimit - IN.worldPos.x < 0 ||
				IN.worldPos.y - _TopLimit < 0 || _BottomLimit - IN.worldPos.y < 0) {
				clip(-1.0);
			}

			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
