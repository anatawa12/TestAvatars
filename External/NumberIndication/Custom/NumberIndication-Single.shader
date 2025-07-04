/* 2018-12-05 Written by Ureishi. */
/* 2025-07-04 modified to show single float value by anatawa12. */

Shader "NumberIndication/Single Value" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[IntRange]
		_NumOfDigit ("Number of Digit", Range(5,9)) = 5
		[IntRange]
		_NumOfFract ("Number of Fraction", Range(1,4)) = 1

		_Value ("Value", Float) = 0.0
		
		[NoScaleOffset]
		_MainTex ("Character Map", 2D) = "white" {}
		_CharCut ("Character Cut (X, Y)", Vector) = (8,4,0,0)
		_Emission ("Emission", Range(-1,5)) = 1.0
		_Smoothness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		[Header(Rendering)]
		[Enum(UnityEngine.Rendering.CullMode)]
		_Cull ("Cull", Int) = 2 /* Back */
		[Enum(UnityEngine.Rendering.CompareFunction)]
		_ZTest ("ZTest", Float) = 4 /* LessEqual */
		[Enum(Off, 0, On, 1)]
		_ZWrite ("ZWrite", Int) = 1 /* On */
		[Enum(UnityEngine.Rendering.BlendMode)]
		_SrcFactor ("Src Factor", Int) = 5 /* SrcAlpha */
		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstFactor ("Dst Factor", Int) = 10 /* OneMinusSrcAlpha */
	}
	SubShader {
		Tags {
			"RenderType"="Transparent"
			"Queue"="Transparent"
			"IgnoreProjector"="True"
		}
		
		LOD 100
		
		Cull [_Cull]
		ZTest [_ZTest]
		ZWrite [_ZWrite]
		Blend [_SrcFactor][_DstFactor]
		
        Pass {
            ZWrite On
            ColorMask 0
        }
		
		CGPROGRAM
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0
		
		fixed4 _Color;
		uint _NumOfDigit;
		uint _NumOfFract;
		sampler2D _MainTex;
		uint4 _CharCut;
		half _Emission;
		half _Smoothness;
		half _Metallic;
		float _Value; // The value to display

		struct Input {
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			/* ************************************************** */
			static const uint n = 1; // we only have 1 value to display
			/* ************************************************** */
			static const uint rdx = 10;
			const uint nod = _NumOfDigit;
			const uint nof = _NumOfFract;
			const uint noi = nod - nof;
			const uint noR = nod + 3;
			const uint noC = n;
			const float2 uv = IN.uv_MainTex;
			const uint2 norc = {noR, noC};
			const uint2 crc = floor(uv*norc) * int2(1,-1) + int2(0,noC-1);
			const float2 fuv = frac(uv*norc);
			const uint2 cut = _CharCut.xy;
			const int val = _Value * round(pow((float)rdx, nof));
			const int sgn = val < 0? -1:1;
			const uint nth = noR - 2 - crc.x + (crc.x>noi+2?1:0);
			const uint expRdx = round(pow((float)rdx, nth));
			const uint dgt =
				crc.x == 0? 15 : // blank
				crc.x == 1? (10*2 + uint(1-sgn))/2 :
				crc.x == noi+2? 13 :
				crc.y < n? (uint(sgn*val)/expRdx)%rdx : 0;
			const float2 ruv =
				{(fuv.x+dgt%cut.x)/cut.x, 1-((1-fuv.y)+dgt/cut.x)/cut.y};
			const fixed4 c = tex2D(_MainTex, ruv);
			const fixed4 clr = _Color;
			
			o.Albedo = c.rgb * clr.rgb;
			o.Alpha = c.a * clr.a;
			o.Emission = o.Albedo * _Emission;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
		}
		ENDCG
	}
	Fallback "Unlit/Transparent"
}
