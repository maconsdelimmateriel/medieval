
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LongBow : UdonSharpBehaviour
{
    public GameObject _arrowPrefab; // The arrow to be shot.
    public float _shootStrength = 1f; // The shoot strength.
    [SerializeField]
    private AudioSource _preparingSound; //Sound played when grabbing the bow.
    [SerializeField]
    private AudioSource _shootSound; //Sound played when shooting.

    private Collider[] _bowColliders = null; // All colliders on the bow gameObject and its children.

    private void Start()
    {
        // Retrieving all bow colliders. This is used later to prevent collisions between the arrow and the bow.
        _bowColliders = GetComponentsInChildren<Collider>();
    }

    public override void OnPickup()
    {
        base.OnPickup();
        _preparingSound.Play();
    }

    public override void OnPickupUseDown()
    {
        base.OnPickupUseDown();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Shoot");
        _shootSound.Play();
    }

    //The bow shoots an arrow when trigger is pressed.
    public void Shoot()
    {
        // Instantiating the arrow. Object pooling should be used here in the future.
        GameObject arrow = Instantiate(_arrowPrefab, this.transform.position, this.transform.rotation);

        // Making sure the arrow won't collide with the bow by removing collisions between them.
        Collider arrowCollider = arrow.GetComponentInChildren<Collider>(true);
        if (arrowCollider != null && _bowColliders != null)
            foreach (Collider coll in _bowColliders)
                Physics.IgnoreCollision(coll, arrowCollider);

        // Activating the arrow.
        arrow.SetActive(true);

        // Shooting the arrow.
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * _shootStrength);
    }
}
