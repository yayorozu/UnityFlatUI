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
        private SerializedProperty _shift;
        private SerializedProperty _axis;

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
            _shift = serializedObject.FindProperty("_shift");
            _axis = serializedObject.FindProperty("_axis");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_cornerShape);
                if (check.changed)
                {
                    var shape = (FlatCorner.CornerShape) _cornerShape.intValue;
                    if (shape is FlatCorner.CornerShape.Shift)
                    {
                        _radius.floatValue = 0;
                        _type.intValue = (int) FlatCorner.Type.None;
                    }
                }
            }

            if ((FlatCorner.CornerShape) _cornerShape.intValue is FlatCorner.CornerShape.Shift)
            {
                EditorGUILayout.PropertyField(_axis);
                EditorGUILayout.PropertyField(_shift);
            }
            else
            {
                EditorGUILayout.PropertyField(_flags);
                EditorGUILayout.PropertyField(_radius);
            }
            
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