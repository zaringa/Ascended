using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class SimpleHostServer : NetworkBehaviour
{
    [Header("Connection Settings")]
    public string serverIP = "127.0.0.1";
    public ushort port = 7777;
    
    [Header("UI References (Optional)")]
    public Button hostButton;
    public Button clientButton;
    public Button disconnectButton;
    public Text connectionStatusText;
    
    [Header("Player Prefab")]
    public GameObject playerPrefab;
    
    private void Start()
    {
        InitializeUI();
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }
    
    private void InitializeUI()
    {
        if (hostButton != null)
            hostButton.onClick.AddListener(StartHostServer);
        
        if (clientButton != null)
            clientButton.onClick.AddListener(StartClient);
        
        if (disconnectButton != null)
            disconnectButton.onClick.AddListener(Disconnect);
    }
    
    public void StartHostServer()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        
        if (transport == null)
        {
            Debug.LogError("UnityTransport component not found!");
            return;
        }
        
        transport.SetConnectionData(
            "0.0.0.0", 
            port
        );
        
        NetworkManager.Singleton.StartHost();
        Debug.Log($"Host started on port {port}");
        
        UpdateStatus("Host started");
    }
    
    public void StartClient()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        
        if (transport == null)
        {
            Debug.LogError("UnityTransport component not found!");
            return;
        }
        
        transport.SetConnectionData(
            serverIP,
            port
        );
        
        NetworkManager.Singleton.StartClient();
        Debug.Log($"Connecting to {serverIP}:{port}");
        
        UpdateStatus("Connecting...");
    }
    
    private void OnServerStarted()
    {
        Debug.Log("Server started successfully!");
        
        if (IsServer)
        {
            if (playerPrefab != null)
            {
                NetworkManager.Singleton.AddNetworkPrefab(playerPrefab);
            }
            
            UpdateStatus("Server running");
        }
    }
    
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected");
        
        if (IsServer)
        {
            SpawnPlayerForClient(clientId);
            UpdateStatus($"Clients connected: {NetworkManager.Singleton.ConnectedClients.Count}");
        }
        else
        {
            UpdateStatus("Connected to server");
        }
    }
    
    private void SpawnPlayerForClient(ulong clientId)
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("Player prefab not assigned!");
            return;
        }
        
        GameObject player = Instantiate(playerPrefab);
        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        
        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId, true);
            Debug.Log($"Player spawned for client {clientId}");
        }
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected");
        
        if (IsServer)
        {
            UpdateStatus($"Clients connected: {NetworkManager.Singleton.ConnectedClients.Count}");
        }
        else if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateStatus("Disconnected from server");
        }
    }
    
    public void Disconnect()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Host shutdown");
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Client disconnected");
            }
        }
        
        UpdateStatus("Disconnected");
    }
    
    private void UpdateStatus(string message)
    {
        if (connectionStatusText != null)
        {
            connectionStatusText.text = $"Status: {message}";
        }
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }
}

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private CharacterController characterController;
    private Vector3 moveDirection;
    
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        if (IsLocalPlayer)
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 1, -3);
            
            Debug.Log("Local player initialized");
        }
    }
    
    private void Update()
    {
        if (!IsLocalPlayer)
            return;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed;
        
        moveDirection.y += Physics.gravity.y * Time.deltaTime;
        
        if (characterController != null)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"Player spawned with ID: {OwnerClientId}");
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log($"Player despawned: {OwnerClientId}");
    }
}