using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Assigned at Runtime")]
    public EnemyStats enemyStats; // Assigned by the spawner
    public ContinuousEnemySpawner spawner; // Assigned by the spawner

    private float currentHealth;
    private Animator animator; // Optional: if you have animations like Idle, Walk, Attack
    private Rigidbody2D rb;

    [Header("Behavior Variables")]
    private Transform playerTarget;
    private float lastAttackTime;
    private bool isAttacking = false; // To prevent movement while attacking

    private enum EnemyState { Idle, Chasing, Attacking }
    private EnemyState currentState = EnemyState.Idle;


    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        if (enemyStats == null)
        {
            Debug.LogError(gameObject.name + " was spawned without EnemyStats! Disabling.", this);
            this.enabled = false;
            return;
        }

        currentHealth = enemyStats.health;
        gameObject.name = enemyStats.enemyName + "_Instance";

        if (rb != null)
        {
            if (rb.bodyType != RigidbodyType2D.Kinematic)
            {
                Debug.LogWarning("Enemy '" + gameObject.name + "' Rigidbody2D is not set to Kinematic. Forcing to Kinematic for push immunity.", this);
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        // Find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("EnemyAI: Player not found! Ensure player has 'Player' tag. Enemy will remain idle.", this);
        }

        lastAttackTime = -enemyStats.attackCooldown; // Allow attacking immediately if conditions met
    }

    void Update()
    {
        if (enemyStats == null || !this.enabled || playerTarget == null)
        {
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        if (isAttacking)
        {
             return;
        }


        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // State Transitions
        switch (currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer <= enemyStats.detectionRadius)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                if (distanceToPlayer <= enemyStats.attackRange)
                {
                    currentState = EnemyState.Attacking;
                }
                else if (distanceToPlayer > enemyStats.detectionRadius * 1.2f)
                {
                    currentState = EnemyState.Idle;
                }
                break;

            case EnemyState.Attacking:
                if (distanceToPlayer > enemyStats.attackRange * 1.1f)
                {
                    currentState = EnemyState.Chasing;
                }
                else if (distanceToPlayer > enemyStats.detectionRadius * 1.2f && Time.time > lastAttackTime + enemyStats.attackCooldown)
                {
                    // This condition ensures if player is out of detection and attack cooldown passed, go idle.
                    currentState = EnemyState.Idle;
                }
                break;
        }

        // State Actions
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;
            case EnemyState.Chasing:
                ChaseBehavior(distanceToPlayer);
                break;
            case EnemyState.Attacking:
                AttackBehavior(distanceToPlayer);
                break;
        }

        UpdateAnimatorState();
    }

    void IdleBehavior()
    {
        // Enemy stands still or could have a simple patrol pattern here
        if (rb != null) rb.linearVelocity = Vector2.zero; // Stop movement if using Rigidbody for movement
    }

    void ChaseBehavior(float distanceToPlayer)
    {
        if (playerTarget == null) return;

        Vector2 direction = (playerTarget.position - transform.position).normalized;
        
        if (rb != null)
        {
            rb.linearVelocity = direction * enemyStats.moveSpeed;

            // Flip sprite based on movement direction (simple horizontal flip)
            if (direction.x > 0.01f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < -0.01f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else // Fallback if no Rigidbody, move with Transform (less ideal for physics interactions)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, enemyStats.moveSpeed * Time.deltaTime);
        }
    }

    void AttackBehavior(float distanceToPlayer) // distanceToPlayer is passed from Update()
    {
        if (playerTarget == null) return;

        if (rb != null) rb.linearVelocity = Vector2.zero; // Stop moving when in attack range/preparing to attack

        // Face the player
        if (playerTarget.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // Check if it's time to attack (cooldown) AND if the enemy is still in the Attacking state
        // (The Attacking state itself is determined by being in range, so this is a bit redundant with
        // the main Update loop's state transition, but good for clarity and potential future logic)
        if (Time.time >= lastAttackTime + enemyStats.attackCooldown && currentState == EnemyState.Attacking)
        {
            lastAttackTime = Time.time;

            if (animator != null)
            {
                animator.SetTrigger("isAttacking");
            }

            // --- DEAL DAMAGE ---
            // Re-check distanceToPlayer at the moment of attempting damage
            // This ensures the player hasn't micro-moved out of range during the attack wind-up or decision frame.
            if (distanceToPlayer <= enemyStats.attackRange) // << USING distanceToPlayer HERE
            {
                PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Debug.Log(enemyStats.enemyName + " attacks! Dealing " + enemyStats.attackDamage + " damage.");
                    playerHealth.TakeDamage(enemyStats.attackDamage);
                }
            }
        }
    }
    
    void UpdateAnimatorState()
    {
        if (animator == null) return;

        bool currentlyMoving = (currentState == EnemyState.Chasing && rb != null && rb.linearVelocity.magnitude > 0.1f);
        animator.SetBool("isWalking", currentlyMoving);
    }


    public void TakeDamage(float amount)
    {
        if (!this.enabled || enemyStats == null) return;

        currentHealth -= amount;
        // Debug.Log(enemyStats.enemyName + " took " + amount + " damage. Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Debug.Log(enemyStats.enemyName + " (" + gameObject.name + ") has died.");
        this.enabled = false; // Stop updates immediately

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (spawner != null)
        {
            spawner.OnEnemyDefeated(gameObject);
        }
        else
        {
            Debug.LogWarning(gameObject.name + " died but had no spawner reference.", this);
        }
        Destroy(gameObject);
    }
}