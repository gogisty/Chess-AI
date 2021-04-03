using UI;
using UnityEngine;

namespace Core {
	public class HumanPlayer : Player {

		public enum InputState {
			None,
			PieceSelected,
			DraggingPiece
		}

		private InputState currentState;

		private BoardUI boardUI;
		private Camera cam;
		private Coord selectedPieceSquare;
		private Board board;
		public HumanPlayer (Board board) {
			boardUI = GameObject.FindObjectOfType<BoardUI> ();
			cam = Camera.main;
			this.board = board;
		}

		public override void NotifyTurnToMove () {

		}

		public override void Update () {
			HandleInput ();
		}

		private void HandleInput () {
			Vector2 mousePos = cam.ScreenToWorldPoint (Input.mousePosition);

			if (currentState == InputState.None) {
				HandlePieceSelection (mousePos);
			} else if (currentState == InputState.DraggingPiece) {
				HandleDragMovement (mousePos);
			} else if (currentState == InputState.PieceSelected) {
				HandlePointAndClickMovement (mousePos);
			}

			if (Input.GetMouseButtonDown (1)) {
				CancelPieceSelection ();
			}
		}

		private void HandlePointAndClickMovement (Vector2 mousePos) {
			if (Input.GetMouseButton (0)) {
				HandlePiecePlacement (mousePos);
			}
		}

		private void HandleDragMovement (Vector2 mousePos) {
			boardUI.DragPiece (selectedPieceSquare, mousePos);
			// If mouse is released, then try place the piece
			if (Input.GetMouseButtonUp (0)) {
				HandlePiecePlacement (mousePos);
			}
		}

		private void HandlePiecePlacement (Vector2 mousePos) {
			Coord targetSquare;
			if (boardUI.TryGetSquareUnderMouse (mousePos, out targetSquare)) {
				if (targetSquare.Equals (selectedPieceSquare)) {
					boardUI.ResetPiecePosition (selectedPieceSquare);
					if (currentState == InputState.DraggingPiece) {
						currentState = InputState.PieceSelected;
					} else {
						currentState = InputState.None;
						boardUI.DeselectSquare (selectedPieceSquare);
					}
				} else {
					int targetIndex = BoardRepresentation.IndexFromCoord (targetSquare.fileIndex, targetSquare.rankIndex);
					if (Piece.IsColour (board.Square[targetIndex], board.ColourToMove) && board.Square[targetIndex] != 0) {
						CancelPieceSelection ();
						HandlePieceSelection (mousePos);
					} else {
						TryMakeMove (selectedPieceSquare, targetSquare);
					}
				}
			} else {
				CancelPieceSelection ();
			}

		}

		private void CancelPieceSelection () {
			if (currentState != InputState.None) {
				currentState = InputState.None;
				boardUI.DeselectSquare (selectedPieceSquare);
				boardUI.ResetPiecePosition (selectedPieceSquare);
			}
		}

		private void TryMakeMove (Coord startSquare, Coord targetSquare) {
			int startIndex = BoardRepresentation.IndexFromCoord (startSquare);
			int targetIndex = BoardRepresentation.IndexFromCoord (targetSquare);
			bool moveIsLegal = false;
			Move chosenMove = new Move ();

			MoveGenerator moveGenerator = new MoveGenerator ();
			bool wantsKnightPromotion = Input.GetKey (KeyCode.LeftAlt);

			var legalMoves = moveGenerator.GenerateMoves (board);
			for (int i = 0; i < legalMoves.Count; i++) {
				var legalMove = legalMoves[i];

				if (legalMove.StartSquare == startIndex && legalMove.TargetSquare == targetIndex) {
					if (legalMove.IsPromotion) {
						if (legalMove.MoveFlag == Move.Flag.PromoteToQueen && wantsKnightPromotion) {
							continue;
						}
						if (legalMove.MoveFlag != Move.Flag.PromoteToQueen && !wantsKnightPromotion) {
							continue;
						}
					}
					moveIsLegal = true;
					chosenMove = legalMove;
					//	Debug.Log (legalMove.PromotionPieceType);
					break;
				}
			}

			if (moveIsLegal) {
				ChoseMove (chosenMove);
				currentState = InputState.None;
			} else {
				CancelPieceSelection ();
			}
		}

		private void HandlePieceSelection (Vector2 mousePos) {
			if (Input.GetMouseButtonDown (0)) {
				if (boardUI.TryGetSquareUnderMouse (mousePos, out selectedPieceSquare)) {
					int index = BoardRepresentation.IndexFromCoord (selectedPieceSquare);
					// If square contains a piece, select that piece for dragging
					if (Piece.IsColour (board.Square[index], board.ColourToMove)) {
						boardUI.HighlightLegalMoves (board, selectedPieceSquare);
						boardUI.SelectSquare (selectedPieceSquare);
						currentState = InputState.DraggingPiece;
					}
				}
			}
		}
	}
}