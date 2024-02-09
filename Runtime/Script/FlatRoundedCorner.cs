using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    public class FlatRoundedCorner : FlatUI
    {
        public enum Type
        {
            None,
            OutLine,
            Separate,
        }
        
        protected override string GetMaterialName()
        {
            return _type switch
            {
                Type.OutLine => "FlatRoundedCornerOutline",
                Type.None => "FlatRoundedCorner",
                Type.Separate => "FlatRoundedCornerSeparate",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [Flags]
        private enum CurveFlags
        {
            LeftTop = 1 << 0,
            RightTop = 1 << 1,
            LeftBottom = 1 << 2,
            RightBottom = 1 << 3,
        }

        [SerializeField]
        private CurveFlags _flags =
            CurveFlags.LeftTop | CurveFlags.RightTop | CurveFlags.LeftBottom | CurveFlags.RightBottom;

        [SerializeField, Range(0, 0.5f)]
        private float _radius;

        [SerializeField]
        private Type _type;

        [SerializeField, Range(0, 0.05f)]
        private float _outline;

        [SerializeField, Range(0, 1f)]
        private float _separate = 0.8f;

        [SerializeField, ColorUsage(false)]
        private Color _color;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            var rect = _rectTransform.rect;

            var color = _color.r / 10 +
                        Mathf.FloorToInt(_color.g * 100) +
                        Mathf.FloorToInt(_color.b * 100) * 1000;

            var flagsClamp = (int)_flags / 15f;
            var uv1Param = new Vector4(_radius, flagsClamp, color);
            if (_type == Type.OutLine)
                uv1Param.w = _outline;
            if (_type == Type.Separate)
                uv1Param.w = _separate;
                
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv0.z = rect.width;
                vertex.uv0.w = rect.height;
                vertex.uv1 = uv1Param;
                vertexList[i] = vertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
    }
}