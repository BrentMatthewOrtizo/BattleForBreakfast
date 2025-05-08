using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rigidBody;
    private Vector2 moveInput;
    private Animator animator;
    private Camera mainCamera;

    private bool canMove = true; // Flag to control if player can move (e.g., restricted during attack)

    void Awake() // Using Awake to ensure components are ready for PlayerAttack's Awake
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        if (animator == null)
        {
            Debug.LogError("PlayerMovement: Animator component not found!");
        }
        if (mainCamera == null)
        {
            Debug.LogError("PlayerMovement: Main Camera not found! Tag camera as 'MainCamera'.");
        }
    }

    void Update()
    {
        if (canMove)
        {
            rigidBody.linearVelocity = moveInput * moveSpeed; // Apply physics-based movement

            // If no keyboard/gamepad input AND movement is allowed, aim towards the mouse
            if (moveInput == Vector2.zero && !animator.GetBool("isWalking")) // isWalking check helps ensure it's truly idle
            {
                UpdateAimDirectionTowardsMouse();
            }
        }
        else
        {
            // If movement is disabled (e.g., during an attack), ensure velocity is zero.
            rigidBody.linearVelocity = Vector2.zero;
        }
    }

    // Updates LastInputX/Y based on mouse direction. Called for idle aiming and before attacks.
    public void UpdateAimDirectionTowardsMouse()
    {
        if (mainCamera == null || animator == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));
        Vector2 directionToMouse = (mouseWorldPos - transform.position).normalized;

        if (directionToMouse.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("LastInputX", directionToMouse.x);
            animator.SetFloat("LastInputY", directionToMouse.y);
        }
    }

    // Called by the Player Input component when the "Move" action is performed
    public void Move(InputAction.CallbackContext context)
    {
        if (animator == null) return;

        moveInput = context.ReadValue<Vector2>();

        if (canMove) // Only update walking state and animation if movement is allowed
        {
            if (moveInput != Vector2.zero)
            {
                animator.SetBool("isWalking", true);
                animator.SetFloat("InputX", moveInput.x);    // For walk animation blend tree
                animator.SetFloat("InputY", moveInput.y);
                animator.SetFloat("LastInputX", moveInput.x); // Also set LastInput for facing during movement
                animator.SetFloat("LastInputY", moveInput.y);
            }
            else
            {
                animator.SetBool("isWalking", false);
                // Idle aiming (UpdateAimDirectionTowardsMouse) will be handled by Update()
            }
        }
        else // If canMove is false (e.g., during an attack)
        {
            // Ensure the player is not considered "walking" by the animator
            // even if movement keys are held down during the attack.
            animator.SetBool("isWalking", false);
        }
    }

    // Public method to allow other scripts (like PlayerAttack) to control movement
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled)
        {
            // If disabling movement, immediately stop physical movement
            rigidBody.linearVelocity = Vector2.zero;
            // Also ensure the "isWalking" animation state is false, as we've programmatically stopped movement.
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }
        // When movement is re-enabled, the next 'Move' input callback or 'Update' frame
        // will correctly determine the 'isWalking' state based on 'moveInput'.
    }
}