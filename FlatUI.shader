Shader "Hidden/FlatUI"
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

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				fixed4 color : COLOR;
				fixed4 tangent : TANGENT0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				fixed4 tangent : TANGENT0;
				float4 worldPosition : TEXCOORD4;
			};
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition = v.vertex;
				o.uv = v.uv;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.color = v.color;
				o.tangent = v.tangent;
				return o;
			}
			
			float4 _ClipRect;

			// 負数:0, 正数:1 に変換
			// 条件を見たしているかどうかチェック
			#define IS(value) ceil(saturate(value))
			// a がb よりも小さいかどうか
			#define IS_SMALL(a, b) IS( b - a)

			fixed4 frag(v2f i) : SV_Target
			{
				fixed radius = i.texcoord1.x;
				fixed width = i.texcoord2.x;
				fixed height = i.texcoord2.y;
				
				half4 orig = i.color;
				half4 outline = half4(0, 0, 0, 1);
				float r = min(width, height) * radius;
				float2 XY = float2(i.uv.x * width, i.uv.y * height);
				
				// Calc Distance from each center of circle
				// LeftTop, Center:(r, r)
				float d_lt = (XY.x - r) * (XY.x - r) + (XY.y - r) * (XY.y - r);
				// LeftBot, Center:(r, h - r)
				float d_lb = (XY.x - r) * (XY.x - r) + (XY.y- (height - r)) * (XY.y - (height - r));
				// RightTop, Center:(w - r, r)
				float d_rt = (XY.x - (width - r)) * (XY.x - (width - r)) + (XY.y - r) * (XY.y - r);
				// RightBot, Center:(w - r, h - r)
				float d_rb = (XY.x - (width - r)) * (XY.x - (width - r)) + (XY.y - (height - r)) * (XY.y - (height - r));
				
				d_lt *= i.tangent.y;
				d_lb *= i.tangent.x;
				d_rt *= i.tangent.w;
				d_rb *= i.tangent.z;
				
				float isNotCorner = 
					IS(IS_SMALL(r, XY.x) + IS_SMALL(XY.x, (width - r)) - 1) // r < x < 1-r
					+ IS(IS_SMALL(r, XY.y) + IS_SMALL(XY.y, (height - r)) - 1); // r < y < 1-r
				
				float left = lerp(
					lerp(1, 0, IS(d_lt -r * r)),
					lerp(1, 0, IS(d_lb -r * r)),
					IS(i.uv.y > 0.5)
				);
				float right = lerp(
					lerp(1, 0, IS(d_rt -r * r)),
					lerp(1, 0, IS(d_rb -r * r)),
					IS(i.uv.y > 0.5)
				);
				orig.a = lerp( 
					lerp(left, right, IS(i.uv.x > 0.5)),
					1,
					IS(isNotCorner) // r < x < 1-r && r < y < 1-r
				);
				
				orig.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
				
				return orig;

			}
			ENDCG
		}
	}
}