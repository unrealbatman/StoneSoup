using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FireSpread : MonoBehaviour
{
    public Vector2 ExitPoint;
    public Vector2 StartPoint;

    public GameObject fire;


    void Start()
    {
        ExitPoint = FindAnyObjectByType<Exit>().gameObject.transform.position;
        StartPoint = transform.position;
    }

    public void SpawnFire(Vector2 pos)
    {
        Instantiate(fire, pos, Quaternion.identity);
    }

    private void SearchSurroundingArea(float x, float y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                float newX = x + i;
                float newY = y + j;

                if (newX >= 0 && newX < LevelGenerator.ROOM_WIDTH && newY >= 0 && newY < LevelGenerator.ROOM_HEIGHT)
                {
                    Vector2 currentPos = new Vector2(newX, newY);
                    if (Tile.tileAtPoint(currentPos, TileTags.Wall))
                    {
                        SpawnFire(currentPos);
                        SearchSurroundingArea(currentPos.x, currentPos.y);
                    }
                }
            }
        }
    }
}
