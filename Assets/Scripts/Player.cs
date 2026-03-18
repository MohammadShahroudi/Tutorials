using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

	public class OnSelectedCounterChangedEventArgs : EventArgs
	{
		public ClearCounter selectedCounter;
	}

	[SerializeField] private float moveSpeed = 7.0f;
	[SerializeField] private GameInput gameInput;
	[SerializeField] private LayerMask countersLayerMask;

	private bool isWalking;
	private Vector3 lastInteractDirection;
	private ClearCounter selectedCounter;

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
		if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance,
			    countersLayerMask))
		{
			// Debug.Log(raycastHit.transform);
			if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
			{
				// Has ClearCounter
				if (clearCounter != selectedCounter)
				{
					SetSelectedCounter(clearCounter);
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
			canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
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
				canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
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

	private void SetSelectedCounter(ClearCounter selectedCounter)
	{
		this.selectedCounter = selectedCounter;

		//OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
		//{
		//	selectedCounter = selectedCounter;
		//});
	}
}
