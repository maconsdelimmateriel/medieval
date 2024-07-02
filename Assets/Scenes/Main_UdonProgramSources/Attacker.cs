
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
    private int _initialHealth;
    private bool _isRange = false; //Is the attacker in front of the door?
    [SerializeField]
    private AudioSource _movingSound; //Sound played when knight is moving.
    [SerializeField]
    private AudioSource _dyingSound; //Sound played when the knight dies.
    [SerializeField]
    private AudioSource _attackingSound; //Sound played when the knight attacks the door.
    [SerializeField]
    private AudioSource _damagedSound; //Sound played when the knight takes damage.
    private Animator _anim; //Animator controller of the knight.
    private bool _isDead = false; //Is the knight dead?
    private bool isCoroutineRunning = true;
    private float _timer = 0f; //A timer that updates the behavior of the knight.
    private float _durationAttacks = 6f; // Time between attacks.
    private float _durationWalks = 2f; //Time between changing locomotion.
    private float _initialSpeed; //Walking speed.
    private Vector3 _lastPosition; // Last known position to check movement.
    private float _checkMovementInterval = 1f; // Interval to check movement.
    private float _timeSinceLastCheck = 0f; // Time since the last movement check.
    [SerializeField]
    public SpawnAttackers _spawn;
    [SerializeField]
    private Collider _collider;

    /*Current state values:
     * 0 Walking
     * 1 Idling after door reached
     * 2 Attacking door
     * 4 Running
     * 5 Idling with shield up
     */

    private void OnEnable()
    {
        //_door = GameObject.Find("Door").transform; //Finding by tag or component not handled by Udonsharp? Need to find better ways to find objects.

        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _anim = GetComponent<Animator>();
        _initialHealth = _health;
        _agent.speed = 4f;
        _initialSpeed = _agent.speed;
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Move");
    }

    private void Update()
    {
        if (!_isDead)
        {
            if (!_isRange)
            {
                if (Vector3.Distance(gameObject.transform.position, door.position) <= 3) //Distance fixed for prototyping, should be changed to agent.stoppingdistance
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Idle");
                    _isRange = true;
                }
            }

            if (isCoroutineRunning)
            {
                _timer += Time.deltaTime;
                _timeSinceLastCheck += Time.deltaTime;

                // Check if the attacker is moving every _checkMovementInterval seconds
                if (_timeSinceLastCheck >= _checkMovementInterval)
                {
                    //CheckMovement();
                }

                if (_isRange) //Updating behavior when in range with door.
                {
                    if (_timer >= _durationAttacks)
                    {
                        if (door.gameObject.GetComponent<CastleDoor>().DoodModel.activeInHierarchy)
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AttackDoor");

                        _timer = 0f;
                    }
                }
                else //Updating behavior when not in range with door.
                {
                    if (_timer >= _durationWalks)
                    {
                        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "BlockOrRun");
                        _timer = 0f;
                    }
                }

            }
        }
    }

    //The attacker moves toward the castle door.
    public void Move()
    {
        _agent.destination = door.position;
        _movingSound.Play();
    }

    //The attacker randomly chooses between running or blocking.
    public void BlockOrRun()
    {
        if (_isDead)
            return;

        int currentStateValue = _anim.GetInteger("CurrentState");

        if(currentStateValue == 0)
        {
            int flip = Random.Range(0, 2);

            if (flip == 0) //Running
            {
                _anim.SetInteger("CurrentState", 4);
                _agent.speed *= 2f;
            }
            else //Blocking
            {
                _anim.SetInteger("CurrentState", 5);
                _agent.speed = 0f;
            }
        }
        else //Walking
        {
            _anim.SetInteger("CurrentState", 0);
            _agent.speed = _initialSpeed;
        }
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
        _attackingSound.Play();
        _anim.SetTrigger("AttackDoor");
        _anim.SetInteger("CurrentState", 2);
        StartCoroutineReplacement();
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
        if (_isDead)
            return;

        _movingSound.Stop();
        _dyingSound.Play(); 
        _anim.SetBool("IsDead", true);
        _agent.destination = transform.position;
        _collider.enabled = false;
        _isDead = true;
    }

    // Reset the attacker's state
    public void ResetAttacker()
    {
        _health = _initialHealth;
        _isRange = false;
        //isCoroutineRunning = false;
        _timer = 0f;
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _agent.enabled = true;
        _isDead = true;
        _isDead = false;
        //_agent.destination = door.position;
        _anim.SetBool("IsDeath", true);
        _anim.SetBool("IsDeath", false);
        _anim.SetInteger("CurrentState", 0);
        _collider.enabled = true;
        gameObject.SetActive(false);
    }

    private void CheckMovement()
    {
        Debug.Log("check1");
        if (Vector3.Distance(_lastPosition, transform.position) < 1f)
        {
            Debug.Log("check2");
            _spawn.Interact();
            Debug.Log("check3");
        }
    }
}
