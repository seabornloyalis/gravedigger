using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {
	private GameObject tutImage;
	private Text tutText; // to be removed when controls finalized

	void Start () {
		clearCredits ();
		tutImage = GameObject.Find ("TutorialImage");
		tutText = GameObject.Find ("TutText").GetComponent<Text> ();
		tutImage.SetActive (false);
	}

	public void playButton() {
		if (GameManager.instance != null)
			GameObject.Destroy(GameManager.instance);
		Application.LoadLevel ("Gravedigger2d");
	}

	public void TutorialButtonHelper()
	{
		tutImage.SetActive (true);
	}
	
	
	public void CloseTutButtonHelper()
	{
		tutImage.SetActive (false);
	}
	// to be removed on control finalization
	public void TutKeyboardHelper()
	{
		tutText.text = "Controls:\nArrow Keys to Rotate\nWASD to move\nSpace bar to attack\nH to dig a hole\nB to pick up a body\nB to place body in a hole";
	}
	public void TutXboxHelper()
	{
		tutText.text = "Controls:\nLeft Joystick to move\nX to pickup a body\nA to dig a hole\nB to attack";
	}
	public void TutPlayStationHelper()
	{
		tutText.text = "Controls:\nRotate with the Analog Stick\nLeft Joystick to move\nX to pickup a body\nA to dig a hole\nB to attack";
	}

	public void rollCredits() {
		GameObject.Find ("CreditScreen").GetComponent<Image> ().enabled = true;
		GameObject.Find ("CreditScreen/Crew").GetComponent<Text> ().enabled = true;
		Invoke ("clearCredits", 5f);
	}

	void clearCredits() {
		GameObject.Find ("CreditScreen").GetComponent<Image> ().enabled = false;
		GameObject.Find ("CreditScreen/Crew").GetComponent<Text> ().enabled = false;
	}

	/*protected IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		
		while (sqrRemainingDistance > float.Epsilon) 
		{
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, 5*Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}*/
}
