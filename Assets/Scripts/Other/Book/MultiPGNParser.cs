using System.IO;
using UnityEngine;

namespace Other.Book
{
	public class MultiPGNParser : MonoBehaviour {
		public TextAsset[] inputFiles;
		public TextAsset outputFile;
		public bool append;

		[ContextMenu ("Parse All")]
		private void ParseAll () {
			string allGames = "";
			foreach (var f in inputFiles) {
				allGames += Parse (f.text);
			}

			FileWriter.WriteToTextAsset_EditorOnly (outputFile, allGames, append);
		}

		private string Parse (string text) {

			bool isReadingPGN = false;
			string currentPgn = "";
			string parsedGames = "";

			StringReader reader = new StringReader (text);

			string line;
			while ((line = reader.ReadLine ()) != null) {
				if (line.Contains ("[")) {
					if (isReadingPGN) {
						isReadingPGN = false;
						parsedGames += currentPgn.Replace ("  ", " ").Trim () + '\n';
						currentPgn = "";
					}
					continue;
				} else {
					isReadingPGN = true;
					string[] moves = line.Split (' ');
					foreach (string move in moves) {
						string formattedMove = move;
						if (formattedMove.Contains (".")) {
							formattedMove = formattedMove.Split ('.') [1];
						}
						currentPgn += formattedMove.Trim () + " ";
					}
				}
			}

			return parsedGames;

		}
	}
}