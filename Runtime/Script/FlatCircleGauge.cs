using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    public class FlatCircleGauge : FlatUI
    {
        protected override string GetMaterialName() => "FlatCircleGauge";

        [SerializeField, Range(0.1f, 1f)]
        private float _width = 0.1f;

        [SerializeField, Range(0f, 359f)]
        private float _startAngle = 90;

        [SerializeField, Range(0f, 1f)]
        private float _fillAmount = 1;

        [SerializeField]
        private bool _isReverse;

        [SerializeField, Range(0.1f, 1f)]
        private float _length = 1f;
        
        [SerializeField, Range(0, 0.1f)]
        private float _frameWidth = 0.1f;

        [SerializeField, ColorUsage(false)]
        private Color _frameColor;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            var radian = _startAngle * (Mathf.PI / 180);
            var angleVector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
            var uv1 = new Vector4(
                angleVector.x, angleVector.y,
                _frameWidth, ColorToFloat(_frameColor)
            );

            var value1 = _width / 10f + Mathf.FloorToInt(_fillAmount * 100);
            var value2 = (_isReverse ? 0.1f : 0f) + Mathf.FloorToInt(_length * 100);
            
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv0.z = value1;
                vertex.uv0.w = value2;
                vertex.uv1 = uv1;
                vertexList[i] = vertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
    }
}