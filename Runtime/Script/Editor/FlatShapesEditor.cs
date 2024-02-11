using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
    [CustomEditor(typeof(FlatShapes))]
    public class FlatShapesEditor : Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _raycast;
        private SerializedProperty _shapeType;
        private SerializedProperty _floatValue;
        private SerializedProperty _outlineWidth;
        private SerializedProperty _outlineColor;

        private void OnEnable()
        {
            _color = serializedObject.FindProperty("m_Color");
            _raycast = serializedObject.FindProperty("m_RaycastTarget");
            
            _shapeType = serializedObject.FindProperty("_shapeType");
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
            
            var wv = (int)_floatValue.vector4Value.w;
            using (var check =  new EditorGUI.ChangeCheckScope())
            {
                wv = shape switch
                {
                    FlatShapes.ShapeType.Polygon => EditorGUILayout.IntSlider("Polygon", wv, 3, 12),
                    FlatShapes.ShapeType.Polar => EditorGUILayout.IntSlider("Polygon", wv, 2, 20),
                    FlatShapes.ShapeType.Star or FlatShapes.ShapeType.RoundStar => EditorGUILayout.IntSlider("Polygon", wv, 5, 12),
                    _ => wv
                };
                if (check.changed)
                {
                    var vec = _floatValue.vector4Value;
                    vec.w = wv;
                    _floatValue.vector4Value = vec;
                }
            }

            var vec4 = _floatValue.vector4Value;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                switch (shape)
                {
                    case FlatShapes.ShapeType.Cross:
                        vec4.x = EditorGUILayout.Slider(_floatValue.displayName, vec4.x, 0.01f, 0.3f);
                        break;
                    case FlatShapes.ShapeType.RoundStar or FlatShapes.ShapeType.Ring:
                        vec4.x = EditorGUILayout.Slider(_floatValue.displayName, vec4.x, 0.01f, 0.99f);
                        break;
                    case FlatShapes.ShapeType.Superellipse:
                        vec4.x = EditorGUILayout.Slider(_floatValue.displayName, vec4.x, 0.2f, 10f);
                        vec4.y = EditorGUILayout.Slider("Blur", vec4.y, 0, 0.5f);
                        break;
                    case FlatShapes.ShapeType.Polar:
                    {
                        var v = (int)vec4.x;
                        vec4.x = (float)EditorGUILayout.IntSlider("Mode", v, 0, 3);
                        break;
                    }
                    case FlatShapes.ShapeType.Arrow:
                    {
                        vec4.x = EditorGUILayout.Slider("Width", vec4.x, 0.1f, 0.9f);
                        vec4.y = EditorGUILayout.Slider("LineWidth", vec4.y, 0.01f, 0.4f);
                    }
                        break;
                }
                if (check.changed)
                {
                    _floatValue.vector4Value = vec4;
                }
            }

            if (shape is FlatShapes.ShapeType.Circle or
                FlatShapes.ShapeType.Heart or
                FlatShapes.ShapeType.Polygon or
                FlatShapes.ShapeType.Star or
                FlatShapes.ShapeType.RoundStar or
                FlatShapes.ShapeType.Cross or
                FlatShapes.ShapeType.Polar or
                FlatShapes.ShapeType.Arrow
               )
            {
                EditorGUILayout.PropertyField(_outlineWidth);
                EditorGUILayout.PropertyField(_outlineColor);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}