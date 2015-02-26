using UnityEngine;
using System.Collections;

public class Player : MovingObject {

	public int attackDamage = 1;
	public int digPower = 1;
	public float restartLevelDelay = 1f;
	public float playerTurnDelay = 0.2f;

	public int health;

	// Use this for initialization
	protected override void Start ()
	{
		health = GameManager.instance.playerHealth;

		base.Start ();
	}

	private void OnDisable()
	{
		GameManager.instance.playerHealth = health;
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
		
		if (horizontal != 0 || vertical != 0)
		{
			AttemptMove<Enemy>(horizontal, vertical);

		}
	}

	protected override void AttemptMove<T> (int xDir, int yDir)
	{
		base.AttemptMove<T> (xDir, yDir);
		Debug.Log ("Attempt Player Move\n");

		RaycastHit2D hit;


		GameManager.instance.playersTurn = false;
	}

	protected override void OnCantMove<T> (T component)
	{
		Debug.Log ("HIT AN ENEMY");
		Enemy hitEnemy = component as Enemy;
		hitEnemy.DamageEnemy (attackDamage);
	}

	private void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseHealth(int loss)
	{
		health -= loss;
		CheckIfGameOver ();
	}

	private void CheckIfGameOver()
	{
		if (health <= 0) {
	 		GameManager.instance.Gameover ();
		}
	}
}
