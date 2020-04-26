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
			
			static const float PI = 3.14159265f;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
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
				return o;
			}
			
			float4 _ClipRect;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 pos = (i.uv - float2(0.5, 0.5)) * 2.0;
				float angle = (atan2(pos.y, pos.x) - atan2(i.texcoord2.y, i.texcoord2.x)) / (PI * 2);
	
				if (angle < 0) {
					angle += 1.0;
				}
				
				float value1 = i.texcoord1.x;
				float value2 = i.texcoord1.y;
				float width = 1 - frac(value1) * 10;
				float amount = floor(value1) / 100;
				float reverse = frac(value2) * 10;
				float maxLength = floor(value2) / 100;
				if (reverse > 0)
				{
					angle = 1 - angle;
				}
				
				amount = lerp(0, amount, maxLength); 
				
				float len = length(pos);
				float edge = 0.03;
				float inner = smoothstep(width, width + edge, len);
				float outer = smoothstep(1.0 - edge, 1.0, len);
				float opaque = inner - outer;
				
				fixed4 c = i.color;
				fixed cutoff = angle > amount ? 0 : 1;
				c.a = i.color.a * opaque * cutoff * UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
				
				return c;
			}
			ENDCG
		}
	}
	
	CustomEditor "Yorozu.FlatUI.Tool.FlatShaderGUI"
}