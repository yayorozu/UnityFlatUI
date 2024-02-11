using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
    [CustomEditor(typeof(FlatRoundedCorner))]
    public class FlatRoundedCornerEditor : Editor
    {
        private SerializedProperty _mcolor;
        private SerializedProperty _raycast;
        private SerializedProperty _cornerShape;
        private SerializedProperty _flags;
        private SerializedProperty _radius;
        private SerializedProperty _type;
        private SerializedProperty _outline;
        private SerializedProperty _separate;
        private SerializedProperty _color;

        private void OnEnable()
        {
            _mcolor = serializedObject.FindProperty("m_Color");
            _raycast = serializedObject.FindProperty("m_RaycastTarget");
            _cornerShape = serializedObject.FindProperty("_cornerShape");
            _flags = serializedObject.FindProperty("_flags");
            _radius = serializedObject.FindProperty("_radius");
            _type = serializedObject.FindProperty("_type");
            _outline = serializedObject.FindProperty("_outline");
            _separate = serializedObject.FindProperty("_separate");
            _color = serializedObject.FindProperty("_color");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_mcolor);
            EditorGUILayout.PropertyField(_raycast);
            EditorGUILayout.PropertyField(_cornerShape);
            EditorGUILayout.PropertyField(_flags);
            EditorGUILayout.PropertyField(_radius);
            EditorGUILayout.PropertyField(_type);
            if (_type.intValue == (int) FlatRoundedCorner.Type.Outline)
            {
                EditorGUILayout.PropertyField(_outline);
                EditorGUILayout.PropertyField(_color);
            }

            if (_type.intValue == (int) FlatRoundedCorner.Type.Separate)
            {
                EditorGUILayout.PropertyField(_separate);
                EditorGUILayout.PropertyField(_color);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}