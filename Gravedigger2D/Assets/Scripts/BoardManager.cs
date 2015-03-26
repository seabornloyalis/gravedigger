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
	
	public int columns = 7;
	public int rows = 7;
	
	public Count obstacleCount = new Count(1, 3);
	public GameObject[] floorTiles; // all arrays for easy change later if/when we have multiple of each
	public GameObject[] obstacleTiles;
	public GameObject outerWallTile;
	public GameObject[] enemyTiles;
	public GameObject cornTile;
	public List<int> mazeLevels = new List<int> ();
	
	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();
	
	void InitialiseList(int level)
	{
		gridPositions.Clear ();
		float cameraModifier = (level * 0.5f);
		Camera.main.orthographicSize = 4.5f + cameraModifier;
		//Camera.main.transform.position.x = 3.0f + cameraModifier; // can't set the x y like this but
		//Camera.main.transform.position.y = 3.0f + cameraModifier; // this position will capture the game board in full
		int levelColumns = columns + level;
		int levelRows = rows + level;
		for(int x = 1; x < levelColumns - 1; x++)
		{
			for(int y = 1; y < levelRows - 1; y++)
			{
				gridPositions.Add(new Vector3(x, y , 0f));
			}
		}
	}
	
	void BoardSetup(int level)
	{
		boardHolder = new GameObject ("Board").transform;
		
		int levelColumns = columns + level;
		int levelRows = rows + level;
		
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
	
	void LayoutCornMaze(int level) {
		int levelColumns = columns + level;
		int levelRows = rows + level;
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
		CreateInteriorWalls (walls, level);
	}
	
	void CreateInteriorWalls(List<Vector3> walls, int level) {
		for (int i = 0; i < walls.Count; i++) {
			Vector3 randLoc = walls[Random.Range(0, walls.Count)];
			if (BuildNearWalls(randLoc, level)) {
				//walls.Remove(randLoc);
				return;
			}
		}  //still got issues
	}
	
	bool BuildNearWalls(Vector3 loc, int level) {
		int colsMid = (columns + level)/2;
		int rowsMid = (rows + level)/2;
		if (/*loc.x > colsMid &&*/ gridPositions.Contains (loc + new Vector3 (-1f, 0f, 0f)) &&
		    gridPositions.Contains (loc + new Vector3 (-2f, 0, 0f)) &&
		    gridPositions.Contains (loc + new Vector3 (-1f, 1, 0f)) &&
		    gridPositions.Contains (loc + new Vector3 (-1f, -1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (-1, 0, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, level);
			return true;
		}
		else if (/*loc.x < colsMid &&*/ gridPositions.Contains (loc + new Vector3 (1f, 0f, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (2, 0, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, 1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, -1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (1, 0, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, level);
			return true;
		}
		else if (/*loc.y > rowsMid &&*/ gridPositions.Contains (loc + new Vector3 (0f, -1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (0, -2, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (-1f, -1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, -1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (0, -1, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, level);
			return true;
		}
		else if (/*loc.x < rowsMid &&*/ gridPositions.Contains (loc + new Vector3 (0, 1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (0, 2, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (1f, 1, 0f)) &&
		         gridPositions.Contains (loc + new Vector3 (-1f, 1, 0f))) {
			Vector3 nextLoc = loc + new Vector3 (0, 1, 0f);
			Instantiate(cornTile, nextLoc, Quaternion.identity);
			gridPositions.Remove(nextLoc);
			
			BuildNearWalls(nextLoc, level);
			return true;
		}
		return false;
	}
	
	public void SetupScene(int level)
	{
		BoardSetup (level);
		InitialiseList (level);
		if (mazeLevels.Contains (level))
			LayoutCornMaze(level);
		else
			LayoutObjectAtRandom (obstacleTiles, obstacleCount.minimum + level, obstacleCount.maximum + level);
		//int enemyCount = (int)Mathf.Log (level, 2f); // incase we want to change the difficulty system
		LayoutObjectAtRandom (enemyTiles, level + 3, level + 3);
	}
	
}
