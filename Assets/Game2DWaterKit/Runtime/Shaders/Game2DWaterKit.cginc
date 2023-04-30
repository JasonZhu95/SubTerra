#ifndef Game2D_WaterKit_INCLUDED
#define Game2D_WaterKit_INCLUDED

uniform float4 _G2DWK_Frame_Time;

#if defined(Game2DWaterKit_SRP_Lit) || defined(Game2DWaterKit_SRP_Unlit)
#define ComputeClipPosition(pos) TransformObjectToHClip(pos.xyz)
#define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
#else
#define ComputeClipPosition(pos) UnityObjectToClipPos(pos.xyz)
#endif

#define fmod2(v) fmod(v, 2.0)

// Texture Sheet

#define SampleTextureSheet(textureName, uv) SampleTextureSheetNoLerp(textureName, uv, textureName##SheetFramesCount, textureName##SheetFramesPerSecond, textureName##SheetInverseColumns, textureName##SheetInverseRows)
#define SampleTextureSheetLerp(textureName, uv) SampleTextureSheetWithLerp(textureName, uv, textureName##SheetFramesCount, textureName##SheetFramesPerSecond, textureName##SheetInverseColumns, textureName##SheetInverseRows)

inline half4 SampleTextureSheetNoLerp(sampler2D textureSheet, float2 uv, half framesCount, half framesPerSecond, half columnCountInverse, half rowCountInverse )
{
	uv = frac(uv);

	float frame = fmod(_G2DWK_Frame_Time.y * framesPerSecond, framesCount);

	half column = floor(frame);
	half row = floor(column * columnCountInverse);
	float2 frameCoord;
	frameCoord.x = (uv.x + column) * columnCountInverse;
	frameCoord.y = (uv.y - row) * rowCountInverse;
	
	return tex2D(textureSheet, frameCoord);
}

inline half4 SampleTextureSheetWithLerp(sampler2D textureSheet, float2 uv, half framesCount, half framesPerSecond, half columnCountInverse, half rowCountInverse)
{
	uv = frac(uv);

	float currentFrame = fmod(_G2DWK_Frame_Time.y * framesPerSecond, framesCount);

	half currentFrameColumn = floor(currentFrame);
	half currentFrameRow = floor(currentFrameColumn * columnCountInverse);
	float2 currentFrameCoord;
	currentFrameCoord.x = (uv.x + currentFrameColumn) * columnCountInverse;
	currentFrameCoord.y = (uv.y - currentFrameRow) * rowCountInverse;
	half4 currentFrameColor = tex2D(textureSheet, currentFrameCoord);

	half nextFrameColumn = currentFrameColumn + 1;
	half nextFrameRow = floor(nextFrameColumn * columnCountInverse);
	float2 nextFrameCoord;
	nextFrameCoord.x = (uv.x + nextFrameColumn) * columnCountInverse;
	nextFrameCoord.y = (uv.y - nextFrameRow) * rowCountInverse;
	half4 nextFrameColor = tex2D(textureSheet, nextFrameCoord);

	return lerp(currentFrameColor, nextFrameColor, frac(currentFrame));
}

inline half4 MixColors(half4 color1, half4 color2)
{
	half4 output;
	output.rgb = lerp(color1.rgb, color2.rgb, color2.a);
	#if (Is_Water2D_Refraction_Enabled || Is_Waterfall2D_Refraction_Enabled) && !defined(Game2DWaterKit_SRP_Lit)
	output.a = 1.0;
	#else
	output.a = color2.a + (1.0 - color2.a) * color1.a;
	#endif
	return output;
}

// Lighting

// URP 2D Renderer
#if defined(Game2DWaterKit_SRP_Lit)

half _HDREmulationScale;
half _UseSceneLighting;

half4 CombinedShapeLightShared(half4 color, half4 mask, half2 lightingUV)
{
    if (color.a == 0.0)
        discard;

#if USE_SHAPE_LIGHT_TYPE_0
    half4 shapeLight0 = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, lightingUV);

    if (any(_ShapeLightMaskFilter0))
    {
        half4 processedMask = (1 - _ShapeLightInvertedFilter0) * mask + _ShapeLightInvertedFilter0 * (1 - mask);
        shapeLight0 *= dot(processedMask, _ShapeLightMaskFilter0);
    }

    half4 shapeLight0Modulate = shapeLight0 * _ShapeLightBlendFactors0.x;
    half4 shapeLight0Additive = shapeLight0 * _ShapeLightBlendFactors0.y;
#else
    half4 shapeLight0Modulate = 0;
    half4 shapeLight0Additive = 0;
#endif

#if USE_SHAPE_LIGHT_TYPE_1
    half4 shapeLight1 = SAMPLE_TEXTURE2D(_ShapeLightTexture1, sampler_ShapeLightTexture1, lightingUV);

    if (any(_ShapeLightMaskFilter1))
    {
        half4 processedMask = (1 - _ShapeLightInvertedFilter1) * mask + _ShapeLightInvertedFilter1 * (1 - mask);
        shapeLight1 *= dot(processedMask, _ShapeLightMaskFilter1);
    }

    half4 shapeLight1Modulate = shapeLight1 * _ShapeLightBlendFactors1.x;
    half4 shapeLight1Additive = shapeLight1 * _ShapeLightBlendFactors1.y;
#else
    half4 shapeLight1Modulate = 0;
    half4 shapeLight1Additive = 0;
#endif

#if USE_SHAPE_LIGHT_TYPE_2
    half4 shapeLight2 = SAMPLE_TEXTURE2D(_ShapeLightTexture2, sampler_ShapeLightTexture2, lightingUV);

    if (any(_ShapeLightMaskFilter2))
    {
        half4 processedMask = (1 - _ShapeLightInvertedFilter2) * mask + _ShapeLightInvertedFilter2 * (1 - mask);
        shapeLight2 *= dot(processedMask, _ShapeLightMaskFilter2);
    }

    half4 shapeLight2Modulate = shapeLight2 * _ShapeLightBlendFactors2.x;
    half4 shapeLight2Additive = shapeLight2 * _ShapeLightBlendFactors2.y;
#else
    half4 shapeLight2Modulate = 0;
    half4 shapeLight2Additive = 0;
#endif

#if USE_SHAPE_LIGHT_TYPE_3
    half4 shapeLight3 = SAMPLE_TEXTURE2D(_ShapeLightTexture3, sampler_ShapeLightTexture3, lightingUV);

    if (any(_ShapeLightMaskFilter3))
    {
        half4 processedMask = (1 - _ShapeLightInvertedFilter3) * mask + _ShapeLightInvertedFilter3 * (1 - mask);
        shapeLight3 *= dot(processedMask, _ShapeLightMaskFilter3);
    }

    half4 shapeLight3Modulate = shapeLight3 * _ShapeLightBlendFactors3.x;
    half4 shapeLight3Additive = shapeLight3 * _ShapeLightBlendFactors3.y;
#else
    half4 shapeLight3Modulate = 0;
    half4 shapeLight3Additive = 0;
#endif

    half4 finalOutput;
#if !USE_SHAPE_LIGHT_TYPE_0 && !USE_SHAPE_LIGHT_TYPE_1 && !USE_SHAPE_LIGHT_TYPE_2 && ! USE_SHAPE_LIGHT_TYPE_3
    finalOutput = color;
#else
    half4 finalModulate = shapeLight0Modulate + shapeLight1Modulate + shapeLight2Modulate + shapeLight3Modulate;
    half4 finalAdditve = shapeLight0Additive + shapeLight1Additive + shapeLight2Additive + shapeLight3Additive;
    finalOutput = _HDREmulationScale * (color * finalModulate + finalAdditve);
#endif

    finalOutput.a = color.a;
    finalOutput = lerp(color, finalOutput, _UseSceneLighting);

    return max(0, finalOutput);
}
#endif // Game2DWaterKit_SRP_Lit

// Pixel-Lit
#if defined(Game2DWaterKit_PixelLit_Base)

#ifndef LIGHTMAP_ON
#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
inline half3 ComputeSH(float3 worldPos)
{
	half3 sh = 0;
	// Approximated illumination from non-important point lights
#ifdef VERTEXLIGHT_ON
  // to light vectors
	float4 toLightX = unity_4LightPosX0 - worldPos.x;
	float4 toLightY = unity_4LightPosY0 - worldPos.y;
	float4 toLightZ = unity_4LightPosZ0 - worldPos.z;

	// squared lengths
	float4 lengthSq = float4(0.0, 0.0, 0.0, 0.0);
	lengthSq += toLightX * toLightX;
	lengthSq += toLightY * toLightY;
	lengthSq += toLightZ * toLightZ;
	lengthSq = max(lengthSq, 0.000001); // don't produce NaNs if some vertex position overlaps with the light

	float4 ndotl = max(float4(0.0, 0.0, 0.0, 0.0), -toLightZ * rsqrt(lengthSq));

	// attenuation
	float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
	float4 diff = ndotl * atten;

	// final color
	sh += unity_LightColor[0].rgb * diff.x;
	sh += unity_LightColor[1].rgb * diff.y;
	sh += unity_LightColor[2].rgb * diff.z;
	sh += unity_LightColor[3].rgb * diff.w;

#endif
	sh = ShadeSHPerVertex(half3(0.0, 0.0, -1.0), sh);

	return sh;
}
#endif
#endif // !LIGHTMAP_ON

#ifdef LIGHTMAP_ON
inline half3 ComputeLightColor(float3 worldPos, float2 lightmapCoord)
#else
#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
inline half3 ComputeLightColor(float3 worldPos, half3 sh)
#else
inline half3 ComputeLightColor(float3 worldPos)
#endif
#endif
{
#ifndef USING_DIRECTIONAL_LIGHT
	half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
	half3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
	UNITY_LIGHT_ATTENUATION(atten, 0, worldPos)
	// Setup lighting environment
	UnityGIInput giInput;
	UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
	giInput.light.color = _LightColor0.rgb;
	giInput.light.dir = lightDir;
	giInput.worldPos = worldPos;
	giInput.atten = atten;

#ifdef LIGHTMAP_ON
	giInput.lightmapUV = float4(lightmapCoord, 0.0, 0.0);
	giInput.ambient.rgb = 0.0;
#else
	giInput.lightmapUV = 0.0;
	#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
	giInput.ambient.rgb = sh;
	#else
	giInput.ambient.rgb = 0.0;
	#endif
#endif

	giInput.probeHDR[0] = unity_SpecCube0_HDR;
	giInput.probeHDR[1] = unity_SpecCube1_HDR;
#if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
	giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
#endif
#ifdef UNITY_SPECCUBE_BOX_PROJECTION
	giInput.boxMax[0] = unity_SpecCube0_BoxMax;
	giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
	giInput.boxMax[1] = unity_SpecCube1_BoxMax;
	giInput.boxMin[1] = unity_SpecCube1_BoxMin;
	giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif

	UnityGI gi = UnityGI_Base(giInput, 1.0, half3(0.0, 0.0, -1.0));
	half diff = max(0.0, -lightDir.z);
	half4 c = 0.0;

	half3 lightingColor = gi.light.color.rgb * diff;

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
	lightingColor += gi.indirect.diffuse;
#endif

#if Is_Water2D_ApplyEmissionColor_Enabled
	lightingColor += _WaterEmissionColor * _WaterEmissionColorIntensity;
#elif Is_Waterfall2D_ApplyEmissionColor_Enabled
	lightingColor += _EmissionColor * _EmissionColorIntensity;
#endif

	return lightingColor;
}
#endif // Game2DWaterKit_PixelLit_Base

// Vertex-Lit
#if defined(Game2DWaterKit_VertexLit_Vertex) || defined(Game2DWaterKit_VertexLit_VertexLM)
int4 unity_VertexLightParams;

// ES2.0/WebGL/3DS can not do loops with non-constant-expression iteration counts.
#if defined(SHADER_API_GLES)
#define LIGHT_LOOP_LIMIT 8
#elif defined(SHADER_API_N3DS)
#define LIGHT_LOOP_LIMIT 4
#else
#define LIGHT_LOOP_LIMIT unity_VertexLightParams.x // x: vertex lights count
#endif

// Compute attenuation & illumination from one light
#if defined(POINT) || defined(SPOT)
inline half3 computeLight(int idx, float3 mvPos) {
#else
inline half3 computeLight(int idx) {
#endif
	float4 lightPos = unity_LightPosition[idx];
	float3 dirToLight = lightPos.xyz;
	half attenuation = 1.0;

#if defined(POINT) || defined(SPOT)
	dirToLight -= mvPos * lightPos.w;
	// distance attenuation

	half4 lightAtten = unity_LightAtten[idx];
	float distSqr = dot(dirToLight, dirToLight);
	if (lightPos.w != 0.0 && distSqr > lightAtten.w)
		attenuation = 0.0; // set to 0 if outside of range
	else
		attenuation /= (1.0 + lightAtten.z * distSqr);

	distSqr = max(distSqr, 0.000001); // don't produce NaNs if some vertex position overlaps with the light 
	dirToLight *= rsqrt(distSqr);
#if defined(SPOT)
	// spot angle attenuation
	half rho = max(dot(dirToLight, unity_SpotDirection[idx].xyz), 0.0);
	half spotAtt = (rho - lightAtten.x) * lightAtten.y;
	attenuation *= saturate(spotAtt);
#endif
#endif

	// Compute illumination from one light, given attenuation
	attenuation *= max(dirToLight.z, 0.0);
	return attenuation * unity_LightColor[idx].rgb;
}

inline half3 ComputeLightColor(float3 positionObjectSpace)
{
	half3 lcolor = UNITY_LIGHTMODEL_AMBIENT.rgb;
	#if defined(POINT) || defined(SPOT)
	float3 mvPos = UnityObjectToViewPos(positionObjectSpace);
	#endif
	for (int il = 0; il < LIGHT_LOOP_LIMIT; ++il) 
	{
		#if defined(POINT) || defined(SPOT)
		lcolor += computeLight(il, mvPos);
		#else
		lcolor += computeLight(il);
		#endif
	}
	return lcolor;
}
#endif // Game2DWaterKit_VertexLit_Vertex || Game2DWaterKit_VertexLit_VertexLM

#endif // Game2D_WaterKit_INCLUDED
