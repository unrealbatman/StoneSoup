using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jfm_room : Room
{
	public Vector2 perlinFreq;
	public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
	{
		int height = LevelGenerator.ROOM_HEIGHT;
		int width = LevelGenerator.ROOM_WIDTH;

		bool[,] wallMap = new bool[width, height];
		int[,] indexGrid = new int[width, height];

		// Start completely filled with walls. 
		Vector2 RandomPerlinPoint = Random.insideUnitCircle * Random.Range(-1000, 1000);
		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
		{
			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
			{
				if (y == height - 1|| y == 0) {
					if ((Random.value < 0.9f)) {
						wallMap[x, y] = true;
					}
				}
				if (Mathf.PerlinNoise(perlinFreq.x * x + RandomPerlinPoint.x, perlinFreq.y * y + RandomPerlinPoint.y) < 0.4f)
				{
					wallMap[x, y] = true;
				}
			}
		}

		bool foundStartPos = false;
		Vector2 startPos = new Vector2(Random.Range(0, LevelGenerator.ROOM_WIDTH), Random.Range(0, LevelGenerator.ROOM_HEIGHT));
		List<Vector2Int> criticalPath = new();

		foreach (Vector2Int exitLocation in requiredExits.requiredExitLocations())
		{
			wallMap[exitLocation.x, exitLocation.y] = false;
			if (!foundStartPos)
			{
				startPos = exitLocation;
				foundStartPos = true;
			}
			Vector2Int start = new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y));
			Vector2Int[] path = FindPath(start, exitLocation, wallMap);
			criticalPath.AddRange(path);
			if(path == null) {
				Debug.Log("null path");
			}
			else {
				for(int i = 0; i < path.Length; i++) {
					wallMap[path[i].x, path[i].y] = false;
				}
			}
		}
		int index = 0;
		while(index < criticalPath.Count - 1) {
			index++;
			Vector2Int pos = criticalPath[index] + Vector2Int.up;
			if (!IsValid(pos)) continue;
			if (wallMap[pos.x, pos.y])
			{
				pos = criticalPath[index] + Vector2Int.down;
				if (!IsValid(pos)) continue;
				if (wallMap[pos.x, pos.y])
				{
					if(Random.value < 0.3) {
						//Debug.Log("spawning door!");
						indexGrid[criticalPath[index].x, criticalPath[index].y] = 5;
						break;
					}

				}
			}
			pos = criticalPath[index] + Vector2Int.right;
			if (!IsValid(pos)) continue;
			if (wallMap[pos.x, pos.y])
			{
				pos = criticalPath[index] + Vector2Int.left;
				if (!IsValid(pos)) continue;
				if (wallMap[pos.x, pos.y])
				{
					if (Random.value < 0.3)
					{
						Debug.Log("spawning door!");
						indexGrid[criticalPath[index].x, criticalPath[index].y] = 5;
						break;
					}
				}
			}
		}

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				//only doors in the way
				if (criticalPath.Contains(new Vector2Int(i, j))) {
					//Debug.Log(indexGrid[i, j]);
					if (indexGrid[i, j] != 5)
					{
						continue;
					}
					else {
						if (Random.value < 0.4)
						{
							indexGrid[i, j] = 8; //door mimic
						}
					}
				}

				int tileIndex = indexGrid[i, j];
				if (wallMap[i, j]) {
					tileIndex = 1;
				}
				else if(tileIndex == 0) {
					if (Mathf.PerlinNoise(perlinFreq.x * i + RandomPerlinPoint.x, perlinFreq.y * j + RandomPerlinPoint.y) < 0.45f)
					{
						if(Random.value < 0.1) {
							tileIndex = 6; // key!
						}
						else if(Random.value < 0.1)
						{
							tileIndex = 7; // molly
						}
						else {
							tileIndex = 4; // fire
						}

					}
				}

				if (tileIndex == 0)
				{
					continue; // 0 is nothing.
				}
				GameObject tileToSpawn;
				if (tileIndex < LevelGenerator.LOCAL_START_INDEX)
				{
					tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex - 1];
				}
				else
				{
					tileToSpawn = localTilePrefabs[tileIndex - LevelGenerator.LOCAL_START_INDEX];
				}
				Tile.spawnTile(tileToSpawn, transform, i, j);
			}
		}
	}

	//fuck it, a*
	// adapted from code adapted that was adapted from code i wrote a few years ago
	// that was in turn half copying a sebastian lague video
	Vector2Int[] FindPath(Vector2Int startPos, Vector2Int endPos, bool[,] wallMap) {
		List<Node> open = new();
		Dictionary<Vector2Int, Node> nodeLookup = new Dictionary<Vector2Int, Node>();

		Node startNode = CreateNode(startPos, endPos, 0, startPos, false);
		nodeLookup.Add(startPos, startNode);
		open.Add(startNode);
		int lowf = int.MaxValue;
		int triesRemaining = 800;


		while (lowf > 0)
		{
			triesRemaining--;
			if (open.Count < 1)
			{
				Debug.Log("closed all nodes");
				return null;
			}
			if (triesRemaining < 1)
			{
				Debug.Log("exiting having failed after max tries");
				break;
			}

			//A* main loop

			Node toeval = NextNode(open, out int r);
			if (toeval.fcost < lowf)
			{
				lowf = toeval.fcost;
			}
			open.RemoveAt(r);

			//evaluate surrounding nodes
			for (int i = 0; i < 9; i++)
			{
				if (i == 4) continue;
				Vector2Int off = new Vector2Int((i % 3) - 1, Mathf.FloorToInt(i / 3) - 1);

				//silly little way of removing diagonals
				if (off.x + off.y != 1 && off.x + off.y != -1) continue;

				Vector2Int pos = off + toeval.pos;
				if (!IsValid(pos)) continue;
				bool isWall = wallMap[pos.x, pos.y];

				if (nodeLookup.ContainsKey(pos)) {
					//we're re-evaluating an already opened node

					//recreate
					Node reeval = CreateNode(pos, endPos, toeval.gcost, toeval.pos, isWall);
					// overwrite its gcost if it would have a lower gcost with 
					// the current node as its parent
					if (reeval.gcost < nodeLookup[reeval.pos].gcost)
					{
						nodeLookup.Remove(reeval.pos);
						nodeLookup.Add(reeval.pos, reeval);
					}
				}
				else {
					//new node
					Node c = CreateNode(pos, endPos, toeval.gcost, toeval.pos, isWall);
					open.Add(c);
					nodeLookup.Add(c.pos, c);
				}
			}
		}
		if (!nodeLookup.ContainsKey(endPos)) {
			Debug.LogError("no end found");
			return null;
		}
		List<Vector2Int> path = new List<Vector2Int>();
		Vector2Int retrace = endPos;
		int tries = 0;
		while (retrace != startPos) {
			tries++;
			if (tries > 800) {
				Debug.LogError("couldnt retrace path");
				return null;
			}

			path.Add(retrace);
			retrace = nodeLookup[retrace].parentPosition;
		}
		path.Add(startPos);
		path.Reverse();
		return path.ToArray();

	}
	public Node NextNode(List<Node> open, out int r)
	{
		int lowc = int.MaxValue;
		Node ln = open[0];
		r = 0;
		for (int i = 0; i < open.Count; i++)
		{
			int cnv = open[i].fcost + open[i].gcost;
			if (cnv < lowc)
			{
				lowc = cnv;
				ln = open[i];
				r = i;
			}
			else if (cnv == lowc)
			{
				if (open[i].fcost < ln.fcost)
				{
					ln = open[i];
					r = i;
				}
			}
		}
		return ln;
	}

	public Node CreateNode(Vector2Int pos, Vector2Int endNode, int parentG, Vector2Int parent, bool isWall)
	{
		int dx = Mathf.Abs(endNode.x - pos.x);
		int dy = Mathf.Abs(endNode.y - pos.y);
		int comb = Mathf.Abs(dy - dx);
		int diag = Mathf.Min(dy, dx);
		int f = comb * 10 + diag * 14;
		f += isWall ? 30 : 0;
		int eg = Mathf.RoundToInt(Vector2Int.Distance(pos, parent) * 10);
		int g = eg + parentG;
		return new Node(pos, f, g, parent);
	}

	public struct Node {
		public Vector2Int pos;
		public int fcost;
		public int gcost;
		public Vector2Int parentPosition;

		public Node(Vector2Int po, int f, int g, Vector2Int par) {
			pos = po;
			fcost = f;
			gcost = g;
			parentPosition = par;
		}
	}

	public bool IsValid(Vector2Int pos){
		if(pos.x < 0 || pos.x > LevelGenerator.ROOM_WIDTH - 1) {
			return false;
		}
		if (pos.y < 0 || pos.y > LevelGenerator.ROOM_HEIGHT - 1)
		{
			return false;
		}
		return true;
	}
}
