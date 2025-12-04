using UnityEngine;
using System.Linq;

public class UnitMovement : MonoBehaviour
{
    private UnitBase unit;
    private Rigidbody2D rb;
    private bool isMoving = true;
    private float moveDirection;
    private Vector3? targetPosition = null;

    [Header("8방향 회피")]
    [SerializeField] private float rayDistance = 1.5f;
    [SerializeField] private float rayThickness = 0.3f;
    [SerializeField] private LayerMask allyLayer;

    [Header("Stuck 감지")]
    [SerializeField] private float stuckCheckInterval = 0.2f;  // 체크 주기
    [SerializeField] private float stuckThreshold = 0.05f;  // 이 거리 이하면 막힌 것
    [SerializeField] private int maxFallbackAttempts = 6;  // 최대 차선책 시도 횟수

    [Header("디버그")]
    [SerializeField] private bool showDebug = false;
    [SerializeField] private bool showGizmos = true;

    private Vector2[] directions = new Vector2[8];
    private bool[] isBlocked = new bool[8];
    private float[] weights = new float[8];

    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private int currentFallbackLevel = 0;  // 현재 차선책 레벨 (0 = 최선, 1 = 차선...)

    public bool IsMoving => isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeDirections();
        Physics2D.queriesStartInColliders = false;
    }

    private void InitializeDirections()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    public void Init(UnitBase unitBase)
    {
        unit = unitBase;
        moveDirection = unit.Team == Team.Player ? 1f : -1f;

        string allyLayerName = unit.Team == Team.Player ? "PlayerUnit" : "EnemyUnit";
        allyLayer = LayerMask.GetMask(allyLayerName);

        lastPosition = transform.position;

    }

    private void FixedUpdate()
    {
        if (unit == null || unit.IsDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!isMoving)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        CheckIfStuck();

        Vector2 desiredDirection = GetDesiredDirection();
        CheckAllDirections();

        Vector2 bestDirection = ChooseBestDirectionWithFallback(desiredDirection);

        if (bestDirection == Vector2.zero)
        {
            rb.velocity = desiredDirection * unit.Data.MoveSpeed;

            return;
        }

        rb.velocity = bestDirection * unit.Data.MoveSpeed;
    }

    /// <summary>
    /// Stuck 감지 및 차선책 레벨 조정
    /// </summary>
    private void CheckIfStuck()
    {
        stuckTimer += Time.fixedDeltaTime;

        if (stuckTimer >= stuckCheckInterval)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved < stuckThreshold)
            {
                currentFallbackLevel++;
                currentFallbackLevel = Mathf.Min(currentFallbackLevel, maxFallbackAttempts);
            }
            else
            {
                currentFallbackLevel = 0;
            }

            lastPosition = transform.position;
            stuckTimer = 0f;
        }
    }

    private Vector2 GetDesiredDirection()
    {
        if (targetPosition.HasValue)
        {
            return ((Vector2)targetPosition.Value - (Vector2)transform.position).normalized;
        }
        else
        {
            return Vector2.right * moveDirection;
        }
    }

    private void CheckAllDirections()
    {
        for (int i = 0; i < 8; i++)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, rayThickness, directions[i], rayDistance, allyLayer);

            bool foundAlly = false;
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == gameObject) continue;
                foundAlly = true;
                break;
            }

            isBlocked[i] = foundAlly;
        }
    }

    /// <summary>
    /// 가중치 순서대로 방향 선택 (차선책 포함)
    /// </summary>
    private Vector2 ChooseBestDirectionWithFallback(Vector2 desiredDirection)
    {
        for (int i = 0; i < 8; i++)
        {
            if (isBlocked[i])
            {
                weights[i] = -1f;  // 막힌 곳은 -1
            }
            else
            {
                float similarity = Vector2.Dot(directions[i], desiredDirection);
                weights[i] = (similarity + 1f) / 2f;
            }
        }

        var sortedIndices = Enumerable.Range(0, 8)
            .Where(i => weights[i] >= 0)  // 막히지 않은 것만
            .OrderByDescending(i => weights[i])
            .ToList();

        if (sortedIndices.Count == 0)
        {
            return Vector2.zero;  // 모든 방향 막힘
        }

        int selectedIndex = Mathf.Min(currentFallbackLevel, sortedIndices.Count - 1);
        int bestIndex = sortedIndices[selectedIndex];

        return directions[bestIndex];
    }

    public void MoveTowards(Vector3 position)
    {
        targetPosition = position;
        if (!isMoving)
        {
            Resume();
        }
    }

    public void MoveDefault()
    {
        targetPosition = null;
        if (!isMoving)
        {
            Resume();
        }
    }

    public void Stop()
    {
        isMoving = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void Resume()
    {
        if (unit != null && !unit.IsDead)
        {
            isMoving = true;
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || !Application.isPlaying) return;

        for (int i = 0; i < 8; i++)
        {
            Vector3 start = transform.position;
            Vector3 end = start + (Vector3)directions[i] * rayDistance;

            if (isBlocked[i])
            {
                Gizmos.color = Color.red;
            }
            else
            {
                float brightness = weights[i];
                Gizmos.color = new Color(0f, brightness, 0f);
            }

            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(end, rayThickness);
        }

        if (rb != null && rb.velocity.magnitude > 0.1f)
        {
            Gizmos.color = Color.cyan;
            Vector3 vel = rb.velocity.normalized * 1.5f;
            Gizmos.DrawRay(transform.position, vel);

            // 차선책 레벨 표시
            if (currentFallbackLevel > 0)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, 0.5f + currentFallbackLevel * 0.2f);
            }
        }

        // 목표 방향
        Vector2 desired = GetDesiredDirection();
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, desired * 1.2f);
    }
}

//이전 코스트 값을 기록하다가
//새로 계산한 코스트 값이 큰 차이가 없다면 둘 중 하나 취사 선택