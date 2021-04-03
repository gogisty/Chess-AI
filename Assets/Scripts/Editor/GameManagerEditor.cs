using Core;
using UnityEditor;
using UnityEngine;

namespace Editor {
	[CustomEditor (typeof (GameManager))]
	public class GameManagerEditor : UnityEditor.Editor {
		private UnityEditor.Editor aiSettingsEditor;

		public override void OnInspectorGUI () {
			base.OnInspectorGUI ();
			var manager = target as GameManager;

			bool foldout = true;
			DrawSettingsEditor (manager.aiSettings, ref foldout, ref aiSettingsEditor);
		}

		private void DrawSettingsEditor (Object settings, ref bool foldout, ref UnityEditor.Editor editor) {
			if (settings != null) {
				foldout = EditorGUILayout.InspectorTitlebar (foldout, settings);
				if (foldout) {
					CreateCachedEditor (settings, null, ref editor);
					editor.OnInspectorGUI ();
				}
			}
		}

	}
}