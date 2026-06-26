using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Движение и повороты")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3f;
    public float idleTime = 2f;
    private bool facingRight = true;
    private Vector2 moveDirection;
    private float idleTimer;
    private float changeDirectionTimer;

    [Header("Атака")]
    public float attackCooldown = 1.5f;
    public int attackDamage = 1;
    public float attackRange = 1.2f;
    private float nextAttackTime;

    [Header("Здоровье")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Преследование")]
    public float detectionRange = 5f;
    public float loseRange = 7f;
    private Transform player;

    [Header("Отбрасывание")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack;
    private float knockbackTimer;

    private Animator animator;
    private EnemyState currentState;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Death
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.bodyType = RigidbodyType2D.Kinematic;

        currentState = EnemyState.Patrol;
        idleTimer = idleTime;
        changeDirectionTimer = Random.Range(1f, 3f);
        moveDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        // Отбрасывание
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
            }
            return;
        }

        if (currentState == EnemyState.Death) return;

        if (animator != null)
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        UpdateState();
        ExecuteState();
    }

    private void UpdateState()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        float distancePlayer = Vector2.Distance(transform.position, player.position);

        if (distancePlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distancePlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        else if (distancePlayer > loseRange)
        {
            currentState = EnemyState.Patrol;
        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                    Debug.Log("Анимация атаки вызвана");
                }
                Attack();
                break;
        }
    }

    private void Idle()
    {
        rb.linearVelocity = Vector2.zero;
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            currentState = EnemyState.Patrol;
            idleTimer = idleTime;
            changeDirectionTimer = Random.Range(1f, 3f);
        }
    }

    private void Patrol()
    {
        RandomPatrol();
    }

    private void RandomPatrol()
    {
        changeDirectionTimer -= Time.deltaTime;

        if (changeDirectionTimer <= 0)
        {
            moveDirection = Random.insideUnitCircle.normalized;
            changeDirectionTimer = Random.Range(1f, 3f);
        }

        if (IsWallInDirection(moveDirection))
        {
            rb.linearVelocity = Vector2.zero;
            moveDirection = -moveDirection; // развернуться
            return;
        }

        rb.linearVelocity = moveDirection * moveSpeed;

        if (moveDirection.x > 0 && !facingRight)
            Flip();
        else if (moveDirection.x < 0 && facingRight)
            Flip();
    }

    private void Chase()
    {

        Vector2 direction = (player.position - transform.position).normalized;

        if (IsWallInDirection(direction))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (direction.x > 0 && !facingRight)
            Flip();
        else if (direction.x < 0 && facingRight)
            Flip();
    }

    private void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            if (animator != null)
                animator.SetBool("IsAttacking", true);

            Invoke(nameof(ResetAttack), 0.3f);

            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage);
                Debug.Log($"Враг атакует! Нанесено {attackDamage} урона");
            }
        }
    }

    private void ResetAttack()
    {
        if (animator != null)
            animator.SetBool("IsAttacking", false);
    }

    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Death) return;
        if (currentHealth <= 0) return;

        // Отбрасывание
        Vector2 direction = (transform.position - player.position).normalized;
        ApplyKnockback(direction);

        if (animator != null)
            animator.SetTrigger("Hit");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        if (rb != null)
            rb.linearVelocity = direction * knockbackForce;
    }

    private void Die()
    {
        currentState = EnemyState.Death;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Die");

        Destroy(gameObject, 0.5f);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsWallInDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.4f, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }

}