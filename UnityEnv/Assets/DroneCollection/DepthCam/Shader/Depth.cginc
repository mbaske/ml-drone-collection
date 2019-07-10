// Code adapted from https://github.com/keijiro/KinoVision

#include "Common.cginc"

sampler2D_float _CameraDepthTexture;
int _Channel;
int _Invert;
int _Isolate;

float LinearizeDepth(float z)
{
    float isOrtho = unity_OrthoParams.w;
    float isPers = 1 - unity_OrthoParams.w;
    z *= _ZBufferParams.x;
    return (1 - isOrtho * z) / (isPers * z + _ZBufferParams.y);
}

half3 DepthFragment(CommonVaryings input) : SV_Target
{
    half4 src = tex2D(_MainTex, input.uv0);
    half3 rgb = src;

    if (_Channel != 0)
    {
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.uv1);
        depth = _Invert ? LinearizeDepth(depth) : 1 - LinearizeDepth(depth);
        
        // No bit operators? 
        // http://developer.download.nvidia.com/CgTutorial/cg_tutorial_chapter03.html
        if (_Channel == 1)
        {
            rgb = half3(1 - depth, _Isolate ? 0 : src.g * depth, _Isolate ? 0 : src.b * depth);
        }
        else if (_Channel == 2)
        {
            rgb = half3(_Isolate ? 0 : src.r * depth, 1 - depth, _Isolate ? 0 : src.b * depth);
        }
        else if (_Channel == 4)
        {
            rgb = half3(_Isolate ? 0 : src.r * depth, _Isolate ? 0 : src.g * depth, 1 - depth);
        }
        else if (_Channel == 7)
        {
            rgb = half3(1 - depth, 1 - depth, 1 - depth);
        }
    }
    else if (_Invert)
    {
        rgb = half3(1 - src.r, 1 - src.g, 1 - src.b);
    }

#if !UNITY_COLORSPACE_GAMMA
    rgb = GammaToLinearSpace(rgb);
#endif

    return rgb;
}
