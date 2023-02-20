using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if ( spawnRecipeTimer <= 0 )
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax )
            {
                RecipeSO recipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range( 0 , recipeListSO.recipeSOList.Count )];
                waitingRecipeSOList.Add( recipeSO );

                OnRecipeSpawned?.Invoke( this, EventArgs.Empty );
            }
        }
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
                    waitingRecipeSOList.RemoveAt( i );
                    OnRecipeCompleted?.Invoke( this , EventArgs.Empty );
                    return;
                }
            }
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }
}
