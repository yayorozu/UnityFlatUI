using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
	[CustomEditor(typeof(FlatCircleGauge))]
	public class FlatCircleGaugeEditor : Editor
	{
		private SerializedProperty _color;
		private SerializedProperty _raycast;
		private SerializedProperty _width;
		private SerializedProperty _startAngle;
		private SerializedProperty _fillAmount;
		private SerializedProperty _isReverse;
		
		private void OnEnable()
		{
			_color = serializedObject.FindProperty("m_Color");
			_raycast = serializedObject.FindProperty("m_RaycastTarget");
			_width = serializedObject.FindProperty("_width");
			_startAngle = serializedObject.FindProperty("_startAngle");
			_fillAmount = serializedObject.FindProperty("_fillAmount");
			_isReverse = serializedObject.FindProperty("_isReverse");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(_color);
			EditorGUILayout.PropertyField(_raycast);
			EditorGUILayout.PropertyField(_width);
			EditorGUILayout.PropertyField(_startAngle);
			EditorGUILayout.PropertyField(_fillAmount);
			EditorGUILayout.PropertyField(_isReverse);

			serializedObject.ApplyModifiedProperties();
		}
	}
}