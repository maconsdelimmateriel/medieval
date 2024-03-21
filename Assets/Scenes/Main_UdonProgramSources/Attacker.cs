
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.AI;

public class Attacker : UdonSharpBehaviour
{
    public Transform door; //Position of the castle door.
    private NavMeshAgent _agent; //NavMeshAgent of the attacker.
    [SerializeField]
    private int _health = 10; //Health of the attacker.

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Move");
    }

    //The attacker moves toward the castle door.
    public void Move()
    {
        _agent.destination = door.position;
    }

    //Called when the attacker is taking damaged.
    public void TakingDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Dyting");
        }
    }

    //Called when the attacker dies.
    public void Dying()
    {
        _agent.destination = transform.position;
    }
}
