using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float enemySpeed = 3.5f;
    public float attackCooldown = 1f;
    public float attackHitboxDuration = 0.2f;

    [Header("Directional Attack Hitboxes (Assign all 8)")]
    public GameObject EnemyHitBoxU; // North - Corrected from your provided code
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
    private float wanderDuration = 2f;
    private float wanderCooldown = 1f;
    private float wanderSpeed = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAwareness = GetComponent<EnemyAwareness>();
        animator = GetComponent<Animator>();
        DeactivateAllHitboxes();
    }

    void FixedUpdate()
    {
        if (attackTimer > 0f) attackTimer -= Time.fixedDeltaTime;
        UpdateTargetDirection();

        if (enemyAwareness.awareOfPlayer && enemyAwareness.hasLineOfSight)
        {
            isWandering = false;
            SetVelocityBasedOnTarget();
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
        if (enemyAwareness.awareOfPlayer) targetDirectionToPlayer = enemyAwareness.directionToPlayer;
        else targetDirectionToPlayer = Vector2.zero;
    }

    private void SetVelocityBasedOnTarget()
    {
        if (!enemyAwareness.awareOfPlayer || isTouchingPlayer || targetDirectionToPlayer == Vector2.zero) {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("isWalking", false);
        } else {
            rb.linearVelocity = targetDirectionToPlayer * enemySpeed;
            if (animator != null) {
                animator.SetBool("isWalking", true);
                animator.SetFloat("InputX", targetDirectionToPlayer.x);
                animator.SetFloat("InputY", targetDirectionToPlayer.y);
                animator.SetFloat("LastInputX", targetDirectionToPlayer.x);
                animator.SetFloat("LastInputY", targetDirectionToPlayer.y);
            }
        }
    }
    
    private void Wander() {
        wanderTimer -= Time.fixedDeltaTime;
        if (wanderTimer <= 0f) {
            isWandering = !isWandering;
            wanderTimer = isWandering ? wanderDuration : wanderCooldown;
            if (isWandering) {
                wanderDirection = Random.insideUnitCircle.normalized;
                if (animator != null) {
                    animator.SetFloat("InputX", wanderDirection.x);
                    animator.SetFloat("InputY", wanderDirection.y);
                    animator.SetFloat("LastInputX", wanderDirection.x);
                    animator.SetFloat("LastInputY", wanderDirection.y);
                }
            }
        }

        if (isWandering) {
            rb.linearVelocity = wanderDirection * wanderSpeed;
            if (animator != null) animator.SetBool("isWalking", true);
        } else {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("isWalking", false);
        }
    }

    private void ApplySeparation() {/* ... your existing logic ... */} // Assuming this is implemented elsewhere or fine
    private void OnCollisionEnter2D(Collision2D collision) { if (collision.gameObject.CompareTag("Player")) isTouchingPlayer = true; }
    private void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject.CompareTag("Player")) isTouchingPlayer = false; }

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

            GameObject hitboxToActivate = GetPreciseHitboxFor8WayDirection(attackDirX, attackDirY); // Using the simpler logic
            if (hitboxToActivate != null)
            {
                StartCoroutine(ActivateEnemyHitboxCoroutine(hitboxToActivate));
            }
            else
            {
                Debug.LogWarning(gameObject.name + " couldn't determine an 8-way hitbox for attack dir: " + attackDirX + "," + attackDirY + ". Defaulting to Right hitbox if available.");
                // Optional: Default to a specific hitbox if null, e.g. right or forward based on enemy type
                // if (hitboxR != null) StartCoroutine(ActivateEnemyHitboxCoroutine(hitboxR));
            }
            attackTimer = attackCooldown;
        }
    }

    // --- Using the simpler dominant-axis/quadrant style logic for 8 directions ---
    private GameObject GetPreciseHitboxFor8WayDirection(float x, float y)
    {
        if (Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f))
        {
            Debug.LogWarning(gameObject.name + ": GetPreciseHitbox called with x=0, y=0. Defaulting to Right hitbox.");
            return hitboxR; // Or your preferred default, or null to see the warning in TryAttack
        }

        // --- Normalize the direction vector to ensure consistent magnitude for comparisons ---
        Vector2 dir = new Vector2(x, y).normalized;
        float normX = dir.x;
        float normY = dir.y;

        // --- Define thresholds for cardinal vs. diagonal ---
        // If a component's absolute value is greater than this, it's strongly cardinal.
        // cos(67.5 degrees) is approx 0.3827. Anything with a component larger than this is likely NOT in the 45-degree diagonal cone of the other axis.
        // A simpler threshold: if one component is, say, 3x the other, it's cardinal.
        // Let's use a threshold based on how "flat" or "steep" the vector is.
        // If slope is between tan(22.5) and tan(67.5), it's diagonal.
        // tan(22.5) approx 0.414
        // tan(67.5) approx 2.414
        // So, if abs(y)/abs(x) is between 0.414 and 2.414, it's diagonal.

        float absNormX = Mathf.Abs(normX);
        float absNormY = Mathf.Abs(normY);

        // Check for North (purely up)
        if (normY > 0.9239f && absNormX < 0.3827f) return EnemyHitBoxU; // cos(22.5), sin(22.5)
        // Check for South (purely down)
        if (normY < -0.9239f && absNormX < 0.3827f) return hitboxD;
        // Check for East (purely right)
        if (normX > 0.9239f && absNormY < 0.3827f) return hitboxR;
        // Check for West (purely left)
        if (normX < -0.9239f && absNormY < 0.3827f) return hitboxL;

        // If not purely cardinal, check diagonals
        if (normY > 0) // Upper half
        {
            if (normX > 0) return hitboxUR; // North-East
            else return hitboxUL;           // North-West
        }
        else // Lower half (normY < 0, since y=0 case for L/R is handled by cardinal checks)
        {
            if (normX > 0) return hitboxDR; // South-East
            else return hitboxDL;           // South-West
        }

        // This fallback should ideally not be reached if the above logic is exhaustive
        // for non-zero vectors. The initial x=0,y=0 check handles that.
        // Debug.LogWarning($"{gameObject.name} fell through GetPreciseHitbox. x:{x}, y:{y}, normX:{normX}, normY:{normY}");
        // return hitboxR; // Default if logic somehow fails
    }

    private IEnumerator ActivateEnemyHitboxCoroutine(GameObject hitboxToActivate)
    {
        if (hitboxToActivate != null)
        {
            // Debug.Log("Activating enemy hitbox: " + hitboxToActivate.name); // Useful for testing
            hitboxToActivate.SetActive(true);
            yield return new WaitForSeconds(attackHitboxDuration);
            hitboxToActivate.SetActive(false);
        }
    }

    private void DeactivateAllHitboxes()
    {
        if (EnemyHitBoxU != null) EnemyHitBoxU.SetActive(false); // Corrected variable name
        if (hitboxD != null) hitboxD.SetActive(false);
        if (hitboxL != null) hitboxL.SetActive(false);
        if (hitboxR != null) hitboxR.SetActive(false);
        if (hitboxUL != null) hitboxUL.SetActive(false);
        if (hitboxUR != null) hitboxUR.SetActive(false);
        if (hitboxDL != null) hitboxDL.SetActive(false);
        if (hitboxDR != null) hitboxDR.SetActive(false);
    }
}