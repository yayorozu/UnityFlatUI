using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
	[CustomEditor(typeof(FlatCircle))]
	public class FlatCircleEditor : Editor
	{
		private SerializedProperty _color;
		private SerializedProperty _raycast;

		private void OnEnable()
		{
			_color = serializedObject.FindProperty("m_Color");
			_raycast = serializedObject.FindProperty("m_RaycastTarget");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(_color);
			EditorGUILayout.PropertyField(_raycast);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
