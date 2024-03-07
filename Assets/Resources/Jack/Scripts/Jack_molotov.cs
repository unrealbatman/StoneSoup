using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack_molotov : Tile
{

	/// <summary>
	/// Large portions of code are copied from "apt283Rock.cs"
	/// Copied passages will be bracketed by HACK code comments to mark which sections
	/// are not my own work. Changes to copied passages are also annotated with HACK comments
	/// </summary>

	// HACK The following is copied from apt283Rock.cs
	#region APdecs
	// How much force to add when thrown
	public float throwForce = 3000f;
	// How slow we need to be going before we consider ourself "on the ground" again
	public float onGroundThreshold = 0.8f;
	// How much relative velocity we need with a target on a collision to cause damage.
	public float damageThreshold = 14;
	// How much force we apply to a target when we deal damage. 
	public float damageForce = 1000;
	// We keep track of the tile that threw us so we don't collide with it immediately.
	protected Tile _tileThatThrewUs = null;
	// Keep track of whether we're in the air and whether we were JUST thrown
	protected bool _isInAir = false;
	protected float _afterThrowCounter;
	public float afterThrowTime = 0.2f;
	#endregion
	//HACK Above was copied from apt238Rock.cs
	
	public GameObject firePrefab;
	public int fireWidth;

	// HACK The following is copied from apt283Rock.cs
	#region AP_useAs
	// The idea is that we get thrown when we're used
	public override void useAsItem(Tile tileUsingUs)
	{
		if (_tileHoldingUs != tileUsingUs)
		{
			return;
		}
		if (onTransitionArea())
		{
			return; // Don't allow us to be thrown while we're on a transition area.
		}
		//AudioManager.playAudio(throwSound);

		_sprite.transform.localPosition = Vector3.zero;

		_tileThatThrewUs = tileUsingUs;
		_isInAir = true;

		// We use IgnoreCollision to turn off collisions with the tile that just threw us.
		if (_tileThatThrewUs.GetComponent<Collider2D>() != null)
		{
			Physics2D.IgnoreCollision(_tileThatThrewUs.GetComponent<Collider2D>(), _collider, true);
		}

		// We're thrown in the aim direction specified by the object throwing us.
		Vector2 throwDir = _tileThatThrewUs.aimDirection.normalized;

		// Have to do some book keeping similar to when we're dropped.
		_body.bodyType = RigidbodyType2D.Dynamic;
		transform.parent = tileUsingUs.transform.parent;
		_tileHoldingUs.tileWereHolding = null;
		_tileHoldingUs = null;

		_collider.isTrigger = false;

		// Since we're thrown so fast, we switch to continuous collision detection to avoid tunnelling
		// through walls.
		_body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

		// Finally, here's where we get the throw force.
		_body.AddForce(throwDir * throwForce);

		_afterThrowCounter = afterThrowTime;
	}
	#endregion
	//HACK Above was copied from apt238Rock.cs

	// HACK The following is copied (with one marked edit) from apt283Rock.cs
	#region AP_update
	protected virtual void Update()
	{
		if (_isInAir)
		{
			if (_afterThrowCounter > 0)
			{
				_afterThrowCounter -= Time.deltaTime;
			}
			// If we've been in the air long enough, need to check if it's time to consider ourself "on the ground"
			else if (_body.velocity.magnitude <= onGroundThreshold)
			{
				// HACK I removed some code here. 
				Explode();
			}
		}

		if (_tileHoldingUs != null)
		{
			// We aim the rock behind us.
			_sprite.transform.localPosition = new Vector3(-0.5f, 0, 0);
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
	//HACK Above was copied from apt238Rock.cs

	void Explode() {
		_body.velocity = Vector2.zero;
		_isInAir = false;
		die();

		Room r = GetRoomOfPosition(transform.position);
		Vector2 gc = toGridCoord(transform.position) - toGridCoord(r.transform.position);
		Debug.Log(gc);
		for(int i = -1; i < fireWidth; i++) {
			for (int j = -1; j < fireWidth; j++)
			{
				int x = Mathf.RoundToInt(gc.x) + i;
				int y = Mathf.RoundToInt(gc.y) + j;
				//check if it is occupied somehow

				if (!InBounds(x, y)) continue;
				Tile t = spawnTile(firePrefab, r.transform ,x, y);
				t.GetComponent<Jack_fire>().baseLifeTime = 3;
				t.GetComponent<Jack_fire>().CalculateLifeTime();
			}
		}

	}

	public virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (_isInAir)
		{
			Explode();
		}
	}

	bool InBounds(int x , int y) {
		Debug.Log("bounds " + LevelGenerator.ROOM_WIDTH + " " + LevelGenerator.ROOM_HEIGHT);
		if (x < 0 || x >= LevelGenerator.ROOM_WIDTH || y < 0 || y >= LevelGenerator.ROOM_HEIGHT)
		{
			return false;
		}
		return true;
	}

	Room GetRoomOfPosition(Vector2 worldPosition) {
		int roomX = Mathf.FloorToInt(worldPosition.x / (LevelGenerator.ROOM_WIDTH * Tile.TILE_SIZE));
		int roomY = Mathf.FloorToInt(worldPosition.y / (LevelGenerator.ROOM_HEIGHT * Tile.TILE_SIZE));
		//no checks to ensure we dont overflow or whatever
		return GameManager.instance.roomGrid[roomX, roomY];
    }

	public override void takeDamage(Tile tileDamagingUs, int damageAmount, DamageType damageType)
	{
		if (_isInAir) return;
		base.takeDamage(tileDamagingUs, damageAmount, damageType);
		Explode();
	}
}
