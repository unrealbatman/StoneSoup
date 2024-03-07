using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rs8385PinballSword : apt283Sword
{
    // Add a variable to control the charge level

    public float chargeRate = 20f; // Rate at which the sword charges per second

    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
    {
        if (_swinging || _tileHoldingUs != null)
        {
            return;
        }

        base.takeDamage(tileDamagingUs, amount, damageType);
    }

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);

        if (_tileHoldingUs != null)
        {
            AudioManager.playAudio(pickupSound);
        }
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        if (_swinging || _tileHoldingUs != tileUsingUs)
        {
            return;
        }

        AudioManager.playAudio(swingSound);

        _swinging = true;
        _pivotStartAngle = Mathf.Rad2Deg * Mathf.Atan2(tileUsingUs.aimDirection.y, tileUsingUs.aimDirection.x);
        swingPivot.transform.parent = tileUsingUs.transform;
        swingPivot.transform.localPosition = Vector3.zero;
        swingPivot.transform.localRotation = Quaternion.Euler(0, 0, _pivotStartAngle);
        transform.parent = swingPivot;
        transform.localPosition = new Vector3(1.2f, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, -90);
        _swingAngle = 0;
    }

    public override void dropped(Tile tileDroppingUs)
    {
        if (_swinging)
        {
            return;
        }

        base.dropped(tileDroppingUs);
    }
    // Modify the Update method to handle charging
    void Update()
    {
        if (_swinging)
        {

            _swingAngle += swingSpeed * Time.deltaTime;
            swingPivot.transform.localRotation = Quaternion.Euler(0, 0, _pivotStartAngle + _swingAngle);

            if (_swingAngle >= 360)
            {
                transform.parent = _tileHoldingUs.transform;
                transform.localPosition = new Vector3(heldOffset.x, heldOffset.y, -0.1f);
                transform.localRotation = Quaternion.Euler(0, 0, heldAngle);
                swingPivot.transform.parent = transform;
                _swinging = false;
            }
        }
    }

    // Modify the OnTriggerEnter2D method
    void OnTriggerEnter2D(Collider2D other)
    {
        rs8385Pinball pinball = other.GetComponent<rs8385Pinball>();

        if (_swinging && pinball != null)
        {
            float adjustedBounceForce = CalculateAdjustedBounceForce(pinball, swingSpeed);
            pinball.addForce((pinball.transform.position - _tileHoldingUs.transform.position).normalized * adjustedBounceForce);
            pinball.GetComponent<Collider2D>().isTrigger = false;
        }
    }
    float CalculateAdjustedBounceForce(rs8385Pinball pinball, float swingSpeed)
    {
        // Adjust the bounce force based on the sword swing speed or any other desired logic
        // You can experiment with different formulas based on your preferences
        return pinball.bounceForce * (swingSpeed / 2000); // Example: Adjust based on swing speed
    }


}