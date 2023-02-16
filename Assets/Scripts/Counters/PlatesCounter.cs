using System;
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
        spawnPlateTimer += Time.deltaTime;
        if ( spawnPlateTimer > spawnPlateTimerMax )
        {
            spawnPlateTimer = 0;

            if ( platesSpawnAmount < platesSpawnAmountMax )
            {
                platesSpawnAmount++;

                OnPlateSpawn?.Invoke( this, EventArgs.Empty );
            }
        }
    }

    public override void Interact( Player player )
    {
        if ( !player.HasKitchenObject() )
        {
            if (platesSpawnAmount > 0 )
            {
                platesSpawnAmount--;

                KitchenObject.SpawnKitchenObject( kitchenObjectSO , player );

                OnPlateRemoved?.Invoke( this , EventArgs.Empty );
            }
        }
    }
}
