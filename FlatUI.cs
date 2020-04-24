using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.FlatUI
{
	public class FlatUI : MaskableGraphic
	{
#if UNITY_EDITOR

		protected override void Reset()
		{
			if (m_Material == null)
				m_Material = Resources.Load<Material>("FlatUI/FlatUI");

			var control = GetComponent<FlatUIControl>();
			if (control == null)
				gameObject.AddComponent<FlatUIControl>();
		}
		
#endif
	}
}