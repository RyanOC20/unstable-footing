using PurrNet;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

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
        // Only the server should spawn players, and only once per join event.
        if (!asServer) return;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : new Vector3(0, 1, 0);
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        // Instantiate locally, then register with PurrNet (passing the prefab reference
        // so clients know which prefab to instantiate on their end), then hand off ownership.
        var go = Instantiate(playerPrefab, pos, rot);
        var identity = go.GetComponent<NetworkIdentity>();
        identity.Spawn(playerPrefab);
        identity.GiveOwnership(player);
    }
}
