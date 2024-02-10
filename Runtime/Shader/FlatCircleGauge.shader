Shader "Flat/FlatCircleGuage"
{
	Properties
	{
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _OUTLINE_DISABLE _OUTLINE_ENABLE

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "FlatUI.hlsl"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float4 worldPosition : TEXCOORD4;
			};
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition = v.vertex;
				o.uv = v.uv;
				o.texcoord1 = v.texcoord1;
				o.color = v.color;
				return o;
			}
			
			float4 _ClipRect;

			half4 frag(v2f i) : SV_Target
			{
				half4 color = CircleGaugeFragmentAlpha(i.color, i.uv, i.texcoord1);
				color.a *= i.color.a * UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
				return color;
			}
			ENDCG
		}
	}
	
	CustomEditor "Yorozu.FlatUI.Tool.FlatShaderGUI"
}