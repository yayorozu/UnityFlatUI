Shader "Hidden/FlatUI"
{
	Properties
	{
		[PerRendererData]
		_MainTex ("Sprite Texture", 2D) = "white" {}
		
		_Radius("Radius", Range(0,0.5)) = 0.1   // 角丸の円の半径. Width/height の小さい方に対する割合

		_Color("Tint", Color) = (1,1,1,1)

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
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Radius;
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = v.uv1;
				o.uv2 = v.uv2;
				o.color = v.color;
				return o;
			}

			// 負数:0, 正数:1 に変換
            // 条件を見たしているかどうかチェック
            #define IS(value) ceil(saturate(value))
            // a がb よりも小さいかどうか
			#define IS_SMALL(a, b) IS( b - a)

			fixed4 frag(v2f i) : SV_Target
			{
				half4 orig = i.color;                
                float r = min(i.uv2.x, i.uv2.y) * i.uv1.x;
                float2 XY = float2(i.uv.x * i.uv2.x, i.uv.y * i.uv2.y);

                // Calc Distance from each center of circle
                // LeftTop, Center:( r, r)               
                float d_lt = (XY.x - r) * (XY.x - r) + (XY.y-r) * (XY.y-r);
                
                // LeftBot, Center:( r, h-r)
                float d_lb = (XY.x - r) * (XY.x - r) + (XY.y- (i.uv2.y - r)) * (XY.y - (i.uv2.y - r));
                
                // RightTop, Center:( w-r, r)
                float d_rt = (XY.x - (i.uv2.x - r)) * (XY.x - (i.uv2.x - r)) + (XY.y - r) * (XY.y - r);
                
                // RightBot, Center:( w-r, h-r)
                float d_rb = (XY.x - (i.uv2.x - r)) * (XY.x - (i.uv2.x - r)) + (XY.y - (i.uv2.y - r)) * (XY.y - (i.uv2.y - r));

                //
                // The code which is implemented present the code which is comment outed.
                // 以下のコードは下記if 文を表現するためlerp などを用いて実装しています
                //
                // if( r < u < 1-r || r < v < 1-r)
                // {
                //     ret = original;
                // }
                // else
                // {
                //     if( u < 0.5 )
                //     {
                //         if( v > 0.5)
                //         {
                //             ret = D_lb > r ? alpha : orig;
                //         }
                //         else
                //         {
                //             ret = D_lt > r ? alpha : orig;
                //         }
                //     }
                //     else
                //     {
                //         if( v > 0.5)
                //         {
                //             ret = D_rb > r ? alpha : orig;
                //         }
                //         else
                //         {
                //             ret = D_rt > r ? alpha : orig;
                //         }
                //     }
                // }
                

                float isNotCorner = 
                    IS(IS_SMALL( r,XY.x ) + IS_SMALL(XY.x, (i.uv2.x - r)) - 1) // r < x < 1-r
                    + IS(IS_SMALL( r, XY.y) + IS_SMALL(XY.y, (i.uv2.y - r)) - 1); // r < y < 1-r
                
                float left = lerp(
                    lerp(1, 0, IS(d_lt -r * r)),
                    lerp(1, 0, IS(d_lb -r * r)),
                     IS( i.uv.y> 0.5)
                );
                float right = lerp(
                    lerp(1, 0, IS(d_rt -r * r)),
                    lerp(1, 0, IS(d_rb -r * r)),
                    IS(i.uv.y > 0.5)
                );
                float a = lerp( 
                    lerp(left, right, IS(i.uv.x > 0.5)),
                    1,
                    IS(isNotCorner) // r < x < 1-r && r < y < 1-r
                );
                orig.a = a;
                return orig;

			}
			ENDCG
		}
	}
}