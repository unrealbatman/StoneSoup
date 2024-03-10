using System.Collections;
using UnityEngine;

public class rs8385ExitRoom : Room
{
    public bool isFurthest = false;
    public TextAsset withTreasureKey;
    public TextAsset withoutTreasureKey;
    public TextAsset bossUnlocked;
    public GameObject keyPrefab;
    public LevelGenerator OurGenerator;
    public bool isBossUnlocked = false;
    private Vector2Int index;

    private void Update()
    {
        if (isFurthest)
        {
            designedRoomFile = withTreasureKey;
            string initialGridString = designedRoomFile.text;
            string[] rows = initialGridString.Trim().Split('\n');

            for (int j = 0; j < rows.Length; j++)
            {
                string[] cols = rows[j].Trim().Split(',');

                for (int i = 0; i < cols.Length; i++)
                {
                    int tileIndex = int.Parse(cols[i]);

                    // Assuming a specific value in the layout file indicates where the key should be spawned
                    if (tileIndex == 4) // Adjust this condition based on your layout
                    {
                        // Instantiate the key prefab at the specified position within the room
                        Tile.spawnTile(keyPrefab, transform, i, rows.Length - j - 1);
                    }
                }
            }
            isFurthest = false;
        }
        else
        {
        }

        if (isBossUnlocked)
        {
            designedRoomFile = bossUnlocked;
            string initialGridString = designedRoomFile.text;
            string[] rows = initialGridString.Trim().Split('\n');

            for (int j = 0; j < rows.Length; j++)
            {
                string[] cols = rows[j].Trim().Split(',');

                for (int i = 0; i < cols.Length; i++)
                {
                    int tileIndex = int.Parse(cols[i]);

                    if (tileIndex == 0)
                    {
                        continue;
                    }

                    GameObject tileToSpawn = OurGenerator.globalTilePrefabs[tileIndex - 1];
                    Tile.spawnTile(tileToSpawn, transform, i, rows.Length - j - 1);
                }
            }
            isBossUnlocked = false;
        }
    }
}
