using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    private FollowTransform followTranstorm;



    private void Awake()
    {
        followTranstorm = GetComponent<FollowTransform>();
    }



    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }



    public void SetKitchenObjectParent( IKitchenObjectParent kitchenObjectParent )
    {
        SetKitchenObjectParentServerRpc( kitchenObjectParent.GetNetworkObject() );
    }




    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc( NetworkObjectReference kitchenObjectParentObjectReference )
    {
        SetKitchenObjectParentClientRpc( kitchenObjectParentObjectReference );
    }



    [ClientRpc]
    private void SetKitchenObjectParentClientRpc( NetworkObjectReference kitchenObjectParentObjectReference )
    {
        kitchenObjectParentObjectReference.TryGet( out NetworkObject kitchenObjectParentNetworkObject );
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if ( this.kitchenObjectParent != null )
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if ( kitchenObjectParent.HasKitchenObject() )
        {
            Debug.LogError( "already have a kitchen object" );
        }

        kitchenObjectParent.SetKitchenObject( this );


        followTranstorm.SetParentTransform( kitchenObjectParent.GetKitchenFollowObjectTransform() );
    }




    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()
    {
        Destroy( gameObject );
    }


    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }




    public bool TryGetPlate( out PlateKitchenObject plateKitchenObject)
    {
        if ( this is PlateKitchenObject )
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        plateKitchenObject = null;
        return false;
    }


    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent )
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject( kitchenObjectSO , kitchenObjectParent );
    }

    public static void DestroyKitchenObject( KitchenObject kitchenObject )
    {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject( kitchenObject );
    }

}
