using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    public class FlatCorner : FlatUI
    {
        public enum Type
        {
            None,
            Outline,
            Separate,
        }
        
        protected override string GetMaterialName()
        {
            return $"FlatCorner{_cornerShape}{_type}";
        }

        internal enum CornerShape
        {
            Rounded,
            Cut,
            Shift,
        }
        
        [Flags]
        internal enum EdgeFlags
        {
            LeftTop = 1 << 0,
            RightTop = 1 << 1,
            LeftBottom = 1 << 2,
            RightBottom = 1 << 3,
        }
        
        [SerializeField]
        private CornerShape _cornerShape;

        [SerializeField]
        private EdgeFlags _flags =
            EdgeFlags.LeftTop | EdgeFlags.RightTop | EdgeFlags.LeftBottom | EdgeFlags.RightBottom;

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
        
        [SerializeField, Range(-100f, 100f)]
        private float _shift;

        [SerializeField]
        private RectTransform.Axis _axis;

        protected override void OnPopulateMesh(ref List<UIVertex> vertexList, bool shadow)
        {
            var rect = _rectTransform.rect;

            var colorFloat = ColorToFloat(_color);

            var flagsClamp = (int)_flags / 15f;
            var uv1Param = new Vector4(_radius, flagsClamp, colorFloat);
            uv1Param.w = _type switch
            {
                Type.Outline => shadow ? 0 : _outline,
                Type.Separate => shadow ? 1 : _separate,
                _ => uv1Param.w
            };

            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv0.z = rect.width;
                vertex.uv0.w = rect.height;
                vertex.uv1 = uv1Param;
                vertexList[i] = vertex;
            }

            var shiftValue = Vector2.zero;
            if (_cornerShape == CornerShape.Shift)
            {
                var i = _axis is RectTransform.Axis.Horizontal ? 0 : 1;
                shiftValue[i] = _shift;

                foreach (var index in LeftTopIndexes)
                    UpdatePosition(ref vertexList, index, shiftValue);
                
                shiftValue[i] = -_shift;
                foreach (var index in RightBottomIndexes)
                    UpdatePosition(ref vertexList, index, shiftValue);
            }
        }
    }
}