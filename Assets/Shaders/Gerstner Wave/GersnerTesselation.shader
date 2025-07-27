// This shader adds tessellation in URP
Shader "Example/GersnerTesselation"
{

    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    {
        [Header(Gersner waves)]
        [Space(25)]
    	_WaveCount("Wave count", Integer) = 1
        _WaveSpeed("Wave speed", Float) = 1
        _Steepness ("Steepness", Range(0, 1)) = 0.5
        _Amplitude("Amplitude", Float) = 1
        _WaveLength("Wave length",Float) = 10
        _Direction("Direction",Vector) = (1,0,0,0)
    	_Randomness("Randomness",Float) = 0
    	_Displace("Displace",Float) = 0
        
        [Header(Render)]
        [Space(25)]
    	_LightDirection("Light direction",Float) = (0,0,0)
    	_TopFactor("Top factor",Integer) = 0
    	[HDR]_TopColor("Top color", Color) = (1,1,1,0)
    	[HDR]_BottomColor("Bottom color", Color) = (1,1,1,0)
        _SpecularSize("Specular size",Float) = 1
        _SpecularShininess("Specular shininess",float) = 1
        [Space(10)]
        [Header(Tesselation)]
        [Space(25)]
        _Tess("Tessellation", Range(1, 32)) = 20
        _MaxTessDistance("Max Tess Distance", Range(1, 128)) = 20
        _Noise("Noise", 2D) = "gray" {}
        _Weight("Displacement Amount", Range(0, 1)) = 0
    }

    // The SubShader block containing the Shader code. 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }


            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "CustomTessellation.hlsl"


            #pragma require tessellation
            // This line defines the name of the vertex shader. 
            #pragma vertex TessellationVertexProgram
            // This line defines the name of the fragment shader. 
            #pragma fragment frag
            // This line defines the name of the hull shader. 
            #pragma hull hull
            // This line defines the name of the domain shader. 
            #pragma domain domain


            sampler2D _Noise;
            float _Weight;
            float _WaveSpeed,_Amplitude, _WaveLength;
            float _SpecularShininess, _SpecularSize
            ,_Steepness,_Randomness,_Displace;

            float3 _LightDirection;
            int _TopFactor;
            float2 _Direction;
            int _WaveCount;
            float4 _TopColor,_BottomColor;
            // pre tesselation vertex program
            ControlPoint TessellationVertexProgram(Attributes v)
            {
                ControlPoint p;

                p.vertex = v.vertex;
                p.uv = v.uv;
                p.normal = v.normal;
                p.color = v.color;
                return p;
            }
            float3 SingleGersnerWave(float2 dir, float steep, float length, float amp, float speed,float3 p, inout float3 tangent, inout float3 binormal )
            {
            	float pi = 3.14159265359;
                float k = 2 * pi / length;
                float c = sqrt(9.8 / k);
                float2 d = normalize(dir);
                float f = k * (dot(d,p.xz) - speed * c * _Time.y);
			    float a = steep / k;
			    tangent += float3(
				    - d.x * d.x * (steep * sin(f)),
				    d.x * (steep * cos(f)),
				    -d.x * d.y * (steep * sin(f))
			    );
			    binormal += float3(
				    -d.x * d.y * (steep * sin(f)),
				    d.y * (steep * cos(f)),
				    - d.y * d.y * (steep * sin(f))
			    );
            	return float3(
            		d.x * (a * cos(f)),
					amp * a * sin(f),
					d.y * (a * cos(f))
            	);
            }
            // after tesselation
            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 gridPoint = input.vertex.xyz;
                //do gersnerwave
			    float3 tangent = float3(1,0,0);
			    float3 binormal = float3(0,0,1);
            	float3 p = gridPoint;

            	//multiple gersner wave
            	float3 totalDisplacement = float3(0,0,0);
            	float2 dir = _Direction;
            	int randx = 100072;
				int randy = 802709;
            	for (int i=0;i<_WaveCount;i++)
            	{
            		randx = (randx * 9753) + 19833;
			        randy = (randy * 2353) + 2392039;
			        float2 rd = normalize(float2(randx,randy));
			        float2 d = lerp(dir,rd * 2,_Randomness);
            		float thisIndexFrac = 1.0-(float)(i/_WaveCount-1);
					float alpha = pow(thisIndexFrac,_Displace);
            		float l = _WaveLength * pow(0.9,i);
			        float s = _Steepness * pow(0.9,i);
			        float a = 0.1 + (_Amplitude - 0.1) * alpha;
            		totalDisplacement += SingleGersnerWave(d,s,l,a,_WaveSpeed,gridPoint,tangent,binormal);
            		
            	}
            	p += totalDisplacement;
			    float3 normal = normalize(cross(binormal, tangent));
                input.normal = normal;
            	
                //apply to transfer data
                float3 worldPos = TransformObjectToWorld(input.vertex);
                output.vertex = TransformObjectToHClip(p);
            	output.data = float4(totalDisplacement.y,0,0,0);
                output.color = input.color;
                output.normal = normal;
                output.viewDir = GetWorldSpaceNormalizeViewDir(worldPos);
                output.uv = input.uv;
                return output;
            }

            [UNITY_domain("tri")]
            Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch,
                           float3 barycentricCoordinates : SV_DomainLocation)
            {
                Attributes v;

                #define DomainPos(fieldName) v.fieldName = \
					patch[0].fieldName * barycentricCoordinates.x + \
					patch[1].fieldName * barycentricCoordinates.y + \
					patch[2].fieldName * barycentricCoordinates.z;

                DomainPos(vertex)
                DomainPos(uv)
                DomainPos(color)
                DomainPos(normal)

                return vert(v);
            }

            // The fragment shader definition.            
            half4 frag(Varyings IN) : SV_Target
            {
                
                Light mainLight = GetMainLight();
            	float3 LightDirection = normalize(_LightDirection);
                float3 normal = IN.normal;
            	float3 wnormal = TransformObjectToWorldNormal(IN.normal);
                float brightness = dot(IN.normal,LightDirection);
                float3 h = normalize(IN.viewDir + LightDirection);
            	float smoothness = exp2((10 * _SpecularSize)+1);
                float specular = pow(saturate(dot(h,wnormal)),smoothness);
            	float subsurfaceFactor =pow(saturate(dot(normal,IN.viewDir)),_TopFactor);
            	float4 color = lerp(_BottomColor,_TopColor,subsurfaceFactor);// * brightness;
                color += specular > _SpecularShininess;
                return color;
            }
            ENDHLSL
        }
    }
}