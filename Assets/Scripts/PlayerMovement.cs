using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rigidBody;
    private Vector2 moveInput;
    private Animator animator;
    private Camera mainCamera;

    private bool canMove = true; // Controls if movement logic is active (e.g., restricted during attacks)

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        if (SceneManager.GetActiveScene().name == "BeforeGameScene")
        {
            
        }
    }

    void Update()
    {
        if (canMove)
        {
            rigidBody.linearVelocity = moveInput * moveSpeed;

            // Aim towards mouse when idle (no keyboard/gamepad input)
            if (moveInput == Vector2.zero && !animator.GetBool("isWalking"))
            {
                UpdateAimDirectionTowardsMouse();
            }
        }
        else
        {
            rigidBody.linearVelocity = Vector2.zero; // Stop movement if disabled
        }
    }

    // Updates Animator's LastInputX/Y based on mouse position for aiming
    public void UpdateAimDirectionTowardsMouse()
    {
        if (mainCamera == null || animator == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        // Convert screen mouse coords to world coords; Z-depth is distance from camera to game plane
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));
        Vector2 directionToMouse = (mouseWorldPos - transform.position).normalized;

        if (directionToMouse.sqrMagnitude > 0.01f) // Avoid jitter if mouse is very close to player center
        {
            animator.SetFloat("LastInputX", directionToMouse.x);
            animator.SetFloat("LastInputY", directionToMouse.y);
        }
    }

    // Handles movement input from the Input System
    public void Move(InputAction.CallbackContext context)
    {
        if (animator == null) return;
        moveInput = context.ReadValue<Vector2>();

        if (canMove)
        {
            bool isTryingToMove = moveInput != Vector2.zero;
            animator.SetBool("isWalking", isTryingToMove);

            if (isTryingToMove)
            {
                animator.SetFloat("InputX", moveInput.x);    // For walking animation blend tree
                animator.SetFloat("InputY", moveInput.y);
                animator.SetFloat("LastInputX", moveInput.x); // Face movement direction
                animator.SetFloat("LastInputY", moveInput.y);
            }
            // If not tryingToMove, idle aiming is handled by Update()
        }
        else // If canMove is false (e.g., during an attack)
        {
            animator.SetBool("isWalking", false); // Ensure not in walking animation state
        }
    }

    // Allows other scripts to enable/disable player movement
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
        {
            rigidBody.linearVelocity = Vector2.zero; // Immediately stop physical movement
            if (animator != null)
            {
                animator.SetBool("isWalking", false); // Ensure walking animation stops
            }
        }
    }
}