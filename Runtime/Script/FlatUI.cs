using System;
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

        protected float ColorToFloat(Color color)
        {
            return color.r / 10 +
                Mathf.FloorToInt(color.g * 100) +
                Mathf.FloorToInt(color.b * 100) * 1000;
        }
        
    }
}