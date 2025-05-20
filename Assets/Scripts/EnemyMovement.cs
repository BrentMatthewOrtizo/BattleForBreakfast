using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float enemySpeed = 3.5f;
    public float attackCooldown = 1f;
    public float attackHitboxDuration = 0.2f;

    [Header("Directional Attack Hitboxes (Assign all 8)")]
    public GameObject EnemyHitBoxU; // North
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
        if (attackTimer > 0f)
        {
            attackTimer -= Time.fixedDeltaTime;
        }

        UpdateTargetDirection();

        if (enemyAwareness != null && enemyAwareness.awareOfPlayer && enemyAwareness.hasLineOfSight)
        {
            isWandering = false;
            SetVelocityBasedOnTarget();
            TryAttack();
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
            animator.SetFloat("LastInputX", targetDirectionToPlayer.x);
            animator.SetFloat("LastInputY", targetDirectionToPlayer.y);
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
                animator.SetFloat("LastInputX", wanderDirection.x);
                animator.SetFloat("LastInputY", wanderDirection.y);
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
                StartCoroutine(ActivateEnemyHitboxCoroutine(hitboxToActivate));
            }
            attackTimer = attackCooldown;
        }
    }
    
    private GameObject GetPreciseHitboxFor8WayDirection(float x, float y)
{
    GameObject chosenHitbox = null;
    string intendedDirectionName = "None_LogicPathNotTaken";

    if (Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f))
    {
        intendedDirectionName = "hitboxR (default for zero input)";
        chosenHitbox = hitboxR;
    }
    else
    {
        Vector2 dir = new Vector2(x, y).normalized;
        float normX = dir.x;
        float normY = dir.y;
        
        if (normY > 0.9239f) { chosenHitbox = EnemyHitBoxU; intendedDirectionName = "EnemyHitBoxU (North)"; }
        else if (normY < -0.9239f) { chosenHitbox = hitboxD; intendedDirectionName = "hitboxD (South)"; }
        else if (normX > 0.9239f) { chosenHitbox = hitboxR; intendedDirectionName = "hitboxR (East)"; }
        else if (normX < -0.9239f) { chosenHitbox = hitboxL; intendedDirectionName = "hitboxL (West)"; }
        else if (normY > 0)
        {
            if (normX > 0) { chosenHitbox = hitboxUR; intendedDirectionName = "hitboxUR (North-East)"; }
            else { chosenHitbox = hitboxUL; intendedDirectionName = "hitboxUL (North-West)"; }
        }
        else
        {
            if (normX > 0) { chosenHitbox = hitboxDR; intendedDirectionName = "hitboxDR (South-East)"; }
            else { chosenHitbox = hitboxDL; intendedDirectionName = "hitboxDL (South-West)"; }
        }
    }

    if (chosenHitbox == null)
    {
        Debug.LogError($"[EnemyMovement] {gameObject.name}: Intended to use hitbox variable '{intendedDirectionName}' for attack dir ({x:F2},{y:F2}), BUT this variable is NULL (i.e., not assigned in the Inspector, or no logic path chose a hitbox).");
        return null;
    }

    return chosenHitbox;
    }

    private IEnumerator ActivateEnemyHitboxCoroutine(GameObject hitboxToActivate)
    {
        if (hitboxToActivate != null)
        {
            hitboxToActivate.SetActive(true);
            yield return new WaitForSeconds(attackHitboxDuration);
            if (hitboxToActivate != null)
            {
                hitboxToActivate.SetActive(false);
            }
        }
    }

    private void DeactivateAllHitboxes()
    {
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