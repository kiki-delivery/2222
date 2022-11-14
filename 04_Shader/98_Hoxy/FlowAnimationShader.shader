Shader "HandMade/FlowAnimationShader"
{
	Properties	// �ν�����â ���� �뵵
	{
		_TintColor("Test Color", color) = (1, 1, 1, 1)
		_Intensity("_Intensity", Range(0, 100)) = 0.5
		_MainTex("Main Texture", 2D) = "white"{}

		_FlowTime("Flow Time", Range(0, 10)) = 1
		_FlowIntensity("Flow Intensity", Range(0, 2)) = 1

		_AlphaCut("AlphaCut", Range(0,1)) = 0.05
		_TextureSize("TextureSize", Range(1, 10)) = 1

		[NoScaleOffset]_Flowmap("Flowmap", 2D) = "white"{}  // [NoScaleOffset] �ν����� ����뵵		

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
	}

		SubShader
		{

			Tags // �ᱹ �׸��� ���� �����ε�
			{
				"RenderPipeline" = "UniversalPipeline"
				"RenderType" = "Opaque"
				"Queue" = "Geometry"
			}

			Pass	// ȭ�鿡 �׷��� ���� �� �н��� �� ����, �װ� 1������	
			{
				Name "Universal Forward"
				Tags { "LightMode" = "UniversalForward" }

				Blend[_SrcBlend][_DstBlend]  	// ���������� �� �� ��ġ�� �ڸ��� �ִ� �ȼ��� ��� ó������ ����

				HLSLPROGRAM
				#pragma prefer_hlslcc gles	
				#pragma exclude_renderers d3d11_9x
				#pragma vertex vert
				#pragma fragment frag

			//cg shader�� .cginc�� hlsl shader�� .hlsl�� include�ϰ� �˴ϴ�.
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"        	

			half4 _TintColor;
			float _Intensity, _FlowTime, _FlowIntensity;

			float4 _MainTex_ST;
			Texture2D _MainTex, _Flowmap;
			SamplerState sampler_MainTex;

			float _AlphaCut;
			int _TextureSize;
			// ���̴��� ũ�� 4���� ����

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


			// 3. ���ؽ� ���̴�(�ֱٿ� ���� �ػ󵵰� �� �������� ���⼭ ����� �� ������ �ϴ°� ����)
			// ���ؽ��� ȭ�鿡 �׸��� ����
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;

				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.positionWS = TransformObjectToWorld(v.vertex.xyz);

				o.uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}

			// 4. �ȼ� ���̴�
			half4 frag(VertexOutput i) : SV_Target
			{
				float4 flow = _Flowmap.Sample(sampler_MainTex, i.uv);

				i.positionWS.y = i.positionWS.y + frac(_Time.y * _FlowTime) + (flow.rg * _FlowIntensity);	//_Time�� float4 �����̶� xyzw�߿� �ϳ� �����(x=1/20, y = t, z = 2��, w=��)
													// Ÿ�� ���� �� �����Ӹ��� ������
													// _FlowTime�� ���ϸ� ���� ��ġ�� ������ �������ų� ���� ����
													// frac���ϸ� Ÿ�� ���� ������ Ŀ���ٴµ� ������..

				float4 color = _MainTex.Sample(sampler_MainTex, i.positionWS.xy / _TextureSize);
				color.rgb = color.rgb * _Intensity;
				clip(color.b - _AlphaCut);
				return color;
			}

			ENDHLSL
		}
		}
}