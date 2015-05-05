using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour {
	// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			if (GameManager.instance != null)
				GameObject.Destroy(GameManager.instance);
			Application.LoadLevel ("Gravedigger2d");
		}
	}
}
