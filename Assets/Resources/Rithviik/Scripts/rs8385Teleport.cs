using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class rs8385Teleport : Tile
{
    public AudioClip teleportSFX;
    public GameObject teleportEffect;

    private List<rs8385Teleport> teleportTiles;
    private bool canTeleport = true;

    private void Start()
    {
        // Find and store all teleport tiles in the scene (excluding the current one)
        teleportTiles = FindOtherTeleportTilesInRoom();
    }

    public override void tileDetected(Tile detectedTile)
    {
        // Call the base class method (if any)
        base.tileDetected(detectedTile);

       // Debug.Log(detectedTile);
        // Check if the detected tile is friendly and teleport is allowed
        if (((detectedTile.hasTag(TileTags.Friendly) && detectedTile.hasTag(TileTags.Wall)) || (detectedTile.hasTag(TileTags.Friendly))) && canTeleport)
        {

            // Play teleport sound effect
            AudioManager.playAudio(teleportSFX);

            // Start the teleport cooldown coroutine
            StartCoroutine(TeleportCooldown(detectedTile));
        }
    }
    private Vector3 GetRandomTeleportPosition(rs8385Teleport currentTeleportTile)
    {
        // Exclude the current teleport tile from the list of available teleport tiles
        List<rs8385Teleport> eligibleTiles = teleportTiles.Where(tile => tile != currentTeleportTile).ToList();

        // Get a random teleport position from the eligible tiles
        if (eligibleTiles.Count > 0)
        {
            return eligibleTiles[Random.Range(0, eligibleTiles.Count)].transform.position;
        }
        else
        {
            // If there are no eligible tiles, return the current tile's position
            return currentTeleportTile.transform.position;
        }
    }


    private IEnumerator TeleportCooldown(Tile detectedTile)
    {
        // Disable teleport temporarily during cooldown
        canTeleport = false;

        GetComponent<ParticleSystem>().Play();

        // Wait for the specified cooldown duration
        yield return new WaitForSeconds(0.5f); // Adjust the cooldown duration as needed

        // Re-enable teleport after cooldown
        canTeleport = true;

        // Teleport the detected tile to a random teleport position
        if (teleportTiles.Count > 0)
        {
            rs8385Teleport randomTeleportTile = teleportTiles[Random.Range(0, teleportTiles.Count)];
            Vector3 randomPosition = GetRandomTeleportPosition(randomTeleportTile);

            // Check if the destination is clear using raycasts
            Vector3 offset = GetOffsetBasedOnDirection(randomPosition);

            if (detectedTile != null)
            {
                // Offset the detected tile's position based on the clear direction
                detectedTile.transform.position = randomPosition + offset;
            }
        }

        GetComponent<ParticleSystem>().Stop();
    }

    private Vector3 GetOffsetBasedOnDirection(Vector3 destination)
    {
        float raycastDistance = 2.0f; // Adjust the raycast distance as needed
        int numRaycasts = 10; // Increase the number of raycasts

        // Check in all directions (up, down, left, right)
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        foreach (Vector3 direction in directions)
        {
            float angleIncrement = 360f / numRaycasts;

            for (int i = 0; i < numRaycasts; i++)
            {
                Vector3 raycastDirection = Quaternion.Euler(0, 0, angleIncrement * i) * direction;

                RaycastHit2D hit = Physics2D.Raycast(destination, raycastDirection, raycastDistance);

                // If the ray doesn't hit anything, return the offset corresponding to the clear direction
                if (hit.collider == null)
                {
                    return raycastDirection * 1.5f; // Adjust the offset distance as needed
                }
            }
        }

        // If no clear direction is found, return zero offset
        return Vector3.zero;
    }


    private List<rs8385Teleport> FindOtherTeleportTilesInRoom()
    {
        // Find all teleport tiles that are direct children of the same parent GameObject
        return transform.parent.GetComponentsInChildren<rs8385Teleport>()
            .Where(tile => tile != this)
            .ToList();
    }

 
}
