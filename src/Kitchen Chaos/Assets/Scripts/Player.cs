using UnityEngine;
using System;

public class Player : MonoBehaviour , IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private Transform KitchenObjectHoldPoint;
    [SerializeField] private LayerMask countersLayerMask;

    // Helper Variables
    RaycastHit raycastHit;
    private bool isWalking;
    private Vector3 lastinteractionDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance in the scene!");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (!IsInteractionAllowed()) return;
        if (selectedCounter == null) return;
        if (TutorialManager.Instance != null && !TutorialManager.Instance.CanInteractAlternate(selectedCounter)) return;

        selectedCounter.InteractAlternate(this);
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!IsInteractionAllowed()) return;
        if (selectedCounter == null) return;
        if (TutorialManager.Instance != null && !TutorialManager.Instance.CanInteract(selectedCounter)) return;

        selectedCounter.Interact(this);
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }
    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction() 
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDirection != Vector3.zero)
        {
            lastinteractionDirection = moveDirection;
        }

        if (Physics.Raycast(transform.position, lastinteractionDirection, out raycastHit, interactionDistance, countersLayerMask))
        {
           if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                    
                }
            } else 
                {
                    SetSelectedCounter(null);
                }
            } else 
                {
                    SetSelectedCounter(null);
                }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        float moveDistance = moveSpeed * Time.deltaTime;
        
        // Initial check: Can we move in the intended direction?
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance);

        if (!canMove)
        {
            // Cannot move towards moveDirection, try sliding along X
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0).normalized;
            // Check if movement is significant enough to bother casting (prevents jitter)
            canMove = moveDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Slide on X
                moveDirection = moveDirX;
            }
            else
            {
                // Cannot move on X, try sliding along Z
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = moveDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Slide on Z
                    moveDirection = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDirection * moveDistance;
        }

        isWalking = moveDirection != Vector3.zero;

        float rotationSpeed = 10f;
        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)

    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
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

    private bool IsInteractionAllowed()
    {
        if (TutorialManager.Instance != null) return TutorialManager.Instance.IsActive;
        return KitchenGameManager.Instance.IsGamePlaying();
    }
}