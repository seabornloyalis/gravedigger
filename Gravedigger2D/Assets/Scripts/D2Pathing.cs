using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class D2Pathing : MonoBehaviour {
	struct Position{
		int x;
		int y;
	};

	struct Scores{
		int f;
		int g;
		int h;
	};

	struct check{
		Position pos;
		int Gscore;
	};

	struct PathNode{
		Position pos;
		Scores score;
		string parent;
	};

	private Dictionary<string, PathNode> path;
	private Dictionary<string, bool> used;
	private List<check> toCheck;
	private bool [,] valids;
	
	public void FindPath(Vector2 start, Vector2 end)
	{

	}

	public void InitValids(int x, int y)
	{
		valids = new bool[x, y];
		for (int i = 0; i < x; i++) {
			for(int j = 0; j < y; j++){	

			}
		}
	}
}