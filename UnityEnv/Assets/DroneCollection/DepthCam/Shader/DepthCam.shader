// Code adapted from https://github.com/keijiro/KinoVision

Shader "Custom/DepthCam"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _Channel("Channel", Int) = 0
        [Toggle] _Invert("Invert", Int) = 0
        [Toggle] _Isolate("Isolate", Int) = 0
    }
    Subshader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "Depth.cginc"
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #pragma vertex CommonVertex
            #pragma fragment DepthFragment
            #pragma target 3.0
            ENDCG
        }
    }
}
