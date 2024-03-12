using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpread : MonoBehaviour
{
    //public Vector2 ExitPoint;
    public Vector2 StartPoint;
    public LevelGenerator ourGenerator;
    public GameObject fire;
    public float spreadDelay = 2f;
    private List<Vector2> burnedPositions = new List<Vector2>();


    public TextAsset ExitUnlocked;
    public GameObject newExit;
    Transform _transform;
    bool triggered = false;
    Vector2Int index;
    void Start()
    { 
        StartPoint = transform.position;

        index = FindObjectIndexWithTag("Exit", GameManager.instance.roomGrid);

        _transform = GameManager.instance.roomGrid[index.x, index.y].transform;

    }



    private void Update()
    {
        triggered = (this.GetComponent<Jack_lockedDoor>().health < 100);
        

        if (triggered)
        {
            this.GetComponent<Jack_lockedDoor>().health = 1000;
            GameManager.isBossOpen = true;
            GameManager.instructionChange = true;
            StartCoroutine(SpreadFire(StartPoint));
           /* if (GameManager.spawnNewExit == true)
            {
                if (GameManager.instance.roomGrid[index.x, index.y].TryGetComponent(out rs8385ExitRoom exit))
                {

                    GameObject go = GameObject.FindGameObjectWithTag("Exit");

                    if (go != null)
                    {_transform = go.transform;
                        Destroy(go);
                    }
                }

                newExit = Instantiate(newExit, _transform.position, Quaternion.identity);
                newExit.GetComponent<rs8385ExitRoom>().isBossUnlocked = true;
                this.GetComponent<Jack_lockedDoor>().health += 200;
                triggered = false;

                GameManager.spawnNewExit = false;
            }
            
               */

            
          
           
            }
    }

    Vector2Int FindObjectIndexWithTag(string tagToFind, Room[,] roomGrid)
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Room room = roomGrid[x, y];
                if (room != null && room.CompareTag(tagToFind))
                {
                    // Found the GameObject with the specified tag, return its index
                    return new Vector2Int(x, y);
                }
            }
        }

        // GameObject with the specified tag not found
        return new Vector2Int(-1, -1);
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


    private void SpawnFire(Vector2 pos)
    {
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
                if (newX >= 0 && newX < LevelGenerator.ROOM_WIDTH * 3 * Tile.TILE_SIZE && newY >= 0 && newY < LevelGenerator.ROOM_HEIGHT * 3 * Tile.TILE_SIZE)
                {
                    Vector2 newPos = new Vector2(newX, newY);
                    if (Tile.tileAtPoint(newPos, TileTags.Wall) == null && !burnedPositions.Contains(newPos))
                    {
                        SpawnFire(newPos);
                        pointsToSpread.Enqueue(newPos);
                    }
                }
            }
        }
    }

    

  
        

  
}
