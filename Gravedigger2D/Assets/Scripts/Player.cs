using UnityEngine;
using System.Collections;

public class Player : MovingObject {

	public int attackDamage = 1;
	public int digPower = 1;
	public float restartLevelDelay = 1f;
	public float playerTurnDelay = 0.2f;

	public int health;
	public GameObject hole;

	private string isCarrying;
	private Vector2 lookDir;

	// Use this for initialization
	protected override void Start ()
	{
		health = GameManager.instance.playerHealth;
		isCarrying = "None";
		lookDir = new Vector2 (1.0f, 0.0f);
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
			GameManager.instance.playersTurn = false;
		} 
		if(hit.transform == null)
		{
			//Debug.Log("null hit");
			//GameManager.instance.playersTurn = false;
			return;
		}

		//T hitComponent = hit.transform.GetComponent<T>();	
		//Debug.Log (isCarrying);
		//Debug.Log (hit.transform.tag);

		if (hit.transform.tag == "Body" && isCarrying == "None" && (Input.GetAxisRaw("Pick/put") != 0.0f)) {
			isCarrying = "Body";
			hit.transform.gameObject.SetActive(false);
			acted = true;
		} else if (hit.transform.tag == "Hole" && isCarrying == "Body" && (Input.GetAxisRaw("Pick/put") != 0.0f)) {
			isCarrying = "None";
			hit.transform.gameObject.SetActive(false);
			acted = true;
		}
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

	private void RotateFacing (Vector2 newFacing) {
		lookDir = newFacing;

		while (transform.rotation.z > 0.0f) {
			transform.Rotate (0.0f, 0.0f, -90.0f);
		}
		if (lookDir.y == 1.0f)
			transform.Rotate (0.0f, 0.0f, 90.0f);
		else if (lookDir.x == -1.0f) {
			transform.Rotate (0.0f, 0.0f, 90.0f);
			transform.Rotate(0.0f, 0.0f, 90.0f);
		}else if (lookDir.y == -1.0f)
			transform.Rotate(0.0f, 0.0f, 270.0f);

	}
}
