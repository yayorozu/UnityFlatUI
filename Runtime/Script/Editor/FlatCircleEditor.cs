using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
    [CustomEditor(typeof(FlatCircle))]
    public class FlatCircleEditor : Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _raycast;
        private SerializedProperty _outlineWidth;
        private SerializedProperty _outlineColor;

        private void OnEnable()
        {
            _color = serializedObject.FindProperty("m_Color");
            _raycast = serializedObject.FindProperty("m_RaycastTarget");
            _outlineWidth = serializedObject.FindProperty("_outlineWidth");
            _outlineColor = serializedObject.FindProperty("_outlineColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_color);
            EditorGUILayout.PropertyField(_raycast);
            EditorGUILayout.PropertyField(_outlineWidth);
            EditorGUILayout.PropertyField(_outlineColor);

            serializedObject.ApplyModifiedProperties();
        }
    }
}