using System;
using System.Collections;
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
        }
        
        [SerializeField]
        private ShapeType _shapeType = ShapeType.Circle;
        
        [SerializeField]
        private Vector4 _floatValue;
        
        [SerializeField, Range(0, 0.25f)]
        private float _outlineWidth;
        
        [SerializeField, ColorUsage(false)]
        private Color _outlineColor;

        protected override void OnPopulateMesh(ref List<UIVertex> vertexList)
        {
            var uv1Param = _floatValue;
            // w は int で使う
            uv1Param.w /= 50f;

            var colorFloat = ColorToFloat(_outlineColor);
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv0.z = _outlineWidth;
                vertex.uv0.w = colorFloat;
                vertex.uv1 = uv1Param;
                vertexList[i] = vertex;
            }
        }
        
#if UNITY_EDITOR

        internal class InspectorParam
        {
            public string Name;
            public float Min;
            public float Max;
            public bool Int;

            public InspectorParam(string name, float min, float max, bool i = false)
            {
                Name = name;
                Min = min;
                Max = max;
                Int = i;
            }

            public float Clamp(float value)
            {
                return Mathf.Clamp(value, Min, Max);
            }    
        }

        internal InspectorParam[] GetInspectorParam(ShapeType shapeType)
        {
            var p = new InspectorParam[4];
            switch (shapeType)
            {
                case ShapeType.Circle:
                    break;
                case ShapeType.Polygon:
                    p[3] = new InspectorParam("Polygon", 3f, 12f, true);
                    break;
                case ShapeType.RoundedPolygon:
                    p[0] = new InspectorParam("Rounded", 0f, 0.7f);
                    p[3] = new InspectorParam("Polygon", 3f, 12f, true);
                    break;
                case ShapeType.Heart:
                    break;
                case ShapeType.Cross:
                    p[0] = new InspectorParam("Value", 0.01f, 0.3f);
                    break;
                case ShapeType.Star:
                    p[0] = new InspectorParam("Rounded", 0.01f, 1f);
                    p[3] = new InspectorParam("Polygon", 5f, 12f, true);
                    break;
                case ShapeType.Ring:
                    p[0] = new InspectorParam("Rounded", 0.01f, 1f);
                    break;
                case ShapeType.Polar:
                    p[0] = new InspectorParam("Mode", 0f, 3, true);
                    p[3] = new InspectorParam("Polygon", 2f, 20f, true);
                    break;
                case ShapeType.Superellipse:
                    p[0] = new InspectorParam("Value", 0.2f, 10f);
                    p[1] = new InspectorParam("Blur", 0f, 0.5f);
                    break;
                case ShapeType.Arrow:
                    p[0] = new InspectorParam("Width", 0.1f, 0.9f);
                    p[1] = new InspectorParam("LineWidth", 0.01f, 0.4f);
                    break;
                case ShapeType.CheckMark:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return p;
        }

        internal bool OutlineEnable(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.Circle:
                case ShapeType.Polygon:
                case ShapeType.RoundedPolygon:
                case ShapeType.Star:
                case ShapeType.Heart:
                case ShapeType.Polar:
                case ShapeType.Arrow:
                    return true;
                case ShapeType.CheckMark:
                case ShapeType.Superellipse:
                case ShapeType.Cross:
                case ShapeType.Ring:
                    return false;
                 default:
                    throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null);
            }
        }
#endif
    }
}