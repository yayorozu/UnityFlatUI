using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class FlatCircle : MaskableGraphic
    {
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            if (m_Material == null)
                m_Material = Resources.Load<Material>("FlatUI/FlatCircle");
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            var graphic = GetComponent<Graphic>();
            graphic.SetVerticesDirty();
        }

#endif
        
        [SerializeField, Range(0, 0.25f)]
        private float _outlineWidth;
        
        [SerializeField, ColorUsage(false)]
        private Color _outlineColor;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            var uv1Param = new Vector4(_outlineWidth, _outlineColor.r, _outlineColor.g, _outlineColor.b);
                
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv1 = uv1Param;
                vertexList[i] = vertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
    }
}