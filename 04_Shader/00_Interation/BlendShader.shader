Shader "HandMade/BlendShader"
{
	Properties	// �ν�����â ���� �뵵
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

			Tags // �ᱹ �׸��� ���� �����ε�
			{
				"RenderPipeline" = "UniversalPipeline"
				"RenderType" = "Transparent"
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


			// 3. ���ؽ� ���̴�(�ֱٿ� ���� �ػ󵵰� �� �������� ���⼭ ����� �� ������ �ϴ°� ����)
			// ���ؽ��� ȭ�鿡 �׸��� ����
			// �̰� �׸� �� ��ġ�� �ٲ��ָ� ���� �Ž��� ������..
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;

				o.vertex = TransformObjectToHClip(v.vertex.xyz);

				o.uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}

			// 4. �ȼ� ���̴�
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