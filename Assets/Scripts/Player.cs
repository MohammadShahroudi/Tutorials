using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
	public static Player Instance { get; private set; }

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

	public class OnSelectedCounterChangedEventArgs : EventArgs {
		public BaseCounter selectedCounter;
	}

	[SerializeField] private float moveSpeed = 7.0f;
	[SerializeField] private GameInput gameInput;
	[SerializeField] private LayerMask countersLayerMask;
	[SerializeField] private Transform kitchenObjectHoldPoint;
	
	private bool isWalking;
	private Vector3 lastInteractDirection;
	private BaseCounter selectedCounter;
	private KitchenObject kitchenObject;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There is more than one Player instance");
		}
		Instance = this;
	}

	private void Start()
	{
		gameInput.OnInteractAction += GameInput_OnInteractAction;
		gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
	}

	private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
	{
		if (selectedCounter != null)
		{
			selectedCounter.InteractAlternate(this);
		}
	}

	private void GameInput_OnInteractAction(object sender, System.EventArgs e)
	{
		if (selectedCounter != null)
		{
			selectedCounter.Interact(this);
		}
	}

	private void Update()
	{
		HandleMovement();
		HandleInteractions();
	}

	public bool IsWalking()
	{
		return isWalking;
	}

	private void HandleInteractions()
	{
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();

		Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

		if (moveDirection != Vector3.zero)
		{
			lastInteractDirection = moveDirection;
		}

		float interactDistance = 2f;
		if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
		{
			// Debug.Log(raycastHit.transform);
			if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
			{
				// Has ClearCounter
				if (baseCounter != selectedCounter)
				{
					SetSelectedCounter(baseCounter);
				}
			}
			else
			{
				SetSelectedCounter(null);
			}
		}
		else
		{
			SetSelectedCounter(null);
		}
	}

	private void HandleMovement()
	{
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();

		Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

		float moveDistance = moveSpeed * Time.deltaTime;
		float playerRadius = .7f;
		float playerHeight = 2.0f;
		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
			playerRadius, moveDirection, moveDistance);

		if (!canMove)
		{
			// Cannot move towards moveDirection

			// Attempt only X movement
			Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
			canMove = moveDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
				playerRadius, moveDirectionX, moveDistance);

			if (canMove)
			{
				// Can move only on the X
				moveDirection = moveDirectionX;
			}
			else
			{
				// Cannot move only on the X

				// Attempt only Z movement
				Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
				canMove = moveDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
					playerRadius, moveDirectionZ, moveDistance);

				if (canMove)
				{
					// can move only on the Z
					moveDirection = moveDirectionZ;
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

		float rotateSpeed = 10.0f;
		transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

		// Debug.Log(Time.deltaTime);
	}

	private void SetSelectedCounter(BaseCounter selectedCounter)
	{
		this.selectedCounter = selectedCounter;

		OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { 
			selectedCounter = selectedCounter
		});
	}
	
	public Transform GetKitchenObjectFollowTransform()
	{
		return kitchenObjectHoldPoint;
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;
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
