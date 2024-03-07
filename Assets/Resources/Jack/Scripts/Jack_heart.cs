using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack_heart : Tile
{
	
	protected virtual void Update()
	{
		//_sprite.transform.localPosition = new Vector3(0, 0.2f * Mathf.Sin(Time.time * 3), 0);
	}

	public override void pickUp(Tile tilePickingUsUp)
	{
		base.pickUp(tilePickingUsUp);
		tilePickingUsUp.health++;
		takeDamage(tilePickingUsUp, 1);
	}
}
