using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Enemy : MovingObject {

	public int health = 2;
	public int playerDamage = 1;
	public GameObject body;

	private bool grapple = false;
	private Transform target;
	private bool skipMove;
	private AudioSource audioSrc;
	private Animator anim;


	protected override void Start () 
	{
		audioSrc = GetComponent<AudioSource> ();
		anim = GetComponent<Animator> ();
		GameManager.instance.AddEnemyToList (this);
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		base.Start ();

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
	}

	private void OnDisable()
	{
		GameManager.instance.RemoveEnemyFromList(this);
	}
	
	public void DamageEnemy(int loss)
	{
		health -= loss;
		grapple = true;
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
		RotateFacing (new Vector2 (xDir, yDir));
		base.AttemptMove<T> (xDir, yDir);
		anim.SetTrigger ("Start");
		
		skipMove = true;
	}

	public override void RotateFacing(Vector3 newFacing) {
		lookDir = newFacing;
		anim.SetBool("Up", false);
		anim.SetBool("Left", false);
		anim.SetBool("Down", false);
		anim.SetBool("Right", false);
		if (lookDir.y == 1.0f) {
			anim.SetBool("Up", true);
		} else if (lookDir.x == -1.0f) {
			anim.SetBool("Left", true);
		} else if (lookDir.y == -1.0f) {
			anim.SetBool("Down", true);
		} else if (lookDir.x == 1) {
			anim.SetBool("Right", true);
		}
		base.RotateFacing (newFacing);
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

		if (!grapple) {
			audioSrc.Play ();
			hitPlayer.LoseHealth (playerDamage * bonusMod);
		}
		grapple = false;
	}

}
