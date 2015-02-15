using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	const int BASE_COLS = 8;
	const int BASE_ROWS = 8;

	public int columns; //Should make alterable
	public int rows;
	public int maxDepth = -1;
	public Count obstacleCount;
	public Count enemyCount;
	public GameObject[] floorBlocks;
	public GameObject[] subfloorBlocks;
	public GameObject[] obstacles;
	public GameObject boundingBlock;
	//public GameObject[] enemyBlocks;

	private Transform boardHolder;
	private List <Vector3> gridPositions = new List<Vector3> ();

	void InitLevelParams(int level) {
		columns = level - 1 + BASE_COLS;
		rows = level - 1 + BASE_ROWS;

		obstacleCount = new Count (3 + level, 5 + level);
		//This will need tweaking

		//enemyCount = SOMETHING
	}
	
	void InitialiseList()
	{
		gridPositions.Clear ();

		for (int x = 1; x < columns -1; x++) {
			for (int z = 0; z < rows -1; z++) {
				gridPositions.Add (new Vector3 (x, -1.0f, z));
				gridPositions.Add (new Vector3 (x, 0.0f, z));
				gridPositions.Add (new Vector3 (x, 1.0f, z));
			}
		}
	}

	void BoardSetup() {
		boardHolder = new GameObject ("Board").transform;

		for (int x = -1; x < columns + 1; x++) {
			for (int z = -1; z < rows +1; z++) {
				for (int y = maxDepth; y < 1; y++) {
					GameObject toInstantiate = floorBlocks[Random.Range (0, floorBlocks.Length)];

					if (x == -1 || x == columns || z == -1 || z == rows) {
						toInstantiate = boundingBlock;
					} else if (y < 0) {
						toInstantiate = subfloorBlocks[Random.Range (0, subfloorBlocks.Length)];
					}

					GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, z),
					                                   Quaternion.identity) as GameObject;

					instance.transform.SetParent (boardHolder);
				}
			}
		}
	}

	Vector3 RandomPosition() {
		int randomIndex = Random.Range (0, gridPositions.Count);

		Vector3 randomPosition = gridPositions [randomIndex];

		gridPositions.RemoveAt (randomIndex);

		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum) {
		int objectCount = Random.Range (minimum, maximum+1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition ();

			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	public void SetupScene (int level) {
		InitLevelParams (level);
		BoardSetup ();
		InitialiseList ();
		LayoutObjectAtRandom (obstacles, obstacleCount.minimum, obstacleCount.maximum);
	}
}
