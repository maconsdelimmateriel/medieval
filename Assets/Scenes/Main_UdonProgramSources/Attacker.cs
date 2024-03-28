
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.AI;

public class Attacker : UdonSharpBehaviour
{
    private Transform _door; //Position of the castle door.
    private NavMeshAgent _agent; //NavMeshAgent of the attacker.
    [SerializeField]
    private int _health = 10; //Health of the attacker.

    void Start()
    {
        _door = GameObject.Find("Door").transform; //Finding by tag or component not handled by Udonsharp? Need to find better ways to find objects.
        _agent = GetComponent<NavMeshAgent>();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Move");
    }

    private void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, _door.position) <= 3) //Distance fixed for prototyping, should be changed to agent.stoppingdistance
        {
            _door.gameObject.GetComponent<CastleDoor>().TakingDamage(100);
        }
    }

    //The attacker moves toward the castle door.
    public void Move()
    {
        _agent.destination = _door.position;
    }

    //Called when the attacker is taking damaged.
    public void TakingDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Dying");
        }
    }

    //Called when the attacker dies.
    public void Dying()
    {
        Destroy(gameObject); //Should be replaced by effect of death
        _agent.destination = transform.position;
    }
}
