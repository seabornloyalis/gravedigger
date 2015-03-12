using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Enemy : MovingObject {

	public int health = 2;
	public int playerDamage = 1;
	public GameObject body;

	private Transform target;
	private bool skipMove;

	protected override void Start () 
	{
		GameManager.instance.AddEnemyToList (this);
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		int randDir =(int) Random.Range (1f, 4f);
		switch (randDir) {
		case 1:
			lookDir = new Vector2 (0f, 1f);
			RotateFacing(lookDir);
			break;

		case 2:
			lookDir = new Vector2 (0f, -1f);
			RotateFacing(lookDir);
			break;

		case 3:
			lookDir = new Vector2 (-1f, 0f);
			RotateFacing(lookDir);
			break;

		default:
			lookDir = new Vector2 (1f, 0f);
			break;
		}

		base.Start ();
	}

	private void OnDisable()
	{
		GameManager.instance.RemoveEnemyFromList(this);
	}
	
	public void DamageEnemy(int loss)
	{
		health -= loss;
		if (health <= 0) {
			gameObject.SetActive(false);
			Instantiate(body, transform.position, Quaternion.identity);
		}
	}

	protected override void AttemptMove<T> (int xDir, int yDir)
	{
		if (skipMove)
		{
			skipMove = false;
			return;
		}

		base.AttemptMove<T> (xDir, yDir);

		RotateFacing (new Vector2 (xDir, yDir));

		skipMove = true;
	}

	public void MoveEnemy()
	{
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
			yDir = target.position.y > transform.position.y ? 1 : -1;
		} else {
			xDir = target.position.x > transform.position.x ? 1 : -1;
		}

		AttemptMove<Player> (xDir, yDir);
	}

	protected override void OnCantMove<T> (T component)
	{
		Player hitPlayer = component as Player;
		int bonusMod = 1; //Multiplier for bonus dam
		if (hitPlayer.lookDir == lookDir) //if hitting from behind
			bonusMod = 2;
		hitPlayer.LoseHealth (playerDamage * bonusMod);

	}

}
