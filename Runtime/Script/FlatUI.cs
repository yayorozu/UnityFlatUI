using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public abstract class FlatUI : MaskableGraphic
    {
        protected abstract string GetMaterialName();
        
        protected RectTransform _rectTransform => transform as RectTransform;
        
#if UNITY_EDITOR
        [NonSerialized]
        private int _parentId;

        [NonSerialized]
        private Canvas _cacheCanvas;

        protected override void Reset()
        {
            base.Reset();
            ReplaceMaterialIfNeeded(GetMaterialName());
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

            if (m_Material == null)
            {
                Debug.LogError($"Not Found Material {name}");
            }
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            ReplaceMaterialIfNeeded(GetMaterialName());

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
        }
#endif
        
        [SerializeField, ColorUsage(false)]
        private Color _leftTopColor = Color.white;
        [SerializeField]
        private bool _overrideLeftTopColor;
        
        [SerializeField, ColorUsage(false)]
        private Color _leftBottomColor = Color.white;
        [SerializeField]
        private bool _overrideLeftBottomColor;
        
        [SerializeField, ColorUsage(false)]
        private Color _rightTopColor = Color.white;
        [SerializeField]
        private bool _overrideRightTopColor;
        
        [SerializeField, ColorUsage(false)]
        private Color _rightBottomColor = Color.white;
        [SerializeField]
        private bool _overrideRightBottomColor;
        
        protected float ColorToFloat(Color color)
        {
            var rgb = new Vector3Int(
                Mathf.FloorToInt(color.r * 255f + 0.5f),
                Mathf.FloorToInt(color.g * 255f + 0.5f),
                Mathf.FloorToInt(color.b * 255f + 0.5f)
            );
            float packed = rgb.x * 65536 + rgb.y * 256 + rgb.z;
            return packed / (256 * 256 * 256);
        }
        
        protected sealed override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            if (_overrideLeftTopColor)
                UpdateColor(1, _leftTopColor);
            if (_overrideRightTopColor)
            {
                UpdateColor(2, _rightTopColor);
                UpdateColor(3, _rightTopColor);
            }
            if (_overrideRightBottomColor)
            {
                UpdateColor(4, _rightBottomColor);
            }

            if (_overrideLeftBottomColor)
            {
                UpdateColor(0, _leftBottomColor);
                UpdateColor(5, _leftBottomColor);
            }

            void UpdateColor(int index, Color color)
            {
                var v = vertexList[index];
                v.color = color;
                vertexList[index] = v;
            }
            
            OnPopulateMesh(ref vertexList);
            
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
        
        protected abstract void OnPopulateMesh(ref List<UIVertex> vertexList);
    }
}