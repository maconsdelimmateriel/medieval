
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.AI;

public class Attacker : UdonSharpBehaviour
{
    public Transform door; //Position of the castle door.
    private NavMeshAgent agent; //NavMeshAgent of the attacker.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Move");
    }

    //The attacker moves toward the castle door.
    public void Move()
    {
        agent.destination = door.position;
    }
}
