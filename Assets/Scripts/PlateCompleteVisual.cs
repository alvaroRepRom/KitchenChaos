using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectsList;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        foreach ( var kitchenObjectSO in kitchenObjectSOGameObjectsList )
        {
            kitchenObjectSO.gameObject.SetActive( false );
        }
    }

    private void PlateKitchenObject_OnIngredientAdded( object sender , PlateKitchenObject.OnIngredientAddedEventArgs e )
    {
        foreach ( var kitchenObjectSO in kitchenObjectSOGameObjectsList )
        {
            if ( kitchenObjectSO.kitchenObjectSO == e.kitchenObjectSO )
            {
                kitchenObjectSO.gameObject.SetActive( true );
            }
        }
    }
}
