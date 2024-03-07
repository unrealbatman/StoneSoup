using System.Collections;
using UnityEngine;

public class MultiBall : Tile
{
    public GameObject smallPinballPrefab;
    public int numberOfBallsToSpawn = 2;

    private GameObject collObj;
    private bool triggered=false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Wall) && collision.gameObject.CompareTag("Pinball") && collision.gameObject!=null)
        {
            triggered = true;
            collObj = collision.gameObject;
                
        }
    }

    private void Update()
    {
        if (triggered)
        {
            SplitPinball(collObj);
            
        }
    }


    private void SplitPinball(GameObject originalPinball)
    {
        // Spawn multiple smaller pinballs
        for (int i = 0; i < numberOfBallsToSpawn; i++)
        {
            GameObject smallPinball = Instantiate(smallPinballPrefab, originalPinball.transform.position, Quaternion.identity);

            // Apply force to the small pinballs (you can customize this force)
            Vector2 forceDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            float forceMagnitude = 500f; // Adjust this value based on your preference
            smallPinball.GetComponent<Rigidbody2D>().AddForce(forceDirection * forceMagnitude);
        }

        triggered = false;
        // Destroy the original pinball
        Destroy(originalPinball);
    }
}
