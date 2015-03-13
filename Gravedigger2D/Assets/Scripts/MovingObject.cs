using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;
	public Sprite front;
	public Sprite back;
	public Sprite leftSide;
	public Sprite rightSide;
	public Vector2 lookDir;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;
	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	protected virtual void Start ()
	{
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		inverseMoveTime = 1f / moveTime;
	}

	protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) 
		{
			StartCoroutine(SmoothMovement (end));
			return true;
		}

		return false;
	}

	protected IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon) 
		{
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime*Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}

	protected virtual void AttemptMove<T> (int xDir, int yDir)
		where T : Component
	{
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);

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

	protected abstract void OnCantMove<T> (T component)
		where T : Component;

	public void RotateFacing (Vector2 newFacing) {
		lookDir = newFacing;
		
		if (back == null || leftSide == null || rightSide == null) {
			while (transform.rotation.z > 0.0f) {
				transform.Rotate (0.0f, 0.0f, -90.0f);
			}
		}
		if (lookDir.y == 1.0f) {
			if (back != null)
				spriteRenderer.sprite = back;
			else
				transform.Rotate (0.0f, 0.0f, 90.0f);
		} else if (lookDir.x == -1.0f) {
			if (leftSide != null)
				spriteRenderer.sprite = leftSide;
			else {
				transform.Rotate (0.0f, 0.0f, 90.0f);
				transform.Rotate (0.0f, 0.0f, 90.0f);
			}
		} else if (lookDir.y == -1.0f) {
			/*if (front != null)
				spriteRenderer.sprite = front;
			else
			 //Commented out until other facings implemented*/ 
				transform.Rotate (0.0f, 0.0f, 270.0f);
		} else if (lookDir.x == 1 && rightSide != null) {
			spriteRenderer.sprite = rightSide;
		}
	}
}
