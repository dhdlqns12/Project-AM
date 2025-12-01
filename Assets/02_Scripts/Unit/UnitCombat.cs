using UnityEngine;

/// <summary>
/// 유닛 전투 처리
/// </summary>
public class UnitCombat : MonoBehaviour
{
    private UnitBase unit;
    private UnitMovement movement;

    private UnitBase currentTarget;
    private float attackTimer;

    [SerializeField] private LayerMask enemyLayer;  // Inspector에서 설정

    public void Init(UnitBase unitBase)
    {
        unit = unitBase;
        movement = GetComponent<UnitMovement>();
        attackTimer = 0f;

        Debug.Log($"{unit.Data.Name} 전투 시스템 초기화");
    }

    private void Update()
    {
        if (unit == null || unit.IsDead) return;

        // 타겟이 없거나 죽었으면 새로 찾기
        if (currentTarget == null || currentTarget.IsDead)
        {
            FindTarget();
        }

        // 타겟이 있으면 공격
        if (currentTarget != null && !currentTarget.IsDead)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= unit.Data.AttackSpeed)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 공격 범위 내에서 가장 가까운 적 찾기
    /// </summary>
    private void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,unit.Data.AttackRange,enemyLayer);

        float closestDistance = float.MaxValue;
        UnitBase closestEnemy = null;

        foreach (var hit in hits)
        {
            UnitBase enemy = hit.GetComponent<UnitBase>();

            if (enemy != null && unit.IsEnemy(enemy) && !enemy.IsDead)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        currentTarget = closestEnemy;

        if (currentTarget != null)
        {
            movement?.Stop();  // 적 발견 이동 중지
            Debug.Log($"[UnitCombat] {unit.Data.Name} 타겟 발견: {currentTarget.Data.Name}");
        }
        else
        {
            movement?.Resume();  // 적 없으면 이동 재개
        }
    }

    /// <summary>
    /// 타겟 공격
    /// </summary>
    private void Attack()
    {
        if (currentTarget == null || currentTarget.IsDead) return;

        // 공격 실행
        currentTarget.TakeDamage(unit.CurAtk);

        Debug.Log($"{unit.Data.Name} → {currentTarget.Data.Name} 공격 (데미지: {unit.CurAtk})");
    }

    /// <summary>
    /// 전투 중지 (사망 시)
    /// </summary>
    public void Stop()
    {
        currentTarget = null;
        attackTimer = 0f;
    }

    /// <summary>
    /// 디버그용 공격 범위 표시 (Gizmos)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (unit != null && unit.Data != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, unit.Data.AttackRange);
        }
    }
}