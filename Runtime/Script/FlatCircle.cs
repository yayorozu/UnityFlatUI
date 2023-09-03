using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
	public class FlatCircle : MaskableGraphic
	{
		
#if UNITY_EDITOR
		
		protected override void Reset()
		{
			base.Reset();
			if (m_Material == null)
				m_Material = Resources.Load<Material>("FlatUI/FlatCircle");
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			var graphic = GetComponent<Graphic>();
			graphic.SetVerticesDirty();
		}
		
#endif
		
	}
}