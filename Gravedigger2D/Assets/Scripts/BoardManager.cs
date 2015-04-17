using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour 
{
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;
		
		public Count(int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
	
	public int columns = 4;
	public int rows = 4;
	
	public Count obstacleCount = new Count(5, 7);
	public GameObject[] floorTiles; // all arrays for easy change later if/when we have multiple of each
	public GameObject[] obstacleTiles;
	public GameObject outerWallTile;
	public GameObject[] enemyTiles;
	public GameObject cornTile;

	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();
	
	void InitialiseList(int levelRows, int levelColumns)
	{
		gridPositions.Clear ();
		//float cameraModifier = (level * 0.5f);
		//Camera.main.orthographicSize = 4.5f + cameraModifier;
		//Camera.main.transform.position.x = 3.0f + cameraModifier; // can't set the x y like this but
		//Camera.main.transform.position.y = 3.0f + cameraModifier; // this position will capture the game board in full
		for(int x = 1; x < levelColumns - 1; x++)
		{
			for(int y = 1; y < levelRows - 1; y++)
			{
				gridPositions.Add(new Vector3(x, y , 0f));
			}
		}
	}
	
	void BoardSetup(int levelRows, int levelColumns)
	{
		boardHolder = new GameObject ("Board").transform;
		for(int x = -1; x < levelColumns + 1; x++)
		{
			for(int y = -1; y < levelRows + 1; y++)
			{
				GameObject toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];
				if (x == -1 || x == levelColumns || y == -1 || y == levelRows)
				{
					toInstantiate = outerWallTile;
				}
				
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				
				instance.transform.SetParent(boardHolder);
			}
		}
	}

	Vector3 RandomPosition()
	{
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		
		return randomPosition;
	}
	
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = Random.Range (minimum, maximum + 1);
		
		for (int i = 0; i < objectCount; i++) 
		{
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
			Instantiate(tileChoice,randomPosition, Quaternion.identity);
		}
		
	}

	void LayoutCornRows(int levelRows, int levelColumns) {
		int cornDir = Random.Range (0, 2); //0 for vert, 1 for horiz
		for(int x = 1; x < levelColumns -1; x++)
		{
			for(int y = 1; y < levelRows -1; y++)
			{
				Vector3 loc = new Vector3(x,y,0f);
				if ((x%2) != 0 && (y%2) != 0) {
					Instantiate(cornTile, loc, Quaternion.identity);
					gridPositions.Remove(loc);
				}
				if (cornDir == 0 && (x%2) != 0) {
					Instantiate(cornTile, loc, Quaternion.identity);
					gridPositions.Remove(loc);
				}
				else if (cornDir == 1 && (y%2) != 0) {
					Instantiate(cornTile, loc, Quaternion.identity);
					gridPositions.Remove(loc);
				}
			}
		}
	}
	
	void LayoutCornMaze(int levelRows, int levelColumns) {
		int cornMazeExits = 2;
		List<Vector3> walls = new List<Vector3>();
		List<Vector3> exits = new List<Vector3> ();
		
		for (int x = 1; x < levelColumns -1; x++) {
			for (int y = 1; y < levelRows -1; y++) {
				if ((x == 1 || x == levelColumns -2) ^ (y == 1 || y == levelRows-2))
					walls.Add(new Vector3(x, y, 0f));
			}
		}
		for (int i = 0; i < cornMazeExits; i++) {
			Vector3 randLoc = walls[Random.Range (0, walls.Count)];
			exits.Add(randLoc);
			walls.Remove(randLoc + new Vector3(1f, 0f, 0f));
			walls.Remove(randLoc + new Vector3(-1f, 0f, 0f));
			walls.Remove(randLoc + new Vector3(0f, 1f, 0f));
			walls.Remove(randLoc + new Vector3(0f, -1f, 0f));
			walls.Remove(randLoc);
		}
		walls.Clear ();
		for (int x = 1; x < levelColumns -1; x++) {
			for (int y = 1; y < levelRows -1; y++) {
				if (x == 1 || x == levelColumns -2 || y == 1 || y == levelRows-2) {
					Vector3 loc = new Vector3(x,y,0f);
					if (!exits.Contains(loc)) {
						walls.Add(loc);
						Instantiate(cornTile, loc, Quaternion.identity);
						gridPositions.Remove(loc);
					} else
						gridPositions.Remove(loc);
				}
			}
		}
		CreateInteriorWalls (walls, levelRows, levelColumns);
	}
	
	void CreateInteriorWalls(List<Vector3> walls, int levelRows, int levelColumns) {
		for (int i = 0; i < walls.Count; i++) {
			Vector3 randLoc = walls[Random.Range(0, walls.Count)];
			if (BuildNearWalls(randLoc, levelRows, levelColumns)) {
				//walls.Remove(randLoc);
				return;
			}
		}
	}
	
	bool BuildNearWalls(Vector3 loc, int levelRows, int levelColumns) {
		int colsMid = levelColumns/2;
		int rowsMid = levelRows/2;
		if ( gridPositions.Contains (loc + new Vector3 (-1f, 0f, 0f)) &&
		    gridPositions.Contains (loc + new Vector3 (-2f, 0, 0f)) &&
		    gridPositions.Contains (loc + new Vector3 (-1f, 1, 0f)) &&
		    gridPositions.Contains (loc + new Vector3 (-1f, -1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (-1, 0, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, levelRows, levelColumns);
			return true;
		}
		else if (gridPositions.Contains (loc + new Vector3 (1f, 0f, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (2, 0, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, 1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, -1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (1, 0, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, levelRows, levelColumns);
			return true;
		}
		else if (/*loc.y > rowsMid &&*/ gridPositions.Contains (loc + new Vector3 (0f, -1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (0, -2, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (-1f, -1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, -1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (0, -1, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, levelRows, levelColumns);
			return true;
		}
		else if (/*loc.x < rowsMid &&*/ gridPositions.Contains (loc + new Vector3 (0, 1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (0, 2, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, 1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (-1f, 1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (0, 1, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, levelRows, levelColumns);
			return true;
		}
		return false;
	}
	
	public void SetupScene(int level)
	{
		switch (level) {
		case 1:
			BoardSetup (rows + level, columns + level);
			InitialiseList (rows + level, columns + level);
			LayoutObjectAtRandom (enemyTiles, level, level);
			break;

		case 2:
			BoardSetup (rows + level, columns + level);
			InitialiseList (rows + level, columns + level);
			LayoutObjectAtRandom (obstacleTiles, level, level + 2);
			LayoutObjectAtRandom (enemyTiles, level, level);
			break;

		case 3:
			BoardSetup (rows + level, columns + level);
			InitialiseList (rows + level, columns + level);
			LayoutCornMaze(rows + level, columns + level);
			LayoutObjectAtRandom (enemyTiles, level, level);
			break;

		case 4:
			BoardSetup (rows + level, columns + level);
			InitialiseList (rows + level, columns + level);
			LayoutCornRows(rows + level, columns + level);
			LayoutObjectAtRandom (enemyTiles, level, level);
			break;

		case 5:
			BoardSetup (level + columns, level + rows);//TO FIGURE OUT
			InitialiseList (level + columns, level + rows);
			LayoutObjectAtRandom (obstacleTiles, obstacleCount.minimum + level, obstacleCount.maximum + level);
			//int enemyCount = (int)Mathf.Log (level, 2f); // incase we want to change the difficulty system
			LayoutObjectAtRandom (enemyTiles, level, level);
			break;

		default:
			int levelCols;
			if (level < 12)
				levelCols = level + columns;
			else
				levelCols = 16;

			BoardSetup (8, levelCols);
			InitialiseList (8, levelCols);
			if ((level % 2) == 0)
				LayoutCornMaze(8, levelCols);
			else {
				int randType = Random.Range(0, 2); //0 for clear, 1 for rows
				if (randType == 0)
					LayoutObjectAtRandom (obstacleTiles, obstacleCount.minimum + level, obstacleCount.maximum + level);
				else if (randType == 1)
					LayoutCornRows(8, levelCols);
			}
			//int enemyCount = (int)Mathf.Log (level, 2f); // incase we want to change the difficulty system
			LayoutObjectAtRandom (enemyTiles, level, level);
			break;
		}
	}
	
}
