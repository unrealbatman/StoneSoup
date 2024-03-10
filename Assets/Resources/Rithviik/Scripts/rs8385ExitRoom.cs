using System.Collections;
using UnityEngine;

public class rs8385ExitRoom : Room
{
    public bool isFurthest = false;
    public TextAsset withTreasureKey;
    public TextAsset withoutTreasureKey;
    public GameObject keyPrefab;

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

/*                        Instantiate(keyPrefab, transform.position + keyPosition, Quaternion.identity);
*/                    }
                }
            }
            isFurthest = false;
        }
        else
        {
            designedRoomFile = withoutTreasureKey;
        }
    }

    
   
}
