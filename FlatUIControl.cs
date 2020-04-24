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

			for (int i = 0; i < vertexList.Count; i++)
			{
				var vertex = vertexList[i];
				vertex.uv1 = new Vector2(_radius, 0);
				vertex.uv2 = new Vector2(rect.rect.width, rect.rect.height);
				vertexList[i] = vertex;
			}
	
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexList);
		}
	}
}