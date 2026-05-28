using PurrNet;
using PurrNet.Transports;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_Text statusText;

    private void Start()
    {
        if (statusText != null)
            statusText.text = "";

        networkManager.onServerConnectionState += OnServerStateChanged;
        networkManager.onClientConnectionState += OnClientStateChanged;
    }

    private void OnDestroy()
    {
        networkManager.onServerConnectionState -= OnServerStateChanged;
        networkManager.onClientConnectionState -= OnClientStateChanged;
    }

    private void OnServerStateChanged(ConnectionState state)
    {
        if (state == ConnectionState.Connected)
            SceneManager.LoadScene("SampleScene");
    }

    private void OnClientStateChanged(ConnectionState state)
    {
        if (state == ConnectionState.Connected && !networkManager.isServer)
            SceneManager.LoadScene("SampleScene");

        if (state == ConnectionState.Disconnected && !networkManager.isServer)
            statusText.text = "Failed to connect.";
    }

    public void OnHostClicked()
    {
        statusText.text = "Starting host...";
        networkManager.StartHost();
    }

    public void OnJoinClicked()
    {
        string ip = ipInputField.text.Trim();

        if (string.IsNullOrEmpty(ip))
        {
            statusText.text = "Please enter an IP address.";
            return;
        }

        var udpTransport = networkManager.transport as UDPTransport;
        if (udpTransport != null)
            udpTransport.address = ip;

        statusText.text = "Connecting to " + ip + "...";
        networkManager.StartClient();
    }
}
