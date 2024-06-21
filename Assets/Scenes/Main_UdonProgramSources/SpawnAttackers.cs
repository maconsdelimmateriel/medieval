using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

//Test script to spawn attackers when button pushed. Need game designing.
public class SpawnAttackers : UdonSharpBehaviour
{
    [SerializeField]
    private Transform[] _spawnPoints; //All the spawn points for a wave.
    [SerializeField]
    private GameObject[] _knights; //Pool of knights for wave.
    [SerializeField]
    private AudioSource _startSound; //Sound played at the beginning of a wave.
    [SerializeField]
    private CastleDoor _castleDoor; // Reference to the castle door.
    [SerializeField]
    private GameObject _knightPrefab;


    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetAndSpawn");
    }

    // Method to reset and spawn attackers
    public void ResetAndSpawn()
    {
        _castleDoor.ResetDoor();

        foreach (GameObject knight in _knights)
        {
            Attacker attacker = knight.GetComponent<Attacker>();
            if (attacker != null)
            {
                attacker.ResetAttacker();
            }
        }

        Spawn();
    }

    // Instantiating attackers on each spawn point. Object pooling should be used here in the future.
    public void Spawn()
    {
        _startSound.Play();

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _knights[i].transform.position = _spawnPoints[i].position;
            _knights[i].SetActive(true);

            /*GameObject knight = (GameObject)Instantiate(_knightPrefab, _spawnPoints[i].position, _spawnPoints[i].rotation);
            Attacker knightScript = knight.GetComponent<Attacker>();
            knightScript.door = _castleDoor.transform;
            knightScript._spawn = this;*/
        }
    }
}
