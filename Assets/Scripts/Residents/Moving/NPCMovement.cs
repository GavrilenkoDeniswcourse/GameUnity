using System.Collections;
using UnityEngine;

public class NPCMovementWaypoints : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0.2f;

    [Header("Точки маршрута")]
    [SerializeField] private Transform[] waypoints;

    [Header("Ожидание")]
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private bool[] waitAtPoint;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool isInteracting = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    private const string Walk = "Walk";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Нормализуем массивы
        if (waitAtPoint == null || waitAtPoint.Length != waypoints.Length)
        {
            bool[] newWait = new bool[waypoints.Length];
            if (waitAtPoint != null)
            {
                for (int i = 0; i < waypoints.Length && i < waitAtPoint.Length; i++)
                    newWait[i] = waitAtPoint[i];
            }
            waitAtPoint = newWait;
        }

        Debug.Log($"Начальная точка: {currentWaypointIndex}, ждать: {waitAtPoint[currentWaypointIndex]}");
    }

    void Update()
    {
        if (waypoints.Length == 0) return;
        if (isWaiting || isInteracting)
        {
            if (animator != null) animator.SetBool(Walk, false);
            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= stopDistance)
        {
            // Дошли до точки
            if (waitAtPoint[currentWaypointIndex])
            {
                StartCoroutine(WaitAndMoveNext());
            }
            else
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                Debug.Log($"Переход к точке {currentWaypointIndex}, ждать: {waitAtPoint[currentWaypointIndex]}");
            }
        }
        else
        {
            // Идём к точке
            if (animator != null) animator.SetBool(Walk, true);
            rb.MovePosition(transform.position + (Vector3)direction * moveSpeed * Time.deltaTime);

            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;
        }
    }

    private IEnumerator WaitAndMoveNext()
    {
        Debug.Log($"Ожидание на точке {currentWaypointIndex}");
        isWaiting = true;
        if (animator != null) animator.SetBool(Walk, false);

        yield return new WaitForSeconds(waitTime);

        // Переход к следующей точке
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        Debug.Log($"Ожидание закончено, иду к точке {currentWaypointIndex}");

        isWaiting = false;
    }

    public void SetInteracting(bool interacting)
    {
        isInteracting = interacting;

        if (!interacting && isWaiting)
        {
            StopAllCoroutines();
            isWaiting = false;
            Debug.Log("Прервано ожидание");
        }
    }
}