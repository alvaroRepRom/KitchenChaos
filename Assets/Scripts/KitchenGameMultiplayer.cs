using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;

    public static KitchenGameMultiplayer Instance { get; private set; }


    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;


    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
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

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback( ulong clientId )
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

}
