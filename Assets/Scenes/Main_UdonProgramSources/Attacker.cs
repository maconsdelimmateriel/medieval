
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
    private int _health = 14; //Health of the attacker.
    private bool _isRange = false; //Is the attacker in front of the door?
    [SerializeField]
    private AudioSource _movingSound; //Sound played when knight is moving.
    [SerializeField]
    private AudioSource _dyingSound; //Sound played when the knight dies.
    [SerializeField]
    private AudioSource _damagedSound; //Sound played when the knight takes damage.
    private Animator _anim; //Animator controller of the knight.

    private void OnEnable()
    {
        //_door = GameObject.Find("Door").transform; //Finding by tag or component not handled by Udonsharp? Need to find better ways to find objects.

        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Move");
    }

    private void Update()
    {
        if (!_isRange)
        {
            if (Vector3.Distance(gameObject.transform.position, door.position) <= 3) //Distance fixed for prototyping, should be changed to agent.stoppingdistance
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AttackDoor");
                _isRange = true;
            }
        }
    }

    //The attacker moves toward the castle door.
    public void Move()
    {
        _agent.destination = door.position;
        _movingSound.Play();
    }

    //The knight attacks the door when in range.
    public void AttackDoor()
    {
        _anim.SetInteger("CurrentState", 1);
        door.gameObject.GetComponent<CastleDoor>().TakingDamage(100);
        _agent.destination = transform.position;
        _movingSound.Stop();
    }

    //Called when the attacker is taking damaged.
    public void TakingDamage(int damage)
    {
        _health -= damage;
        _damagedSound.Play();

        _anim.SetBool("IsHit", true);

        if (_health <= 0)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Dying");
        }
    }

    //Called when the attacker dies.
    public void Dying()
    {
        _movingSound.Stop();
        _dyingSound.Play(); //To decomment when dying animation.
        //gameObject.SetActive(false);
        _anim.SetInteger("CurrentState", 3);
        _agent.destination = transform.position;
    }
}
