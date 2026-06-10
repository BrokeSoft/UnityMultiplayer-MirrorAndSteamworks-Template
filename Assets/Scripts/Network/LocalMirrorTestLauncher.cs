using Mirror;
using UnityEngine;

public class LocalMirrorTestLauncher : MonoBehaviour
{
    [Header("Local Test")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Transport localTransport;
    [SerializeField] private string localAddress = "localhost";

    public void StartLocalHost()
    {
        NetworkManager manager = GetNetworkManager();
        if (manager == null)
        {
            Debug.LogError("[LocalMirrorTestLauncher] Cannot start host because no NetworkManager was found.");
            return;
        }

        if (!PrepareLocalTransport(manager))
            return;

        if (NetworkServer.active || NetworkClient.active)
        {
            Debug.LogWarning("[LocalMirrorTestLauncher] Mirror is already active. Host was not started.");
            return;
        }

        manager.networkAddress = localAddress;
        manager.StartHost();

        Debug.Log($"[LocalMirrorTestLauncher] Started local host at '{localAddress}'.");
    }

    public void StartLocalClient()
    {
        NetworkManager manager = GetNetworkManager();
        if (manager == null)
        {
            Debug.LogError("[LocalMirrorTestLauncher] Cannot start client because no NetworkManager was found.");
            return;
        }

        if (!PrepareLocalTransport(manager))
            return;

        if (NetworkServer.active || NetworkClient.active)
        {
            Debug.LogWarning("[LocalMirrorTestLauncher] Mirror is already active. Client was not started.");
            return;
        }

        manager.networkAddress = localAddress;
        manager.StartClient();

        Debug.Log($"[LocalMirrorTestLauncher] Started local client connecting to '{localAddress}'.");
    }

    public void StopLocalNetwork()
    {
        NetworkManager manager = GetNetworkManager();
        if (manager == null)
        {
            Debug.LogWarning("[LocalMirrorTestLauncher] Cannot stop network because no NetworkManager was found.");
            return;
        }

        if (NetworkServer.active && NetworkClient.active)
        {
            manager.StopHost();
            Debug.Log("[LocalMirrorTestLauncher] Stopped local host.");
            return;
        }

        if (NetworkClient.active)
        {
            manager.StopClient();
            Debug.Log("[LocalMirrorTestLauncher] Stopped local client.");
            return;
        }

        if (NetworkServer.active)
        {
            manager.StopServer();
            Debug.Log("[LocalMirrorTestLauncher] Stopped local server.");
            return;
        }

        Debug.Log("[LocalMirrorTestLauncher] No local Mirror session was active.");
    }

    public void ChangeScene()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[LocalMirrorTestLauncher] Cannot change scene because NetworkServer is not active.");
            return;
        }

        CustomNetworkManager customManager = GetNetworkManager() as CustomNetworkManager;
        if (customManager == null)
        {
            Debug.LogError("[LocalMirrorTestLauncher] Cannot change scene because the NetworkManager is not CustomNetworkManager.");
            return;
        }

        customManager.ChangeScene();

        Debug.Log("[LocalMirrorTestLauncher] Requested scene change through CustomNetworkManager.ChangeScene().");
    }

    private bool PrepareLocalTransport(NetworkManager manager)
    {
        if (manager == null)
            return false;

        if (localTransport == null)
        {
            Debug.LogError("[LocalMirrorTestLauncher] localTransport is not assigned. Assign the TelepathyTransport component.");
            return false;
        }

        manager.transport = localTransport;
        Transport.active = localTransport;

        Debug.Log(
            $"[LocalMirrorTestLauncher] Using local transport: {localTransport.GetType().Name}. " +
            $"NetworkManagerTransport={manager.transport.GetType().Name}, " +
            $"MirrorActiveTransport={Transport.active.GetType().Name}."
        );

        return true;
    }

    private NetworkManager GetNetworkManager()
    {
        if (networkManager != null)
            return networkManager;

        networkManager = FindObjectOfType<NetworkManager>(true);
        return networkManager;
    }
}