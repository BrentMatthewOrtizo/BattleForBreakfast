using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Added: Player direction follows cursor
    public float moveSpeed = 5f;

    private Rigidbody2D rigidBody;
    private Vector2 moveInput;
    private Animator animator;
    private Camera mainCamera; // For mouse-to-world position conversion

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main; // Cache main camera

        if (mainCamera == null)
        {
            Debug.LogError("PlayerMovement: Main Camera not found! Tag camera as 'MainCamera'.");
        }
    }

    void Update()
    {
        rigidBody.linearVelocity = moveInput * moveSpeed; // Apply movement

        // If idle, aim towards the mouse cursor
        if (moveInput == Vector2.zero && !animator.GetBool("isWalking"))
        {
            AimAtMouse();
        }
    }

    // Makes the player face the mouse cursor
    private void AimAtMouse()
    {
        if (mainCamera == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        // Convert screen mouse coords to world coords. Z-depth is distance from camera to game plane.
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));

        Vector2 directionToMouse = (mouseWorldPos - transform.position).normalized;

        // Update animator for idle facing direction, avoid jitter near player center
        if (directionToMouse.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("LastInputX", directionToMouse.x);
            animator.SetFloat("LastInputY", directionToMouse.y);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", moveInput.x);    // For walk animation
            animator.SetFloat("InputY", moveInput.y);
            animator.SetFloat("LastInputX", moveInput.x); // For immediate idle facing if mouse aim was off
            animator.SetFloat("LastInputY", moveInput.y);
        }
        else
        {
            animator.SetBool("isWalking", false); // Transition to idle
            // Idle direction now handled by AimAtMouse() in Update
        }
    }
}