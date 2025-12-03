using UnityEngine;

/// <summary>
/// 유닛 전투 처리
/// </summary>
public class UnitCombat : MonoBehaviour
{
    private UnitBase unit;
    private UnitMovement unitMovement;

    private UnitBase currentTargetUnit;
    private Nexus currentTargetNexus;
    private float attackTimer;

    [Header("전투 설정")]
    [SerializeField] private float detectionRange = 10f;  // 감지 범위
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask enemyNexusLayer;

    [Header("타게팅 우선순위 설정")]
    [SerializeField][Tooltip("true: 넥서스 공격 중에도 유닛이 나타나면 즉시 전환\nfalse: 넥서스 공격 중이면 유닛 무시")] private bool firstTargetUnit = true;

    private bool HasTarget => currentTargetUnit != null || currentTargetNexus != null;

    public void Init(UnitBase unitBase, UnitMovement movement)
    {
        unit = unitBase;
        unitMovement = movement;

        // 적 레이어 설정
        string enemyLayerName = unit.Team == Team.Player ? "EnemyUnit" : "PlayerUnit";
        string enemyNexusLayerName = unit.Team == Team.Player ? "EnemyNexus" : "PlayerNexus";

        enemyLayer = LayerMask.GetMask(enemyLayerName);
        enemyNexusLayer = LayerMask.GetMask(enemyNexusLayerName);
    }


    private void Update()
    {
        if (unit == null || unit.IsDead) return;

        FindTarget();

        if (HasTarget)
        {
            float distanceToTarget = GetDistanceToTarget();

            if (distanceToTarget <= unit.Data.AttackRange)
            {
                if (unitMovement.IsMoving)
                {
                    unitMovement.Stop();
                }

                Attack();
            }
            else
            {
                Vector3 targetPosition = GetTargetPosition();
                unitMovement.MoveTowards(targetPosition);
            }
        }
        else
        {
            unitMovement.MoveDefault();
        }
    }



    /// <summary>
    /// 타겟 찾기 (우선순위 옵션 적용)
    /// </summary>
    private void FindTarget()
    {
        if (firstTargetUnit)
        {
            FindTargetWithUnitPriority();
        }
        else
        {
            FindTargetKeepCurrent();
        }
    }

    /// <summary>
    /// 방식 1: 항상 유닛 우선 (넥서스 공격 중에도 유닛 발견 시 즉시 전환)
    /// </summary>
    private void FindTargetWithUnitPriority()
    {
        if (currentTargetUnit != null)
        {
            if (currentTargetUnit.IsDead || !IsInDetectionRange(currentTargetUnit.transform.position))
            {
                currentTargetUnit = null;
            }
            else
            {
                return;  
            }
        }

        if (currentTargetNexus != null)
        {
            if (currentTargetNexus.IsDestroyed || !IsInDetectionRange(currentTargetNexus.transform.position))
            {
                currentTargetNexus = null;
            }
        }

        FindClosestUnit();

        if (currentTargetUnit != null)
        {
            if (currentTargetNexus != null)
            {
                currentTargetNexus = null;
            }
            return;
        }

        if (currentTargetNexus == null)
        {
            FindClosestNexus();
        }
    }

    /// <summary>
    /// 방식 2: 현재 타겟 유지 (넥서스 공격 중이면 유닛 무시)
    /// </summary>
    private void FindTargetKeepCurrent()
    {
        if (currentTargetUnit != null)
        {
            if (currentTargetUnit.IsDead || !IsInDetectionRange(currentTargetUnit.transform.position))
            {
                currentTargetUnit = null;
            }
            else
            {
                return; 
            }
        }

        if (currentTargetNexus != null)
        {
            if (currentTargetNexus.IsDestroyed || !IsInDetectionRange(currentTargetNexus.transform.position))
            {
                Debug.Log($"{unit.Data.Name} 넥서스 타겟 해제");
                currentTargetNexus = null;
            }
            else
            {
                return; 
            }
        }

        FindClosestUnit();

        if (currentTargetUnit == null)
        {
            FindClosestNexus();
        }
    }

    private void FindClosestUnit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            detectionRange,
            enemyLayer
        );

        if (hits.Length == 0) return;

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
            currentTargetUnit = closestEnemy;
        }
    }

    private void FindClosestNexus()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,detectionRange,enemyNexusLayer);

        if (hits.Length == 0) return;

        Nexus closestNexus = null;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            Nexus nexus = hit.GetComponent<Nexus>();

            if (nexus != null && !nexus.IsDestroyed)
            {
                float distance = Vector3.Distance(transform.position, nexus.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNexus = nexus;
                }
            }
        }

        if (closestNexus != null)
        {
            currentTargetNexus = closestNexus;
        }
    }

    private float GetDistanceToTarget()
    {
        if (currentTargetUnit != null)
        {
            return Vector3.Distance(transform.position, currentTargetUnit.transform.position);
        }
        else if (currentTargetNexus != null)
        {
            return Vector3.Distance(transform.position, currentTargetNexus.transform.position);
        }
        return float.MaxValue;
    }

    private Vector3 GetTargetPosition()
    {
        if (currentTargetUnit != null)
        {
            return currentTargetUnit.transform.position;
        }
        else if (currentTargetNexus != null)
        {
            return currentTargetNexus.transform.position;
        }
        return transform.position;
    }

    private bool IsInDetectionRange(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        return distance <= detectionRange;
    }

    private void Attack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= unit.Data.AttackSpeed)
        {
            attackTimer = 0f;
            int damage = unit.CurAtk;

            if (currentTargetUnit != null)
            {
                currentTargetUnit.TakeDamage(damage);

                if (currentTargetUnit.IsDead)
                {
                    Debug.Log($"{currentTargetUnit.Data.Name} 처치!");
                    currentTargetUnit = null;
                }
            }
            else if (currentTargetNexus != null)
            {
                currentTargetNexus.TakeDamage(damage);
                Debug.Log($"{unit.Data.Name} → {currentTargetNexus.Team} 넥서스 공격! (데미지: {damage})");

                if (currentTargetNexus.IsDestroyed)
                {
                    currentTargetNexus = null;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (unit == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, unit.Data.AttackRange);

        if (currentTargetUnit != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTargetUnit.transform.position);

            if (firstTargetUnit)
            {
                Gizmos.DrawWireSphere(currentTargetUnit.transform.position, 0.3f);
            }
        }
        else if (currentTargetNexus != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, currentTargetNexus.transform.position);
        }
    }
}