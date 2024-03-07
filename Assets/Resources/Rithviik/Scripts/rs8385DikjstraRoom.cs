using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rs8385DikjstraRoom : rs8385Dijkstra
{
    public GameObject teleportalPrefab;
    public GameObject enemyPrefab;
    public GameObject ricochetGunPrefab;
    public GameObject multiTilePrefab;
    public GameObject pinballExplosivePrefab;
    public GameObject pinballSwordPrefab;

    public int minNumTeleportals = 2, maxNumTeleportals = 4;
    public int minNumEnemies = 3, maxNumEnemies = 5;
    public int minNumExplosives = 2, maxNumExplosives = 5;
    private List<Vector2> deadEnds = new List<Vector2>(); // List to store teleports

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        base.fillRoom(ourGenerator, requiredExits);

        // Define sets with ranges for specific GameObjects
        List<List<GameObject>> validSets = new List<List<GameObject>>()
        {
            new List<GameObject> {
                teleportalPrefab,
                pinballExplosivePrefab,
                pinballSwordPrefab,
                enemyPrefab,
                multiTilePrefab,
            },
            new List<GameObject> {
                teleportalPrefab,
                enemyPrefab,
                ricochetGunPrefab,
            },
            new List<GameObject> {
                pinballExplosivePrefab,
                pinballSwordPrefab,
                multiTilePrefab,
                enemyPrefab,
            },
            // Add more sets as needed
        };

        // Randomly choose a set
        List<GameObject> chosenSet = GlobalFuncs.randElem(validSets);


        // Spawn tiles from the chosen set
        foreach (GameObject prefab in chosenSet)
        {
            spawnTiles(prefab, getCountForPrefab(prefab), requiredExits);
        }
    }

    private void checkDeadEnd(GameObject prefab, int count, ExitConstraint requiredExits)
    {
        foreach (SearchVertex vertex in _closed)
        {
            // Only look at vertices that were dead ends and weren't neighboring the exits.
            if (!vertex.isDeadEnd)
            {
                continue;
            }

            bool closeToExit = false;
            foreach (Vector2Int exitPoint in requiredExits.requiredExitLocations())
            {
                int manDistanceToExit = (int)Mathf.Abs(exitPoint.x - vertex.gridPos.x) + (int)Mathf.Abs(exitPoint.y - vertex.gridPos.y);
                if (manDistanceToExit <= 1)
                {
                    closeToExit = true;
                    break;
                }
            }

            if (closeToExit)
            {
                continue;
            }

            deadEnds.Add(vertex.gridPos);
        }
    }



    private void spawnTiles(GameObject prefab, int count, ExitConstraint requiredExits)
    {
        List<Vector2> possibleSpawnPositions = getUnoccupiedPositions();

        if (prefab == teleportalPrefab)
        {
            // Only spawn teleports in dead ends
            checkDeadEnd(prefab, count, requiredExits);

            if (deadEnds.Count != 1)
            {


                for (int i = 0; i < count && i < deadEnds.Count; i++)
                {

                    Vector2 spawnPos = deadEnds[i];

                    // Ensure the position is not occupied before spawning
                    if (!IsPositionOccupied(spawnPos))
                    {
                        Tile.spawnTile(prefab, transform, (int)spawnPos.x, (int)spawnPos.y);

                        // Add the position to the list of occupied positions
                        AddOccupiedPosition(spawnPos);
                    }
                }
            }
        }
        else
        {
            // For other prefabs, spawn them at random positions
            for (int i = 0; i < count && i < possibleSpawnPositions.Count; i++)
            {
                Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);

                // Ensure the position is not occupied before spawning
                if (!IsPositionOccupied(spawnPos))
                {
                    Tile.spawnTile(prefab, transform, (int)spawnPos.x, (int)spawnPos.y);

                    // Add the position to the list of occupied positions
                    AddOccupiedPosition(spawnPos);
                }

                possibleSpawnPositions.Remove(spawnPos);
            }
        }
    }


    // Method to get unoccupied positions in the room, excluding walls
    private List<Vector2> getUnoccupiedPositions()
    {
        List<Vector2> possibleSpawnPositions = new List<Vector2>();

        for (int x = 1; x < LevelGenerator.ROOM_WIDTH - 1; x++)
        {
            for (int y = 1; y < LevelGenerator.ROOM_HEIGHT - 1; y++)
            {
                if (!IsOccupied(x, y))
                {
                    possibleSpawnPositions.Add(new Vector2(x, y));
                }
            }
        }

        return possibleSpawnPositions;
    }

    // Method to check if a position in the room is occupied
    private bool IsOccupied(int x, int y)
    {
        return IsPositionOccupied(new Vector2(x, y));
    }


    // Method to get count for a prefab based on its type
private int getCountForPrefab(GameObject prefab)
{
    if (prefab == teleportalPrefab)
    {
            int x = Random.Range(minNumTeleportals, maxNumTeleportals+1);
        return x;
    }
    else if (prefab == enemyPrefab)
    {
        return Random.Range(minNumEnemies, maxNumEnemies + 1);
    }
    else if (prefab == ricochetGunPrefab || prefab == pinballSwordPrefab)
    {
        return 1; // Only one instance of ricochet gun or pinball sword
    }
    else if (prefab == multiTilePrefab)
    {
        return 1; // Only one instance of multi-tile
    }
    else if (prefab == pinballExplosivePrefab)
    {
        return Random.Range(minNumExplosives, maxNumExplosives + 1);
    }

    return 0; // Default case (should not happen if prefabs are correctly set up)
}

}
