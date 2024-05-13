
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.AI;
using System.Collections;

public class Attacker : UdonSharpBehaviour
{
    public Transform door; //Position of the castle door.
    private NavMeshAgent _agent; //NavMeshAgent of the attacker.
    [SerializeField]
    private int _health = 6; //Health of the attacker.
    private bool _isRange = false; //Is the attacker in front of the door?
    [SerializeField]
    private AudioSource _movingSound; //Sound played when knight is moving.
    [SerializeField]
    private AudioSource _dyingSound; //Sound played when the knight dies.
    [SerializeField]
    private AudioSource _damagedSound; //Sound played when the knight takes damage.
    private Animator _anim; //Animator controller of the knight.
    private bool _isDead = false; //Is the knight dead?
    private bool isCoroutineRunning = true;
    private float timer = 0f;
    private float duration = 6f; // Adjust as needed
    private float tickInterval = 1f; // Adjust as needed

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
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Idle");
                _isRange = true;
            }
        }

        // Check if your "coroutine" is running
        if (isCoroutineRunning && _isRange)
        {
            // Update the timer
            timer += Time.deltaTime;

            // Check if the timer exceeds the duration
            if (timer >= duration)
            {
                // Perform actions here
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AttackDoor");

                // Reset timer
                timer = 0f;
            }
        }
    }

    //The attacker moves toward the castle door.
    public void Move()
    {
        _agent.destination = door.position;
        _movingSound.Play();
    }

    //Knight gets in idle position when reaching door.
    public void Idle()
    {
        _anim.SetInteger("CurrentState", 1);
        _agent.destination = transform.position;
        _movingSound.Stop();
    }

    //One attack against the door.
    public void AttackDoor()
    {
        door.gameObject.GetComponent<CastleDoor>().TakingDamage(1);
        _anim.SetTrigger("AttackDoor");
        StartCoroutineReplacement();
        Debug.Log("yay");
    }

    void StartCoroutineReplacement()
    {
        // Set flag to indicate the "coroutine" is running
        isCoroutineRunning = true;
    }

    //Called when the attacker is taking damaged.
    public void TakingDamage(int damage)
    {
        if (!_isDead)
        {
            _health -= damage;
            _damagedSound.Play();

            _anim.SetTrigger("Hit");

            if (_health <= 0)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Dying");
            }
        }
    }

    //Called when the attacker dies.
    public void Dying()
    {
        _movingSound.Stop();
        _dyingSound.Play(); 
        _anim.SetInteger("CurrentState", 3);
        _agent.destination = transform.position;
        _isDead = true;
    }
}
