Shader "HandMade/BlendShader"
{
	Properties	// 인스펙터창 노출 용도
	{
		_TintColor("Test Color", color) = (1, 1, 1, 1)
		_Intensity("_Intensity", Range(0, 100)) = 0.5
		_MainTex("Main Texture", 2D) = "white"{}

		_AlphaCut("AlphaCut", Range(0,1)) = 0.05

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
	}

		SubShader
		{

			Tags // 결국 그리는 순서 결정인듯
			{
				"RenderPipeline" = "UniversalPipeline"
				"RenderType" = "Transparent"
				"Queue" = "Geometry"
			}

			Pass	// 화면에 그려질 때는 이 패스를 다 돌고, 그게 1프레임	
			{
				Name "Universal Forward"
				Tags { "LightMode" = "UniversalForward" }

				Blend[_SrcBlend][_DstBlend]  	// 반투명으로 할 때 겹치는 자리에 있는 픽셀은 어떻게 처리할지 지정

				HLSLPROGRAM
				#pragma prefer_hlslcc gles	
				#pragma exclude_renderers d3d11_9x
				#pragma vertex vert
				#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"        	

			half4 _TintColor;
			float _Intensity;

			float4 _MainTex_ST;
			Texture2D _MainTex;
			SamplerState sampler_MainTex;

			float _AlphaCut;

			struct VertexInput
			{
				float4 vertex : POSITION;
				float2 uv 	  : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 vertex  	: SV_POSITION;
				float2 uv 	    : TEXCOORD0;
			};


			// 3. 버텍스 셰이더(최근에 기기들 해상도가 다 높아져서 여기서 계산할 수 있으면 하는게 나음)
			// 버텍스를 화면에 그리는 역할
			// 이걸 그릴 때 위치를 바꿔주면 실제 매쉬가 움직임..
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;

				o.vertex = TransformObjectToHClip(v.vertex.xyz);

				o.uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}

			// 4. 픽셀 셰이더
			half4 frag(VertexOutput i) : SV_Target
			{
				float4 color = _MainTex.Sample(sampler_MainTex, i.uv);
				color.rgb = color.rgb * _TintColor * _Intensity;
				clip(color.g - _AlphaCut);
				return color;
			}

			ENDHLSL
		}
		}
}