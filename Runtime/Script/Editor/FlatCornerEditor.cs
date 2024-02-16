using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FlatCorner))]
    public class FlatCornerEditor : FlatUIEditor
    {
        private SerializedProperty _cornerShape;
        private SerializedProperty _flags;
        private SerializedProperty _radius;
        private SerializedProperty _type;
        private SerializedProperty _outline;
        private SerializedProperty _separate;
        private SerializedProperty _color;

        protected override void OnEnable()
        {
            base.OnEnable();
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
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(_cornerShape);
            EditorGUILayout.PropertyField(_flags);
            EditorGUILayout.PropertyField(_radius);
            EditorGUILayout.PropertyField(_type);
            if (_type.intValue == (int) FlatCorner.Type.Outline)
            {
                EditorGUILayout.PropertyField(_outline);
                EditorGUILayout.PropertyField(_color);
            }

            if (_type.intValue == (int) FlatCorner.Type.Separate)
            {
                EditorGUILayout.PropertyField(_separate);
                EditorGUILayout.PropertyField(_color);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}