using UnityEngine;
using System.Collections;

public class Player : MovingObject 
{

	public int digPower = 1;
	public int attack = 1;

	public int health = 10; // will be changed to private after tweaked
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();
	}

	private void OnDisable()
	{
		//will be used to save data between levels
	}

	// Update is called once per frame
	void Update ()
	{
		if (!GameManager.instance.playersTurn) {
			return;
		}

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0) {
			vertical = 0;
		}
		//May need to rework this to determine what objects would be collided with
		// either obstacle or enemy
		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Enemy>(horizontal, vertical);
		}
	}

	protected override void AttemptMove<T> (int xDir, int zDir)
	{
		base.AttemptMove<T> (xDir, zDir);

		RaycastHit hit;

		if (Move (xDir, zDir, out hit)) {
		
		}

		GameManager.instance.playersTurn = false;
	}

	private void OnTrigerEnter(Collider other)
	{
		//CODE HERE FOR INTERACTING WITH OBJECTS
	}

	protected override void OnCantMove<T> (T component)
	{
		Enemy hitEnemy = component as Enemy;
		health -= hitEnemy.damage;
		CheckIfGameOver ();
		hitEnemy.attacked(attack);
		//MAYBE CAN USE THE "as" OPERATOR TO HANDLE MULTIPLE CASES
	}

	private void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	private void CheckIfGameOver()
	{
		if (health <= 0) {
			//GameManager.instance.GameOver ();
		}
	}
}
