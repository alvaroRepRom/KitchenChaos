using System;
using UnityEngine;
using Unity.Netcode;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabObject;


    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact( Player player )
    {
        if ( !player.HasKitchenObject() )
        {
            KitchenObject.SpawnKitchenObject( kitchenObjectSO , player );

            InteractLogicServerRpc();
        }
    }


    [ServerRpc( RequireOwnership = false )]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabObject?.Invoke( this , EventArgs.Empty );
    }

}
