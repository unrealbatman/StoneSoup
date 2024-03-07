using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newRoomGenerator : Room
{
    public GameObject triggerBomb;
    public GameObject changeBomb;
    public GameObject transformPotion;
    public GameObject enemyPrefab;

    private bool[,] emptySpace;
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        emptySpace = spawnWalls(ourGenerator, requiredExits);
        SpawnEnemies(enemyPrefab, Random.Range(2, 4));
        SpawnWeapon(triggerBomb, Random.Range(1, 3));
        SpawnWeapon(transformPotion, Random.Range(2, 3));
        SpawnWeapon(changeBomb, Random.Range(1, 2));

        //placeDamagingTiles();
        //placeEnemies(enemyType1Prefab, minEnemiesType1, maxEnemiesType1);
        //placeEnemies(enemyType2Prefab, minEnemiesType2, maxEnemiesType2);
    }


    protected bool[,] spawnWalls(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        System.Random random = new System.Random();

        bool[,] emptySpots = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                emptySpots[i, j] = true;
            }
        }

        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            int index = random.Next(LevelGenerator.ROOM_HEIGHT);
            emptySpots[i, index] = false;
        }

        for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
        {
            int index = random.Next(LevelGenerator.ROOM_WIDTH);
            emptySpots[index, j] = false;
        }

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                bool fill = false;
                if (x == 0 && requiredExits.rightExitRequired )
                {
                    emptySpots[x, y] = false;
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                    break;
                }
                if (x == LevelGenerator.ROOM_WIDTH - 1 && requiredExits.leftExitRequired)
                {
                    emptySpots[x, y] = false;
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                    break;
                }

                if ((requiredExits.downExitRequired && y == 0 && x == (LevelGenerator.ROOM_HEIGHT - 1)/2) || (y == LevelGenerator.ROOM_HEIGHT - 1 && requiredExits.upExitRequired && x == (LevelGenerator.ROOM_HEIGHT - 1) / 2))
                {
                    emptySpots[x, y] = false;
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                    break;
                }

                if (!fill && !emptySpots[x, y])
                {
                    emptySpots[x, y] = false;
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
            }
        }
        return emptySpots;
    }


    protected void SpawnEnemies(GameObject enemyPrefab, int enemyAmt)
    {
        System.Random random = new System.Random();
        while (enemyAmt > 0)
        {
            int xPos = random.Next(LevelGenerator.ROOM_WIDTH);
            int yPos = random.Next(LevelGenerator.ROOM_HEIGHT);
            if(emptySpace[xPos, yPos])
            {
                emptySpace[xPos, yPos] = false;
                Tile.spawnTile(enemyPrefab, transform, xPos, yPos);
                enemyAmt--;
            }
        }
    }

    protected void SpawnWeapon(GameObject prefab, int amt)
    {
        System.Random random = new System.Random();
        while (amt > 0)
        {
            int xPos = random.Next(LevelGenerator.ROOM_WIDTH);
            int yPos = random.Next(LevelGenerator.ROOM_HEIGHT);
            if (emptySpace[xPos, yPos] && CountSurroundingArea(xPos, yPos) >= 4)
            {
                emptySpace[xPos, yPos] = false;
                Tile.spawnTile(prefab, transform, xPos, yPos);
                amt--;
            }
        }
    }

    private int CountSurroundingArea(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newX = x + i;
                int newY = y + j;

                if (newX >= 0 && newX < LevelGenerator.ROOM_WIDTH && newY >= 0 && newY < LevelGenerator.ROOM_HEIGHT)
                {
                    if (!emptySpace[newX, newY])
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }


}
