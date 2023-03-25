using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;

    public static KitchenGameMultiplayer Instance { get; private set; }


    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;


    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<Color> playerColorList;


    private NetworkList<PlayerData> playerDataNetworkList;


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged( NetworkListEvent<PlayerData> changeEvent )
    {
        OnPlayerDataNetworkListChanged?.Invoke( this , EventArgs.Empty );
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback( ulong clientId )
    {
        for ( int i = 0; i < playerDataNetworkList.Count; i++ )
        {
            PlayerData playerData = playerDataNetworkList[i];
            if ( playerData.clientId == clientId )
            {
                // Disconnected
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback( ulong clientId )
    {
        playerDataNetworkList.Add( new PlayerData() {
            clientId = clientId,
            colorId = GetFirstUnsedColorId()
        } );        
    }

    private void NetworkManager_ConnectionApprovalCallback( NetworkManager.ConnectionApprovalRequest connectionApprovalRequest , NetworkManager.ConnectionApprovalResponse connectionApprovalResponse )
    {
        if ( SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString() )
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game already Started";
            return;
        }

        if ( NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT )
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is Full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke( this , EventArgs.Empty );

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback( ulong clientId )
    {
        OnFailedToJoinGame?.Invoke( this, EventArgs.Empty );
    }

    public void SpawnKitchenObject( KitchenObjectSO kitchenObjectSO , IKitchenObjectParent kitchenObjectParent )
    {
        SpawnKitchenObjectServerRpc( GetKitchenObjectSOIndex( kitchenObjectSO ) , kitchenObjectParent.GetNetworkObject() );
    }




    [ServerRpc( RequireOwnership = false )]
    private void SpawnKitchenObjectServerRpc( int kitchenObjectSOIndex , NetworkObjectReference kitchenObjectParentObjectReference )
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex( kitchenObjectSOIndex );

        Transform kitchenObjectTransform = Instantiate( kitchenObjectSO.prefab );

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn( true );

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();


        kitchenObjectParentObjectReference.TryGet( out NetworkObject kitchenObjectParentNetworkObject );
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent( kitchenObjectParent );
    }





    public int GetKitchenObjectSOIndex( KitchenObjectSO kitchenObjectSO )
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf( kitchenObjectSO );
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex( int index )
    {
        return kitchenObjectListSO.kitchenObjectSOList[index];
    }



    public void DestroyKitchenObject( KitchenObject kitchenObject )
    {
        DestroyKitchenObjectServerRpc( kitchenObject.NetworkObject );
    }


    [ServerRpc( RequireOwnership = false )]
    private void DestroyKitchenObjectServerRpc( NetworkObjectReference kitchenObjectObjectReference )
    {
        kitchenObjectObjectReference.TryGet( out NetworkObject kitchenObjectNetworkObject );
        KitchenObject kitchenObject =kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRpc( kitchenObjectObjectReference );

        kitchenObject.DestroySelf();
    }


    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc( NetworkObjectReference kitchenObjectObjectReference )
    {
        kitchenObjectObjectReference.TryGet( out NetworkObject kitchenObjectNetworkObject );
        KitchenObject kitchenObject =kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }

    public int GetPlayerDataIndexFromClientId( ulong playerId )
    {
        for ( int i = 0; i < playerDataNetworkList.Count; i++ )
            if ( playerDataNetworkList[i].clientId == playerId )
                return i;
        return -1;
    }


    public PlayerData GetPlayerDataFromClientId( ulong playerId )
    {
        foreach ( PlayerData playerData in playerDataNetworkList )
            if ( playerData.clientId == playerId )
                return playerData;
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId( NetworkManager.Singleton.LocalClientId );
    }


    public bool IsPlayerIndexConnected( int index )
    {
        return index < playerDataNetworkList.Count;
    }


    public PlayerData GetPlayerDataFromPlayerIndex( int playerIndex )
    {
        return playerDataNetworkList[playerIndex];
    }


    public Color GetPlayerColor( int colorId )
    {
        return playerColorList[colorId];
    }


    public void ChangePlayerColor( int colorId )
    {
        ChagePlayerColorServerRpc( colorId );
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChagePlayerColorServerRpc( int colorId, ServerRpcParams serverRpcParams = default )
    {
        if ( !IsColorAvaible(colorId ) )
            return;

        int playerDataIndex = GetPlayerDataIndexFromClientId( serverRpcParams.Receive.SenderClientId );

        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvaible( int colorId )
    {
        foreach(PlayerData playerData in playerDataNetworkList )
            if ( playerData.colorId == colorId )
                return false;
        return true;
    }

    private int GetFirstUnsedColorId()
    {
        for(int i = 0; i < playerColorList.Count; i++)
            if ( IsColorAvaible(i) )
                return i;
        return -1;
    }


    public void KickPlayer( ulong clientId )
    {
        NetworkManager.Singleton.DisconnectClient( clientId );
        NetworkManager_Server_OnClientDisconnectCallback( clientId );
    }

}
