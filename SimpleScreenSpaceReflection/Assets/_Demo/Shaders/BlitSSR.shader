Shader "Custom/BlitSSR"
{
    SubShader
    {
        Cull Off 
        ZWrite Off 
        ZTest Never
        
        Pass
        {
            Name "BlitSSR"

            Blend One One
            BlendOp Add

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_SSRColorTexture);
            SAMPLER(sampler_SSRColorTexture);

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
                half4 ssrColor = SAMPLE_TEXTURE2D_X(_SSRColorTexture, sampler_SSRColorTexture, uv);
                return lerp(color, ssrColor, ssrColor.a);
            }
            ENDHLSL
        }
    }
}
