
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

//Code copied from attacker for quick prototyping. Need creation of a destructible class instead.
public class CastleDoor : UdonSharpBehaviour
{
    [SerializeField]
    private int _health = 10; //Health of the door.
    [SerializeField]
    private GameObject _doorModel; //The 3D model of the castle door and its collider.
    [SerializeField]
    private AudioSource _destroyedSound; //Sound played when door is destroyed.
    private bool _isDestroyed = false; //Is the door destroyed?

    //Called when the door is taking damaged.
    public void TakingDamage(int damage)
    {
        _health -= damage;
        Debug.Log("damage");

        if (_health <= 0 && !_isDestroyed)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Dying");
        }
    }

    //Called when the door dies.
    public void Dying()
    {
        _destroyedSound.Play();
        _doorModel.SetActive(false);
        _isDestroyed = true;
    }
}
