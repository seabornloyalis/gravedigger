using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	struct ScoreListing {
		public string name;
		public int score;
	};


	public static ScoreManager instance = null;

	ScoreListing[] leaderboard = new ScoreListing[5];

	public string getName(int pos) {
		return leaderboard[pos].name;
	}

	public int getScore(int pos) {
		return leaderboard[pos].score;
	}

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad (gameObject);
		readFromFile ();
	}

	private void readFromFile () {
		string path = Application.persistentDataPath;
		string scoreBrd = "";
		if (File.Exists (path + "\\scores.txt")) {
			scoreBrd = System.IO.File.ReadAllText (path + "\\scores.txt");
		} else {
			File.WriteAllText(path + "\\scores.txt", " ");
		}
		int startIndex = 0;
		string[] subs = new string[5];
		for (int i =0; i < 5 && startIndex != -1 && scoreBrd.IndexOf ('\n') != -1; i++) {
			subs[i] = scoreBrd.Substring(startIndex, scoreBrd.IndexOf ('\n', startIndex) - startIndex);
			startIndex = scoreBrd.IndexOf ('\n', startIndex) + 1;
			leaderboard[i].name = subs[i].Substring(0, subs[i].IndexOf(' '));
			leaderboard[i].score = System.Convert.ToInt32(subs[i].Substring(subs[i].IndexOf(' '), subs[i].Length - subs[i].IndexOf(' ')));
		}
	}

	public bool removeAt (int index) {
		if (index > 4 || index < 0)
			return false;
		if (leaderboard [index].name == "")
			return false;
		for (int i = index; i < 4; i++) {
			leaderboard[i] = leaderboard[i+1];
		}
		leaderboard [4].name = "";
		leaderboard [4].score = 0;
		return true;
	}
	
	public int addNewScore (int newScore, string newName) {
		int placedIndex = -1;
		for (int i = 0; i < 5 && placedIndex == -1; i++) {
			if (newScore >= leaderboard[i].score) {
				placedIndex = i;
			}
		}
		if (placedIndex == -1)
			return -1;
		for (int j = 4; j >= placedIndex; j--) {
			if (j == placedIndex) {
				leaderboard [j].score = newScore;
				leaderboard [j].name = newName;
			} else
				leaderboard [j] = leaderboard [j-1];
		}
		saveFile ();
		return placedIndex;
	}

	private void saveFile() {
		string path = Application.persistentDataPath;
		string scoreData = "";
		for (int i = 0; i<5; i++) {
			scoreData = scoreData + leaderboard[i].name + " " + leaderboard[i].score + "\n";
		}
		File.WriteAllText(path + "\\scores.txt", scoreData);
	}
}
