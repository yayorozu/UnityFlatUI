using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
	public class FlatCircleGauge : MaskableGraphic
	{
		
#if UNITY_EDITOR
		
		[NonSerialized]
		private int parentId;
		
		protected override void Reset()
		{
			base.Reset();
			if (m_Material == null)
				m_Material = Resources.Load<Material>("FlatUI/FlatCircleGauge");
			SetCanvasChannel();
		}

		protected override void OnValidate()
		{
			base.OnValidate();

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
					break;
				}
				parent = parent.parent;
			}
		}
		
#endif

		[SerializeField, Range(0.1f, 1f)]
		private float _width;
		[SerializeField, Range(0f, 360f)]
		private float _startAngle = 90;
		[SerializeField, Range(0f, 1f)]
		private float _fillAmount;
		
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);
			var vertexList = new List<UIVertex>();
			vh.GetUIVertexStream(vertexList);
			
			var radian = _startAngle * (Mathf.PI / 180);
			var angleVector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
			
			for (var i = 0; i < vertexList.Count; i++)
			{
				var vertex = vertexList[i];
				vertex.uv1 = new Vector2( _width, _fillAmount);
				vertex.uv2 = angleVector;
				vertexList[i] = vertex;
			}
	
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexList);
		}
	}
}