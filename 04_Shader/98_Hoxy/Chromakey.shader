Shader "HandMade/ChromakeyShader"
{
    Properties	// 인스펙터창 노출 용도
    {       
        _TintColor("Color", color) = (1, 1, 1, 1)

        _MainTex("Closer", 2D) = "white"{}

        _AlphaCut("AlphaCut", Range(0,1)) = 0.05

        _BlackPower("DownBlackPower", Range(0,1)) = 0.1
        _BlackPower2("UpBlackPower", Range(0,1)) = 0.05
        _BlackRange("UpBlackRange", Range(0,0.5)) = 0.15
                
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 0

    }

        SubShader
        {
            Tags // 결국 그리는 순서 결정인듯
            {
                "RenderPipeline" = "UniversalPipeline"
                "RenderType" = "TransparentCutout"
                "Queue" = "AlphaTest"
            }

            Pass	// 화면에 그려질 때는 이 패스를 다 돌고, 그게 1프레임	
            {
                Blend [_SrcBlend] [_DstBlend]  	// 반투명으로 할 때 겹치는 자리에 있는 픽셀은 어떻게 처리할지 지정
                ZWrite[_ZWrite]
                ZTest[_ZTest]
                Stencil
                {
                    Ref 1
                    Comp GEqual
                }
                Name "Universal Forward"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM

                #pragma prefer_hlslcc gles	
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"        	

            // 1. 버텍스 버퍼에서 매쉬정보에 뭘 가져올지 읽어오기 	
            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

        // 2. 버텍스 버퍼에서 계산한 걸 픽셀쉐이더로 넘겨줄 때 어떻게 넘길지 선언(= 보간기)
        struct VertexOutput
        {
            float4 vertex  	: SV_POSITION;
            float2 uv : TEXCOORD0;

        };

        half4 _TintColor;

        float4 _MainTex_ST;
        Texture2D _MainTex;
        SamplerState sampler_MainTex;

        float _AlphaCut, _BlackPower, _BlackPower2, _BlackRange;
        float _Intensity;


        // 3. 버텍스 셰이더
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
            half4 color = _MainTex.Sample(sampler_MainTex, i.uv);
            half4 color2 = _TintColor;
            

            clip(color.b - _AlphaCut);

            if ((1 - i.uv.y)< _BlackRange)
            {
                color2.a = (1 - i.uv.y) * _BlackPower2;
            }
            else
            {
                color2.a = (1 - i.uv.y) * _BlackPower;
            }


            return color2;
        }
        ENDHLSL
    }
        }
}