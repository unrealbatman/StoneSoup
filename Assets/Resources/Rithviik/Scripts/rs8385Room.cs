using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rs8385Room : Room
{
    // Reference to the teleporter prefab
    public GameObject teleportPrefab;

    // Reference to the boomerang gun prefab
    public GameObject boomerangGunPrefab;

    // Minimum and maximum number of teleportals to spawn
    public int minNumTeleportals = 2, maxNumTeleportals = 4;

    // List to keep track of spawned teleportals
    public List<rs8385Teleport> teleportal = new List<rs8385Teleport>();

    // Probability of spawning a wall on the border
    public float borderWallProbability = 0.7f;

    // Override the fillRoom method
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        // In this version of room generation, walls are generated first.
        generateWalls(ourGenerator, requiredExits);

        // Number of teleportals to spawn
        int numTeleportals = Random.Range(minNumTeleportals, maxNumTeleportals + 1);

        // Ensure that the number of teleportals is either 2 or 4
        numTeleportals = (numTeleportals == 3) ? 4 : numTeleportals;

        // Array to keep track of occupied positions
        bool[,] occupiedPositions = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];

        // Loop through the room to mark border zones as occupied
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1 || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1)
                {
                    // All border zones are occupied.
                    occupiedPositions[x, y] = true;
                }
                else
                {
                    occupiedPositions[x, y] = false;
                }
            }
        }

        // List to store possible spawn positions
        List<Vector2> possibleSpawnPositions = new List<Vector2>(LevelGenerator.ROOM_WIDTH * LevelGenerator.ROOM_HEIGHT);

        // Spawn a single weapon compulsorily
        bool weaponSpawned = false;
        for (int i = 0; i < numTeleportals; i++)
        {
            possibleSpawnPositions.Clear();

            // Loop through the room to find unoccupied positions
            for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
            {
                for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
                {
                    if (occupiedPositions[x, y])
                    {
                        continue;
                    }
                    possibleSpawnPositions.Add(new Vector2(x, y));
                }
            }

            // Check if there are possible spawn positions
            if (possibleSpawnPositions.Count > 0)
            {
                // Randomly select a spawn position
                Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);

                // Spawn teleporter
                Tile teleporterObj = Tile.spawnTile(teleportPrefab, transform, (int)spawnPos.x, (int)spawnPos.y);
                // Assuming rs8385Teleport script is attached to the teleporter prefab
                rs8385Teleport teleportScript = teleporterObj.GetComponent<rs8385Teleport>();
                if (teleportal.Count < 4)
                {
                    teleportal.Add(teleportScript);
                }

                // Mark the position as occupied
                occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;

                
            }

            // Spawn weapon only once
            if (!weaponSpawned)
            {
                possibleSpawnPositions.Clear();

                // Loop through the room to find unoccupied positions
                for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
                {
                    for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
                    {
                        if (occupiedPositions[x, y])
                        {
                            continue;
                        }
                        possibleSpawnPositions.Add(new Vector2(x, y));
                    }
                }
                if (possibleSpawnPositions.Count > 0)
                {
                    // Randomly select a spawn position
                    Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);

                    Tile.spawnTile(boomerangGunPrefab, transform, (int)spawnPos.x, (int)spawnPos.y);
                    // Assuming rs8385Teleport script is attached to the teleporter prefab

                    // Spawn boomerang gun
                    weaponSpawned = true;

                    // Mark the position as occupied
                    occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;
                }
            }
        }
    }

    // Method to generate walls in the room
    protected void generateWalls(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        // Wall map to determine where to spawn walls
        bool[,] wallMap = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];

        // Loop through the room
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1 || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1)
                {
                    // Check for exit conditions and set wallMap accordingly
                    if (x == LevelGenerator.ROOM_WIDTH / 2 && y == LevelGenerator.ROOM_HEIGHT - 1 && requiredExits.upExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else if (x == LevelGenerator.ROOM_WIDTH - 1 && y == LevelGenerator.ROOM_HEIGHT / 2 && requiredExits.rightExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else if (x == LevelGenerator.ROOM_WIDTH / 2 && y == 0 && requiredExits.downExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else if (x == 0 && y == LevelGenerator.ROOM_HEIGHT / 2 && requiredExits.leftExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else
                    {
                        // Randomly determine if a wall should be spawned on the border
                        wallMap[x, y] = Random.value <= borderWallProbability;
                    }
                    continue;
                }
                wallMap[x, y] = false;
            }
        }

        // Spawn walls based on the wallMap
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (wallMap[x, y])
                {
                    // Spawn a wall at the given position
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
            }
        }
    }
}
