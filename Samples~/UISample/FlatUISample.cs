using UnityEngine;
using UnityEngine.UI;

public class FlatUISample : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	private void Awake()
	{
		_button.onClick.AddListener(Click);
	}

	private void Click()
	{
		Debug.Log("Click");
	}
}
