Shader "DefenseRoguelike/CharacterRift"
{
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; half4 color : COLOR; };
            struct Varyings { float4 positionCS : SV_POSITION; half4 color : COLOR; float2 localPosition : TEXCOORD0; };
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                output.localPosition = input.positionOS.xy;
                return output;
            }
            half4 frag(Varyings input) : SV_Target
            {
                half4 color = input.color;
                float2 p = input.localPosition;
                if (color.a < 0.55)
                {
                    float cloud = sin(p.x * 4 + sin(p.y * 3 + _Time.y)) * sin(p.y * 5 - p.x * 2 + _Time.y);
                    float dust = pow(saturate(sin(p.x * 73 + p.y * 29) * sin(p.y * 91 - p.x * 41)), 28);
                    color.rgb += dust * 0.7;
                    color.a *= 0.45 + cloud * 0.3 + dust;
                }
                return color;
            }
            ENDHLSL
        }
    }
}
