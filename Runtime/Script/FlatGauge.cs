using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    public class FlatGauge : FlatUI
    {
        protected override string GetMaterialName() => $"FlatGauge{_gaugeType}";
        
        public enum GaugeType
        {
            Circle,
            Horizontal,
            Vertical,
        }
        
        [SerializeField]
        private GaugeType _gaugeType = GaugeType.Circle;
        
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
        
        [SerializeField, ColorUsage(false)]
        private Color _backColor;
        
        public float FillAmount
        {
            get => _fillAmount;
            set
            {
                _fillAmount = Mathf.Clamp01(value);
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(ref List<UIVertex> vertexList)
        {
            var rect = _rectTransform.rect;
            
            var backColorFloat = ColorToFloat(_backColor);
            var uv1 = new Vector4(0, 0, _frameWidth, ColorToFloat(_frameColor));
            if (_gaugeType is GaugeType.Circle)
            {
                var radian = _startAngle * (Mathf.PI / 180);
                var angleVector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
                uv1.x = angleVector.x;
                uv1.y = angleVector.y;
            }
            else
            {
                uv1.x = rect.width;
                uv1.y = rect.height;
            }
            
            var value1 = (_isReverse ? 0.1f : 0f) + Mathf.FloorToInt(_fillAmount * 100);
            var value2 = _gaugeType is GaugeType.Circle ?
                _width / 10f + Mathf.FloorToInt(_length * 100) :
                backColorFloat;
            
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv0.z = value1;
                vertex.uv0.w = value2;
                vertex.uv1 = uv1;
                vertexList[i] = vertex;
            }
        }
    }
}