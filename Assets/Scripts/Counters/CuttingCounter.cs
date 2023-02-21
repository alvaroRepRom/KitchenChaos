using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

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
                    player.GetKitchenObject().SetKitchenObjectParent( this );
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipe = GetCuttingRecipeSO( GetKitchenObject().GetKitchenObjectSO() );

                    OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipe.cuttingProgressMax
                    } );
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
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent( player );
            }
        }
    }

    public override void InteractAlternate( Player player )
    {
        if ( HasKitchenObject() && HasRecipeWithInput( GetKitchenObject().GetKitchenObjectSO() ) ) 
        {
            cuttingProgress++;

            OnCut?.Invoke( this, EventArgs.Empty );
            OnAnyCut?.Invoke( this, EventArgs.Empty );

            CuttingRecipeSO cuttingRecipe = GetCuttingRecipeSO( GetKitchenObject().GetKitchenObjectSO() );

            OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
            {
                progressNormalized = ( float )cuttingProgress / cuttingRecipe.cuttingProgressMax
            } );

            if ( cuttingProgress >= cuttingRecipe.cuttingProgressMax )
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputFromInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject( outputKitchenObjectSO , this );
            }

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
