#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Yorozu.FlatUI.Tool
{
    [CanEditMultipleObjects]
    public class FlatUIEditor : Editor
    {
        private SerializedProperty _mcolor;
        private SerializedProperty _raycast;
        
        private SerializedProperty _leftTopColor;
        private SerializedProperty _overrideLeftTopColor;
        
        private SerializedProperty _leftBottomColor;
        private SerializedProperty _overrideLeftBottomColor;
        
        private SerializedProperty _rightTopColor;
        private SerializedProperty _overrideRightTopColor;
        
        private SerializedProperty _rightBottomColor;
        private SerializedProperty _overrideRightBottomColor;
        
        private SerializedProperty _shadow;
        private SerializedProperty _shadowColor;
        private SerializedProperty _shadowOffset;

        private static class Styles
        {
            public static readonly GUILayoutOption ToggleWidth = GUILayout.Width(20f);
        }
        
        protected virtual void OnEnable()
        {
            _mcolor = serializedObject.FindProperty("m_Color");
            _raycast = serializedObject.FindProperty("m_RaycastTarget");
            
            _leftTopColor = serializedObject.FindProperty("_leftTopColor");
            _overrideLeftTopColor = serializedObject.FindProperty("_overrideLeftTopColor");
            _leftBottomColor = serializedObject.FindProperty("_leftBottomColor");
            _overrideLeftBottomColor = serializedObject.FindProperty("_overrideLeftBottomColor");
            _rightTopColor = serializedObject.FindProperty("_rightTopColor");
            _overrideRightTopColor = serializedObject.FindProperty("_overrideRightTopColor");
            _rightBottomColor = serializedObject.FindProperty("_rightBottomColor");
            _overrideRightBottomColor = serializedObject.FindProperty("_overrideRightBottomColor");
            
            _shadow = serializedObject.FindProperty("_shadow");
            _shadowColor = serializedObject.FindProperty("_shadowColor");
            _shadowOffset = serializedObject.FindProperty("_shadowOffset");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_mcolor);
            EditorGUILayout.PropertyField(_raycast);
            
            var inspectorHalfWidth = EditorGUIUtility.currentViewWidth / 2f - 14f;
            
            EditorGUILayout.LabelField("Override Vertex Color", EditorStyles.boldLabel);
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    var width = GUILayout.Width(inspectorHalfWidth);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUILayout.HorizontalScope(width))
                            {
                                EditorGUILayout.PropertyField(_overrideLeftTopColor, GUIContent.none, Styles.ToggleWidth);
                                if (_overrideLeftTopColor.boolValue)
                                    EditorGUILayout.PropertyField(_leftTopColor, GUIContent.none);
                            }

                            using (new EditorGUILayout.HorizontalScope(width))
                            {
                                EditorGUILayout.PropertyField(_overrideRightTopColor, GUIContent.none, Styles.ToggleWidth);
                                if (_overrideRightTopColor.boolValue)
                                    EditorGUILayout.PropertyField(_rightTopColor, GUIContent.none);
                            }
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUILayout.HorizontalScope(width))
                            {
                                EditorGUILayout.PropertyField(_overrideLeftBottomColor, GUIContent.none, Styles.ToggleWidth);
                                if (_overrideLeftBottomColor.boolValue)
                                    EditorGUILayout.PropertyField(_leftBottomColor, GUIContent.none);
                            }
                            
                            using (new EditorGUILayout.HorizontalScope(width))
                            {
                                EditorGUILayout.PropertyField(_overrideRightBottomColor, GUIContent.none, Styles.ToggleWidth);
                                if (_overrideRightBottomColor.boolValue)
                                    EditorGUILayout.PropertyField(_rightBottomColor, GUIContent.none);
                            }
                        }
                    }
                }
            }
            
            EditorGUILayout.PropertyField(_shadow);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUI.DisabledScope(!_shadow.boolValue))
            {
                EditorGUILayout.PropertyField(_shadowColor, new GUIContent("Color"));
                EditorGUILayout.PropertyField(_shadowOffset, new GUIContent("Offset"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif