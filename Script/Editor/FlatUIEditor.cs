using UnityEditor;

namespace Yorozu.FlatUI.Tool
{
	[CustomEditor(typeof(FlatUI))]
	public class FlatUIEditor : Editor
	{
		private SerializedProperty _color;
		private SerializedProperty _raycast;
		private SerializedProperty _flags;
		private SerializedProperty _radius;
		private SerializedProperty _isValidOutline;
		private SerializedProperty _outline;
		private SerializedProperty _outlineColor;
		
		private void OnEnable()
		{
			_color = serializedObject.FindProperty("m_Color");
			_raycast = serializedObject.FindProperty("m_RaycastTarget");
			_flags = serializedObject.FindProperty("_flags");
			_radius = serializedObject.FindProperty("_radius");
			_isValidOutline = serializedObject.FindProperty("_isValidOutline");
			_outline = serializedObject.FindProperty("_outline");
			_outlineColor = serializedObject.FindProperty("_outlineColor");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(_color);
			EditorGUILayout.PropertyField(_raycast);
			EditorGUILayout.PropertyField(_flags);
			EditorGUILayout.PropertyField(_radius);
			EditorGUILayout.PropertyField(_isValidOutline);
			if (_isValidOutline.boolValue)
			{
				EditorGUILayout.PropertyField(_outline);
				EditorGUILayout.PropertyField(_outlineColor);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}