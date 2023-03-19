using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact( Player player )
    {
        if ( !HasKitchenObject() ) // counter doesn't have anything on top
        {
            if ( player.HasKitchenObject() )
            {
                player.GetKitchenObject().SetKitchenObjectParent( this );
            }
        }
        else // counter has something on top
        {
            if ( player.HasKitchenObject() )
            {
                if ( player.GetKitchenObject().TryGetPlate( out PlateKitchenObject plateKitchenObject) )
                {
                    if ( plateKitchenObject.TryAddIngredient( GetKitchenObject().GetKitchenObjectSO() ) )
                    {
                        KitchenObject.DestroyKitchenObject( GetKitchenObject() );
                    }
                }
                else // if not holding a plate but something else
                {
                    if ( GetKitchenObject().TryGetPlate( out plateKitchenObject ) )
                    {
                        if ( plateKitchenObject.TryAddIngredient( player.GetKitchenObject().GetKitchenObjectSO() ) )
                        {
                            KitchenObject.DestroyKitchenObject( player.GetKitchenObject() );
                        }
                    }
                }
            }
            else // if player has nothing then take from counter
            {
                GetKitchenObject().SetKitchenObjectParent( player );
            }
        }
    }
}
