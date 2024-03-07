using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rs8385Dijkstra : Room
{
    public class SearchVertex
    {
        public Vector2 gridPos;
        public SearchVertex parent;
        public int distance; // New field to store the distance from the start vertex
        public bool isDeadEnd = false;
    }

    protected static List<SearchVertex> _agenda = new List<SearchVertex>(80);
    protected static List<SearchVertex> _closed = new List<SearchVertex>(80);
    bool[,] occupiedPositions = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];

    public int extraWallsToRemove = 0;

    protected bool listContainsVertex(List<SearchVertex> list, Vector2 gridPos)
    {
        foreach (SearchVertex vertex in list)
        {
            if (vertex.gridPos == gridPos)
            {
                return true;
            }
        }
        return false;
    }

    protected static List<Vector2> _neighbors = new List<Vector2>(4);

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        bool[,] wallMap = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                wallMap[x, y] = true;
                occupiedPositions[x, y] = true;
            }
        }

        bool foundStartPos = false;
        Vector2 startPos = new Vector2(Random.Range(0, LevelGenerator.ROOM_WIDTH), Random.Range(0, LevelGenerator.ROOM_HEIGHT));

        foreach (Vector2Int exitLocation in requiredExits.requiredExitLocations())
        {
            wallMap[exitLocation.x, exitLocation.y] = false;
            if (!foundStartPos)
            {
                startPos = exitLocation;
                foundStartPos = true;
            }
        }

        _agenda.Clear();
        _closed.Clear();

        SearchVertex startVertex = new SearchVertex
        {
            gridPos = startPos,
            parent = null,
            distance = 0 // Starting vertex has distance 0
        };

        _agenda.Add(startVertex);

        while (_agenda.Count > 0)
        {
            int randomIndex = Random.Range(0, _agenda.Count);
            SearchVertex currentVertex = _agenda[randomIndex];
            _agenda.RemoveAt(randomIndex);

            if (listContainsVertex(_closed, currentVertex.gridPos))
            {
                continue;
            }

            _closed.Add(currentVertex);

            _neighbors.Clear();

            // Add neighbors based on your conditions
            AddRandomizedNeighbors(currentVertex.gridPos + Vector2.up * 2);
            AddRandomizedNeighbors(currentVertex.gridPos + Vector2.right * 2);
            AddRandomizedNeighbors(currentVertex.gridPos - Vector2.up * 2);
            AddRandomizedNeighbors(currentVertex.gridPos - Vector2.right * 2);

            if (_neighbors.Count == 0)
            {
                currentVertex.isDeadEnd = true;
            }

            foreach (Vector2 neighbor in _neighbors)
            {
                SearchVertex neighborVertex = new SearchVertex();
                neighborVertex.gridPos = neighbor;
                neighborVertex.parent = currentVertex;
                neighborVertex.distance = currentVertex.distance + 1; // Assuming the weight of each edge is 1
                _agenda.Add(neighborVertex);
            }
        }

        // Now go through the closed set and carve out space for all neighbors.
        foreach (SearchVertex vertex in _closed)
        {
            if (vertex.parent == null)
            {
                wallMap[(int)vertex.gridPos.x, (int)vertex.gridPos.y] = false;
                // Add the position to the list of occupied positions
                occupiedPositions[(int)vertex.gridPos.x, (int)vertex.gridPos.y] = false;
                continue;
            }

            int currentX = (int)vertex.gridPos.x;
            int currentY = (int)vertex.gridPos.y;
            wallMap[currentX, currentY] = false;
            occupiedPositions[currentX, currentY] = false;

            Vector2 endPos = vertex.parent.gridPos;
            int targetX = (int)endPos.x;
            int targetY = (int)endPos.y;

            while (currentX != targetX || currentY != targetY)
            {
                if (currentX < targetX)
                {
                    currentX++;
                }
                else if (currentX > targetX)
                {
                    currentX--;
                }

                if (currentY < targetY)
                {
                    currentY++;
                }
                else if (currentY > targetY)
                {
                    currentY--;
                }

                wallMap[currentX, currentY] = false;
                // Add the position to the list of occupied positions
                occupiedPositions[currentX, currentY] = false;
            }
        }

        // Now we remove some extra walls
        List<Vector2> wallLocations = new List<Vector2>();
        // Randomize the number of walls to remove
        int randomWallsToRemove = Random.Range(0, extraWallsToRemove);

        for (int i = 0; i < randomWallsToRemove; i++)
        {
       
            wallLocations.Clear();
            for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
            {
                for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
                {
                    if (wallMap[x, y])
                    {
                        wallLocations.Add(new Vector2(x, y));
                    }
                }
            }

            if (wallLocations.Count > 1)
            {
                Vector2 wallToRemove = GlobalFuncs.randElem(wallLocations);
                wallMap[(int)wallToRemove.x, (int)wallToRemove.y] = false;
                occupiedPositions[(int)wallToRemove.x, (int)wallToRemove.y] = false;
            }
        }

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (wallMap[x, y])
                {
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
            }
        }
    }

    // Helper method to check if a neighbor is within the grid and not closed
    private void AddRandomizedNeighbors(Vector2 neighborPos)
    {
        if (InGrid(neighborPos) && !listContainsVertex(_closed, neighborPos))
        {
            _neighbors.Add(neighborPos);
        }
    }

    protected bool InGrid(Vector2 gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < LevelGenerator.ROOM_WIDTH && gridPos.y >= 0 && gridPos.y < LevelGenerator.ROOM_HEIGHT;
    }

    protected Vector2 GetCoordinateFromExit(Dir exitDir)
    {
        if (exitDir == Dir.Up)
        {
            return new Vector2(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);
        }
        else if (exitDir == Dir.Right)
        {
            return new Vector2(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);
        }
        else if (exitDir == Dir.Down)
        {
            return new Vector2(LevelGenerator.ROOM_WIDTH / 2, 0);
        }
        else if (exitDir == Dir.Left)
        {
            return new Vector2(0, LevelGenerator.ROOM_HEIGHT / 2);
        }
        return Vector2.zero;
    }

    // Method to check if a position in the room is occupied
    public bool IsPositionOccupied(Vector2 position)
    {
        return occupiedPositions[(int)position.x, (int)position.y] == true;
    }

    // Method to add an occupied position
    public void AddOccupiedPosition(Vector2 position)
    {
        occupiedPositions[(int)position.x, (int)position.y] = true;
    }
}
