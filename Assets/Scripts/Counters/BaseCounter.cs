using System;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlaceHere;

    public static void ResetStaticData()
    {
        OnAnyObjectPlaceHere = null;
    }

    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public virtual void Interact( Player player ) { Debug.LogError( "base Counter has interact" ); }
    public virtual void InteractAlternate( Player player ) { }//Debug.LogError( "base Counter has interact alternate" ); }

    public void SetKitchenObject( KitchenObject kitchenObject )
    {
        this.kitchenObject = kitchenObject;

        if ( kitchenObject != null )
            OnAnyObjectPlaceHere?.Invoke( this , EventArgs.Empty );
    }


    public Transform GetKitchenFollowObjectTransform()
    {
        return counterTopPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
