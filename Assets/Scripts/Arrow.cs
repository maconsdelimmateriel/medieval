
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

[RequireComponent(typeof(VRCPickup))]
public class Arrow : UdonSharpBehaviour
{
    public float _sharpness = 1f;

    private bool _isPlanted = false;
    private bool _isPickedUp = false;
    private float _plantStrength = 0f; // Can be used later to determine if the player has enough strength to pickup a planted arrow.

    private VRCPickup _vrcPickup = null;

    private void Start()
    {
        _vrcPickup = GetComponent<VRCPickup>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // The arrow cannot get planted once it's already planted.
        if (_isPlanted)
            return;

        /// Note 1: the velocity of the arrow shouldn't matter.
        /// If the arrow is not moving, and an object is moving fast towards the arrow, then the arrow should get planted.
        /// What matters is the impact force.

        /// Note 2: we could use a formula that computes the sharpness of the arrow, against the rigidity of the collided object
        /// to determine if the arrow can get planted or should rebound.
        
        if (collision.impulse.magnitude < 20f)
            return;

        // When the arrow enters collision, we check if it should plant into the collider.
        foreach (ContactPoint contact in collision.contacts)
        {
            // The arrow should only plant in items on frontal collisions.
            if (Vector3.Dot(contact.normal, transform.forward) > -0.5f)
                continue;

            PlantArrow(collision.impulse.magnitude, collision.rigidbody.transform);
            break;
        }
    }

    public override void OnPickup()
    {
        base.OnPickup();

        _isPickedUp = true;
        UnplantArrow();
    }

    public override void OnDrop()
    {
        base.OnDrop();

        _isPickedUp = false;
    }

    private void PlantArrow(float force, Transform target)
    {
        if (_isPickedUp)
            _vrcPickup.Drop();

        _isPlanted = true;
        _plantStrength = force;
        transform.SetParent(target);
    }

    private void UnplantArrow()
    {
        _isPlanted = false;
    }
}
