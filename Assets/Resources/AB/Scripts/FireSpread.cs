using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpread : MonoBehaviour
{
    //public Vector2 ExitPoint;
    public Vector2 StartPoint;

    public GameObject fire;
    public float spreadDelay = 0.1f;
    private List<Vector2> burnedPositions = new List<Vector2>();
    private int maxDepth = 2;

    void Start()
    { 
        StartPoint = transform.position;
        StartSpreadFire();
    }

    
    public void StartSpreadFire()
    {
        //if (FindAnyObjectByType<Exit>())
        //{
        //    ExitPoint = FindAnyObjectByType<Exit>().gameObject.transform.position;
        //}

        StartCoroutine(SpreadFire(StartPoint));

        print("start fire");
    }

    private IEnumerator SpreadFire(Vector2 startPoint)
    {
        Queue<Vector2> pointsToSpread = new Queue<Vector2>();
        pointsToSpread.Enqueue(startPoint);

        while (pointsToSpread.Count > 0)
        {
            Vector2 currentPoint = pointsToSpread.Dequeue();
            SearchSurroundingArea(currentPoint.x, currentPoint.y, pointsToSpread);
            yield return new WaitForSeconds(spreadDelay);
        }
    }

    //recursion
    //private IEnumerator SpreadFire(Vector2 startPoint, int depth)
    //{
    //    if (depth > maxDepth) yield break;

    //    SearchSurroundingArea(startPoint.x, startPoint.y, depth);
    //    yield return new WaitForSeconds(spreadDelay);
    //}
    private void SpawnFire(Vector2 pos)
    {
        print("Fire");
        Instantiate(fire, pos, Quaternion.identity);
        burnedPositions.Add(pos);
    }

    private void SearchSurroundingArea(float x, float y, Queue<Vector2> pointsToSpread)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the current tile

                float newX = x + i;
                float newY = y + j;
                //Debug.Log("hah" + newX + " "+newY + "   a " + LevelGenerator.ROOM_WIDTH);
                if (newX >= 0 && newX < LevelGenerator.ROOM_WIDTH * 10 && newY >= 0 && newY < LevelGenerator.ROOM_HEIGHT * 10)
                {
                    Vector2 newPos = new Vector2(newX, newY);
                    //Debug.Log(Tile.tileAtPoint(newPos, TileTags.Wall));
                    if (Tile.tileAtPoint(newPos, TileTags.Wall) == null && !burnedPositions.Contains(newPos))
                    {
                        SpawnFire(newPos);
                        pointsToSpread.Enqueue(newPos);
                    }
                }
            }
        }
    }

    //private void SearchSurroundingArea(float x, float y, int depth)
    //{
    //    for (int i = -1; i <= 1; i++)
    //    {
    //        for (int j = -1; j <= 1; j++)
    //        {
    //            if (i == 0 && j == 0) continue; // Skip the current tile

    //            float newX = x + i;
    //            float newY = y + j;

    //            if (newX >= 0 && newX < LevelGenerator.ROOM_WIDTH * 10 && newY >= 0 && newY < LevelGenerator.ROOM_HEIGHT * 10)
    //            {
    //                Vector2 newPos = new Vector2(newX, newY);

    //                if (Tile.tileAtPoint(newPos, TileTags.Wall) == null && !burnedPositions.Contains(newPos))
    //                {
    //                    SpawnFire(newPos);
    //                    //StartCoroutine(SpreadFire(newPos));
    //                    StartCoroutine(SpreadFire(newPos, depth + 1));
    //                }
    //            }
    //        }
    //    }
    //}
}