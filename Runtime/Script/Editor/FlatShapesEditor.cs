using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

namespace Yorozu.FlatUI.Tool
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FlatShapes))]
    public class FlatShapesEditor : FlatUIEditor
    {
        private SerializedProperty _shapeType;
        private SerializedProperty _floatValue;
        private SerializedProperty _outlineWidth;
        private SerializedProperty _outlineColor;

        private FlatShapes _shapes;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _shapeType = serializedObject.FindProperty("_shapeType");
            _floatValue = serializedObject.FindProperty("_floatValue");
            
            _outlineWidth = serializedObject.FindProperty("_outlineWidth");
            _outlineColor = serializedObject.FindProperty("_outlineColor");
            
            _shapes = target as FlatShapes; 
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            
            var shape = (FlatShapes.ShapeType)_shapeType.intValue;
            var p =  _shapes.GetInspectorParam(shape);
            var vec = _floatValue.vector4Value;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_shapeType);
                if (check.changed)
                {
                    var newP = _shapes.GetInspectorParam((FlatShapes.ShapeType)_shapeType.intValue);
                    for (var i = 0; i < p.Length; i++)
                    {
                        if (newP[i] == null)
                            continue;

                        vec[i] = newP[i].Clamp(vec[i]);
                    }

                    _floatValue.vector4Value = vec;;
                }
            }
            
            
            var outline = _shapes.OutlineEnable(shape);
            
            if (outline)
            {
                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                using (var check = new EditorGUI.ChangeCheckScope())
                using (new EditorGUI.IndentLevelScope())
                {
                    bool clip = vec.z > 0;
                    using (new EditorGUI.DisabledGroupScope(_outlineWidth.floatValue <= 0))
                    {
                        clip = EditorGUILayout.Toggle("Inner Clip", clip); 
                    }
                    EditorGUILayout.PropertyField(_outlineWidth, new GUIContent("Width"));
                    EditorGUILayout.PropertyField(_outlineColor, new GUIContent("Color"));

                    if (check.changed)
                    {
                        if (_outlineWidth.floatValue <= 0)
                        {
                            clip = false;
                        }
                        vec.z = clip ? 1 : 0;
                        _floatValue.vector4Value = vec;
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
                        iv = EditorGUILayout.IntSlider(p[i].Name, iv, min, max);
                        if (check.changed)
                        {
                            vec[i] = iv;
                            _floatValue.vector4Value = vec;
                        }
                    }
                    else
                    {
                        vec[i] = EditorGUILayout.Slider(p[i].Name, vec[i], p[i].Min, p[i].Max);
                        if (check.changed)
                        {
                            _floatValue.vector4Value = vec;
                        }
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }

    }
}