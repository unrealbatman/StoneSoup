using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack_lockedDoorMimic : apt283AStarEnemy
{
	public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
	{
		base.takeDamage(tileDamagingUs, amount, damageType);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Jack_key j)) {
			takeDamage(j, 100);
			//j.takeDamage(this, 100);
		}
	}
}
