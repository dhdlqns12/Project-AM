using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private UnitBase unit;
    private bool isMoving = true;
    private float moveDirection;  // 1: 오른쪽, -1: 왼쪽

    private Rigidbody2D rb;

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

        rb.velocity = Vector2.right * moveDirection * unit.Data.MoveSpeed;
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
