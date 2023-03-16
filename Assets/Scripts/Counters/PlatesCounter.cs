using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawn;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnAmount;
    private int platesSpawnAmountMax = 4;

    private void Update()
    {
        if ( !IsServer ) return;

        spawnPlateTimer += Time.deltaTime;
        if ( spawnPlateTimer > spawnPlateTimerMax )
        {
            spawnPlateTimer = 0;

            if ( KitchenGameManager.Instance.IsGamePlaying() && platesSpawnAmount < platesSpawnAmountMax )
            {
                SpawnPlateServerRpc();
            }
        }
    }


    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }
    
    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnAmount++;

        OnPlateSpawn?.Invoke( this , EventArgs.Empty );
    }



    public override void Interact( Player player )
    {
        if ( !player.HasKitchenObject() )
        {
            if (platesSpawnAmount > 0 )
            {
                KitchenObject.SpawnKitchenObject( kitchenObjectSO , player );
                InteractLogicServerRpc();
            }
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
        platesSpawnAmount--;
        OnPlateRemoved?.Invoke( this , EventArgs.Empty );
    }


}
