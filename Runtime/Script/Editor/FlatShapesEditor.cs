using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
    [CustomEditor(typeof(FlatShapes))]
    public class FlatShapesEditor : Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _raycast;
        private SerializedProperty _shapeType;
        private SerializedProperty _polygon;
        private SerializedProperty _floatValue;
        private SerializedProperty _outlineWidth;
        private SerializedProperty _outlineColor;

        private void OnEnable()
        {
            _color = serializedObject.FindProperty("m_Color");
            _raycast = serializedObject.FindProperty("m_RaycastTarget");
            
            _shapeType = serializedObject.FindProperty("_shapeType");
            _polygon = serializedObject.FindProperty("_polygon");
            _floatValue = serializedObject.FindProperty("_floatValue");
            
            _outlineWidth = serializedObject.FindProperty("_outlineWidth");
            _outlineColor = serializedObject.FindProperty("_outlineColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_color);
            EditorGUILayout.PropertyField(_raycast);
            EditorGUILayout.PropertyField(_shapeType);

            var shape = (FlatShapes.ShapeType)_shapeType.intValue;
            
            if (shape is FlatShapes.ShapeType.Polygon)
            {
                _polygon.intValue = EditorGUILayout.IntSlider(_polygon.displayName, _polygon.intValue, 3, 12);
            }

            if (shape is FlatShapes.ShapeType.Star or FlatShapes.ShapeType.RoundStar)
            {
                _polygon.intValue = EditorGUILayout.IntSlider(_polygon.displayName, _polygon.intValue, 5, 12);
            }

            if (shape is FlatShapes.ShapeType.Cross)
            {
                _floatValue.floatValue = EditorGUILayout.Slider(_floatValue.displayName, _floatValue.floatValue, 0.01f, 0.3f);
            }
            if (shape is FlatShapes.ShapeType.RoundStar or FlatShapes.ShapeType.Ring)
            {
                _floatValue.floatValue = EditorGUILayout.Slider(_floatValue.displayName, _floatValue.floatValue, 0.01f, 0.99f);
            }

            if (shape is FlatShapes.ShapeType.Circle or
                FlatShapes.ShapeType.Heart or
                FlatShapes.ShapeType.Polygon or
                FlatShapes.ShapeType.Star or
                FlatShapes.ShapeType.RoundStar
               )
            {
                EditorGUILayout.PropertyField(_outlineWidth);
                EditorGUILayout.PropertyField(_outlineColor);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}