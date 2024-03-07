using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Transform : apt283Rock
{
	[SerializeField]private float timeRemaining = 1f;
	private GameObject player;
	apt283FollowEnemy[] attackers;
	public Sprite snake;
	private Sprite self;
	private Animator animator;
	public Sprite emptyBottle;
	public override void OnCollisionEnter2D(Collision2D collision)
	{
		bool hasTagPlayer = collision.gameObject.GetComponent<Tile>().hasTag(TileTags.Player);
		if (hasTagPlayer)
		{
			if (transform.Find("rock_sprite").gameObject != null)
            {
				SpriteRenderer bottle = transform.Find("rock_sprite").gameObject.GetComponent<SpriteRenderer>();
				bottle.sprite = emptyBottle;
			}
			
			player = collision.gameObject;
			player.GetComponent<Tile>().removeTag(TileTags.Friendly);
			self = player.GetComponent<SpriteRenderer>().sprite;
            animator = player.GetComponent<Animator>();
			animator.enabled = false;
			player.GetComponent<SpriteRenderer>().sprite = snake;
			attackers = GameObject.FindObjectsOfType<apt283FollowEnemy>();
			foreach(apt283FollowEnemy s in attackers)
            {
				s.tileWereChasing = null;
			}
			StartCoroutine(Timer());
			//Destroy(gameObject);
		}
	}

	IEnumerator Timer()
	{
		while (timeRemaining > 0)
		{
			yield return new WaitForSeconds(1f);
			timeRemaining--;
		}
		player.GetComponent<Tile>().addTag(TileTags.Friendly);
		player.GetComponent<SpriteRenderer>().sprite = self;
		animator.enabled = true;
	}


}
