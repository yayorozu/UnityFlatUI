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
		private int _parentId;
		[NonSerialized]
		private Canvas _cacheCanvas;
		
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
		}
		
#endif

		[SerializeField, Range(0.1f, 1f)]
		private float _width = 0.1f;
		[SerializeField, Range(0f, 359f)]
		private float _startAngle = 90;
		[SerializeField, Range(0f, 1f)]
		private float _fillAmount;
		[SerializeField]
		private bool _isReverse;
		[SerializeField, Range(0.1f, 1f)]
		private float _length = 1f;
		
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);
			var vertexList = new List<UIVertex>();
			vh.GetUIVertexStream(vertexList);
			
			var radian = _startAngle * (Mathf.PI / 180);
			var angleVector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

			var value1 = _width / 10f + Mathf.FloorToInt(_fillAmount * 100);
			var value2 = (_isReverse ? 0.1f : 0f) + Mathf.FloorToInt(_length * 100); 
			
			for (var i = 0; i < vertexList.Count; i++)
			{
				var vertex = vertexList[i];
				vertex.uv1 = new Vector2( value1, value2);
				vertex.uv2 = angleVector;
				vertexList[i] = vertex;
			}
	
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexList);
		}
	}
}