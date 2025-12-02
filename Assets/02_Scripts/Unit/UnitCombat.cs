using UnityEngine;

/// <summary>
/// 유닛 전투 처리
/// </summary>
public class UnitCombat : MonoBehaviour
{
    private UnitBase unit;
    private UnitMovement unitMovement;

    private UnitBase currentTarget;
    private float attackTimer;

    [Header("전투 설정")]
    [SerializeField] private float detectionRange = 10f;  // 감지 범위
    [SerializeField] private LayerMask enemyLayer;

    public void Init(UnitBase unitBase, UnitMovement movement)
    {
        unit = unitBase;
        unitMovement = movement;

        // 적 레이어 설정
        string enemyLayerName = unit.Team == Team.Player ? "EnemyUnit" : "PlayerUnit";
        enemyLayer = LayerMask.GetMask(enemyLayerName);
    }

    private void Update()
    {
        if (unit == null || unit.IsDead) return;

        // 1. 타겟 찾기
        FindClosestTarget();

        if (currentTarget != null)
        {
            // 2. 타겟까지 거리
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

            // 3. 공격 범위 안이면 → 멈추고 공격
            if (distanceToTarget <= unit.Data.AttackRange)
            {
                if (unitMovement.IsMoving)
                {
                    unitMovement.Stop();
                    Debug.Log($"{unit.Data.Name} 공격 범위 진입! 공격 시작");
                }

                Attack();
            }
            // 4. 공격 범위 밖이면 → 타겟 쪽으로 이동 (추격)
            else
            {
                // ✅ 타겟 위치로 이동 지시
                unitMovement.MoveTowards(currentTarget.transform.position);
            }
        }
        else
        {
            // 5. 타겟 없으면 → 기본 방향(좌우)으로 이동
            unitMovement.MoveDefault();
        }
    }



    /// <summary>
    /// 감지 범위 내에서 가장 가까운 적 찾기
    /// </summary>
    private void FindClosestTarget()
    {
        if (currentTarget != null)
        {
            if (currentTarget.IsDead || !IsInDetectionRange(currentTarget))
            {
                currentTarget = null;
                attackTimer = 0f;
            }
            else
            {
                return;  
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            detectionRange,
            enemyLayer
        );

        if (hits.Length == 0)
        {
            return;
        }

        UnitBase closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            UnitBase enemy = hit.GetComponent<UnitBase>();

            if (enemy != null && !enemy.IsDead)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            currentTarget = closestEnemy;
        }
    }

    /// <summary>
    /// 타겟이 감지 범위 안에 있는지
    /// </summary>
    private bool IsInDetectionRange(UnitBase target)
    {
        if (target == null) return false;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= detectionRange;
    }

    /// <summary>
    /// 공격 처리
    /// </summary>
    private void Attack()
    {
        if (currentTarget == null) return;

        attackTimer += Time.deltaTime;

        // 공격 속도마다 공격
        if (attackTimer >= unit.Data.AttackSpeed)
        {
            attackTimer = 0f;

            int damage = unit.CurAtk;
            currentTarget.TakeDamage(damage);

            Debug.Log($"{unit.Data.Name} -> {currentTarget.Data.Name} 공격! (데미지: {damage}, 남은 HP: {currentTarget.CurHp})");

            // 타겟이 죽었는지 확인
            if (currentTarget.IsDead)
            {
                Debug.Log($"{currentTarget.Data.Name} 처치");
                currentTarget = null;
                attackTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Gizmos로 범위 표시
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (unit == null) return;

        // 감지 범위 (초록색)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 공격 범위 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, unit.Data.AttackRange);

        // 현재 타겟에게 선 (빨간색)
        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);

            // 타겟까지의 거리 표시
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance <= unit.Data.AttackRange)
            {
                Gizmos.color = Color.red; 
            }
            else
            {
                Gizmos.color = Color.green; 
            }

            Gizmos.DrawWireSphere(currentTarget.transform.position, 0.3f);
        }
    }
}