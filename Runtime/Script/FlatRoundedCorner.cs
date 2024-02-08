using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class FlatRoundedCorner : MaskableGraphic
    {
        public enum Type
        {
            None,
            OutLine,
            Separate,
        }
#if UNITY_EDITOR
        [NonSerialized]
        private int _parentId;

        [NonSerialized]
        private Canvas _cacheCanvas;

        protected override void Reset()
        {
            base.Reset();
            ReplaceMaterialIfNeeded(TargetMaterialName);
            SetCanvasChannel();
        }

        private Material FindMaterial(string name)
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:Material {name}", new[] {"Packages"});
            for (int i = 0; i < guids.Length; i++)
            {
                var path = guids[i];
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(path);
                var material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (material.name != name)
                    continue;
                
                return material;
            }
            
            return null;
        }

        private void ReplaceMaterialIfNeeded(string name)
        {
            if (m_Material == null || m_Material.name != name)
                m_Material = FindMaterial(name);
        }
        
        private string TargetMaterialName => _type switch
        {
            Type.OutLine => "FlatRoundedCornerOutline",
            Type.None => "FlatRoundedCorner",
            Type.Separate => "FlatRoundedCornerSeparate",
            _ => throw new ArgumentOutOfRangeException()
        };

        protected override void OnValidate()
        {
            base.OnValidate();
            ReplaceMaterialIfNeeded(TargetMaterialName);

            var graphic = GetComponent<Graphic>();
            graphic.SetVerticesDirty();
            SetCanvasChannel();
        }

        /// <summary>
        /// Canvasのチャンネルを操作
        /// </summary>
        private void SetCanvasChannel()
        {
            // 親
            if (_parentId == 0 ||
                transform.parent.GetInstanceID() != _parentId)
            {
                if (transform.parent == null)
                    return;
                _parentId = transform.parent.GetInstanceID();
                if (_cacheCanvas == null)
                {
                    var parent = transform.parent;
                    while (parent != null)
                    {
                        var canvas = parent.GetComponent<Canvas>();
                        if (canvas != null)
                        {
                            _cacheCanvas = canvas;
                            break;
                        }

                        parent = parent.parent;
                    }
                }
            }

            if (_cacheCanvas == null)
                return;

            _cacheCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            _cacheCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            if (_type != Type.None)
                _cacheCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
        }

#endif

        [Flags]
        private enum CurveFlags
        {
            LeftTop = 1 << 0,
            RightTop = 1 << 1,
            LeftBottom = 1 << 2,
            RightBottom = 1 << 3,
        }

        [SerializeField]
        private CurveFlags _flags =
            CurveFlags.LeftTop | CurveFlags.RightTop | CurveFlags.LeftBottom | CurveFlags.RightBottom;

        [SerializeField, Range(0, 0.5f)]
        private float _radius;

        [SerializeField]
        private Type _type;

        [SerializeField, Range(0, 0.05f)]
        private float _outline;

        [SerializeField, Range(0, 1f)]
        private float _separate = 0.8f;

        [SerializeField]
        private Color _color;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            var rect = transform as RectTransform;

            var color = _color.r / 10 +
                        Mathf.FloorToInt(_color.g * 100) +
                        Mathf.FloorToInt(_color.b * 100) * 1000;

            var flagsClamp = (int)_flags / 15f;
            for (var i = 0; i < vertexList.Count; i++)
            {
                var vertex = vertexList[i];
                vertex.uv1 = new Vector2(_radius, flagsClamp);
                vertex.uv2 = new Vector2(rect.rect.width, rect.rect.height);
                if (_type == Type.OutLine)
                    vertex.uv3 = new Vector2(_outline, color);
                if (_type == Type.Separate)
                    vertex.uv3 = new Vector2(_separate, color);

                vertexList[i] = vertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
    }
}