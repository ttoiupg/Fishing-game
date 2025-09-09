#ifndef BLUR_NODE_INCLUDED
#define BLUR_NODE_INCLUDED
#define PI 3.14159265359
#define E 2.718281828459045

float4 BlurHorizontal(UnityTexture2D tex,float2 textureSize, float2 uv,UnitySamplerState samplerState,int offset){
    float4 baseColor = float4(0,0,0,0);
    for (int i= -offset; i <= offset; i++)
    {
        baseColor += tex.Sample(samplerState,uv + float2(i/textureSize.x,0));
    }
    baseColor /=  offset + 1;
    return baseColor;
}
float4 BlurVertical(UnityTexture2D tex,float2 textureSize, float2 uv,UnitySamplerState samplerState,int offset){
    float4 baseColor = float4(0,0,0,0);
    for (int i= -offset; i <= offset; i++)
    {
        baseColor += tex.Sample(samplerState,uv + float2(0,i/textureSize.y));
    }
    baseColor /=  offset + 1;
    return baseColor;
}

void Blur_float(UnityTexture2D tex,float2 textureSize, float2 uv,UnitySamplerState samplerState,int offset,out float3 RGB,out float Alpha)
{
    float4 bH = BlurHorizontal(tex,textureSize,uv,samplerState,offset);
    float4 bV = BlurVertical(tex,textureSize,uv,samplerState,offset);
    float4 baseColor = (bH + bV)/2;
    RGB = baseColor.xyz;
    Alpha = baseColor.w;
}

#endif