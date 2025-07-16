#ifndef GERSNER_WAVE_NODE_INCLUDED
#define GERSNER_WAVE_NODE_INCLUDED
#define PI 3.14159265
#define E 2.718281828459045
float3 SingleGersner (float waveLength,float amplitude,float steepness,float2 waveDirection, float waveSpeed, float3 p, inout float3 tangent, inout float3 binormal,float t) 
{
    float k = 2 * PI / waveLength;
    float2 d = normalize(waveDirection);
    float f = k * (dot(d, p.xz) - waveSpeed * t);
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
    return frac(sin(seed * 12) * 43758);
}
float Lerp(float A, float B, float t)
{
    return A + (B - A) * t;
}
void GersnerWave_float(int count,float displace, float randomness, float3 position,float amplitude,float MinSteep,float MaxSteep,float MinLength,float MaxLength,float2 waveDirection, float waveSpeed,float time, out float3 Result,out float3 normal, out float height){
    float3 tangent = float3(1, 0, 0);
    float3 binormal = float3(0, 0, 1);
    float3 p = position;
    float3 totalWave = float3(0,0,0);
    float sSpeed = waveSpeed;
    float totalAmp = 1;
    for(int i=0; i< count; i++){
        float2 d = float2(
            waveDirection.x  + randomness * (RandomFloat(-i)-0.5),
            waveDirection.y  + randomness * (RandomFloat(i)-0.5)
        );
        float l = MaxLength / pow(Lerp(1,E,displace),i);
        float s = MaxSteep / pow(Lerp(1,E,displace),i);
        float3 gers = SingleGersner(l,amplitude,s,d,sSpeed,p,tangent,binormal,time+10 * RandomFloat(i));
        totalWave += gers;
    }
    p += totalWave;
    float3 n = normalize(cross(binormal, tangent));
    Result = p;
    normal = n;
    height = totalWave.y;
}
#endif