Shader "Custom/FFTWater" {
		
		Properties {
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
			_DisplacementDepthAttenuation("Displacement Depth Attenuation",Float) = 1
			[Header(Render)]
	        [Space(25)]
			_EnviromentMap("EnviromentMap",2D) = "gray" {}
			_EnvironmentLightStrength("_EnvironmentLightStrength",Float) = 0
			_ScatterStrength("_ScatterStrength",Float) = 0
			_ScatterShadowStrength("_ScatterShadowStrength",Float) = 0
			_ScatterColor("_ScatterColor",Color) = (0,0,0)
			_FresnelNormalStrength("FresnelNormalStrength",Float) = 0
			_Roughness("Roughness",Float) = 0
			_SunIrradiance("SunIrradiance", Color) = (0,0,0)
			_Ambient("Ambient", Color) = (0,0,0)
    		_SunDirection("sun direction",Float) = (0,0,0)
    		_TopFactor("Top factor",Integer) = 0
    		[HDR]_TopColor("Top color", Color) = (1,1,1,0)
    		[HDR]_BottomColor("Bottom color", Color) = (1,1,1,0)
	        _SpecularSize("Specular size",Float) = 1
	        _SpecularShininess("Specular shininess",float) = 1
			[Space(10)]
	        [Header(Tesselation)]
	        [Space(25)]
	        _TessellationEdgeLength("Tessellation", Range(0, 1)) = 1
	        _MaxTessDistance("Max Tess Distance", Range(1, 128)) = 20
			[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
		}

	CGINCLUDE

    ENDCG

	SubShader {
		Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"
        }
		Pass {
			Tags {
				"LightMode" = "UniversalForward"
			}
			CGPROGRAM
			#pragma target 4.6
			#pragma vertex dummyvp
			#pragma hull hp
			#pragma domain dp 
			#pragma geometry gp
			#pragma fragment fp
			
			#include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"
			#define NEW_LIGHTING
			
			
	        float _TessellationEdgeLength = 1;
			float _MaxTessDistance;

	        struct TessellationFactors {
	            float edge[3] : SV_TESSFACTOR;
	            float inside : SV_INSIDETESSFACTOR;
	        };

	        float TessellationHeuristic(float3 cp0, float3 cp1) {
	            float edgeLength = distance(cp0, cp1);
	            float3 edgeCenter = (cp0 + cp1) * 0.5;
	            float viewDistance = distance(edgeCenter, _WorldSpaceCameraPos);

	            return edgeLength * _ScreenParams.y / (_TessellationEdgeLength * (pow(viewDistance, _MaxTessDistance)));
	        }

	        bool TriangleIsBelowClipPlane(float3 p0, float3 p1, float3 p2, int planeIndex, float bias) {
	            float4 plane = unity_CameraWorldClipPlanes[planeIndex];

	            return dot(float4(p0, 1), plane) < bias && dot(float4(p1, 1), plane) < bias && dot(float4(p2, 1), plane) < bias;
	        }

	        bool cullTriangle(float3 p0, float3 p1, float3 p2, float bias) {
	            return TriangleIsBelowClipPlane(p0, p1, p2, 0, bias) ||
	                   TriangleIsBelowClipPlane(p0, p1, p2, 1, bias) ||
	                   TriangleIsBelowClipPlane(p0, p1, p2, 2, bias) ||
	                   TriangleIsBelowClipPlane(p0, p1, p2, 3, bias);
	        }
			struct TessellationControlPoint {
                float4 vertex : INTERNALTESSPOS;
                float2 uv : TEXCOORD0;
            };

			struct VertexData {
				float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
			};

			struct v2g {
				float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 normal : NORMAL;
				float depth : TEXCOORD2;
			};


			#define PI 3.14159265358979323846

			float hash(uint n) {
				// integer hash copied from Hugo Elias
				n = (n << 13U) ^ n;
				n = n * (n * n * 15731U + 0x789221U) + 0x1376312589U;
				return float(n & uint(0x7fffffffU)) / float(0x7fffffff);
			}

			float3 _SunDirection, _SunColor;

			float _NormalStrength, _FresnelNormalStrength, _SpecularNormalStrength;

			samplerCUBE _EnvironmentMap;
			int _UseEnvironmentMap;

			float3 _Ambient, _DiffuseReflectance, _SpecularReflectance, _FresnelColor, _TipColor;
			float _Shininess, _FresnelBias, _FresnelStrength, _FresnelShininess, _TipAttenuation;
			float _Roughness, _FoamRoughnessModifier;
			float _Tile0, _Tile1, _Tile2, _Tile3;
			float3 _SunIrradiance, _ScatterColor, _BubbleColor, _FoamColor;
			float _HeightModifier, _BubbleDensity;
			float _DisplacementDepthAttenuation, _FoamDepthAttenuation, _NormalDepthAttenuation;
			float _WavePeakScatterStrength, _ScatterStrength, _ScatterShadowStrength, _EnvironmentLightStrength;

			int _DebugTile0, _DebugTile1, _DebugTile2, _DebugTile3;
			int _ContributeDisplacement0, _ContributeDisplacement1, _ContributeDisplacement2, _ContributeDisplacement3;
			int _DebugLayer0, _DebugLayer1, _DebugLayer2, _DebugLayer3;
			float _FoamSubtract0, _FoamSubtract1, _FoamSubtract2, _FoamSubtract3;

			float4x4 _CameraInvViewProjection;
			sampler2D _CameraDepthTexture;
            UNITY_DECLARE_TEX2DARRAY(_DisplacementTextures);
            UNITY_DECLARE_TEX2DARRAY(_SlopeTextures);
            SamplerState point_repeat_sampler, linear_repeat_sampler, trilinear_repeat_sampler;
			
            float _Weight;
            float _WaveSpeed,_Amplitude, _WaveLength;
            float _SpecularShininess, _SpecularSize,_Steepness,_Randomness,_Displace;

            float3 _LightDirection;
            int _TopFactor;
            float2 _Direction;
            int _WaveCount;
            float4 _TopColor,_BottomColor;
			
            float _Tile;

			TessellationControlPoint dummyvp(VertexData v) {
				TessellationControlPoint p;
				p.vertex = v.vertex;
				p.uv = v.uv;

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

			v2g vp(VertexData v) {
                v2g output;
                float3 gridPoint = v.vertex.xyz;
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
				float4 clipPos = UnityObjectToClipPos(v.vertex);
				float depth = 1 - Linear01Depth(clipPos.z / clipPos.w);
				totalDisplacement = lerp(0.0f, totalDisplacement, pow(saturate(depth), _DisplacementDepthAttenuation));
			    float3 normal = normalize(cross(binormal, tangent));
                v.vertex.xyz += mul(unity_WorldToObject, totalDisplacement.xyz);
            	
                //apply to transfer data
				output.pos = UnityObjectToClipPos(v.vertex);
                output.worldPos = mul(unity_ObjectToWorld,v.vertex);
                output.normal = UnityObjectToWorldNormal(normal);
				output.depth = depth;
                output.uv = output.worldPos.xz;
                return output;
			}

			struct g2f {
				v2g data;
				float2 barycentricCoordinates : TEXCOORD9;
			};

			TessellationFactors PatchFunction(InputPatch<TessellationControlPoint, 3> patch) {
                float3 p0 = mul(unity_ObjectToWorld, patch[0].vertex);
                float3 p1 = mul(unity_ObjectToWorld, patch[1].vertex);
                float3 p2 = mul(unity_ObjectToWorld, patch[2].vertex);

                TessellationFactors f;
                float bias = -0.5 * 100;
                if (cullTriangle(p0, p1, p2, bias)) {
                    f.edge[0] = f.edge[1] = f.edge[2] = f.inside = 0;
                } else {
                    f.edge[0] = TessellationHeuristic(p1, p2);
                    f.edge[1] = TessellationHeuristic(p2, p0);
                    f.edge[2] = TessellationHeuristic(p0, p1);
                    f.inside = (TessellationHeuristic(p1, p2) +
                                TessellationHeuristic(p2, p0) +
                                TessellationHeuristic(p1, p2)) * (1 / 3.0);
                }
                return f;
            }

            [UNITY_domain("tri")]
            [UNITY_outputcontrolpoints(3)]
            [UNITY_outputtopology("triangle_cw")]
            [UNITY_partitioning("integer")]
            [UNITY_patchconstantfunc("PatchFunction")]
            TessellationControlPoint hp(InputPatch<TessellationControlPoint, 3> patch, uint id : SV_OUTPUTCONTROLPOINTID) {
                return patch[id];
            }

            [maxvertexcount(3)]
            void gp(triangle v2g g[3], inout TriangleStream<g2f> stream) {
                g2f g0, g1, g2;
                g0.data = g[0];
                g1.data = g[1];
                g2.data = g[2];

                g0.barycentricCoordinates = float2(1, 0);
                g1.barycentricCoordinates = float2(0, 1);
                g2.barycentricCoordinates = float2(0, 0);

                stream.Append(g0);
                stream.Append(g1);
                stream.Append(g2);
            }

            #define DP_INTERPOLATE(fieldName) data.fieldName = \
				data.fieldName = patch[0].fieldName * barycentricCoordinates.x + \
				patch[1].fieldName * barycentricCoordinates.y + \
				patch[2].fieldName * barycentricCoordinates.z;               

            [UNITY_domain("tri")]
            v2g dp(TessellationFactors factors, OutputPatch<TessellationControlPoint, 3> patch, float3 barycentricCoordinates : SV_DOMAINLOCATION) {
                VertexData data;
                DP_INTERPOLATE(vertex)
                DP_INTERPOLATE(uv)

                return vp(data);
            }

			float SchlickFresnel(float3 normal, float3 viewDir) {
				// 0.02f comes from the reflectivity bias of water kinda idk it's from a paper somewhere i'm not gonna link it tho lmaooo
				return 0.02f + (1 - 0.02f) * (pow(1 - DotClamped(normal, viewDir), 5.0f));
			}

			float SmithMaskingBeckmann(float3 H, float3 S, float roughness) {
				float hdots = max(0.001f, DotClamped(H, S));
				float a = hdots / (roughness * sqrt(1 - hdots * hdots));
				float a2 = a * a;

				return a < 1.6f ? (1.0f - 1.259f * a + 0.396f * a2) / (3.535f * a + 2.181 * a2) : 0.0f;
			}

			float Beckmann(float ndoth, float roughness) {
				float exp_arg = (ndoth * ndoth - 1) / (roughness * roughness * ndoth * ndoth);

				return exp(exp_arg) / (PI * roughness * roughness * ndoth * ndoth * ndoth * ndoth);
			}

			float4 fp(g2f f) : SV_TARGET {
				float3 lightDir = -normalize(_SunDirection);
				float3 viewDir = normalize(_WorldSpaceCameraPos - f.data.worldPos);
                float3 halfwayDir = normalize(lightDir + viewDir);
            	float3 LightDirection = normalize(_LightDirection);
                float brightness = dot(f.data.normal,LightDirection);
            	float smoothness = exp2((10 * _SpecularSize)+1);
                float specular = pow(saturate(dot(halfwayDir,f.data.normal)),smoothness);
            	float subsurfaceFactor =pow(saturate(dot(f.data.normal,viewDir)),_TopFactor);
            	float4 color = lerp(_BottomColor,_TopColor,subsurfaceFactor);// * brightness;
                color += specular > _SpecularShininess;
                return color;
			}

			ENDCG
		}
	}
//Fallback "Unlit/Color"
}