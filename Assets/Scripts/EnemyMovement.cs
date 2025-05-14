using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public float enemySpeed = 3.5f;
    public float attackCooldown = 1f;
    
    private bool isWandering = false;
    private Vector2 wanderDirection;
    private float wanderTimer = 0f;
    private float wanderDuration = 2f;
    private float wanderCooldown = 1f;
    private float wanderSpeed = 2f;
    
    private float attackTimer = 0f;
    private bool isTouchingPlayer = false;
    private Rigidbody2D rb;
    private EnemyAwareness enemyAwareness;
    private Vector2 targetDirection;
    private Animator animator;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAwareness = GetComponent<EnemyAwareness>();
        animator = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.fixedDeltaTime;

        UpdateTargetDirection();

        if (enemyAwareness.awareOfPlayer)
        {
            isWandering = false;
            SetVelocity();
            TryAttack();
            ApplySeparation();
        }
        else
        {
            Wander();
        }
    }

    private void UpdateTargetDirection()
    {
        if (enemyAwareness.awareOfPlayer)
            targetDirection = enemyAwareness.directionToPlayer;
        else
            targetDirection = Vector2.zero;

    }

    private void SetVelocity()
    {
        if (!enemyAwareness.awareOfPlayer || isTouchingPlayer || targetDirection == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
        else
        {
            rb.linearVelocity = targetDirection * enemySpeed;
            animator.SetBool("isWalking", true);

            animator.SetFloat("InputX", targetDirection.x);
            animator.SetFloat("InputY", targetDirection.y);

            animator.SetFloat("LastInputX", targetDirection.x);
            animator.SetFloat("LastInputY", targetDirection.y);
        }
    }
    
    private void Wander()
    {
        wanderTimer -= Time.fixedDeltaTime;

        if (wanderTimer <= 0f)
        {
            if (!isWandering)
            {
                // Start wandering
                isWandering = true;
                wanderTimer = wanderDuration;

                // Pick a random direction
                wanderDirection = Random.insideUnitCircle.normalized;

                animator.SetBool("isWalking", true);
                animator.SetFloat("InputX", wanderDirection.x);
                animator.SetFloat("InputY", wanderDirection.y);
                animator.SetFloat("LastInputX", wanderDirection.x);
                animator.SetFloat("LastInputY", wanderDirection.y);
            }
            else
            {
                // Pause after walking
                isWandering = false;
                wanderTimer = wanderCooldown;

                animator.SetBool("isWalking", false);
            }
        }

        if (isWandering)
        {
            rb.linearVelocity = wanderDirection * wanderSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    private void ApplySeparation()
    {
        Vector2 separationForce = Vector2.zero;
        int neighborCount = 0;

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 1.2f);

        foreach (var other in nearby)
        {
            if (other.gameObject != gameObject && other.CompareTag("Enemy"))
            {
                Vector2 diff = (Vector2)(transform.position - other.transform.position);
                if (diff.sqrMagnitude > 0.01f)
                {
                    separationForce += diff.normalized / diff.magnitude;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            separationForce /= neighborCount;
            rb.AddForce(separationForce * 50f); // Increase multiplier if still weak
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    private void TryAttack()
    {
        if (isTouchingPlayer && attackTimer <= 0f)
        {
            Vector2 roundedDirection = new Vector2(
                Mathf.Abs(targetDirection.x) > Mathf.Abs(targetDirection.y) ? Mathf.Sign(targetDirection.x) : 0,
                Mathf.Abs(targetDirection.y) >= Mathf.Abs(targetDirection.x) ? Mathf.Sign(targetDirection.y) : 0
            );

            animator.SetFloat("LastInputX", roundedDirection.x);
            animator.SetFloat("LastInputY", roundedDirection.y);
            animator.SetTrigger("isAttacking");

            attackTimer = attackCooldown;
        }
    }
    
}
