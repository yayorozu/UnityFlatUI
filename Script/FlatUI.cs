using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
	public class FlatUI : MaskableGraphic
	{
#if UNITY_EDITOR
		
		[NonSerialized]
		private int parentId;
		
		protected override void Reset()
		{
			base.Reset();
			if (m_Material == null)
				m_Material = Resources.Load<Material>("FlatUI/FlatUI");
			SetCanvasChannel();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			if (_isValidOutline && m_Material.name != "FlatUIOutline")
				m_Material = Resources.Load<Material>("FlatUI/FlatUIOutline");
			else if (!_isValidOutline && m_Material.name != "FlatUI")
				m_Material = Resources.Load<Material>("FlatUI/FlatUI");

			var graphic = GetComponent<Graphic>();
			graphic.SetVerticesDirty();
			SetCanvasChannel();
		}
		
		/// <summary>
		/// Canvasのチャンネルを操作
		/// </summary>
		private void SetCanvasChannel()
		{
			// 親
			if (parentId != 0 && transform.parent.GetInstanceID() == parentId)
				return;

			parentId = transform.parent.GetInstanceID();

			var parent = transform.parent;
			while (parent != null)
			{
				var canvas = parent.GetComponent<Canvas>();
				if (canvas != null)
				{
					canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
					canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
					if (_isValidOutline)
						canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;

					break;
				}
				parent = parent.parent;
			}

		}
		
#endif
		
		[Flags]
		private enum CurveFlags
		{
			LeftTop = 1 << 1,
			RightTop = 1 << 2,
			LeftBottom = 1 << 3,
			RightBottom = 1 << 4,
		}

		[SerializeField]
		private CurveFlags _flags = CurveFlags.LeftTop | CurveFlags.RightTop | CurveFlags.LeftBottom | CurveFlags.RightBottom;
		
		[SerializeField, Range(0, 0.5f)]
		private float _radius;
		[SerializeField]
		private bool _isValidOutline;
		[SerializeField, Range(0, 0.05f)]
		private float _outline;
		[SerializeField]
		private Color _outlineColor;
		
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);
			var vertexList = new List<UIVertex>();
			vh.GetUIVertexStream(vertexList);
			
			var rect = transform as RectTransform;

			var flag = ((_flags & CurveFlags.LeftTop) == CurveFlags.LeftTop ? 1 : 0) + 
			           ((_flags & CurveFlags.LeftBottom) == CurveFlags.LeftBottom ? 10 : 0) + 
			           ((_flags & CurveFlags.RightTop) == CurveFlags.RightTop ? 100 : 0) +
			           ((_flags & CurveFlags.RightBottom) == CurveFlags.RightBottom ? 1000 : 0);
			
			var color = _outlineColor.r / 10 +
			            + Mathf.FloorToInt(_outlineColor.g * 100) +
			            + Mathf.FloorToInt(_outlineColor.b * 100) * 1000;

			for (var i = 0; i < vertexList.Count; i++)
			{
				var vertex = vertexList[i];
				vertex.uv1 = new Vector2( _radius, flag);
				vertex.uv2 = new Vector2(rect.rect.width, rect.rect.height);
				if (_isValidOutline)
					vertex.uv3 = new Vector2(_outline, color);
				
				vertexList[i] = vertex;
			}
	
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexList);
		}
	}
}