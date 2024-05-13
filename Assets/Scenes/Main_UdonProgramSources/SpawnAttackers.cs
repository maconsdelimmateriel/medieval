
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
    
    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Spawn");
    }

    // Instantiating attackers on each spawn point. Object pooling should be used here in the future.
    public void Spawn()
    {
        _startSound.Play();

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _knights[i].transform.position = _spawnPoints[i].position;
            _knights[i].SetActive(true);
        }
    }
}
