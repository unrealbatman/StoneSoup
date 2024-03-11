using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class rs8385RichochetGun : Tile
{
    // Reference to the bullet prefab to be instantiated
    public GameObject bulletPrefab;

    // Reference to the muzzle flash object
    public GameObject muzzleFlashObj;

    // Force applied when the gun recoils
    public float recoilForce = 100;

    // Force applied when shooting bullets
    public float shootForce = 1000f;

    // Cooldown time between shots
    public float cooldownTime = 0.1f;

    // Timer to track the cooldown between shots
    protected float _cooldownTimer;

    // Method to handle aiming based on the player's aim direction
    protected void aim()
    {
        transform.parent = _tileHoldingUs.transform;

        // Set the sprite's position for aiming visual representation
        _sprite.transform.localPosition = new Vector3(1f, 0, 0);


        // Calculate the aim angle based on the player's aim direction
        float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x) * Mathf.Rad2Deg;

        // Rotate the gun to face the aim direction
        transform.localRotation = Quaternion.Euler(0, 0, aimAngle);

        // Flip the sprite and adjust the muzzle flash position based on aim direction
        if (_tileHoldingUs.aimDirection.x < 0)
        {
            _sprite.flipY = true;
            muzzleFlashObj.transform.localPosition = new Vector3(muzzleFlashObj.transform.localPosition.x, -Mathf.Abs(muzzleFlashObj.transform.localPosition.y), muzzleFlashObj.transform.localPosition.z);
        }
        else
        {
            _sprite.flipY = false;
            muzzleFlashObj.transform.localPosition = new Vector3(muzzleFlashObj.transform.localPosition.x, Mathf.Abs(muzzleFlashObj.transform.localPosition.y), muzzleFlashObj.transform.localPosition.z);
        }
    }

    // Update method to handle cooldown, aiming, and sprite sorting
    protected virtual void Update()
    {
        // Decrement the cooldown timer if it's greater than 0
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        // Check if the gun is being held by a player
        if (_tileHoldingUs != null)
        {

            // Perform aiming when the gun is held
            aim();
        }
        else
        {
            // Reset visuals when not held
            _sprite.transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        // Update the sprite sorting order
        updateSpriteSorting();
    }

    // Override the useAsItem method to implement shooting logic
    public override void useAsItem(Tile tileUsingUs)
    {
        transform.parent = _tileHoldingUs.transform;
        // Check if the gun is still on cooldown
        if (_cooldownTimer > 0)
        {
            return;
        }

        // Aim the gun
        //aim();

        // Spawn and shoot three bullets in different directions
        for (int i = 0; i < 3; i++)
        {
            // Calculate the offset direction for each bullet
            float offsetAngle = i * 120f;
            Vector2 offsetDirection = Quaternion.Euler(0, 0, offsetAngle) * _tileHoldingUs.aimDirection;

            // Instantiate and shoot the bullet
            GameObject newBullet = Instantiate(bulletPrefab, tileUsingUs.transform.parent); // Set the player as the parent
            Camera.main.GetComponent<PostProcess>().lightObjects.Add(newBullet);

            newBullet.transform.position = muzzleFlashObj.transform.position;
            newBullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, offsetDirection);

            // Initialize and apply force to the bullet
            newBullet.GetComponent<Tile>().init();
            newBullet.GetComponent<Tile>().addForce(offsetDirection.normalized * shootForce);
        }

        // Activate and deactivate the muzzle flash for a visual effect
        muzzleFlashObj.SetActive(true);
        Invoke("deactivateFlash", 0.1f);

        // Apply recoil force to the player holding the gun
        tileUsingUs.addForce(-recoilForce * tileUsingUs.aimDirection.normalized);

        // Set the cooldown timer
        _cooldownTimer = cooldownTime;
    }

    // Method to deactivate the muzzle flash after a short delay
    public void deactivateFlash()
    {
        muzzleFlashObj.SetActive(false);
    }
}
