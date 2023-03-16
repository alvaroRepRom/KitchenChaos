using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{ 
    public static KitchenGameMultiplayer Instance { get; private set; }


    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;


    private void Awake()
    {
        Instance = this;
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





    private int GetKitchenObjectSOIndex( KitchenObjectSO kitchenObjectSO )
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf( kitchenObjectSO );
    }

    private KitchenObjectSO GetKitchenObjectSOFromIndex( int index )
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
