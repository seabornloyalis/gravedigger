using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public float turnDelay = 0.1f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerHealth = 5;
	[HideInInspector] public bool playersTurn = true;

	private int level = 1;
	private List<Enemy> enemies;
	private List<Body> bodies;
	private bool enemiesMoving;

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
		InitGame ();
	
	}

	void InitGame()
	{
		enemies.Clear ();
		bodies.Clear ();
		boardScript.SetupScene(level);
	}

	public void Gameover()
	{
		enabled = false;
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
		if (playersTurn || enemiesMoving) 
		{
			return;
		}

		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add (script);
	}

	public void RemoveEnemyFromList(Enemy script)
	{
		enemies.Remove (script);
		CheckIfNextLevel ();
	}

	void CheckIfNextLevel()
	{
		if (enemies.Count == 0)
		{
			//do the thing
		}
	}
	

	public void AddBodyToList(Body script) {
		bodies.Add (script);
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
