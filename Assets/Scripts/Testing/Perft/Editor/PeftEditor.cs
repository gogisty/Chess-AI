using UnityEditor;
using UnityEngine;

namespace Testing.Perft.Editor {
	[CustomEditor (typeof (Perft))]
	public class PerftEditor : UnityEditor.Editor {
		private Perft perft;

		public override void OnInspectorGUI () {
			DrawDefaultInspector ();
			GUILayout.Space (10);
			if (GUILayout.Button ("Run Single")) {
				perft.RunSingleTest ();
			}
		}

		private void OnEnable () {
			perft = (Perft) target;
		}
	}
}