using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_TrapBomb : apt283ExplosiveRock
{
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        bool hasTagPlayer = collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Player);
        bool hasTagCreature = collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Creature);
        if (hasTagCreature || hasTagPlayer)
        {
            die();
        }
    }
}
