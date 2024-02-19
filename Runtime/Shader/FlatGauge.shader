Shader "Flat/FlatCircleGuage"
{
	Properties
	{
		[KeywordEnum(CIRCLE, HORIZONTAL, VERTICAL)]
		_TYPE("Type", Float) = 0
		
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
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 uv : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 uv : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 mask : TEXCOORD2;
				float4 worldPosition : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _ClipRect;
			float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
			int _UIVertexColorAlwaysGammaSpace;
			
			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float4 vPosition = UnityObjectToClipPos(v.vertex);
				
				o.vertex = vPosition;
				o.worldPosition = v.vertex;
				o.uv = v.uv;
				o.uv1 = v.uv1;
				
				float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
				o.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

				if (_UIVertexColorAlwaysGammaSpace)
                {
                    if(!IsGammaSpace())
                    {
                        v.color.rgb = UIGammaToLinear(v.color.rgb);
                    }
                }

				o.color = v.color;
				
				return o;
			}
			
			half4 frag(v2f i) : SV_Target
			{
				const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                i.color.a = round(i.color.a * alphaPrecision)*invAlphaPrecision;
				
				half4 color = GaugeColor(i.color, i.uv, i.uv1);
				color.a *= i.color.a * UnityGet2DClipping(i.worldPosition.xy, _ClipRect);

				#ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
				
				return color;
			}
			ENDCG
		}
	}
}