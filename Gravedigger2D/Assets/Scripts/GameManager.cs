using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public float turnDelay = 0.5f;
	public float levelStartDelay = 2f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public AudioSource audioPlayer;
	public List<AudioClip> songs;
	public int playerHealth = 5;
	public int playerScore = 0;
	public int playerlvlScore = 0;
	public int zombieKills = 0;
	public int moveCount = 0;
	public int numEnemies;
	public int numBodies;
	public string scoreBreakdown;
	public bool passedLvl = false;
	public bool showingLevel = false;

	[HideInInspector] public bool playersTurn = true;

	private int level = 1;
	private Text levelText;
	private Text scoreBreakText;
	private Text zombieCountText;
	private Text moveBreakText;
	private GameObject levelImage;
	private GameObject tutImage;
	private Text tutText; // to be removed when controls finalized
	private List<Enemy> enemies;
	private List<Body> bodies;
	private bool enemiesMoving;
	private bool doingSetup;
	private int currSongIndex = 0;

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
		audioPlayer = GetComponent<AudioSource> ();
		audioPlayer.clip = songs [0];
		audioPlayer.Play();
		InitGame ();
	
	}
	
	private void OnLevelWasLoaded(int index)
	{
		if (passedLvl) {
			level++;
			InitGame ();
		}
	}

	void InitGame()
	{
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		tutImage = GameObject.Find ("TutorialImage");
		tutText = GameObject.Find ("TutText").GetComponent<Text>();
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		scoreBreakText = GameObject.Find ("ScoreBreakText").GetComponent<Text> ();
		zombieCountText = GameObject.Find ("ZombieCountText").GetComponent<Text>();
		moveBreakText = GameObject.Find ("MoveBreakText").GetComponent<Text>();
		GameObject.Find ("Quote").GetComponent<Text> ().text = QuoteManager.instance.randLevelQuote ();
		levelText.text = "" + level;
		zombieCountText.text = "" + zombieKills;
		moveBreakText.text = "" + moveCount;
		scoreBreakText.text = scoreBreakdown;
		levelImage.SetActive (true);
		enemies.Clear ();
		bodies.Clear ();
		boardScript.SetupScene(level);
		passedLvl = false;
		showingLevel = false;
	}

	public void HideLevelImage()
	{
		levelImage.SetActive (false);
		scoreBreakText.text = "";
		doingSetup = false;
	}

	public void Gameover()
	{
		scoreBreakText.text = "Your score was " + playerScore;
		levelImage.SetActive (true);

		enabled = false;
		Application.LoadLevel ("DeathScreen");
		//dead = true;
		//this.Destroy (this, 0.0f);
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

	void Update()
	{
		if (!showingLevel) {
			if(Input.anyKey) {
				GameObject.Find("Player").GetComponent<Player>().ContinueButton();
				GameObject.Find("Player").GetComponent<Player>().CloseTutButton();
				Input.ResetInputAxes();
				return;
			}
		}
		if (audioPlayer.isPlaying == false) {
			audioPlayer.clip = songs[(currSongIndex + 1) % songs.Count];
			audioPlayer.Play();
			currSongIndex++;
		}
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
