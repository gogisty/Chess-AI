using Core;
using Core.AI;
using TMPro;
using UnityEngine;

namespace Testing.Versus {
	public class VersusPlayer : MonoBehaviour {

		public string playerName;
		public AISettings aiSettings;

		public TMP_Text logUI;
		private PlayerInfo playerInfo;

		private bool playingAsWhite;
		private bool thinking;

		private Search search;
		private Move moveToMake;
		private float endThinkTime;
		private bool hasMove;

		private int gameNumber;
		private int myNextMovePlyCount;
		private Board board;

		private void Awake () {
			board = new Board ();
			ClearLog ();
			search = new Search (board, aiSettings);
			FindObjectOfType<VersusCommunication> ().onManagerUpdated += ManagerUpdated;

		}

		private void Start () {
			playerInfo = new PlayerInfo () { playerName = playerName, id = System.Environment.TickCount };
			VersusCommunication.WritePlayerInfo (playerInfo);
		}

		private void Update () {
			if (thinking && Time.time > endThinkTime && hasMove) {
				SubmitMove ();
			}
		}

		private void SubmitMove () {

			board.MakeMove (moveToMake);
			playerInfo.lastMove = moveToMake.Value;
			VersusCommunication.WritePlayerInfo (playerInfo);
			playerInfo.lastMovePly = myNextMovePlyCount;
			VersusCommunication.WritePlayerInfo (playerInfo);

			myNextMovePlyCount += 2;
			thinking = false;
			hasMove = false;

			Log ("Submitted Move (move = " + moveToMake.Value + ")");
		}

		private void StartThinking () {
			Log ("Started thinking... ply = " + myNextMovePlyCount);
			endThinkTime = Time.time + 1;
			hasMove = false;
			thinking = true;
			search.StartSearch ();
			(Move move, int eval) = search.GetSearchResult ();
			moveToMake = move;
			hasMove = true;
			//SubmitMove ();

		}

		private void ManagerUpdated (VersusInfo versusInfo) {
			Log (Time.time + " Manager updated");
			playingAsWhite = versusInfo.whiteID == playerInfo.id;
			if (versusInfo.gameInProgress) {
				// Start new game
				if (gameNumber != versusInfo.gameNumber) {
					myNextMovePlyCount = (playingAsWhite) ? 0 : 1;
					gameNumber = versusInfo.gameNumber;
					board.LoadStartPosition ();
					string colString = (playingAsWhite) ? "White" : "Black";
					Log ($"Game {gameNumber} started. Playing as {colString}");
				}

				// If it's my turn to move, and haven't already begun thinking, then start now!
				if (versusInfo.numPly == myNextMovePlyCount && !thinking) {
					Move opponentLastMove = new Move (versusInfo.lastMove);
					if (!opponentLastMove.IsInvalid) {
						board.MakeMove (opponentLastMove);
					}
					StartThinking ();
				}
			}
		}

		private void ClearLog () {
			logUI.text = "";
		}

		private void Log (string message) {
			logUI.text += message + "\n";
		}

	}

	public class PlayerInfo {
		public string playerName;
		public int id;

		public int lastMovePly;
		public ushort lastMove;
	}
}