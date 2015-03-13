using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

	public int attackDamage = 1;
	public int digPower = 1;
	public float restartLevelDelay = 6f;
	public float playerTurnDelay = 0.2f;
	public Text scoreText;
	public Text healthText;
	public Text carryText;
	public Text countText;

	public int health;
	public int digScore;
	public int lvlScore;
	public int count = 150;
	public GameObject hole;
	public GameObject gravemarker;

	private string isCarrying;

	// Use this for initialization
	protected override void Start ()
	{
		health = GameManager.instance.playerHealth;
		health += 3; //added because its hard to get further without healing
		digScore = GameManager.instance.playerScore;
		lvlScore = GameManager.instance.playerlvlScore;
		isCarrying = "None";
		carryText.text = "";
		healthText.text = "Health: " + health;
		scoreText.text = "Score: " + (lvlScore + digScore);
		countText.text = "Turns Left: " + count;
		lookDir = new Vector2 (1.0f, 0.0f);
		base.Start ();
	}

	private void OnDisable()
	{
		GameManager.instance.playerHealth = health;
		lvlScore += count;
		GameManager.instance.playerlvlScore = lvlScore;
		GameManager.instance.playerScore = digScore;
		string s = "Score Breakdown:\n Buried zombies: " + digScore + "\n Leveling up: " + lvlScore;
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

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
		{
			vertical = 0;
		} 
		if ((horizontal != 0 || vertical != 0) && Input.GetAxisRaw ("Rotate") == 1) {
			RotateFacing(new Vector2(horizontal, vertical));
		} else if (horizontal != 0 || vertical != 0) {
			AttemptMove<Enemy> (horizontal, vertical);
		} else  {
			Interact<Player>();
		}
	}

	protected override void AttemptMove<T> (int xDir, int yDir)
	{
		base.AttemptMove<T> (xDir, yDir);

		RotateFacing (new Vector2(xDir, yDir));
		count--;
		countText.text = "Turns Left: " + count;
		CheckIfGameOver ();
		GameManager.instance.playersTurn = false;
	}

	protected override void OnCantMove<T> (T component)
	{
		Enemy hitEnemy = component as Enemy;
		int bonusMod = 1;
		if (hitEnemy.lookDir == lookDir)
			bonusMod = 2;
		hitEnemy.DamageEnemy (attackDamage * bonusMod);
	}

	private void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseHealth(int loss)
	{
		health -= loss;
		healthText.text = "Health: " + health;
		CheckIfGameOver ();
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
		RaycastHit2D hit;
		Vector2 start = transform.position;
		Vector2 end = start + lookDir;
		bool noObstacle = InteractWithGround (end, out hit);
		bool acted = false;


		if(noObstacle && hit.transform == null && (Input.GetAxisRaw("Dig") != 0.0f))
		{
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

		if (hit.transform.tag == "Body" && isCarrying == "None" && (Input.GetAxisRaw ("Pick/put") != 0.0f)) {
			isCarrying = "Body";
			carryText.text = "You have a body";
			hit.transform.gameObject.SetActive (false);
			acted = true;
			count--;
			countText.text = "Turns Left: " + count;
			CheckIfGameOver ();
		} else if (hit.transform.tag == "Hole" && isCarrying == "Body" && (Input.GetAxisRaw ("Pick/put") != 0.0f)) {
			isCarrying = "None";
			carryText.text = "";
			digScore += 1;
			scoreText.text = "Score: " + (digScore + lvlScore);
			count--;
			countText.text = "Turns Left: " + count;
			CheckIfGameOver ();
			CheckIfNextLevel ();
			hit.transform.gameObject.SetActive (false);
			Instantiate (gravemarker, new Vector3 (end.x, end.y, 0f), Quaternion.identity);
			acted = true;
		} /*else if (hit.transform.tag == "Enemy" && Input.GetAxisRaw ("Attack") != 0f) {
			AttemptMove<Enemy> ((int) lookDir.x, (int) lookDir.y);
			acted = true;
		}*/
		if (acted) {
			GameManager.instance.playersTurn = false;
		}
	}

	private bool InteractWithGround(Vector2 end, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;

		BoxCollider2D boxCollider = GetComponent<BoxCollider2D> ();
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
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
}
