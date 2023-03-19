using System;
using Unity.Netcode;
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

    private NetworkVariable<State> state = new NetworkVariable<State>( State.Idle );
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);


    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged( float previusValue, float newValue )
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;

        OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        } );
    }
    
    private void BurningTimer_OnValueChanged( float previusValue, float newValue )
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;

        OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        } );
    }
    
    private void State_OnValueChanged( State previusState, State newState )
    {
        OnStateChanged?.Invoke( this , new OnStateChangedEventArgs { state = state.Value } );

        if (state.Value == State.Burned || state.Value == State.Idle )
        {
            OnProgressChanged?.Invoke( this , new IHasProgress.OnProgressChangedEventsArgs
            {
                progressNormalized = 0
            } );
        }
    }


    private void Update()
    {
        if ( !IsServer ) return;

        if ( HasKitchenObject() )
        {
            switch ( state.Value )
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    

                    if ( fryingTimer.Value > fryingRecipeSO.fryingTimerMax )
                    {
                        // Fried
                        KitchenObject.DestroyKitchenObject( GetKitchenObject() );

                        KitchenObject.SpawnKitchenObject( fryingRecipeSO.output , this );

                        state.Value = State.Fried;
                        burningTimer.Value = 0;

                        SetBurningingRecipeSOClientRpc( 
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex( GetKitchenObject().GetKitchenObjectSO() )
                        );                       
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    if ( burningTimer.Value > burningRecipeSO.burningTimerMax )
                    {
                        // Burned
                        KitchenObject.DestroyKitchenObject( GetKitchenObject() );

                        KitchenObject.SpawnKitchenObject( burningRecipeSO.output , this );

                        state.Value = State.Burned;                        
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
                    KitchenObject kitchenObject = player.GetKitchenObject();

                    kitchenObject.SetKitchenObjectParent( this );

                    InteractLogicPlaceObjectCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex( kitchenObject.GetKitchenObjectSO() )
                    );                    
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

                        SetStateIdleServerRpc();
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent( player );
                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }



    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectCounterServerRpc( int kitchenObjectSOIndex )
    {
        fryingTimer.Value = 0;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc( kitchenObjectSOIndex );
    }
    
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc( int kitchenObjectSOIndex )
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex( kitchenObjectSOIndex );

        fryingRecipeSO = GetFryingRecipeSOWithInput( kitchenObjectSO );        
    }
    
    [ClientRpc]
    private void SetBurningingRecipeSOClientRpc( int kitchenObjectSOIndex )
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex( kitchenObjectSOIndex );

        burningRecipeSO = GetBurningRecipeSOWithInput( kitchenObjectSO );        
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


    public bool IsFryied() => state.Value == State.Fried;
}
