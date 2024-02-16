Shader "Flat/FlatShape"
{
	Properties
	{
		[KeywordEnum(CIRCLE, POLYGON, ROUNDED_POLYGON, STAR, HEART, CROSS, RING, POLAR, SUPERELLIPSE, ARROW)]
		_SHAPE("Shape", Float) = 0
		
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

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "FlatUI.hlsl"
			
			struct appdata
			{
                float4 vertex   : POSITION;
                half4 color    : COLOR;
                float4 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
			};

			struct v2f
			{
                float4 vertex   : SV_POSITION;
                half4 color    : COLOR;
                float4 uv  : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 worldPosition : TEXCOORD2;
			};
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv1;
				o.color = v.color;
				o.worldPosition = v.vertex;
				return o;
			}
			
			float4 _ClipRect;

			half4 frag(v2f i) : SV_Target
			{
				half4 color = Shapes(i.color, i.uv, i.uv1);
				color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
				return color;
			}
			ENDCG
		}
	}
}