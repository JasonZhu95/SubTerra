Shader "Hidden/Game2DWaterKit-RefractionMask"
{
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		Pass
		{
			ZWrite On
			Offset 0 , -10
			ColorMask 0

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
