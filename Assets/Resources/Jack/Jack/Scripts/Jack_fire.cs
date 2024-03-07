using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack_fire : Tile
{
	float timeAtSpawn;
	public float baseLifeTime; //set negative for infinite
	public float lifeExtensionRandomScale;
	float _realLifeTime;

	List<Tile> onFire;
	float fireTickDelay = 0.5f;
	float lastFireTick;

	private void Awake()
	{
		timeAtSpawn = Time.time;
		onFire = new List<Tile>();
		CalculateLifeTime(); 
	}
	//used for reseting after spawn;
	public void CalculateLifeTime() {
		_realLifeTime = baseLifeTime + Random.Range(-lifeExtensionRandomScale, lifeExtensionRandomScale);
	}
	private void Update()
	{
		if(Time.time - timeAtSpawn > _realLifeTime) {
			if(baseLifeTime > 0) {
				die();
			}

		}

		//Messy tick system that also needs to sanitize info every tick bc of death system
		List<int> toClear = new List<int>();
		if(Time.time - lastFireTick > fireTickDelay) { 
			for(int i = 0; i < onFire.Count; i++) {
				if (onFire[i] == null) {
					toClear.Add(i);
				}
				else {
					onFire[i].takeDamage(this, 1, DamageType.Normal);
				}
			}
			for (int i = toClear.Count - 1; i > -1; i--)
			{
				onFire.RemoveAt(i);
			}
			toClear.Clear();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("fire hit smth");
		if (collision.gameObject.GetComponent<Tile>() != null)
		{
			Tile otherTile = collision.gameObject.GetComponent<Tile>();
			onFire.Add(otherTile);
			otherTile.takeDamage(this, 1, DamageType.Normal);
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.GetComponent<Tile>() != null)
		{
			Tile otherTile = collision.gameObject.GetComponent<Tile>();
			if (onFire.Contains(otherTile)) {
				onFire.Remove(otherTile);
			}
		}
	}
}
