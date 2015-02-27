using UnityEngine;
using System.Collections;

public class Player : MovingObject {

	public int attackDamage = 1;
	public int digPower = 1;
	public float restartLevelDelay = 1f;
	public float playerTurnDelay = 0.2f;

	public int health;
	public string isCarrying;
	public GameObject hole;

	// Use this for initialization
	protected override void Start ()
	{
		health = GameManager.instance.playerHealth;
		isCarrying = "None";
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
		int interactHoriz = 0;
		int interactVert = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		interactHoriz = (int)Input.GetAxisRaw ("Fire1");
		interactVert = (int)Input.GetAxisRaw ("Fire2");

		if (horizontal != 0)
		{
			vertical = 0;
		}
		if (interactHoriz != 0) {
			interactVert = 0;
		}
		
		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Enemy> (horizontal, vertical);

		} else if (interactHoriz != 0 || interactVert != 0) {
			Interact<Player>(interactHoriz, interactVert);
		}
	}

	protected override void AttemptMove<T> (int xDir, int yDir)
	{
		base.AttemptMove<T> (xDir, yDir);

		RaycastHit2D hit;


		GameManager.instance.playersTurn = false;
	}

	protected override void OnCantMove<T> (T component)
	{
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

	private void Interact<T>(int xDir, int yDir)
		where T : Component //Needed?
	{
		RaycastHit2D hit;
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		bool canDig = InteractWithGround (end, out hit);

		if(canDig && hit.transform == null)
		{
			Dig(end);
		}
		
		if(hit.transform == null)
		{
			Debug.Log("null hit");
			GameManager.instance.playersTurn = false;
			return;
		}

		//T hitComponent = hit.transform.GetComponent<T>();	
		Debug.Log (isCarrying);
		Debug.Log (hit.transform.tag);
		if (hit.transform.tag == "Body" && isCarrying == "None") {
			isCarrying = "Body";
			hit.transform.gameObject.SetActive(false);
		} else if (hit.transform.tag == "Hole" && isCarrying == "Body") {
			isCarrying = "None";
			hit.transform.gameObject.SetActive(false);
		}
		GameManager.instance.playersTurn = false;
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
