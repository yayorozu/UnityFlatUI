#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using ShapeType = Yorozu.FlatUI.FlatShapes.ShapeType;

namespace Yorozu.FlatUI.Tool
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FlatShapes))]
    public class FlatShapesEditor : FlatUIEditor
    {
        private SerializedProperty _shapeType;
        private SerializedProperty _floatValue;
        private SerializedProperty _innerClip;
        private SerializedProperty _outlineWidth;
        private SerializedProperty _outlineColor;

        protected override void OnEnable()
        {
            base.OnEnable();
            _shapeType = serializedObject.FindProperty("_shapeType");
            _floatValue = serializedObject.FindProperty("_floatValue");
            
            _innerClip = serializedObject.FindProperty("_innerClip");
            _outlineWidth = serializedObject.FindProperty("_outlineWidth");
            _outlineColor = serializedObject.FindProperty("_outlineColor");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            
            var shape = (ShapeType)_shapeType.intValue;
            var p =  GetInspectorParam(shape);
            var vec = _floatValue.vector4Value;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_shapeType);
                if (check.changed)
                {
                    var newShape = (ShapeType) _shapeType.intValue;
                    var newP = GetInspectorParam(newShape);
                    for (var i = 0; i < p.Length; i++)
                    {
                        if (newP[i] == null)
                            continue;

                        vec[i] = newP[i].Clamp(vec[i]);
                    }

                    if (!OutlineEnable(newShape))
                    {
                        _innerClip.boolValue = false;
                    }
                    _floatValue.vector4Value = vec;
                }
            }
            
            var outline = OutlineEnable(shape);
            
            if (outline)
            {
                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                using (var check = new EditorGUI.ChangeCheckScope())
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUI.DisabledGroupScope(_outlineWidth.floatValue <= 0))
                    { 
                        EditorGUILayout.PropertyField(_innerClip);
                    }
                    EditorGUILayout.PropertyField(_outlineWidth, new GUIContent("Width"));
                    EditorGUILayout.PropertyField(_outlineColor, new GUIContent("Color"));

                    if (check.changed)
                    {
                        if (_outlineWidth.floatValue <= 0)
                        {
                            _innerClip.boolValue = false;
                        }
                    }
                }
            }
            
            using (var check =  new EditorGUI.ChangeCheckScope())
            {
                for (var i = 0; i < p.Length; i++)
                {
                    if (p[i] == null)
                        continue;

                    if (p[i].Int)
                    {
                        var iv = (int)vec[i];
                        var min = Mathf.RoundToInt(p[i].Min);
                        var max = Mathf.RoundToInt(p[i].Max);
                        vec[i] = EditorGUILayout.IntSlider(p[i].Name, iv, min, max);
                    }
                    else
                    { 
                        vec[i] = EditorGUILayout.Slider(p[i].Name, vec[i], p[i].Min, p[i].Max);
                    }
                }
                
                if (check.changed)
                {
                    _floatValue.vector4Value = vec;
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        
        internal class InspectorParam
        {
            public string Name;
            public float Min;
            public float Max;
            public bool Int;

            public InspectorParam(string name, float min, float max, bool i = false)
            {
                Name = name;
                Min = min;
                Max = max;
                Int = i;
            }

            public float Clamp(float value)
            {
                return Mathf.Clamp(value, Min, Max);
            }    
        }

        private InspectorParam[] GetInspectorParam(ShapeType shapeType)
        {
            var p = new InspectorParam[4];
            switch (shapeType)
            {
                case ShapeType.Circle:
                    break;
                case ShapeType.Polygon:
                    p[3] = new InspectorParam("Polygon", 3f, 12f, true);
                    break;
                case ShapeType.RoundedPolygon:
                    p[0] = new InspectorParam("Rounded", 0f, 0.7f);
                    p[3] = new InspectorParam("Polygon", 3f, 12f, true);
                    break;
                case ShapeType.Heart:
                    break;
                case ShapeType.Cross:
                    p[0] = new InspectorParam("Value", 0.01f, 0.3f);
                    break;
                case ShapeType.Star:
                    p[0] = new InspectorParam("Rounded", 0.01f, 1f);
                    p[3] = new InspectorParam("Polygon", 5f, 12f, true);
                    break;
                case ShapeType.Ring:
                    p[0] = new InspectorParam("Rounded", 0.01f, 1f);
                    break;
                case ShapeType.Polar:
                    p[0] = new InspectorParam("Mode", 0f, 3, true);
                    p[3] = new InspectorParam("Polygon", 2f, 20f, true);
                    break;
                case ShapeType.Superellipse:
                    p[0] = new InspectorParam("Value", 0.2f, 10f);
                    p[1] = new InspectorParam("Blur", 0f, 0.5f);
                    break;
                case ShapeType.Arrow:
                    p[0] = new InspectorParam("ArrowWidth", 0.1f, 0.9f);
                    p[1] = new InspectorParam("LineWidth", 0.01f, 0.4f);
                    break;
                case ShapeType.CheckMark:
                    p[0] = new InspectorParam("Width", 0.01f, 0.4f);
                    p[1] = new InspectorParam("Left", 0.3f, 0.5f);
                    p[2] = new InspectorParam("Right", 0.5f, 0.8f);
                    break;
                case ShapeType.MagnifyingGlass:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return p;
        }

        private bool OutlineEnable(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.Superellipse:
                case ShapeType.Cross:
                case ShapeType.Ring:
                case ShapeType.MagnifyingGlass:
                    return false;
                 default:
                    return true;
            }
        }

    }
}

#endif