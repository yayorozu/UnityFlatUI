using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
    [CustomEditor(typeof(FlatShapes))]
    public class FlatShapesEditor : Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _raycast;
        private SerializedProperty _shapeType;
        private SerializedProperty _poloygon;
        private SerializedProperty _value;
        private SerializedProperty _outlineWidth;
        private SerializedProperty _outlineColor;

        private void OnEnable()
        {
            _color = serializedObject.FindProperty("m_Color");
            _raycast = serializedObject.FindProperty("m_RaycastTarget");
            _shapeType = serializedObject.FindProperty("_shapeType");
            _poloygon = serializedObject.FindProperty("_poloygon");
            _value = serializedObject.FindProperty("_value");
            _outlineWidth = serializedObject.FindProperty("_outlineWidth");
            _outlineColor = serializedObject.FindProperty("_outlineColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_color);
            EditorGUILayout.PropertyField(_raycast);
            EditorGUILayout.PropertyField(_shapeType);
            EditorGUILayout.PropertyField(_poloygon);
            EditorGUILayout.PropertyField(_value);
            EditorGUILayout.PropertyField(_outlineWidth);
            EditorGUILayout.PropertyField(_outlineColor);

            serializedObject.ApplyModifiedProperties();
        }
    }
}