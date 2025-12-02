using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private UnitBase unit;
    private Rigidbody2D rb;
    private bool isMoving = true;
    private float moveDirection;  // 1: 오른쪽, -1: 왼쪽

    private Vector3? targetPosition = null;  // 추적할 타겟 위치

    public bool IsMoving => isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(UnitBase unitBase)
    {
        unit = unitBase;

        // Team에 따라 이동 방향 결정
        moveDirection = unit.Team == Team.Player ? 1f : -1f;

        Debug.Log($"{unit.Data.Name} 이동 시작 (방향: {(moveDirection > 0 ? "->" : "<-")})");
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

        if (targetPosition.HasValue)
        {
            Vector3 direction = (targetPosition.Value - transform.position).normalized;
            rb.velocity = new Vector2(direction.x, direction.y) * unit.Data.MoveSpeed;
        }
        else
        {
            rb.velocity = Vector2.right * moveDirection * unit.Data.MoveSpeed;
        }
    }

    /// <summary>
    /// 특정 위치로 이동 (타겟 추적)
    /// </summary>
    public void MoveTowards(Vector3 position)
    {
        targetPosition = position;

        if (!isMoving)
        {
            Resume();
        }
    }

    /// <summary>
    /// 기본 방향으로 이동 (타겟 없음)
    /// </summary>
    public void MoveDefault()
    {
        targetPosition = null;

        if (!isMoving)
        {
            Resume();
        }
    }

    /// <summary>
    /// 이동 중지 (전투 시작 시)
    /// </summary>
    public void Stop()
    {
        isMoving = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    /// <summary>
    /// 이동 재개 (전투 종료 시)
    /// </summary>
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
}
