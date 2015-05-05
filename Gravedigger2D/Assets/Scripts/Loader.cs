using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

	public GameObject gameManager;

	// Use this for initialization
	void Awake () 
	{
		if (GameManager.instance == null) {
			Instantiate (gameManager);
		}
	}

	/*void Update () {
		if (GameManager.instance != null && GameManager.instance.dead) {
			Debug.Log("Deaded");
			Object.Destroy(GameManager.instance.gameObject);
			GameManager.instance = null;
			Object.Destroy(GameObject.Find("Board"));
			Instantiate (gameManager);
			//Object.Destroy(GameObject.Find ("Player"));
			//Instantiate (GameObject.Find ("Player"), new Vector3 (0f, 0f, 0f), new Quaternion (0f,0f,0f,0f));
		}
	}*/
}
