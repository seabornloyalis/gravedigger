using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class D2Pathing : MonoBehaviour {
	public struct Position{
		public int x;
		public int y;
	};

	public struct Scores{
		public int f;
		public int g;
	};

	public struct Check{
		public Position pos;
		public int Gscore;
	};

	public struct PathNode{
		public Position pos;
		public Scores score;
		public int parent;
	};
	
	private Dictionary<int, bool> used;
	private List<Check> toCheck;
	private Dictionary<int, PathNode> Path;
	private bool[,] possibles;
	
	public void FindPath(Position start, Position end)
	{
		Check temp;
		temp.pos.x = start.x;
		temp.pos.y = start.y;
		temp.Gscore = 0;
		toCheck.Add (temp);

		while (toCheck.Count != 0) {
			Check current = FindLowestFScore();

			if(current.pos.x == end.x && current.pos.y == end.y){
				//DO THE THING!!!!
			}
			toCheck.Remove(current);

			int key = current.pos.x*100 + current.pos.y;
			used.Add(key, true);

			PathNode[] neighbors = DoNeighbors(current);
			//if neighbor used ignore
			for(int i = 0; i < 4; i++){
				int potGsc;
				PathNode temp = neighbors[i];
				if(possibles[temp.pos.x + 1,temp.pos.y + 1]){
					if(!used[temp.pos.x*100+temp.pos.y]){
						potGsc = temp.score.g;
					}
				}

			}
			//if neighbor not in to check or neighbor g lower than g in tocheck
			//put/update neighbor with current's stats
			//else return fail
		}
	}

	private PathNode[] DoNeighbors (Check current)
	{
		PathNode[] Neighbors = new PathNode[4];
		Neighbors [0].pos.x = current.pos.x - 1;
		Neighbors [0].pos.y = current.pos.y;
		Neighbors [0].score.g = current.Gscore + 1;
		Neighbors [1].pos.x = current.pos.x + 1;
		Neighbors [1].pos.y = current.pos.y;
		Neighbors [1].score.g = current.Gscore + 1;
		Neighbors [2].pos.x = current.pos.x;
		Neighbors [2].pos.y = current.pos.y - 1;
		Neighbors [2].score.g = current.Gscore + 1;
		Neighbors [3].pos.x = current.pos.x;
		Neighbors [3].pos.y = current.pos.y + 1;
		Neighbors [3].score.g = current.Gscore + 1;


		return Neighbors;
	}

	private Check FindLowestFScore()
	{
		Check lowest = toCheck [0];
		for (int i = 1; i < toCheck.Count; i++) {
			if(toCheck[i].Gscore < lowest.Gscore){
				lowest = toCheck[i];
			}
		}
		return lowest;
	}

	public void InitValids()
	{
		possibles = new bool[25, 25];
		for (int i = 0; i < 25; i++) {
			for(int j = 0; j < 25; j++){
				possibles[i,j] = true;
			}
		}

		Collider[] hits = Physics.OverlapSphere (new Vector3 (0, 0, 0), 25);
		for (int i = 0; i < hits.Length; i++) {
			if(hits[i].tag == "Enemy" || hits[i].tag == "Obstacles" || hits[i].tag == "Body"){
				possibles[(int)Mathf.Floor(hits[i].transform.position.x) + 1, (int)Mathf.Floor(hits[i].transform.position.y) + 1] = false;
			}
		}
	}
}