#ifndef GERSNER_WAVE_NODE_INCLUDED
#define GERSNER_WAVE_NODE_INCLUDED
#define PI 3.14159265359
#define E 2.718281828459045
float3 SingleGersner (float waveLength,float amplitude,float steepness,float2 waveDirection, float waveSpeed, float3 p, inout float3 tangent, inout float3 binormal,float t) 
{
    float k = 2 * PI / waveLength;
    float2 d = normalize(waveDirection);
    float c = sqrt(9.8 / k);
    float f = k * (dot(d, p.xz) - c*waveSpeed * t);
    float a = steepness / k;

    tangent += float3(
        -d.x * d.x * (steepness * sin(f)),
        d.x * (steepness * cos(f)),
        -d.x * d.y * (steepness * sin(f))
    );
    binormal += float3(
        -d.x * d.y * (steepness * sin(f)),
        d.y * (steepness * cos(f)),
        -d.y * d.y * (steepness * sin(f))
    );
    return float3(
        d.x * (a * cos(f)),
        amplitude * a * sin(f),
        d.y * (a * cos(f))
    );
}
float RandomInRange(float2 seed, float min, float max) {
    float r = frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
    return min + r * (max - min);
}

float2 RandomDirection(float2 seed) {
    float theta = RandomInRange(seed, 0.0, 6.283185307);
    return float2(cos(theta), sin(theta));
}
float RandomFloat(float seed) {
    return frac(sin(seed * 1123152) * 43758);
}
float Lerp(float A, float B, float t)
{
    return A + (B - A) * t;
}
float2 Lerp2(float2 A, float2 B, float t)
{
    return A + (B - A) * t;
}
void GersnerWave_float(int count,float displace, float randomness, float3 position,float amplitude,float MinSteep,float MaxSteep,float MinLength,float MaxLength
    ,float2 waveDirection, float waveSpeed,float time, out float3 Result,out float3 normal, out float height,out float3 color){
    float3 tangent = float3(1, 0, 0);
    float3 binormal = float3(0, 0, 1);
    float3 gridPoint = position;
    float3 p = gridPoint;
    float3 totalWave = float3(0,0,0);
    float sSpeed = waveSpeed;
    int randx = 100072;
    int randy = 802709;
    float amp = amplitude;
    for(int i=0; i< count; i++){
        randx = (randx * 9753) + 19833;
        randy = (randy * 2353) + 2392039;
        float2 rd = normalize(float2(randx,randy));
        float2 d = Lerp2(waveDirection,rd * 2,randomness);
        float thisIndexFrac = 1.0-(float)(i/count-1);
        float alpha = pow(thisIndexFrac,displace);
        
        float l = MaxLength * pow(0.9,i);
        float s = MaxSteep * pow(0.9,i);
        float a = 0.1 + (amp - 0.1) * alpha;
        totalWave += SingleGersner(l,a,s,d ,sSpeed,gridPoint,tangent,binormal,time);
    }
    p += totalWave;
    float3 n = normalize(cross(binormal, tangent));
    Result = p;
    normal = n;
    height = totalWave.y;
    color = lerp(MinLength, MaxLength, displace);
}
#endif