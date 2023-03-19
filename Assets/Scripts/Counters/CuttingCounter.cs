using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler OnCut;
    public event EventHandler<IHasProgress.OnProgressChangedEventsArgs> OnProgressChanged;


    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Interact( Player player )
    {
        if ( !HasKitchenObject() ) // counter doesn't have anything on top
        {
            if ( player.HasKitchenObject() )
            {
                if ( HasRecipeWithInput( player.GetKitchenObject().GetKitchenObjectSO() ) )
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();

                    kitchenObject.SetKitchenObjectParent( this );

                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
        }
        else // counter has something on top
        {
            if ( player.HasKitchenObject() )
            {
                if ( player.GetKitchenObject().TryGetPlate( out PlateKitchenObject plateKitchenObject ) )
                {
                    if ( plateKitchenObject.TryAddIngredient( GetKitchenObject().GetKitchenObjectSO() ) )
                    {
                        KitchenObject.DestroyKitchenObject( GetKitchenObject() );
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent( player );
            }
        }
    }



    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }


    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = 0f
        } );
    }



    public override void InteractAlternate( Player player )
    {
        if ( HasKitchenObject() && HasRecipeWithInput( GetKitchenObject().GetKitchenObjectSO() ) ) 
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }



    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }


    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        OnCut?.Invoke( this , EventArgs.Empty );
        OnAnyCut?.Invoke( this , EventArgs.Empty );

        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeSO( GetKitchenObject().GetKitchenObjectSO() );

        OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = ( float )cuttingProgress / cuttingRecipe.cuttingProgressMax
        } );
    }



    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeSO( GetKitchenObject().GetKitchenObjectSO() );

        if ( cuttingProgress >= cuttingRecipe.cuttingProgressMax )
        {
            KitchenObjectSO outputKitchenObjectSO = GetOutputFromInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject( GetKitchenObject() );

            KitchenObject.SpawnKitchenObject( outputKitchenObjectSO , this );
        }
    }



    private bool HasRecipeWithInput( KitchenObjectSO inputKitchenObject )
    {
        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeSO( inputKitchenObject );
        return cuttingRecipe != null;
    }

    private KitchenObjectSO GetOutputFromInput( KitchenObjectSO inputKitchenObject )
    {
        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeSO( inputKitchenObject );
        return cuttingRecipe ? cuttingRecipe.output : null;
    }


    private CuttingRecipeSO GetCuttingRecipeSO( KitchenObjectSO inputKitchenObjectSO )
    {
        foreach ( CuttingRecipeSO cuttingRecipe in cuttingRecipeSOArray )
            if ( cuttingRecipe.input == inputKitchenObjectSO )
                return cuttingRecipe;
        return null;
    }
}
