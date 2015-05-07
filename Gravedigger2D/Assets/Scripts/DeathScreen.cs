using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour {
	public Object scoreListing;
	public Object editScoreListing;

	string[] names = new string[5];
	int[] scores = new int[5];
	GameObject[] listings = new GameObject[5];
	int newScorePos;
	int yourScore;

	// Use this for initialization
	void Start () {
		yourScore = GameManager.instance.playerScore + GameManager.instance.playerlvlScore;
		GameObject showScore = GameObject.Find ("yourScoreText");
		showScore.GetComponent<Text>().text = "Your Score was: " + System.Convert.ToString (yourScore);
		newScorePos = ScoreManager.instance.addNewScore(yourScore, " ");
		if (newScorePos == -1) {
			GameObject editable = GameObject.Find("EditableScoreListing");
			Destroy(editable);
		}
		for (int i = 0; i < 5; i++) {
			names[i] = ScoreManager.instance.getName(i);
			scores[i] = ScoreManager.instance.getScore(i);
			if (i != newScorePos) {
				GameObject name = GameObject.Find("ScoreListing"+System.Convert.ToString (i)+"/NameText");
				name.GetComponent<Text>().text = names[i];
				GameObject score = GameObject.Find("ScoreListing"+System.Convert.ToString (i)+"/ScoreText");
				score.GetComponent<Text>().text = System.Convert.ToString (scores[i]);
			} else {
				GameObject editable = GameObject.Find("EditableScoreListing");
				editable.transform.localPosition = new Vector3(0, 50 - 20*i, 0);
				GameObject score = GameObject.Find("EditableScoreListing/Text");
				score.GetComponent<Text>().text = System.Convert.ToString (scores[i]);
				GameObject uneditable = GameObject.Find("ScoreListing"+System.Convert.ToString (i));
				Destroy(uneditable);
			}
		}
	}

	public void newText (string s) {
		GameObject editable = GameObject.Find("EditableScoreListing/InputField/Name");
		editable.GetComponent<Text> ().text = s;
	}

	public void submitName (string name) {
		ScoreManager.instance.removeAt (newScorePos);
		newScorePos = ScoreManager.instance.addNewScore(yourScore, name);
	}

	public void restartClicked() {
		if (GameManager.instance != null)
			GameObject.Destroy(GameManager.instance);
		Application.LoadLevel ("Gravedigger2d");
	}
}
