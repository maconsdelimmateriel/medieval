
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LongBow : UdonSharpBehaviour
{
    public GameObject _arrowPrefab;

    public override void OnPickupUseDown()
    {
        base.OnPickupUseDown();

        GameObject arrow = Instantiate(_arrowPrefab, this.transform.position, this.transform.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward);
    }
}
