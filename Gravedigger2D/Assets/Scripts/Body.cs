﻿using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {
	private int turnsDormant;
	public GameObject enemy;
	public int bodyTypeID;

	void Start () {
		turnsDormant = 8;
		GameManager.instance.AddBodyToList (this);
	}

	void Update () {
		if (turnsDormant == 0) {
			gameObject.SetActive(false);
			Instantiate(enemy, transform.position, Quaternion.identity);
		}
	}

	void OnDisable()
	{
		GameManager.instance.RemoveBodyFromList (this);
	}


	public void reduceDormancy() {
		turnsDormant--;
	}
}
