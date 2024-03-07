using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrink : apt283Rock
{
    [SerializeField] private float timeRemaining = 2f;
    GameObject player;
    Vector3 originScale;
    void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        foreach (Transform child in transform)
        {
            SpriteRenderer sp = child.GetComponent<SpriteRenderer>();
            if (sp)
            {
                sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0f);
            }
        }
        originScale = player.transform.localScale;
        Vector3 newScale = originScale;
        player.transform.localScale = new Vector3(newScale.x / 2, newScale.y / 2, newScale.z / 2);
        StartCoroutine(Timer());

    }

    IEnumerator Timer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }
        player.transform.localScale = originScale;
        Destroy(this.gameObject);
    }
}
