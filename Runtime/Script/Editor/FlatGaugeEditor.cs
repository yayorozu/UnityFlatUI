using System;
using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FlatGauge))]
    public class FlatGaugeEditor : Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _raycast;
        private SerializedProperty _gaugeType;
        private SerializedProperty _width;
        private SerializedProperty _startAngle;
        private SerializedProperty _fillAmount;
        private SerializedProperty _isReverse;
        private SerializedProperty _length;
        private SerializedProperty _frameWidth;
        private SerializedProperty _frameColor;
        private SerializedProperty _backColor;

        private void OnEnable()
        {
            _color = serializedObject.FindProperty("m_Color");
            _raycast = serializedObject.FindProperty("m_RaycastTarget");
            _gaugeType = serializedObject.FindProperty("_gaugeType");
            _width = serializedObject.FindProperty("_width");
            _startAngle = serializedObject.FindProperty("_startAngle");
            _fillAmount = serializedObject.FindProperty("_fillAmount");
            _isReverse = serializedObject.FindProperty("_isReverse");
            _length = serializedObject.FindProperty("_length");
            _frameWidth = serializedObject.FindProperty("_frameWidth");
            _frameColor = serializedObject.FindProperty("_frameColor");
            _backColor = serializedObject.FindProperty("_backColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_color);
            EditorGUILayout.PropertyField(_raycast);
            
            EditorGUILayout.PropertyField(_gaugeType);
            EditorGUILayout.PropertyField(_fillAmount);
            EditorGUILayout.PropertyField(_isReverse);
            
            EditorGUILayout.PropertyField(_frameColor);
            EditorGUILayout.PropertyField(_frameWidth);
            
            var type = (FlatGauge.GaugeType)_gaugeType.intValue;
            switch (type)
            {
                case FlatGauge.GaugeType.Circle:
                    EditorGUILayout.PropertyField(_width);
                    EditorGUILayout.PropertyField(_startAngle);
                    EditorGUILayout.PropertyField(_length);
                    break;
                case FlatGauge.GaugeType.Horizontal:
                case FlatGauge.GaugeType.Vertical:
                    EditorGUILayout.PropertyField(_backColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}