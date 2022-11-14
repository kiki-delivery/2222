Shader "HandMade/FlowAnimationShader"
{
	Properties	// 인스펙터창 노출 용도
	{
		_TintColor("Test Color", color) = (1, 1, 1, 1)
		_Intensity("_Intensity", Range(0, 100)) = 0.5
		_MainTex("Main Texture", 2D) = "white"{}

		_FlowTime("Flow Time", Range(0, 10)) = 1
		_FlowIntensity("Flow Intensity", Range(0, 2)) = 1

		_AlphaCut("AlphaCut", Range(0,1)) = 0.05
		_TextureSize("TextureSize", Range(1, 10)) = 1

		[NoScaleOffset]_Flowmap("Flowmap", 2D) = "white"{}  // [NoScaleOffset] 인스펙터 제어용도		

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
	}

		SubShader
		{

			Tags // 결국 그리는 순서 결정인듯
			{
				"RenderPipeline" = "UniversalPipeline"
				"RenderType" = "Opaque"
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

			//cg shader는 .cginc를 hlsl shader는 .hlsl을 include하게 됩니다.
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"        	

			half4 _TintColor;
			float _Intensity, _FlowTime, _FlowIntensity;

			float4 _MainTex_ST;
			Texture2D _MainTex, _Flowmap;
			SamplerState sampler_MainTex;

			float _AlphaCut;
			int _TextureSize;
			// 쉐이더는 크게 4가지 구성

			struct VertexInput
			{
				float4 vertex : POSITION;
				float2 uv 	  : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 vertex  	: SV_POSITION;
				float2 uv 	    : TEXCOORD0;
				float3 positionWS : COLOR;
			};


			// 3. 버텍스 셰이더(최근에 기기들 해상도가 다 높아져서 여기서 계산할 수 있으면 하는게 나음)
			// 버텍스를 화면에 그리는 역할
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;

				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.positionWS = TransformObjectToWorld(v.vertex.xyz);

				o.uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}

			// 4. 픽셀 셰이더
			half4 frag(VertexOutput i) : SV_Target
			{
				float4 flow = _Flowmap.Sample(sampler_MainTex, i.uv);

				i.positionWS.y = i.positionWS.y + frac(_Time.y * _FlowTime) + (flow.rg * _FlowIntensity);	//_Time이 float4 형식이라 xyzw중에 하나 써야함(x=1/20, y = t, z = 2배, w=배)
													// 타임 값을 매 프레임마다 더해줌
													// _FlowTime을 더하면 시작 위치가 변하지 빨라지거나 하지 않음
													// frac안하면 타임 값이 무한히 커진다는데 왜인지..

				float4 color = _MainTex.Sample(sampler_MainTex, i.positionWS.xy / _TextureSize);
				color.rgb = color.rgb * _Intensity;
				clip(color.b - _AlphaCut);
				return color;
			}

			ENDHLSL
		}
		}
}