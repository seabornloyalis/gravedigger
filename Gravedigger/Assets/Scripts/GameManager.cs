using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public float turnDelay = .1f;
	public static GameManager instance = null;
	private BoardManager boardScript;
	//public int playerHealth = 10;
	//stored values from the player between levels go here
	[HideInInspector] public bool playersTurn = true;

	private int level = 1;

	void Awake() {
		boardScript = GetComponent<BoardManager> ();

		InitGame ();
	}

	void InitGame() {
		boardScript.SetupScene (level);
	}
}
