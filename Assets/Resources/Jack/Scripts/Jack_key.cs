using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack_key : Tile
{
	Collider2D col;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
	}

	// HACK The following is copied (with portions removed) from apt283Rock.cs
	#region AP_update
	protected virtual void Update()
	{
		if (_tileHoldingUs != null)
		{
			// We aim the rock behind us.
			_sprite.transform.localPosition = new Vector3(0.5f, 0, 0);
			float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x) * Mathf.Rad2Deg;
			transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
		}
		else
		{
			_sprite.transform.localPosition = Vector3.zero;
		}
		updateSpriteSorting();
	}
	#endregion
	//HACK Above was copied from apt238Rock.c

	public override void pickUp(Tile tilePickingUsUp)
	{
		base.pickUp(tilePickingUsUp);
		col.enabled = false;
	}

	public override void dropped(Tile tileDroppingUs)
	{
		base.dropped(tileDroppingUs);
		col.enabled = true;
	}
	public override void useAsItem(Tile tileUsingUs)
	{
		base.useAsItem(tileUsingUs);
		col.enabled = true;
		Invoke(nameof(DisableCollider), 0.1f);
	}
	void DisableCollider() {
		col.enabled = false;
    }
}
