﻿using UnityEngine;
using System.Collections;

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
		base.Start ();
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

		hitPlayer.LoseHealth (playerDamage);

	}

}
