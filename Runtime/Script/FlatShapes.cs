using System.Collections.Generic;
using UnityEngine;

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
            RoundedPolygon,
            Star,
            Heart,
            Cross,
            Ring,
            Polar,
            /// <summary>
            /// スーパー楕円
            /// </summary>
            Superellipse,
            Arrow,
            CheckMark,
            /// <summary>
            /// 虫眼鏡
            /// </summary>
            MagnifyingGlass,
        }
        
        [SerializeField]
        private ShapeType _shapeType = ShapeType.Circle;
        
        [SerializeField]
        private Vector4 _floatValue;

        [SerializeField]
        private bool _innerClip;
        
        [SerializeField, Range(0, 0.25f)]
        private float _outlineWidth;
        
        [SerializeField, ColorUsage(false)]
        private Color _outlineColor;

        protected override void OnPopulateMesh(ref List<UIVertex> vertexList, bool shadow)
        {
            var uv1Param = _floatValue;
            // w は int で使う
            uv1Param.w /= 50f;

            float zValue = shadow ? (_innerClip ? _outlineWidth : 0) : _outlineWidth;
            if (_innerClip)
            {
                zValue += 1;
            }
            var colorFloat = ColorToFloat(_outlineColor);
            if (shadow && _innerClip)
                colorFloat = ColorToFloat(_shadowColor);
            
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv0.z = zValue;
                vertex.uv0.w = colorFloat;
                vertex.uv1 = uv1Param;
                vertexList[i] = vertex;
            }
        }
    }
}