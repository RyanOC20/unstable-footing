using PurrNet;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject playerPrefab;
    private int _spawnIndex = 0;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        if (networkManager == null) networkManager = GetComponent<NetworkManager>();
        networkManager.onPlayerJoined += HandlePlayerJoined;
    }

    private void OnDestroy()
    {
        if (networkManager != null)
            networkManager.onPlayerJoined -= HandlePlayerJoined;
    }

    private void HandlePlayerJoined(PlayerID player, bool isReconnect, bool asServer)
    {
        if (!asServer) return;

        Vector3 pos = new Vector3(0, 1, 0);
        Quaternion rot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            var sp = spawnPoints[_spawnIndex % spawnPoints.Length];
            pos = sp.position;
            rot = sp.rotation;
            _spawnIndex++;
        }

        var go = Instantiate(playerPrefab, pos, rot);
        go.GetComponent<NetworkIdentity>().GiveOwnership(player);
    }
}
