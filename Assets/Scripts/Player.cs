using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPickSomething;
    public event EventHandler<OnSelectedCounterChangedEventsArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventsArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private Vector3 lastInteractDir;
    private bool isWalking;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null )
        {
            Debug.LogError( "More than one player instante" );
        }
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        gameInput.OnInteractAction += GameInputOnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void OnDisable()
    {
        gameInput.OnInteractAction -= GameInputOnInteractAction;
        gameInput.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction( object sender , EventArgs e )
    {
        if ( selectedCounter != null )
        {
            selectedCounter.InteractAlternate( this );
        }
    }

    private void GameInputOnInteractAction( object sender , System.EventArgs e )
    {
        if (selectedCounter != null )
        {
            selectedCounter.Interact( this );
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }


    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3( inputVector.x, 0f, inputVector.y);

        if ( moveDir != Vector3.zero )
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if ( Physics.Raycast( transform.position , lastInteractDir , out RaycastHit hit , interactDistance , counterLayerMask ) )
        {
            if ( hit.transform.TryGetComponent( out BaseCounter baseCounter ) )
            {
                if (baseCounter != selectedCounter )
                {
                    SetSelectedCounter( baseCounter );
                }
            }
            else
            {
                SetSelectedCounter( null );
            }
        }
        else
        {
            SetSelectedCounter( null );
        }
    }


    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3( inputVector.x, 0f, inputVector.y);

        float moveDistance = Time.deltaTime * moveSpeed;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast( transform.position , transform.position + Vector3.up * playerHeight , playerRadius , moveDir, moveDistance );

        // If diagonal input, check movement on one axis
        if ( !canMove )
        {
            // Attempt to move on X
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast( transform.position , transform.position + Vector3.up * playerHeight , playerRadius , moveDirX , moveDistance );

            if ( canMove )
            {
                moveDir = moveDirX;
            }
            else
            {
                // Attempt to move on Z
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast( transform.position , transform.position + Vector3.up * playerHeight , playerRadius , moveDirZ , moveDistance );

                if ( canMove )
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if ( canMove )
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp( transform.forward , moveDir , Time.deltaTime * rotateSpeed );
    }

    private void SetSelectedCounter( BaseCounter baseCounter )
    {
        this.selectedCounter = baseCounter;

        OnSelectedCounterChanged?.Invoke( this , new OnSelectedCounterChangedEventsArgs
        {
            selectedCounter = baseCounter
        } );
    }

    public Transform GetKitchenFollowObjectTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject( KitchenObject kitchenObject )
    {
        this.kitchenObject = kitchenObject;

        if ( kitchenObject != null )
            OnPickSomething?.Invoke( this , EventArgs.Empty );
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
}
