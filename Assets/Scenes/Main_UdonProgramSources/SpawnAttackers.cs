
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
    private GameObject _knightPrefab; //Prefab for a knight attacker

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Spawn");
    }

    // Instantiating attackers on each spawn point. Object pooling should be used here in the future.
    public void Spawn()
    {
        foreach (Transform spawnPoint in _spawnPoints)
        {
            
            GameObject attacker = Instantiate(_knightPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
