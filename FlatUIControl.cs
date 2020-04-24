using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
	/// <summary>
	/// FlagUIのUV値をセットするために利用
	/// </summary>
	[RequireComponent(typeof(FlatUI))]
	public class FlatUIControl : BaseMeshEffect
	{
		[Flags]
		private enum CurveFlags
		{
			LeftTop = 1 << 1,
			RightTop = 1 << 2,
			LeftBottom = 1 << 3,
			RightBottom = 1 << 4,
		}

		[SerializeField]
		private CurveFlags _flags;
		
		[SerializeField]
		private float _radius;
		
		[SerializeField, HideInInspector]
        private FlatUI _control;
        		
        #if UNITY_EDITOR
		
		protected override void Reset()
		{
			_control = GetComponent<FlatUI>();
		}
		
		protected override void OnValidate()
		{
			base.OnValidate();
		}
		
		#endif
		
		public override void ModifyMesh(VertexHelper vh)
		{
			var _vertexList = new List<UIVertex>();
			vh.GetUIVertexStream(_vertexList);

			var rect = transform as RectTransform;

			for (int i = 0; i < _vertexList.Count; i++)
			{
				var vertex = _vertexList[i];
				vertex.uv1 = new Vector2(_radius, 0);
				vertex.uv2 = new Vector2(rect.rect.width, rect.rect.height);
				_vertexList[i] = vertex;
			}
	
			vh.Clear();
			vh.AddUIVertexTriangleStream(_vertexList);
		}
	}
}