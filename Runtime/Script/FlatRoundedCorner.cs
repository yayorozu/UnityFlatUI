using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
	public class FlatRoundedCorner : MaskableGraphic
	{
		public enum Type
		{
			None,
			OutLine,
			Separate,
		}
#if UNITY_EDITOR
		
		[NonSerialized]
		private int _parentId;
		[NonSerialized]
		private Canvas _cacheCanvas;

		protected override void Reset()
		{
			base.Reset();
			if (m_Material == null)
				m_Material = Resources.Load<Material>("FlatUI/FlatRoundedCorner");
			SetCanvasChannel();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			if (_type == Type.OutLine && m_Material.name != "FlatRoundedCornerOutline")
				m_Material = Resources.Load<Material>("FlatUI/FlatRoundedCornerOutline");
			else if (_type == Type.None && m_Material.name != "FlatRoundedCorner")
				m_Material = Resources.Load<Material>("FlatUI/FlatRoundedCorner");
			else if (_type == Type.Separate && m_Material.name != "FlatRoundedCornerSeparate")
				m_Material = Resources.Load<Material>("FlatUI/FlatRoundedCornerSeparate");

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
			if (_parentId == 0 || transform.parent.GetInstanceID() != _parentId)
			{
				if (transform.parent == null)
					return;
				_parentId = transform.parent.GetInstanceID();
				if (_cacheCanvas == null)
				{
					var parent = transform.parent;
					while (parent != null)
					{
						var canvas = parent.GetComponent<Canvas>();
						if (canvas != null)
						{
							_cacheCanvas = canvas;
							break;
						}
						parent = parent.parent;
					}
				}
			}
			
			if (_cacheCanvas == null)
				return;
			
			_cacheCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
			_cacheCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
			if (_type != Type.None)
				_cacheCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
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
		private Type _type;
		[SerializeField, Range(0, 0.05f)]
		private float _outline;
		[SerializeField, Range(0, 1f)]
        private float _separate = 0.8f;
		[SerializeField]
		private Color _color;
		
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
			
			var color = _color.r / 10 +
			            + Mathf.FloorToInt(_color.g * 100) +
			            + Mathf.FloorToInt(_color.b * 100) * 1000;

			for (var i = 0; i < vertexList.Count; i++)
			{
				var vertex = vertexList[i];
				vertex.uv1 = new Vector2( _radius, flag);
				vertex.uv2 = new Vector2(rect.rect.width, rect.rect.height);
				if (_type == Type.OutLine)
					vertex.uv3 = new Vector2(_outline, color);
				if (_type == Type.Separate)
					vertex.uv3 = new Vector2(_separate, color);
				
				vertexList[i] = vertex;
			}
	
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexList);
		}
	}
}