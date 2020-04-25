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
		private CurveFlags _flags = CurveFlags.LeftTop | CurveFlags.RightTop | CurveFlags.LeftBottom | CurveFlags.RightBottom;
		
		[SerializeField, Range(0, 0.5f)]
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
			var vertexList = new List<UIVertex>();
			vh.GetUIVertexStream(vertexList);

			var rect = transform as RectTransform;

			var flag = ((_flags & CurveFlags.LeftTop) == CurveFlags.LeftTop ? 1 : 0) +
				((_flags & CurveFlags.LeftBottom) == CurveFlags.LeftBottom ? 10 : 0) +
				((_flags & CurveFlags.RightTop) == CurveFlags.RightTop ? 100 : 0) +
				((_flags & CurveFlags.RightBottom) == CurveFlags.RightBottom ? 1000 : 0);
			
			for (var i = 0; i < vertexList.Count; i++)
			{
				var vertex = vertexList[i];
				vertex.uv1 = new Vector2(_radius, flag);
				vertex.uv2 = new Vector2(rect.rect.width, rect.rect.height);
				vertexList[i] = vertex;
			}
	
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexList);
		}
	}
}