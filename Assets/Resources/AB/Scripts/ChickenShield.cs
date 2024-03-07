using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenShield : apt283FollowEnemy
{
    public override void Start()
    {
        base.Start();
        damageAmount = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Enemy)){
            GameObject enemy = collision.gameObject;
            enemy.GetComponent<apt283FollowEnemy>().damageAmount = 0;
            Destroy(gameObject);
        }
    }

}
