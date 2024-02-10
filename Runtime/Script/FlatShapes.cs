using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    public class FlatShapes : FlatUI
    {
        protected override string GetMaterialName()
        {
            return $"FlatShape{_shapeType}";
        }

        public enum ShapeType
        {
            Circle = 1,
            Polygon,
            RoundStar,
            Star,
            Heart,
            Cross,
            Ring,
        }
        
        [SerializeField]
        private ShapeType _shapeType;
        
        [SerializeField]
        private int _polygon = 6;

        [SerializeField]
        private float _floatValue;
        
        [SerializeField, Range(0, 0.25f)]
        private float _outlineWidth;
        
        [SerializeField, ColorUsage(false)]
        private Color _outlineColor;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            var intValue = _polygon / 10f;
            var uv1Param = new Vector4(_outlineWidth, _outlineColor.r, _outlineColor.g, _outlineColor.b);
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv0.z = intValue;
                vertex.uv0.w = _floatValue;
                vertex.uv1 = uv1Param;
                vertexList[i] = vertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
    }
}