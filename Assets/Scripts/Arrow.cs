
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

[RequireComponent(typeof(VRCPickup))]
[RequireComponent(typeof(Rigidbody))]
public class Arrow : UdonSharpBehaviour
{
    public float _sharpness = 1f;

    private bool _isPlanted = false;
    private bool _isPickedUp = false;
    private float _plantStrength = 0f; // Can be used later to determine if the player has enough strength to pickup a planted arrow.

    private Rigidbody _rb = null;
    private VRCPickup _vrcPickup = null;

    [SerializeField]
    private int _damage = 4; //The damage the arrow inflicts on contact;
    [SerializeField]
    private AudioSource _impactSound; //Sound played on impact.

    [SerializeField]
    private float _disparitionDelay = 5f; //Time after which the shot arrows disappear.

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _vrcPickup = GetComponent<VRCPickup>();
        Destroy(gameObject, _disparitionDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // The arrow cannot get planted once it's already planted.
        if (_isPlanted)
            return;

        _impactSound.Play();

        /// Note 1: the velocity of the arrow shouldn't matter.
        /// If the arrow is not moving, and an object is moving fast towards the arrow, then the arrow should get planted.
        /// What matters is the impact force.

        /// Note 2: we could use a formula that computes the sharpness of the arrow, against the rigidity of the collided object
        /// to determine if the arrow can get planted or should rebound.

        //if (collision.impulse.magnitude < 20f)
        //    return;
            // When the arrow enters collision, we check if it should plant into the collider.
            /*foreach (ContactPoint contact in collision.contacts)
            {
                // The arrow should only plant in items on frontal collisions.
                //if (Vector3.Dot(contact.normal, transform.forward) > -0.5f)
                //    continue;

                PlantArrow(collision.impulse.magnitude, collision.rigidbody.transform);
                break;
            }*/ //To decomment after testing

        //Arrow checks if object hit is taking damage.
        if (collision.gameObject.GetComponent<Attacker>() != null)
        {
            Attacker attacker = collision.gameObject.GetComponent<Attacker>();
            attacker.SetProgramVariable("damage", _damage);
            //attacker.OnReceiveDamage(_damage);
            attacker.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnReceiveDamage");
        }
        else if (collision.gameObject.name == "Ground") //Using name because tags not exposed to Udon yet.
        {
            Destroy(gameObject);
        }

        this.gameObject.GetComponent<Collider>().enabled = false;
    }

   

    /*public override void OnPickup()
    {
        base.OnPickup();

        _isPickedUp = true;
        UnplantArrow();
    }

    public override void OnDrop()
    {
        base.OnDrop();

        _isPickedUp = false;
    }*/

    private void PlantArrow(float force, Transform target)
    {
        /*if (_isPickedUp)
            _vrcPickup.Drop();*/

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _isPlanted = true;
        _plantStrength = force;
        transform.SetParent(target);
    }

    private void UnplantArrow()
    {
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _isPlanted = false;
    }
}
