using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive( false );
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded( object sender , PlateKitchenObject.OnIngredientAddedEventArgs e )
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach ( Transform transformChild in transform )
        {
            if ( transformChild == iconTemplate )
                continue;
            Destroy( transformChild.gameObject );
        }

        foreach ( KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList() )
        {
            Transform iconTransform = Instantiate( iconTemplate , transform );
            iconTransform.gameObject.SetActive( true );
            iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO( kitchenObjectSO );
        }
    }
}
