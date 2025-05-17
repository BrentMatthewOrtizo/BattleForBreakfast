using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float enemySpeed = 3.5f;
    public float attackCooldown = 1f;
    public float attackHitboxDuration = 0.2f;

    [Header("Directional Attack Hitboxes (Assign all 8)")]
    public GameObject EnemyHitBoxU; // North (ensure this matches the Inspector name if you changed it there)
    public GameObject hitboxD;      // South
    public GameObject hitboxL;      // West
    public GameObject hitboxR;      // East
    public GameObject hitboxUL;     // North-West
    public GameObject hitboxUR;     // North-East
    public GameObject hitboxDL;     // South-West
    public GameObject hitboxDR;     // South-East

    private Rigidbody2D rb;
    private EnemyAwareness enemyAwareness;
    private Vector2 targetDirectionToPlayer;
    private Animator animator;
    private float attackTimer = 0f;
    private bool isTouchingPlayer = false;

    // Wander variables
    private bool isWandering = false;
    private Vector2 wanderDirection;
    private float wanderTimer = 0f;
    private float wanderDuration = 2f; // How long to wander in one direction
    private float wanderCooldown = 1f; // How long to wait before choosing a new wander direction
    private float wanderSpeed = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("EnemyMovement: Rigidbody2D not found on " + gameObject.name);

        enemyAwareness = GetComponent<EnemyAwareness>();
        if (enemyAwareness == null) Debug.LogError("EnemyMovement: EnemyAwareness component not found on " + gameObject.name);

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("EnemyMovement: Animator component not found on " + gameObject.name);

        DeactivateAllHitboxes();
    }

    void FixedUpdate()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.fixedDeltaTime;
        }

        UpdateTargetDirection(); // Determine if player is primary target

        if (enemyAwareness != null && enemyAwareness.awareOfPlayer && enemyAwareness.hasLineOfSight)
        {
            isWandering = false; // Stop wandering if player is sighted
            SetVelocityBasedOnTarget();
            TryAttack();
            // ApplySeparation(); // Assuming this logic is for multiple enemies, ensure it's implemented if needed
        }
        else
        {
            Wander();
        }
    }

    private void UpdateTargetDirection()
    {
        if (enemyAwareness != null && enemyAwareness.awareOfPlayer)
        {
            targetDirectionToPlayer = enemyAwareness.directionToPlayer;
        }
        else
        {
            targetDirectionToPlayer = Vector2.zero;
        }
    }

    private void SetVelocityBasedOnTarget()
    {
        if (animator == null) return;

        if (!enemyAwareness.awareOfPlayer || isTouchingPlayer || targetDirectionToPlayer == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
        else
        {
            rb.linearVelocity = targetDirectionToPlayer * enemySpeed;
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", targetDirectionToPlayer.x);
            animator.SetFloat("InputY", targetDirectionToPlayer.y);
            animator.SetFloat("LastInputX", targetDirectionToPlayer.x); // Store for attack direction
            animator.SetFloat("LastInputY", targetDirectionToPlayer.y); // Store for attack direction
        }
    }

    private void Wander()
    {
        if (animator == null) return;

        wanderTimer -= Time.fixedDeltaTime;

        if (wanderTimer <= 0f)
        {
            isWandering = !isWandering;
            wanderTimer = isWandering ? wanderDuration : wanderCooldown;

            if (isWandering)
            {
                wanderDirection = Random.insideUnitCircle.normalized;
                animator.SetFloat("InputX", wanderDirection.x);
                animator.SetFloat("InputY", wanderDirection.y);
                animator.SetFloat("LastInputX", wanderDirection.x); // Store for idle facing or potential wander attack
                animator.SetFloat("LastInputY", wanderDirection.y); // Store for idle facing
            }
        }

        if (isWandering)
        {
            rb.linearVelocity = wanderDirection * wanderSpeed;
            animator.SetBool("isWalking", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
    }

    // Placeholder for separation logic if you have multiple enemies
    // private void ApplySeparation() { /* ... your existing logic ... */ }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            // Optionally stop movement immediately on touch if desired
            // rb.linearVelocity = Vector2.zero;
            // if (animator != null) animator.SetBool("isWalking", false);
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
        if (isTouchingPlayer && attackTimer <= 0f && animator != null)
        {
            if (targetDirectionToPlayer != Vector2.zero)
            {
                animator.SetFloat("LastInputX", targetDirectionToPlayer.x);
                animator.SetFloat("LastInputY", targetDirectionToPlayer.y);
            }

            float attackDirX = animator.GetFloat("LastInputX");
            float attackDirY = animator.GetFloat("LastInputY");

            animator.SetTrigger("isAttacking");

            GameObject hitboxToActivate = GetPreciseHitboxFor8WayDirection(attackDirX, attackDirY);
            if (hitboxToActivate != null)
            {
                // This log confirms activation is being attempted
                Debug.Log($"[EnemyAttack] {gameObject.name} attempting to activate hitbox: {hitboxToActivate.name} (determined from {attackDirX:F2}, {attackDirY:F2})");
                StartCoroutine(ActivateEnemyHitboxCoroutine(hitboxToActivate));
            }
            else
            {
                // This warning means GetPreciseHitboxFor8WayDirection returned null.
                // The Debug.LogError inside GetPreciseHitboxFor8WayDirection should have already printed why.
                Debug.LogWarning($"{gameObject.name} couldn't determine/activate an 8-way hitbox for attack dir: ({attackDirX:F2},{attackDirY:F2}). See preceding error from GetPreciseHitboxFor8WayDirection.");
            }
            attackTimer = attackCooldown;
        }
    }

    // Determine which of the 8 directional hitboxes to use based on attack direction
    private GameObject GetPreciseHitboxFor8WayDirection(float x, float y)
{
    GameObject chosenHitbox = null;
    string intendedDirectionName = "None_LogicPathNotTaken"; // Default if no path is chosen

    if (Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f))
    {
        intendedDirectionName = "hitboxR (default for zero input)";
        chosenHitbox = hitboxR; // Make sure hitboxR is assigned if this is a valid fallback
    }
    else
    {
        Vector2 dir = new Vector2(x, y).normalized;
        float normX = dir.x;
        float normY = dir.y;

        // Cardinal directions
        if (normY > 0.9239f) { chosenHitbox = EnemyHitBoxU; intendedDirectionName = "EnemyHitBoxU (North)"; }
        else if (normY < -0.9239f) { chosenHitbox = hitboxD; intendedDirectionName = "hitboxD (South)"; }
        else if (normX > 0.9239f) { chosenHitbox = hitboxR; intendedDirectionName = "hitboxR (East)"; }
        else if (normX < -0.9239f) { chosenHitbox = hitboxL; intendedDirectionName = "hitboxL (West)"; }
        // Diagonal directions
        else if (normY > 0) // Upper half
        {
            if (normX > 0) { chosenHitbox = hitboxUR; intendedDirectionName = "hitboxUR (North-East)"; }
            else { chosenHitbox = hitboxUL; intendedDirectionName = "hitboxUL (North-West)"; }
        }
        else // Lower half (normY <= 0, and not purely cardinal if logic reached here)
        {
            if (normX > 0) { chosenHitbox = hitboxDR; intendedDirectionName = "hitboxDR (South-East)"; }
            else { chosenHitbox = hitboxDL; intendedDirectionName = "hitboxDL (South-West)"; }
        }
    }

    if (chosenHitbox == null)
    {
        // This log is CRITICAL: It tells you WHICH hitbox slot was likely unassigned.
        Debug.LogError($"[EnemyMovement] {gameObject.name}: Intended to use hitbox variable '{intendedDirectionName}' for attack dir ({x:F2},{y:F2}), BUT this variable is NULL (i.e., not assigned in the Inspector, or no logic path chose a hitbox).");
        return null;
    }

    // This log confirms a non-null hitbox was chosen.
    // Debug.Log($"[EnemyMovement] {gameObject.name} selected hitbox '{chosenHitbox.name}' (from variable '{intendedDirectionName}') for attack dir ({x:F2},{y:F2}).");
    return chosenHitbox;
    }

    private IEnumerator ActivateEnemyHitboxCoroutine(GameObject hitboxToActivate)
    {
        if (hitboxToActivate != null)
        {
            // Debug.Log(gameObject.name + " activating enemy hitbox: " + hitboxToActivate.name);
            hitboxToActivate.SetActive(true);
            yield return new WaitForSeconds(attackHitboxDuration);
            if (hitboxToActivate != null) // Check if not destroyed in the meantime
            {
                hitboxToActivate.SetActive(false);
            }
        }
    }

    private void DeactivateAllHitboxes()
    {
        // Ensure all hitbox GameObjects are assigned in the Inspector
        if (EnemyHitBoxU != null) EnemyHitBoxU.SetActive(false);
        if (hitboxD != null) hitboxD.SetActive(false);
        if (hitboxL != null) hitboxL.SetActive(false);
        if (hitboxR != null) hitboxR.SetActive(false);
        if (hitboxUL != null) hitboxUL.SetActive(false);
        if (hitboxUR != null) hitboxUR.SetActive(false);
        if (hitboxDL != null) hitboxDL.SetActive(false);
        if (hitboxDR != null) hitboxDR.SetActive(false);
    }
}