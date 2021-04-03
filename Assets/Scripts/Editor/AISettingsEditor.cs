using Core.AI;
using UnityEditor;
using UnityEngine;

namespace Editor {
	[CustomEditor (typeof (AISettings))]
	public class AISettingsEditor : UnityEditor.Editor {

		public override void OnInspectorGUI () {
			DrawDefaultInspector ();

			AISettings settings = target as AISettings;

			if (settings.useThreading) {
				if (GUILayout.Button ("Abort Search")) {
					settings.RequestAbortSearch ();
				}
			}
		}
	}

}