// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/RadialGrayScale"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_RampTex("Base (RGB)", 2D) = "grayscaleRamp" {}
	}

	SubShader
	{
		Pass
		{

		ZTest Always Cull Off ZWrite Off

	CGPROGRAM
	#pragma vertex vert_img
	#pragma fragment frag
	#include "UnityCG.cginc" 

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	uniform sampler2D _MainTex;
	uniform sampler2D _RampTex;
	uniform half _RampOffset;
	uniform float4 _CenterRadius;

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		//o.uv = v.texcoord - _CenterRadius.xy;
		o.uv = v.texcoord;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 original = tex2D(_MainTex, i.uv);
		fixed grayscale = Luminance(original.rgb);

	
		//grayscale = lerp(grayscale, original.rgb, t);

		//float t = lerp(distortedOffset,, i.uv);

		half2 remap = half2 (grayscale + _RampOffset, 0.5);
		
		//half2 remap = half2(lerp(grayscale, original.rgb, t));

		fixed4 output = tex2D(_RampTex, remap);
		output.a = original.a;

		float t = length((i.uv - _CenterRadius.xy) / _CenterRadius.zw) < 1 ? 0 : 1;
		//float t = min(1, length( (i.uv - _CenterRadius.xy) / _CenterRadius.zw));
		return lerp(output, original, t);
	}
		ENDCG

	}
	}

	Fallback off

}
