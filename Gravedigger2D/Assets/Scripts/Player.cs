﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

	public int attackDamage = 1;
	public int digPower = 1;
	public float restartLevelDelay = 6f;
	public float playerTurnDelay = 0.2f;
	public float damageTime = 0.1f;
	public Text scoreText;
	public Text healthText;
	public Text carryText;
	public Text countText;
	public AudioClip attackClip;
	public AudioClip grunt;
	public AudioClip shovelling;

	public int health;
	public int digScore;
	public int lvlScore;
	public int count = 150;
	public GameObject hole;
	public GameObject gravemarker;

	private string isCarrying;
	private GameObject damageImage;
	private AudioSource audioSrc;
	private bool checkingMove;

	// Use this for initialization
	protected override void Start ()
	{
		checkingMove = false;
		audioSrc = GetComponent<AudioSource> ();
		health = GameManager.instance.playerHealth;
		digScore = GameManager.instance.playerScore;
		damageImage = GameObject.Find ("DamageImage");
		damageImage.SetActive (false);
		lvlScore = GameManager.instance.playerlvlScore;
		isCarrying = "None";
		carryText.text = "";
		healthText.text = "Health: " + health;
		scoreText.text = "Score: " + (lvlScore + digScore);
		countText.text = "Turns Left: " + count;
		lookDir = new Vector3 (1.0f, 0.0f, 0f);
		base.Start ();
	}

	public void ContinueButton()
	{
		GameManager.instance.HideLevelImage ();
	}

	public void TutorialButton()
	{
		GameManager.instance.TutorialButtonHelper ();
	}
	
	public void CloseTutButton()
	{
		GameManager.instance.CloseTutButtonHelper ();
	}
	// Will be removed when images/controls finalized
	public void TutKeyboard()
	{
		GameManager.instance.TutKeyboardHelper ();
	}
	
	public void TutXbox()
	{
		GameManager.instance.TutXboxHelper ();
	}

	public void TutPlayStation()
	{
		GameManager.instance.TutPlayStationHelper ();
	}

	private void OnDisable()
	{
		GameManager.instance.playerHealth = health;
		lvlScore += count;
		GameManager.instance.playerlvlScore = lvlScore;
		GameManager.instance.playerScore = digScore;
		int score = digScore+lvlScore;
		string s = ""+score;
		GameManager.instance.scoreBreakdown = s;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!GameManager.instance.playersTurn)
		{
			return;
		}

		int horizontal = 0;
		int vertical = 0;
		float rotateHoriz = 0; //for joystick only
		float rotateVert = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");
		rotateHoriz = Input.GetAxisRaw ("RotateX");
		rotateVert = Input.GetAxisRaw ("RotateY");

		if (horizontal != 0)
		{
			vertical = 0;
		}

		if ((int) rotateHoriz != 0) {
			if (Mathf.Abs(rotateHoriz) > Mathf.Abs(rotateVert))
				rotateVert = 0;
			else
				rotateHoriz = 0;
		} 
		if ((int) rotateHoriz != 0 || (int) rotateVert != 0) {
			RotateFacing(new Vector2((int) rotateHoriz, (int) rotateVert));
		} else if (horizontal != 0 || vertical != 0) {
			AttemptMove<Enemy> (horizontal, vertical);
		} else  {
			Interact<Player>();
		}
	}

	protected override void AttemptMove<T> (int xDir, int yDir)
	{

		RotateFacing (new Vector2(xDir, yDir));

		if (!checkingMove) {
			checkingMove = true;
			StartCoroutine (MoveHelper (xDir, yDir));
		}
	}

	private IEnumerator MoveHelper(int xDir, int yDir) {
			yield return new WaitForSeconds (0.15f);
			int horizontal = (int)Input.GetAxisRaw ("Horizontal");
			int vertical = (int)Input.GetAxisRaw ("Vertical");

<<<<<<< HEAD
		if ((horizontal == xDir && xDir != 0) || (vertical == yDir && yDir != 0)) {
			base.AttemptMove<Enemy> (xDir, yDir);
			count--;
			countText.text = "Turns Left: " + count;
			CheckIfGameOver ();
			GameManager.instance.playersTurn = false;
		}
		checkingMove = false;
=======
			if ((horizontal == xDir && xDir != 0) || (vertical == yDir && yDir != 0)) {
				base.AttemptMove<Player> (xDir, yDir);
				count--;
				countText.text = "Turns Left: " + count;
				CheckIfGameOver ();
				GameManager.instance.playersTurn = false;
			}
			checkingMove = false;
>>>>>>> 25236d1ba76b90dbafe64205a7fdc2d0b5d73874
	}

	protected override void OnCantMove<T> (T component)
	{
		Enemy hitEnemy = component as Enemy;
		int bonusMod = 1;
		if (hitEnemy.lookDir == lookDir)
			bonusMod = 2;
		hitEnemy.DamageEnemy (attackDamage * bonusMod);
		audioSrc.clip = attackClip;
		audioSrc.Play ();
	}

	private void Restart()
	{
		//Debug.Log ("Loadedlevel was: " + Application.loadedLevel + "\n");
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseHealth(int loss)
	{
		health -= loss;
		healthText.text = "Health: " + health;
		damageImage.SetActive (true);
		Invoke ("DisplayDamage", damageTime);
		CheckIfGameOver ();
	}

	private void DisplayDamage()
	{
			damageImage.SetActive(false);
	}

	private void CheckIfGameOver()
	{
		if (health <= 0 || count <= 0) {
			GameManager.instance.playerScore = digScore;
			GameManager.instance.playerlvlScore = lvlScore;
	 		GameManager.instance.Gameover ();
		}
	}

	private void CheckIfNextLevel()
	{
		if (GameManager.instance.numEnemies == 0 
		    && GameManager.instance.numBodies == 0) {
			Invoke("Restart", restartLevelDelay);
		}
	}

	private void Interact<T>()
		where T : Component //Needed?
	{
		RaycastHit hit;
		Vector3 start = transform.position;
		Vector3 end = start + lookDir;
		bool noObstacle = InteractWithGround (end, out hit);
		bool acted = false;


		if(noObstacle && hit.transform == null && (Input.GetAxisRaw("Dig") != 0.0f))
		{
			audioSrc.clip = shovelling;
			audioSrc.Play();
			Dig(end);
			acted = true;
			count--;
			countText.text = "Turns Left: " + count;
			CheckIfGameOver();
			GameManager.instance.playersTurn = false;
		} 
		if(hit.transform == null)
		{
			return;
		}

		if (hit.transform.tag == "Enemy" && (Input.GetAxisRaw ("Attack") != 0.0f)) {
			Attack<Enemy>();
			acted = true;
		}
		if (hit.transform.tag == "Body" && isCarrying == "None" && (Input.GetAxisRaw ("Pick/put") != 0.0f)) {
			audioSrc.clip = grunt;
			audioSrc.Play();
			isCarrying = "Body" + hit.transform.gameObject.GetComponent<Body>().bodyTypeID;
			carryText.text = "You have a body";
			GameManager.instance.zombieKills += 1;
			hit.transform.gameObject.SetActive (false);
			acted = true;
			count--;
			countText.text = "Turns Left: " + count;
			CheckIfGameOver ();
		} else if (hit.transform.tag == "Hole" && isCarrying.Contains("Body") && (Input.GetAxisRaw ("Pick/put") != 0.0f)) {
			carryText.text = "";
			digScore += (6 - int.Parse(isCarrying[4].ToString())) * 10;
			scoreText.text = "Score: " + (digScore + lvlScore);
			count--;
			countText.text = "Turns Left: " + count;
			CheckIfGameOver ();
			CheckIfNextLevel ();
			hit.transform.gameObject.SetActive (false);
			Instantiate (gravemarker, new Vector3 (end.x, end.y, 0f), Quaternion.identity);
			isCarrying = "None";
			acted = true;
		}
		if (acted) {
			GameManager.instance.moveCount += 1;
			GameManager.instance.playersTurn = false;
		}
	}

	private bool InteractWithGround(Vector3 end, out RaycastHit hit)
	{
		Vector3 start = new Vector3 (transform.position.x, transform.position.y, 0);

		BoxCollider boxCollider = GetComponent<BoxCollider> ();
		boxCollider.enabled = false;
		Physics.Linecast (start, end, out hit, blockingLayer);
		boxCollider.enabled = true;
		
		if (hit.transform == null) 
		{
			return true;
		}
		return false;
	}

	private void Dig (Vector2 digLoc) {
		Instantiate(hole, new Vector3(digLoc.x, digLoc.y, 0f), Quaternion.identity);
	}

	private void Attack<T> () 
		where T : Component
	{
		RaycastHit2D hit;
		bool canMove = Move ((int)lookDir.x, (int)lookDir.y, out hit);
		
		if(hit.transform == null)
		{
			return;
		}
		T hitComponent = hit.transform.GetComponent<T>();
		
		if(!canMove && hitComponent != null)
		{
			OnCantMove(hitComponent);
		}
	}
}
