using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rs8385SmallExplosive : rs8385Pinball
{


    // Start is called before the first frame update
    void Start()
    {
        base.timeBeforeExplode = 5f;
        base.explodeTimer = timeBeforeExplode;
    }

    protected override void Update()
    {

        StartCoroutine(base.ChangeColorOverTime(Color.red,base.timeBeforeExplode, this.gameObject.GetComponentInChildren<SpriteRenderer>()));

        base.explodeTimer -= Time.deltaTime;

        if(base.explodeTimer < 0f)
        {
            base.die();
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != null)
        {
            if(collision.gameObject.GetComponent<Tile>()!=null)
            {

                if (collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Wall))
                {
                    if (collision.gameObject.GetComponent<Collider2D>() != null)
                    {
                        Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());

                    }
                }
            }
            
        }
       
       
    }



}
