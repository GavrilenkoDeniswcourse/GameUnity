using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    [Header("=== Singleton ===")]
    public static Player Instance { get; private set; }

    [Header("=== Движение ===")]
    [SerializeField] private float movingSpeed = 1.00f;
    private float _minMovingSpeed = 0.1f;
    private Vector2 _lastMoveDirection = Vector2.down;
    public bool _isRunning = false;

    [Header("=== Компоненты ===")]
    private PlayerInputActions playerInput;
    private Rigidbody2D rb;

    [Header("=== Здоровье ===")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    private bool isInvincible;
    private float invincibleTimer;
    [SerializeField] private float invincibleDuration = 1f;

    private void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Инициализация
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        playerInput = new PlayerInputActions();
        playerInput.Enable();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    private void FixedUpdate()
    {
        GetMovement();
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        isInvincible = true;
        invincibleTimer = invincibleDuration;

        Debug.Log($"Здоровье игрока: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log($"Здоровье восстановлено: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private Vector2 GetMovementVector()
    {
        return playerInput.Player.Move.ReadValue<Vector2>();
    }

    private void GetMovement()
    {
        Vector2 inputVector = GetMovementVector();
        inputVector = inputVector.normalized;

        if (inputVector.magnitude > _minMovingSpeed)
        {
            _lastMoveDirection = inputVector;
        }

        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));
        _isRunning = inputVector.magnitude > _minMovingSpeed;
    }

    private void Die()
    {
        Debug.Log("Игрок умер");

        // 1. Удаляем случайные предметы из инвентаря
        InventoryDisplay inventory = GetComponent<InventoryDisplay>();
        if (inventory != null)
        {
            int itemsToRemove = Random.Range(3, 5);
            inventory.RemoveRandomItems(itemsToRemove);
        }

        // 2. Возвращаем игрока в точку возрождения
        Transform respawnPoint = GameObject.Find("RespawnPoint")?.transform;
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            // Запасной вариант — перезагрузить сцену
            UnityEngine.SceneManagement.SceneManager.LoadScene("Village");
        }

        // 3. Восстанавливаем здоровье (частично)
        currentHealth = maxHealth / 3;
        isInvincible = false;
    }

    public Vector2 LastMoveDirection => _lastMoveDirection;
}