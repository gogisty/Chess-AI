using Core;
using UI;
using UnityEngine;

namespace Other {
	public class GameViewer : MonoBehaviour {
		[Multiline]
		public string pgn;

		private Move[] gameMoves;
		private int moveIndex;
		private BoardUI boardUI;
		private Board board;

		private void Start () {
			gameMoves = PGNLoader.MovesFromPGN (pgn);
			board = new Board();
			board.LoadStartPosition ();
			boardUI = FindObjectOfType<BoardUI> ();
			boardUI.UpdatePosition (board);
		}

		private void Update () {
			if (Input.GetKeyDown (KeyCode.Space)) {
				if (moveIndex < gameMoves.Length) {
					board.MakeMove (gameMoves[moveIndex]);
					boardUI.OnMoveMade (board,gameMoves[moveIndex]);
					moveIndex++;
				}
			}
		}
	}
}