using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfullRecipesAmount;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if ( !IsServer ) return;

        spawnRecipeTimer -= Time.deltaTime;
        if ( spawnRecipeTimer <= 0 )
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if ( KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax )
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range( 0 , recipeListSO.recipeSOList.Count );
                SpawnNewWaitingRecipeClientRpc( waitingRecipeSOIndex );
            }
        }
    }


    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc( int waitingRecipeSOIndex )
    {

        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add( waitingRecipeSO );

        OnRecipeSpawned?.Invoke( this , EventArgs.Empty );
    }


    public void DeliverRecipe( PlateKitchenObject plateKitchenObject )
    {
        for ( int i = 0; i < waitingRecipeSOList.Count; i++ )
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // if has same number of ingredients
            if ( plateKitchenObject.GetKitchenObjectSOList().Count == waitingRecipeSO.kitchenObjectSOList.Count )
            {
                bool plateContentsMatchRecipe = true;
                foreach ( KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList )
                {
                    bool ingredientFound = false;
                    foreach ( KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList() )
                    {
                        if ( plateKitchenObjectSO == recipeKitchenObjectSO )
                        {
                            // ingredient matches
                            ingredientFound = true;
                            break;
                        }
                    }
                    if ( !ingredientFound )
                    {
                        plateContentsMatchRecipe = false;
                        break;
                    }
                }
                if ( plateContentsMatchRecipe )
                {
                    DeliveredCorrectRecipeServerRpc( i );
                    return;
                }
            }
        }

        DeliveredIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveredIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }


    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke( this , EventArgs.Empty );
    }


    [ServerRpc( RequireOwnership = false )]
    private void DeliveredCorrectRecipeServerRpc( int waitingRecipeSOIndex )
    {
        DeliverCorrectRecipeClientRpc( waitingRecipeSOIndex );
    }


    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc( int waitingRecipeSOIndex )
    {
        waitingRecipeSOList.RemoveAt( waitingRecipeSOIndex );

        successfullRecipesAmount++;

        OnRecipeCompleted?.Invoke( this , EventArgs.Empty );
        OnRecipeSuccess?.Invoke( this , EventArgs.Empty );
    }


    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfullRecipesAmount() => successfullRecipesAmount;
}
