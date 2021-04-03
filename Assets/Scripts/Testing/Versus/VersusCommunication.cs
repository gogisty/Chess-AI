using System.IO;
using UnityEngine;

namespace Testing.Versus {
	public class VersusCommunication : MonoBehaviour {
		private const string folderName = "Communication";
		private const string playerFileExtention = ".player";
		private const string managerFileName = "Manager";
		private const string managerExtension = ".json";

		public event System.Action<PlayerInfo> onPlayerUpdated;
		public event System.Action<VersusInfo> onManagerUpdated;

		private FileSystemWatcher communicationWatcher;
		private bool communicationAlert;
		private System.IO.FileSystemEventArgs communicationArgs;

		private void Awake () {
			Directory.CreateDirectory (CommunicationPath);
			communicationWatcher = new FileSystemWatcher (Path.GetFullPath (CommunicationPath));
			communicationWatcher.NotifyFilter = NotifyFilters.LastWrite;
			communicationWatcher.Created += CommunicationFileChanged;
			communicationWatcher.Changed += CommunicationFileChanged;
			communicationWatcher.Filter = "*.*";
			communicationWatcher.EnableRaisingEvents = true;
		}

		private void Update () {
			if (communicationAlert) {
				communicationAlert = false;

				if (Path.GetFileNameWithoutExtension (communicationArgs.Name) == Path.GetFileNameWithoutExtension (ManagerFilePath)) {
					onManagerUpdated?.Invoke (ReadManagerFile ());
				}
				if (Path.GetExtension (communicationArgs.FullPath) == playerFileExtention) {
					var playerInfo = GetPlayerInfo (communicationArgs.FullPath);
					onPlayerUpdated?.Invoke (playerInfo);
				}
			}
		}

		private void CommunicationFileChanged (object sender, System.IO.FileSystemEventArgs e) {
			// Note that this is called from different thread than Unity main thread, so need to set a flag
			// to pick it up on main thread
			communicationArgs = e;
			communicationAlert = true;
		}

		public static void WriteManagerFile (VersusInfo info) {
			Write (JsonUtility.ToJson (info), ManagerFilePath);
		}

		public static VersusInfo ReadManagerFile () {
			string s = Read (ManagerFilePath);
			return JsonUtility.FromJson<VersusInfo> (s);
		}

		public static void CreateManagerFile () {
			Write ("", ManagerFilePath);
		}

		public static bool ManagerFileExists () {
			return File.Exists (ManagerFilePath);
		}

		public static string[] GetPlayerFiles () {
			return Directory.GetFiles (CommunicationPath, "*" + playerFileExtention);
		}

		private static string testData;

		public static PlayerInfo GetPlayerInfo (string path) {
			string data = Read (path);
			if (string.IsNullOrEmpty (data) || string.IsNullOrWhiteSpace (data)) {
				// Sometimes the result is empty, and reading it again fixes that.
				// Don't have energy to figure out why right now...
				data = Read (path);
			}
			return JsonUtility.FromJson<PlayerInfo> (data);
		}

		public static void WritePlayerInfo (PlayerInfo playerInfo) {
			string path = Path.Combine (CommunicationPath, "Player" + "_" + playerInfo.playerName + "_" + playerInfo.id + playerFileExtention);
			string data = JsonUtility.ToJson (playerInfo);
			Write (data, path);
		}

		private static string CommunicationPath {
			get {
				return Path.Combine (".", folderName);
			}
		}

		private static string ManagerFilePath {
			get {
				return Path.Combine (CommunicationPath, managerFileName + managerExtension);
			}
		}

		private static void Write (string data, string path) {
			StreamWriter writer = new StreamWriter (path);
			writer.Write (data);
			writer.Close ();
		}

		private static string Read (string path) {

			StreamReader reader = new StreamReader (path);
			string data = reader.ReadToEnd ();
			reader.Close ();
			return data;

		}
	}

	public struct VersusMatch {
		public string whitePlayerName;
		public string blackPlayerName;
	}
}