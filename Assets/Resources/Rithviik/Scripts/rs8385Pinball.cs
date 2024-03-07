using System.Collections;
using UnityEngine;

public class rs8385Pinball : apt283Rock
{
    public float explosionRadius = 2f;
    public float explosionForce = 2000;
    public float bounceForce = 800;
    public int maxBounces = 3; // Maximum number of bounces before exploding
    public float timeBeforeExplode = 5f;

    private int currentBounces = 0;
    private float minBounceSpeed = 1f; // Minimum speed to consider for bouncing
    private float bounceForceMultiplier = 0.95f; // You can adjust this value to control the bounce force

    protected float explodeTimer;
    protected bool isBouncing = true;

    protected apt283PulseEffect _pulseEffect;
    public float normalPulsePeriod = 1f;
    public float heldPulsePeriod = 0.5f;

    private float initialHitSpeed; // Store initial hit speed

    public bool startsInAir = false;

    void Start()
    {
        _pulseEffect = GetComponentInChildren<apt283PulseEffect>();
        explodeTimer = timeBeforeExplode;

        if (startsInAir)
        {
            _isInAir = true;
            _afterThrowCounter = afterThrowTime;
        }
    }

    protected override void die()
    {
        _alive = false;
        Collider2D[] maybeColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D maybeCollider in maybeColliders)
        {
            Tile tile = maybeCollider.GetComponent<Tile>();

            if (tile == this)
            {
                continue;
            }

            if (tile != null)
            {
                tile.takeDamage(this, 2, DamageType.Explosive);
                tile.addForce((tile.transform.position - transform.position).normalized * explosionForce);
            }
        }

        base.die();
    }

    // Handle collisions with other tiles

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBouncing)
        {
            initialHitSpeed = collision.relativeVelocity.magnitude;

            Vector2 reflection = Vector2.Reflect(collision.relativeVelocity.normalized, collision.contacts[0].normal);

            float newBounceSpeed = initialHitSpeed * bounceForceMultiplier;

            if (newBounceSpeed > minBounceSpeed)
            {
                GetComponent<Rigidbody2D>().velocity = reflection * newBounceSpeed;

                isBouncing = false;
                explodeTimer = timeBeforeExplode;
                StartCoroutine(ChangeColorOverTime(Color.red, timeBeforeExplode,_sprite));
            }
            else
            {
                die();
            }
        }
        else
        {
            if (currentBounces < maxBounces)
            {
                currentBounces++;

                Vector2 reflection = Vector2.Reflect(collision.relativeVelocity.normalized, collision.contacts[0].normal);
                float newBounceSpeed = initialHitSpeed * bounceForceMultiplier;

                GetComponent<Rigidbody2D>().velocity = reflection * newBounceSpeed;

                isBouncing = false;
                explodeTimer = timeBeforeExplode;
                StartCoroutine(ChangeColorOverTime(Color.red, timeBeforeExplode,_sprite));

                if (newBounceSpeed < minBounceSpeed)
                {
                    explodeTimer = 0f;
                }
            }
            else
            {
                die();
            }
        }
    }

// Handle triggers

protected override void Update()
    {
        base.Update();

        if (!isBouncing)
        {
            explodeTimer -= Time.deltaTime;

            if (explodeTimer <= 0)
            {

                die();
            }
        }

        if (_pulseEffect != null)
        {
            if (_tileHoldingUs != null)
            {
                _pulseEffect.pulsePeriod = heldPulsePeriod;
            }
            else
            {
                _pulseEffect.pulsePeriod = normalPulsePeriod;
            }
        }
    }

    protected IEnumerator ChangeColorOverTime(Color targetColor, float duration, SpriteRenderer _sprite)
    {
        Color initialColor = _sprite.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _sprite.color = Color.Lerp(initialColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _sprite.color = targetColor;
    }
}
