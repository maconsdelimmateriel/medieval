﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LongBow : UdonSharpBehaviour
{
    public GameObject _arrowPrefab; // The arrow to be shot.
    public float _shootStrength = 1f; // The shoot strength.
    private float _mintHeight = 5f; //The minimum height at which the player can shoot.

    [SerializeField]
    private AudioSource _preparingSound; //Sound played when grabbing the bow.
    [SerializeField]
    private AudioSource _shootSound; //Sound played when shooting.
    [SerializeField]
    private AudioSource _failSound; //Sound telling the player they can't shoot if they are not on the wall.
    
    [SerializeField]
    private GameObject _bigCollider; //Real size collider for when the object is idle.
    [SerializeField]
    private GameObject _smallCollider; //Smaller collider for easiest use when object is picked up.
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
        SwitchColliders(false);
    }

    public override void OnDrop()
    {
        base.OnDrop();
        SwitchColliders(true);
    }

    //Switches between small and big colliders
    private void SwitchColliders(bool isSmallCollider)
    {
        if(_smallCollider != null && _bigCollider != null)
        {
            if (!isSmallCollider)
            {
                _smallCollider.SetActive(true);
                _bigCollider.SetActive(false);
            }
            else
            {
                _bigCollider.SetActive(true);
                _smallCollider.SetActive(false);
            }
        }
    }

    public override void OnPickupUseDown()
    {
        base.OnPickupUseDown();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Shoot");
    }

    //The bow shoots an arrow when trigger is pressed.
    public void Shoot()
    {
        if (gameObject.transform.position.y >= _mintHeight)
        {

            // Instantiating the arrow. Object pooling should be used here in the future.
            GameObject arrow = Instantiate(_arrowPrefab, this.transform.position, this.transform.rotation);
            _shootSound.Play();

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
        else
        {
            Debug.Log("Can't shoot if not on the wall");
            //Add sound
            //_failSound.Play();
        }
    }
}
