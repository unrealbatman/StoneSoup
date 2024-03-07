using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rs8385Bullet : Tile
{
    // Damage threshold to determine when the bullet should deal damage
    public float damageThreshold = 14;

    // Threshold for when the bullet is considered on the ground
    public float onGroundThreshold = 1f;

    // Boomerang force applied to the bullet for a curved trajectory
    public float boomerangForce = 5f;

    // Timer for destroying the bullet after hitting the ground
    protected float _destroyTimer = 0.5f;

    // Array to store contact points during collisions
    protected ContactPoint2D[] _contacts = null;

    // Initial angle offset for the bullet's rotation
    public float initialAngleOffset = 0f;

    // Color properties for interpolation
    public Color startColor = Color.green;
    public Color endColor = Color.red;
    public float colorChangeDuration = 1.0f; // Duration for the color change

    // Timer for color change interpolation
    private float colorChangeTimer;

    // Reference to the bullet's starting transform
    private Transform _startTransform;

    private void Awake()
    {
        // Cache the starting transform for reference
        _startTransform = transform;
    }

    void Start()
    {
        // Initialize the contact points array
        _contacts = new ContactPoint2D[10];

        // Clear the TrailRenderer if attached
        if (GetComponent<TrailRenderer>() != null)
        {
            GetComponent<TrailRenderer>().Clear();
        }

        // Set the initial rotation based on the offset
        transform.rotation = Quaternion.Euler(0, 0, initialAngleOffset);

        // Set the initial bullet color
        SetBulletColor(startColor);

        // Initialize the color change timer
        colorChangeTimer = colorChangeDuration; // Start the timer at the full duration
    }

    void Update()
    {
        // Check if the bullet's velocity is below the onGroundThreshold
        if (_body.velocity.magnitude <= onGroundThreshold)
        {
            // Decrease the destroy timer
            _destroyTimer -= Time.deltaTime;

            // Check if it's time to destroy the bullet
            if (_destroyTimer <= 0)
            {
                die();
            }
        }

        // Add boomerang effect by adjusting the velocity over time
        if (boomerangForce > 0)
        {
            float angle = Mathf.Atan2(_body.velocity.y, _body.velocity.x);
            angle += boomerangForce * Time.deltaTime;
            float magnitude = _body.velocity.magnitude;
            _body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
        }

        // Interpolate color based on the timer
        colorChangeTimer -= Time.deltaTime;
        float t = 1.0f - Mathf.Clamp01(colorChangeTimer / colorChangeDuration); // Calculate interpolation factor
        SetBulletColor(Color.Lerp(startColor, endColor, t));

        // Check if it's time to swap colors
        if (colorChangeTimer <= 0)
        {
            // Reset the timer and switch colors
            colorChangeTimer = colorChangeDuration;
            SwapColors();
        }
    }

    // Set the color of the bullet sprite renderer
    void SetBulletColor(Color color)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;
        }
    }

    // Swap startColor and endColor
    void SwapColors()
    {
        Color tempColor = startColor;
        startColor = endColor;
        endColor = tempColor;
    }


    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>() != null)
        {
            // Calculate impact level of the collision
            float impact = collisionImpactLevel(collision);

            // Check if the impact is below the damage threshold
            if (impact < damageThreshold)
            {
                // Reflect the bullet's velocity based on the collision normal
                _body.velocity = Vector2.Reflect(_body.velocity, collision.GetContact(0).normal);
                return;
            }

            // Get the other tile involved in the collision
            Tile otherTile = collision.gameObject.GetComponent<Tile>();
           
                // Deal damage to the enemy tile
                otherTile.takeDamage(this, 1);
                       Debug.Log(otherTile);


        }

    }

    
    // Handle collisions with other tiles
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>() != null)
        {
            // Calculate impact level of the collision
            float impact = collisionImpactLevel(collision);

            // Check if the impact is below the damage threshold
            if (impact < damageThreshold)
            {
                // Reflect the bullet's velocity based on the collision normal
                _body.velocity = Vector2.Reflect(_body.velocity, collision.GetContact(0).normal);
                return;
            }

            // Get the other tile involved in the collision
            Tile otherTile = collision.gameObject.GetComponent<Tile>();

            if (otherTile.tags != TileTags.Friendly && otherTile.tags != TileTags.Player)
            {
                // Deal damage to the enemy tile
                otherTile.takeDamage(this, 1);
            }

        }
    }
}
