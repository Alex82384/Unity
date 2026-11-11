// outline shader for urp
// draws the mesh again pushed out from the pivot with front faces culled
// works for centered shapes like cube sphere cylinder
Shader "VRTraining/OutlineHull"
{
    Properties
    {
        [HDR] _OutlineColor ("Outline Color", Color) = (0.2, 1, 0.45, 1)
        _OutlineWidth ("Outline Width", Float) = 0.02
        // 0 sideways only for flat floor objects 1 all directions
        _VerticalWeight ("Vertical Weight", Range(0, 1)) = 1
        // lifts the outline so it does not flicker on the floor
        _Lift ("Lift", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry+1" }

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
                float _VerticalWeight;
                float _Lift;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                // needed for single pass instanced vr rendering
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

                // direction away from the pivot
                float3 directionOS = float3(input.positionOS.x, input.positionOS.y * _VerticalWeight, input.positionOS.z);
                float3 directionWS = float3(0, 0, 0);
                if (dot(directionOS, directionOS) > 1e-6)
                {
                    directionWS = TransformObjectToWorldDir(directionOS, true);
                }

                positionWS += directionWS * _OutlineWidth;
                positionWS.y += _Lift;
                output.positionCS = TransformWorldToHClip(positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
