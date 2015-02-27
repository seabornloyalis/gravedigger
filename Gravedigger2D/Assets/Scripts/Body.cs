using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {
	private int turnsDormant;
	public GameObject enemy;

	void Start () {
		turnsDormant = 3;
		GameManager.instance.AddBodyToList (this);
	}

	void Update () {
		if (turnsDormant == 0) {
			gameObject.SetActive(false);
			Instantiate(enemy, transform.position, Quaternion.identity);
		}
	}

	public void reduceDormancy() {
		turnsDormant--;
	}
}
