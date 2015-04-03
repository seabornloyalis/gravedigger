using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public float turnDelay = 0.05f;
	public float levelStartDelay = 2f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	//public CameraController cameraScript;
	public int playerHealth = 5;
	public int playerScore = 0;
	public int playerlvlScore = 0;
	public int numEnemies;
	public int numBodies;
	public string scoreBreakdown;

	[HideInInspector] public bool playersTurn = true;

	private int level = 1;
	private Text levelText;
	private Text ScoreBreakText;
	private GameObject levelImage;
	private GameObject tutImage;
	private List<Enemy> enemies;
	private List<Body> bodies;
	private bool enemiesMoving;
	private bool doingSetup;

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad (gameObject);
		enemies = new List<Enemy> ();
		bodies = new List<Body> ();
		boardScript = GetComponent<BoardManager> ();
		/// do the thingcameraScript = 
		InitGame ();
	
	}
	/*
	public void RestartButton()
	{
		level = 1;
		playerHealth = 5;
		playerScore = 0;
		playerlvlScore = 0;
		enemies.Clear ();
		bodies.Clear ();

		boardScript.SetupScene (level);
	}
	*/
	private void OnLevelWasLoaded(int index)
	{
		level++;

		InitGame ();
	}

	void InitGame()
	{
		doingSetup = true;

		levelImage = GameObject.Find ("LevelImage");
		tutImage = GameObject.Find ("TutorialImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		ScoreBreakText = GameObject.Find ("ScoreBreakText").GetComponent<Text> ();
		levelText.text = "" + level;
		ScoreBreakText.text = scoreBreakdown;
		levelImage.SetActive (true);
		//Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();
		bodies.Clear ();
		boardScript.SetupScene(level);
	}

	public void HideLevelImage()
	{
		levelImage.SetActive (false);
		ScoreBreakText.text = "";
		doingSetup = false;
	}

	public void Gameover()
	{
		ScoreBreakText.text = "Your score was " + playerScore;
		levelImage.SetActive (true);

		enabled = false;
	}

	public void TutorialButtonHelper()
	{
		doingSetup = true;
		tutImage.SetActive (true);
	}


	public void CloseTutButtonHelper()
	{
		tutImage.SetActive (false);
		doingSetup = false;
	}

	void Update()
	{
		if (playersTurn) {
			//Debug.Log ("Still player Turn\n");
		}
		if (enemiesMoving) {
			//Debug.Log ("Enemies Moving\n"); 
			//Debug.Log("There are " + enemies.Count + " enemies\n");
		}
		if (playersTurn || enemiesMoving || doingSetup) 
		{
			return;
		}

		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add (script);
		numEnemies = enemies.Count;
	}

	public void RemoveEnemyFromList(Enemy script)
	{
		enemies.Remove (script);
		numEnemies = enemies.Count;
	}

	public void AddBodyToList(Body script)
	{
		bodies.Add (script);
		numBodies = bodies.Count;
	}

	public void RemoveBodyFromList(Body script)
	{
		bodies.Remove (script);
		numBodies = bodies.Count;
		CheckIfNextLevel ();
	}

	public void CheckIfNextLevel()
	{
		if (enemies.Count == 0 && bodies.Count == 0)
		{
			//do the thing
		}
	}
		
	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) 
		{
			yield return new WaitForSeconds(turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++) 
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		for (int i = 0; i < bodies.Count; i++) {
			bodies[i].reduceDormancy();
		}
		playersTurn = true;
		enemiesMoving = false;
	}



}
