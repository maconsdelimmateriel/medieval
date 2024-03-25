
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

//Code copied from attacker for quick prototyping. Need creation of a destructible class instead.
public class CastleDoor : UdonSharpBehaviour
{
    [SerializeField]
    private int _health = 10; //Health of the door.

    void Start()
    {
        
    }

    //Called when the door is taking damaged.
    public void TakingDamage(int damage)
    {
        _health -= damage;
        Debug.Log("damage");

        if (_health <= 0)
        {
            Debug.Log("0");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Dying");
        }
    }

    //Called when the door dies.
    public void Dying()
    {
        Destroy(this.gameObject);
    }
}
