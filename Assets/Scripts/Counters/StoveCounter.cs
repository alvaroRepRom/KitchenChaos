using System;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventsArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private float fryingTimer;
    private float burningTimer;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {

        if ( HasKitchenObject() )
        {
            switch ( state )
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    } );

                    if ( fryingTimer > fryingRecipeSO.fryingTimerMax )
                    {
                        // Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject( fryingRecipeSO.output , this );

                        state = State.Fried;
                        burningTimer = 0;

                        burningRecipeSO = GetBurningRecipeSOWithInput( GetKitchenObject().GetKitchenObjectSO() );

                        OnStateChanged?.Invoke( this , new OnStateChangedEventArgs { state = state } );
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    } );

                    if ( burningTimer > burningRecipeSO.burningTimerMax )
                    {
                        // Burned
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject( burningRecipeSO.output , this );

                        state = State.Burned;
                        OnStateChanged?.Invoke( this , new OnStateChangedEventArgs { state = state } );

                        OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
                        {
                            progressNormalized = 0
                        } );
                    }
                    break;
                case State.Burned:
                    break;
            }            
        }
    }

    public override void Interact( Player player )
    {
        if ( !HasKitchenObject() ) // counter doesn't have anything on top
        {
            if ( player.HasKitchenObject() )
            {
                if ( HasRecipeWithInput( player.GetKitchenObject().GetKitchenObjectSO() ) )
                {
                    player.GetKitchenObject().SetKitchenObjectParent( this );
                    fryingRecipeSO = GetFryingRecipeSOWithInput( GetKitchenObject().GetKitchenObjectSO() );

                    state = State.Frying;
                    fryingTimer = 0;

                    OnStateChanged?.Invoke( this , new OnStateChangedEventArgs { state = state } );

                    OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
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

                        state = State.Idle;

                        OnStateChanged?.Invoke( this , new OnStateChangedEventArgs { state = state } );

                        OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
                        {
                            progressNormalized = 0
                        } );
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent( player );
                state = State.Idle;

                OnStateChanged?.Invoke( this , new OnStateChangedEventArgs { state = state } );

                OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
                {
                    progressNormalized = 0
                } );
            }
        }
    }

    private bool HasRecipeWithInput( KitchenObjectSO inputKitchenObject )
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput( inputKitchenObject );
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputFromInput( KitchenObjectSO inputKitchenObject )
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput( inputKitchenObject );
        return fryingRecipeSO ? fryingRecipeSO.output : null;
    }


    private FryingRecipeSO GetFryingRecipeSOWithInput( KitchenObjectSO inputKitchenObjectSO )
    {
        foreach ( FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray )
            if ( fryingRecipeSO.input == inputKitchenObjectSO )
                return fryingRecipeSO;
        return null;
    }
    
    private BurningRecipeSO GetBurningRecipeSOWithInput( KitchenObjectSO inputKitchenObjectSO )
    {
        foreach ( BurningRecipeSO burningRecipeSO in burningRecipeSOArray )
            if ( burningRecipeSO.input == inputKitchenObjectSO )
                return burningRecipeSO;
        return null;
    }


    public bool IsFryied() => state == State.Fried;
}
