Shader "Hidden/Game2DWaterKit-MeshMask"
{
	Properties
	{
		_StencilReference ("Stencil Reference", int) = 1
		_StencilOperation ("Stencil Operation", int) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			ColorMask 0
			
			Stencil
			{
				Ref [_StencilReference]
				Pass [_StencilOperation]
			}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			float4 vert(float3 pos : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(pos);
			}

			half4 frag() : SV_Target
			{
				return 0.0;
			}

			ENDCG
		}
	}
}
