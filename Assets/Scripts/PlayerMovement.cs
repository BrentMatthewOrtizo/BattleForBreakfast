using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;

    private Rigidbody2D rigidBody;
    private Vector2 moveInput;
    private Animator animator;
    private Camera mainCamera;
    public SlashHitbox slashHitbox;

    private bool canMove = true;
    
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (canMove)
        {
            rigidBody.linearVelocity = moveInput * moveSpeed;
            
            if (moveInput == Vector2.zero && !animator.GetBool("isWalking"))
            {
                    UpdateAimDirectionTowardsMouse();
            }
        }
        else
        {
            rigidBody.linearVelocity = Vector2.zero;
        }
    }
    
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
                animator.SetFloat("InputX", moveInput.x);
                animator.SetFloat("InputY", moveInput.y);
                animator.SetFloat("LastInputX", moveInput.x);
                animator.SetFloat("LastInputY", moveInput.y);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
    
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
        {
            rigidBody.linearVelocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("RedBull"))
        {
            ActivateSpeedBoost();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("SpoiledMilk"))
        {
            ActivateSlowness();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Monster"))
        {
            ActivateDamageBoost();
            Destroy(other.gameObject);
        }
    }
    
    public void ActivateSpeedBoost()
    {
        StopCoroutine("SpeedBoostRoutine"); 
        StartCoroutine(SpeedBoostRoutine());
    }

    private IEnumerator SpeedBoostRoutine()
    {
        float originalSpeed = moveSpeed;
        moveSpeed = 8f;

        yield return new WaitForSeconds(5f);

        moveSpeed = originalSpeed;
    }
    
    public void ActivateSlowness()
    {
        StopCoroutine("SlownessRoutine"); 
        StartCoroutine(SlownessRoutine());
    }

    private IEnumerator SlownessRoutine()
    {
        float originalSpeed = moveSpeed;
        moveSpeed = 2f;

        yield return new WaitForSeconds(5f);

        moveSpeed = originalSpeed;
    }
    
    private IEnumerator DamageBoostRoutine()
    {
        int currentDamage = SlashHitbox.damageAmount;
        SlashHitbox.damageAmount = 10;

        yield return new WaitForSeconds(5f);

        SlashHitbox.damageAmount = 5;
    }
    
    public void ActivateDamageBoost()
    {
        StopCoroutine("DamageBoostRoutine");
        StartCoroutine(DamageBoostRoutine());
    }
    
}