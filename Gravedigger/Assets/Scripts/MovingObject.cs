using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider boxCollider;
	private Rigidbody rigBody;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start ()
	{
		boxCollider = GetComponent<BoxCollider> ();
		rigBody = GetComponent<Rigidbody> ();
		inverseMoveTime = 1f / moveTime;

	}

	protected bool Move(int xDir, int zDir, out RaycastHit hit)
	{
		Vector3 start = transform.position;
		Vector3 end = start + new Vector3 (xDir, 0, zDir); 

		boxCollider.enabled = false;
		bool isBlocked = Physics.Linecast (start, end, out hit, blockingLayer);
		boxCollider.enabled = true;

		if (isBlocked) {
			StartCoroutine (SmoothMovement (end));
			return true;
		}

		else return false;
	}

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > (float.Epsilon)) {
			Vector3 newPosition = Vector3.MoveTowards(rigBody.position, end, inverseMoveTime*Time.deltaTime);
			rigBody.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}

	protected virtual void AttemptMove<T> (int xDir, int zDir)
		where T : Component
	{
		RaycastHit hit;
		bool canMove = Move(xDir, zDir, out hit);

		if(hit.transform == null){
			return;
		}

		T hitComponent = hit.transform.GetComponent<T>();
		if(!canMove && hitComponent != null){
			OnCantMove(hitComponent);
		}
	}
	protected abstract void OnCantMove<T> (T component)
		where T : Component;
	
}
