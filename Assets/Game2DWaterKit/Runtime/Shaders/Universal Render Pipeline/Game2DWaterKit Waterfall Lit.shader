Shader "Game2DWaterKit/Universal Render Pipeline/Lit/Waterfall"
{
	Properties
	{
		// Body Properties
		_BodyColor("Body Color",color) = (0.11,0.64,0.92,0.25)
		_BodyColorGradientStart("Body Color Gradient Start",color) = (1.0,1.0,1.0,0.25)
		_BodyColorGradientEnd("Body Color Gradient End",color) = (1.0,1.0,1.0,0.25)
		_BodyColorGradientOffset("Body Color Gradient Offset",Range(-1.0,1.0)) = 0.0

		_BodyTexture("Body Texture" , 2D) = "white" {}
		_BodyTextureTilingParameters("Body Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_BodyTextureScrollingSpeed("Body Texture Scrolling Speed", float) = 1.0
		_BodyTextureOpacity("Body Texture Opacity",range(0,1)) = 0.5
		_BodyTextureSheetFramesPerSecond("Body Texture Sheet Frames Per Second",float) = 0.0
		_BodyTextureSheetColumns("Body Texture Sheet Columns",float) = 1.0
		_BodyTextureSheetRows("Body Texture Sheet Rows",float) = 1.0
		_BodyTextureSheetFramesCount("Body Texture Sheet Frames Count",float) = 1.0
		_BodyTextureSheetInverseColumns("Body Texture Sheet Inverse of Columns",float) = 1.0
		_BodyTextureSheetInverseRows("Body Texture Sheet Inverse of Rows",float) = 1.0

		_BodySecondTexture("Body Second Texture" , 2D) = "white" {}
		_BodySecondTextureTilingParameters("Body Second Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_BodySecondTextureScrollingSpeed("Body Second Texture Scrolling Speed", float) = 1.0
		_BodySecondTextureOpacity("Body Second Texture Opacity",range(0,1)) = 0.5
		_BodySecondTextureSheetFramesPerSecond("Body Second Texture Sheet Frames Per Second",float) = 0.0
		_BodySecondTextureSheetColumns("Body Second Texture Sheet Columns",float) = 1.0
		_BodySecondTextureSheetRows("Body Second Texture Sheet Rows",float) = 1.0
		_BodySecondTextureSheetFramesCount("Body Second Texture Sheet Frames Count",float) = 1.0
		_BodySecondTextureSheetInverseColumns("Body Second Texture Sheet Inverse of Columns",float) = 1.0
		_BodySecondTextureSheetInverseRows("Body Second Texture Sheet Inverse of Rows",float) = 1.0

		_BodyTextureAlphaCutoff("Body Texture Alpha Cutoff", Range(0.0,1.0)) = 0.1

		_BodyNoiseSpeed("Body Texture Distortion Effect - Speed",float) = 0.5
		_BodyNoiseScaleOffset("Body Texture Distortion Effect - Scale Offset",vector) = (1,1,0,0)
		_BodyNoiseStrength("Body Texture Distortion Effect - Strength",Range(0.001,1.0)) = 0.025
		_BodyNoiseTiling("Body Texture Distortion Effect - Tiling", vector) = (1,1,0,0)

		// Top Edge Properties
		_TopEdgeTexture ("Top Edge Texture", 2D) = "white" {}
		_TopEdgeTextureTilingParameters("Top Edge Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_TopEdgeTextureOpacity ("Top Edge Texture Opacity", Range(0.0,1.0)) = 0.75
		_TopEdgeTextureSheetRows("Top Edge Texture Sheet Rows", float) = 1
		_TopEdgeTextureSheetInverseRows ("Top Edge Texture Sheet Inverse Rows", float) = 1
		_TopEdgeTextureSheetColumns("Top Edge Texture Sheet Columns", float) = 1
		_TopEdgeTextureSheetInverseColumns("Top Edge Texture Sheet Inverse Columns", float) = 1
		_TopEdgeTextureSheetFramesCount ("Top Edge Texture Sheet Frames Count", float) = 1
		_TopEdgeTextureSheetFramesPerSecond ("Top Edge Texture Sheet Frames Per Second", float) = 1

		// Bottom Edge Properties
		_BottomEdgeTexture ("Bottom Edge Texture", 2D) = "white" {}
		_BottomEdgeTextureTilingParameters("Bottom Edge Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_BottomEdgeTextureOpacity ("Bottom Edge Texture Opacity", Range(0.0,1.0)) = 0.75
		_BottomEdgeTextureSheetRows("Bottom Edge Texture Sheet Rows", float) = 1
		_BottomEdgeTextureSheetInverseRows ("Bottom Edge Texture Sheet Inverse Rows", float) = 1
		_BottomEdgeTextureSheetColumns("Bottom Edge Texture Sheet Columns", float) = 1
		_BottomEdgeTextureSheetInverseColumns ("Bottom Edge Texture Sheet Inverse Columns", float) = 1
		_BottomEdgeTextureSheetFramesCount("Bottom Edge Texture Sheet Frames Count", float) = 1
		_BottomEdgeTextureSheetFramesPerSecond ("Bottom Edge Texture Sheet Frames Per Second", float) = 1
		
		// Shared Top-Botom Edges Properties
		_TopBottomEdgesThickness ("Top-Bottom Edges Thickness", vector) = (0.2,5,0.2,5)
		_TopBottomEdgesOffset ("Top-Bottom Edges Offset", vector) = (0,0,0,0)
		_TopBottomEdgesTexture ("Top-Bottom Edges Texture", 2D) = "white" {}
		_TopBottomEdgesTextureTilingParameters("Top-Bottom Edges Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_TopBottomEdgesTextureFlipParameters("Top-Bottom Edges Texture Flip" , vector) = (0,0,0,0)
		_TopBottomEdgesTextureOpacity ("Top-Bottom Edges Texture Opacity", Range(0.0,1.0)) = 0.75
		_TopBottomEdgesTextureSheetRows("Top-Bottom Edges Texture Sheet Rows", float) = 1
		_TopBottomEdgesTextureSheetInverseRows ("Top-Bottom Edges Texture Sheet Rows", float) = 1
		_TopBottomEdgesTextureSheetColumns("Top-Bottom Edges Texture Sheet Columns", float) = 1
		_TopBottomEdgesTextureSheetInverseColumns ("Top-Bottom Edges Texture Sheet Columns", float) = 1
		_TopBottomEdgesTextureSheetFramesCount("Top-Bottom Edge Texture Sheet Frames Count", float) = 1
		_TopBottomEdgesTextureSheetFramesPerSecond ("Top-Bottom Edges Texture Sheet Frames Per Second", float) = 1

		_TopBottomEdgesNoiseStrength ("Top-Bottom Edges Texture Distortion Effect - Strength", Range(0.001,1.0)) = 0.025
		_TopBottomEdgesNoiseSpeed ("Top-Bottom Edges Texture Distortion Effect - Speed", float) = 0.5
		_TopBottomEdgesNoiseTiling ("Top-Bottom Edges Texture Distortion Effect - Tiling", vector) = (1,1,0,0)
		_TopBottomEdgesNoiseScaleOffset ("Top-Bottom Edges Texture Distortion Effect - Scale Offset", vector) = (1.0,1.0,0.0,0.0)

		// Left Edge Properties
		_LeftEdgeTexture ("Left Edge Texture", 2D) = "white" {}
		_LeftEdgeTextureTilingParameters("Left Edge Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_LeftEdgeTextureScrollingSpeed("Left Edge Texture Scrolling Speed", float) = 1.0
		_LeftEdgeTextureOpacity ("Left Edge Texture Opacity", Range(0.0,1.0)) = 0.75
		_LeftEdgeTextureSheetRows("Left Edge Texture Sheet Rows", float) = 1
		_LeftEdgeTextureSheetInverseRows ("Left Edge Texture Sheet Inverse Rows", float) = 1
		_LeftEdgeTextureSheetColumns("Left Edge Texture Sheet Columns", float) = 1
		_LeftEdgeTextureSheetInverseColumns("Left Edge Texture Sheet Inverse Columns", float) = 1
		_LeftEdgeTextureSheetFramesCount ("Left Edge Texture Sheet Frames Count", float) = 1
		_LeftEdgeTextureSheetFramesPerSecond ("Left Edge Texture Sheet Frames Per Second", float) = 1

		// Right Edge Properties
		_RightEdgeTexture ("Right Edge Texture", 2D) = "white" {}
		_RightEdgeTextureTilingParameters("Right Edge Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_RightEdgeTextureScrollingSpeed("Right Edge Texture Scrolling Speed", float) = 1.0
		_RightEdgeTextureOpacity ("Right Edge Texture Opacity", Range(0.0,1.0)) = 0.75
		_RightEdgeTextureSheetRows("Right Edge Texture Sheet Rows", float) = 1
		_RightEdgeTextureSheetInverseRows ("Right Edge Texture Sheet Inverse Rows", float) = 1
		_RightEdgeTextureSheetColumns("Right Edge Texture Sheet Columns", float) = 1
		_RightEdgeTextureSheetInverseColumns ("Right Edge Texture Sheet Inverse Columns", float) = 1
		_RightEdgeTextureSheetFramesCount("Right Edge Texture Sheet Frames Count", float) = 1
		_RightEdgeTextureSheetFramesPerSecond ("Right Edge Texture Sheet Frames Per Second", float) = 1

		// Shared Left-Right Edges Properties
		_LeftRightEdgesThickness ("Left-Right Edges Thickness", vector) = (0.2,5,0.2,5)
		_LeftRightEdgesOffset ("Left-Right Edges Offset", vector) = (0,0,0,0)
		_LeftRightEdgesTexture ("Left-Right Edges Texture", 2D) = "white" {}
		_LeftRightEdgesTextureTilingParameters("Left-Right Edges Texture Tiling Mode Properties" , vector) = (0,0,0,0)
		_LeftRightEdgesTextureScrollingSpeed("Left-Right Edges Texture Scrolling Speed", float) = 1.0
		_LeftRightEdgesTextureFlipParameters("Left-Right Edges Texture Flip" , vector) = (0,0,0,0)
		_LeftRightEdgesTextureOpacity ("Left-Right Edges Texture Opacity", Range(0.0,1.0)) = 0.75
		_LeftRightEdgesTextureSheetRows("Left-Right Edges Texture Sheet Rows",float) = 1.0
		_LeftRightEdgesTextureSheetInverseRows("Left-Right Edges Texture Sheet Inverse of Rows",float) = 1.0
		_LeftRightEdgesTextureSheetColumns("Left-Right Edges Texture Sheet Columns",float) = 1.0
		_LeftRightEdgesTextureSheetInverseColumns("Left-Right Edges Texture Sheet Inverse of Columns",float) = 1.0
		_LeftRightEdgesTextureSheetFramesCount("Left-Right Edges Texture Sheet Frames Count",float) = 1.0
		_LeftRightEdgesTextureSheetFramesPerSecond("Left-Right Edges Texture Sheet Frames Per Second",float) = 0.0
		_LeftRightEdgesTextureAlphaCutoff("Left-Right Edges Texture Alpha Cutoff", Range(0.0,1.0)) = 0.1

		_LeftRightEdgesNoiseStrength("Left-Right Edges Texture Distortion Effect -  Strength", Range(0.001,1.0)) = 0.025
		_LeftRightEdgesNoiseSpeed("Left-Right Edges Texture Distortion Effect -  Speed", float) = 0.5
		_LeftRightEdgesNoiseTiling("Left-Right Edges Texture Distortion Effect - Tiling", vector) = (1,1,0,0)
		_LeftRightEdgesNoiseScaleOffset("Left-Right Edges Texture Distortion Effect -  Scale Offset", vector) = (1.0,1.0,0.0,0.0)

		//Refraction Properties
		_RefractionTexture ("Refraction Texture", 2D) = "white" {}
		_RefractionNoiseStrength ("Refraction Distortion Effect -  Strength", Range(0.001,0.1)) = 0.015
		_RefractionNoiseSpeed ("Refraction Distortion Effect -  Speed", float) = 0.5
		_RefractionNoiseTiling ("Refraction Distortion Effect -  Tiling", vector) = (1, 1, 0, 0)
		_RefractionNoiseScaleOffset ("Refraction Distortion Effect -  Offset", vector) = (1.0,1.0,0.0,0.0)
		
		//Emission Properties
		_EmissionColor("Emission Color",color) = (1.0,1.0,1.0,0.0)
		_EmissionColorIntensity("Emission Color Intensity",float) = 1.0

		//Noise Texture (RGBA): Body(A) , Top-Bottom Edges(B) , Left-Right Edges(G) and Refraction(R)
		_NoiseTexture("Noise Texture", 2D) = "black" {}

		// Keywords States
		// Body
		_Waterfall2D_IsColorGradientEnabled("Body Color - Gradient Color Mode", float) = 0
		_Waterfall2D_IsBodyTextureSheetEnabled("Body Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsBodyTextureSheetWithLerpEnabled("Body Texture Sheet - Lerp Toggle", float) = 0
		_Waterfall2D_IsBodySecondTextureSheetEnabled("Body Second Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsBodySecondTextureSheetWithLerpEnabled("Body Second Texture Sheet - Lerp Toggle", float) = 0
		_Waterfall2D_IsBodyNoiseEnabled("Body Texture - Distortion Effect Toggle", float) = 0
		_Waterfall2D_IsBodyTextureAlphaCutoffEnabled("Body Texture Alpha Cutoff Toggle", float) = 0

		// Left-Right Edges
		_Waterfall2D_IsLeftEdgeEnabled("Left Edge Toggle", float) = 0
		_Waterfall2D_IsLeftEdgeTextureSheetEnabled("Left Edge Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsLeftEdgeTextureSheetWithLerpEnabled("Left Edge Texture Sheet - Lerp Toggle", float) = 0

		_Waterfall2D_IsRightEdgeEnabled("Right Edge Toggle", float) = 0
		_Waterfall2D_IsRightEdgeTextureSheetEnabled("Right Edge Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsRightEdgeTextureSheetWithLerpEnabled("Right Edge Texture Sheet - Lerp Toggle", float) = 0

		_Waterfall2D_IsLeftRightEdgesEnabled("Left-Right Edges Toggle", float) = 0
		_Waterfall2D_IsLeftRightEdgesNoiseEnabled("Left-Right Edges Texture - Distortion Effect Toggle", float) = 0
		_Waterfall2D_IsLeftRightEdgesUseSameTextureEnabled("Left-Right Edges Use Same Texture Toggle", float) = 0
		_Waterfall2D_IsLeftRightEdgesTextureSheetEnabled("Left-Right Edges Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsLeftRightEdgesTextureSheetWithLerpEnabled("Left-Right Edges Texture Sheet - Lerp Toggle", float) = 0
		_Waterfall2D_IsLeftRightEdgesTextureAlphaCutoffEnabled("Left-Right Edges Texture Alpha Cutoff Toggle", float) = 0
		_Waterfall2D_IsLeftRightEdgesAbsoluteThicknessAndOffsetEnabled("Left-Right Edges Absolute Thickness And Offset", float) = 0

		// Top-Bottom Edges
		_Waterfall2D_IsTopEdgeEnabled ("Top Edge Toggle", float) = 0
		_Waterfall2D_IsTopEdgeTextureSheetEnabled ("Top Edge Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsTopEdgeTextureSheetWithLerpEnabled ("Top Edge Texture Sheet - Lerp Toggle", float) = 0

		_Waterfall2D_IsBottomEdgeEnabled ("Bottom Edge Toggle", float) = 0
		_Waterfall2D_IsBottomEdgeTextureSheetEnabled ("Bottom Edge Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsBottomEdgeTextureSheetWithLerpEnabled ("Bottom Edge Texture Sheet - Lerp Toggle", float) = 0

		_Waterfall2D_IsTopBottomEdgesEnabled("Top-Bottom Edges Toggle", float) = 0
		_Waterfall2D_IsTopBottomEdgesNoiseEnabled ("Top-Botom Edges - Distortion Effect Toggle", float) = 0
		_Waterfall2D_IsTopBottomEdgesUseSameTextureEnabled("Top-Bottom Edges Use Same Texture Toggle", float) = 0
		_Waterfall2D_IsTopBottomEdgesTextureSheetEnabled("Top-Bottom Edges Texture - Is A Texture Sheet Toggle", float) = 0
		_Waterfall2D_IsTopBottomEdgesTextureSheetWithLerpEnabled("Top-Bottom Edges Texture Sheet - Lerp Toggle", float) = 0
		_Waterfall2D_IsTopBottomEdgesAbsoluteThicknessAndOffsetEnabled("Top-Bottom Edges Absolute Thickness And Offset", float) = 0

		// Refraction
		_Waterfall2D_IsRefractionEnabled ("Refraction Effect Toggle", float) = 0

		// Emission
		_Waterfall2D_IsEmissionColorEnabled("Emission Toggle", float) = 0
		
		// Sprite Mask (Stencil) Options
		[Enum(None,8,Visible Inside Mask,4,Visible Outside Mask,5)] _SpriteMaskInteraction("Sprite Mask Interaction", float) = 8
		_SpriteMaskInteractionRef("Sprite Mask Interaction Ref", float) = 1

		_Waterfall2D_IsApplyTintColorOnTopOfTextureEnabled("Apply Tint Color On Top Of Texture Toggle", float) = 0.0

		[HideInInspector] _SrcBlend("__src", float) = 5
		[HideInInspector] _DstBlend("__dst", float) = 10
		[HideInInspector] _ZWrite("__zw", float) = 0
			
		[HideInInspector] _Game2DWaterKit_MaterialType("__type", float) = 1.0 // 0 -> water material / 1 -> waterfall material
	}

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		
	#pragma shader_feature_local _ Waterfall2D_ApplyTintColorBeforeTexture
	// Body shader features
	#pragma shader_feature_local _ Waterfall2D_BodyColorGradient
	#pragma shader_feature_local _ Waterfall2D_BodyTexture Waterfall2D_BodyTextureSheet Waterfall2D_BodyTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_BodyTextureStretch Waterfall2D_BodyTextureStretchAutoX Waterfall2D_BodyTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_BodySecondTexture Waterfall2D_BodySecondTextureSheet Waterfall2D_BodySecondTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_BodySecondTextureStretch Waterfall2D_BodySecondTextureStretchAutoX Waterfall2D_BodySecondTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_BodyTextureAlphaCutoff
	#pragma shader_feature_local _ Waterfall2D_BodyTextureNoise
	// Top-Bottom Edges shader features
	#pragma shader_feature_local _ Waterfall2D_TopEdge Waterfall2D_TopEdgeTextureSheet Waterfall2D_TopEdgeTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_TopEdgeTextureStretch Waterfall2D_TopEdgeTextureStretchAutoX Waterfall2D_TopEdgeTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_BottomEdge Waterfall2D_BottomEdgeTextureSheet Waterfall2D_BottomEdgeTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_BottomEdgeTextureStretch Waterfall2D_BottomEdgeTextureStretchAutoX Waterfall2D_BottomEdgeTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_TopBottomEdgesSameTexture Waterfall2D_TopBottomEdgesSameTextureSheet Waterfall2D_TopBottomEdgesSameTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_TopBottomEdgesFlipTopEdgeY Waterfall2D_TopBottomEdgesFlipBottomEdgeY
	#pragma shader_feature_local _ Waterfall2D_TopBottomEdgesTextureStretch Waterfall2D_TopBottomEdgesTextureStretchAutoX Waterfall2D_TopBottomEdgesTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_TopBottomEdgesNoise
	#pragma shader_feature_local _ Waterfall2D_TopBottomEdgesAbsoluteThicknessAndOffset
	// Left-Right Edges shader features
	#pragma shader_feature_local _ Waterfall2D_LeftEdge Waterfall2D_LeftEdgeTextureSheet Waterfall2D_LeftEdgeTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_LeftEdgeTextureStretch Waterfall2D_LeftEdgeTextureStretchAutoX Waterfall2D_LeftEdgeTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_RightEdge Waterfall2D_RightEdgeTextureSheet Waterfall2D_RightEdgeTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_RightEdgeTextureStretch Waterfall2D_RightEdgeTextureStretchAutoX Waterfall2D_RightEdgeTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_LeftRightEdgesSameTexture Waterfall2D_LeftRightEdgesSameTextureSheet Waterfall2D_LeftRightEdgesSameTextureSheetWithLerp
	#pragma shader_feature_local _ Waterfall2D_LeftRightEdgesFlipLeftEdgeX Waterfall2D_LeftRightEdgesFlipRightEdgeX
	#pragma shader_feature_local _ Waterfall2D_LeftRightEdgesTextureStretch Waterfall2D_LeftRightEdgesTextureStretchAutoX Waterfall2D_LeftRightEdgesTextureStretchAutoY
	#pragma shader_feature_local _ Waterfall2D_LeftRightEdgesTextureAlphaCutoff
	#pragma shader_feature_local _ Waterfall2D_LeftRightEdgesNoise
	#pragma shader_feature_local _ Waterfall2D_LeftRightEdgesAbsoluteThicknessAndOffset
	// Refraction
	#pragma multi_compile_local _ Waterfall2D_Refraction
	// Emission
	#pragma shader_feature_local _ Waterfall2D_ApplyEmissionColor
			
    ENDHLSL

	SubShader
	{
		Tags {
		"RenderType"="Transparent"
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"PreviewType"="Plane"
		"RenderPipeline" = "UniversalPipeline"
		}

		Stencil
		{
			Ref[_SpriteMaskInteractionRef]
			Comp[_SpriteMaskInteraction]
		}

		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		Cull off

		Pass
		{
			Tags { "LightMode" = "Universal2D" }
			HLSLPROGRAM

			#pragma prefer_hlslcc gles
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
		
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

			#if USE_SHAPE_LIGHT_TYPE_0
			SHAPE_LIGHT(0)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_1
			SHAPE_LIGHT(1)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_2
			SHAPE_LIGHT(2)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_3
			SHAPE_LIGHT(3)
			#endif
			
			#define Game2DWaterKit_SRP_Lit
			#include "../Game2DWaterKitWaterfall.cginc"

			Varyings vert (Attributes v)
			{
				Varyings o = Waterfall2D_Vert(v);

				float4 clipVertex = o.pos / o.pos.w;
                o.lightingUV = ComputeScreenPos(clipVertex).xy;

				return o;
			}

			half4 frag (Varyings i) : SV_Target
			{
				half4 c = Waterfall2D_Frag(i);
			
				#if Is_Waterfall2D_ApplyEmissionColor_Enabled
					c.rgb += c.rgb * _EmissionColor * _EmissionColorIntensity;
				#endif

				return CombinedShapeLightShared(c, 1.0, i.lightingUV);
			}

			ENDHLSL
		}

		Pass
		{
			Tags { "LightMode" = "UniversalForward" "Queue" = "Transparent" "RenderType" = "Transparent"}

			HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma vertex vert
			#pragma fragment frag
				
			#define Game2DWaterKit_SRP_Unlit

			#include "../Game2DWaterKitWaterfall.cginc"
			
			Varyings vert (Attributes v)
			{
				return Waterfall2D_Vert(v);
			}
			
			half4 frag (Varyings i) : SV_Target
			{
				return Waterfall2D_Frag(i);
			}

			ENDHLSL
		}
	}

	Fallback "Game2DWaterKit/Universal Render Pipeline/Unlit/Waterfall"
	CustomEditor "Game2DWaterKit.Game2DWaterfallShaderGUI"
}
