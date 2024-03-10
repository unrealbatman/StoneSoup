using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rs8385KeySpawner : MonoBehaviour
{
    // Reference to the key prefab
    public GameObject keyPrefab;

    void Start()
    {
        // Find the start room
        Room startRoom = GameManager.instance.roomGrid[0, 0];

        // Find the farthest room from the start room using DFS
        Room farthestRoom = FindFarthestRoom(startRoom);

        // Spawn the key in the farthest room
        SpawnKeyInRoom(farthestRoom);
    }

    private Room FindFarthestRoom(Room startRoom)
    {
        Room[,] roomGrid = GameManager.instance.roomGrid;

        // Initialize DFS variables
        Stack<Room> stack = new Stack<Room>();
        HashSet<Room> visited = new HashSet<Room>();

        stack.Push(startRoom);
        visited.Add(startRoom);

        Room farthestRoom = startRoom;

        while (stack.Count > 0)
        {
            Room currentRoom = stack.Pop();

            // Check if the current room is farther than the farthest room found so far
            if (Vector2.Distance(startRoom.transform.position, currentRoom.transform.position) > Vector2.Distance(startRoom.transform.position, farthestRoom.transform.position))
            {
                farthestRoom = currentRoom;
            }

            // Iterate through neighbors
            foreach (Room neighbor in GetNeighbors(currentRoom, roomGrid))
            {
                if (!visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return farthestRoom;
    }

    private IEnumerable<Room> GetNeighbors(Room room, Room[,] roomGrid)
    {
        
        int x = room.roomGridX;
        int y = room.roomGridY;

        if (x > 0 && roomGrid[x - 1, y] != null)
            yield return roomGrid[x - 1, y];

        if (x < roomGrid.GetLength(0) - 1 && roomGrid[x + 1, y] != null)
            yield return roomGrid[x + 1, y];

        if (y > 0 && roomGrid[x, y - 1] != null)
            yield return roomGrid[x, y - 1];

        if (y < roomGrid.GetLength(1) - 1 && roomGrid[x, y + 1] != null)
            yield return roomGrid[x, y + 1];
    }

    private void SpawnKeyInRoom(Room room)
    {
        if (room != null)
        {
            if (room.GetComponent<rs8385BFSRoom>() != null)
            {
                room.GetComponent<rs8385BFSRoom>().isFurthest = true;
            }
            else if (room.GetComponent<rs8385DFSRoom>() != null)
            {

                room.GetComponent<rs8385DFSRoom>().isFurthest = true;


            }
            else if (room.GetComponent<rs8385DikjstraRoom>() != null)
            {

                room.GetComponent<rs8385DikjstraRoom>().isFurthest = true;

            }
            else if(room is jfm_room) 
            {
                (room as jfm_room).SpawnRSKey();
	        }
            else if(room is rs8385ExitRoom)
            {
               (room as rs8385ExitRoom).isFurthest = true;
            }
        }
        else
        {
            Debug.LogWarning("No room found to spawn the key.");
        }
    }


   
}
