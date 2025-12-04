using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private UnitBase unit;
    private UnitMovement unitMovement;

    [Header("전투 설정")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask enemyNexusLayer;

    [Header("타겟팅")]
    [SerializeField] private float scanInterval = 0.2f;

    private UnitBase currentTargetUnit;
    private Nexus currentTargetNexus;
    private float attackTimer = 0f;
    private float scanTimer = 0f;

    private bool HasTarget => currentTargetUnit != null || currentTargetNexus != null;

    public void Init(UnitBase unitBase, UnitMovement movement)
    {
        unit = unitBase;
        unitMovement = movement;

        string enemyLayerName = unit.Team == Team.Player ? "EnemyUnit" : "PlayerUnit";
        string enemyNexusLayerName = unit.Team == Team.Player ? "EnemyNexus" : "PlayerNexus";

        enemyLayer = LayerMask.GetMask(enemyLayerName);
        enemyNexusLayer = LayerMask.GetMask(enemyNexusLayerName);

        Debug.Log($"{unit.Data.Name} 타겟팅 초기화");
    }

    private void Update()
    {
        if (unit == null || unit.IsDead) return;

        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            scanTimer = 0f;
            FindClosestTarget();
        }

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
    /// 가장 가까운 타겟 찾기 (유닛 우선, 없으면 넥서스)
    /// </summary>
    private void FindClosestTarget()
    {
        FindClosestUnit();

        if (currentTargetUnit == null)
        {
            FindClosestNexus();
        }
    }

    private void FindClosestUnit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);

        if (hits.Length == 0)
        {
            currentTargetUnit = null;
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

        if (closestEnemy != null && closestEnemy != currentTargetUnit)
        {
            currentTargetUnit = closestEnemy;
            currentTargetNexus = null;
        }
        else if (closestEnemy == null)
        {
            currentTargetUnit = null;
        }
    }

    private void FindClosestNexus()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            detectionRange,
            enemyNexusLayer
        );

        if (hits.Length == 0)
        {
            currentTargetNexus = null;
            return;
        }

        Nexus closestNexus = null;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            Nexus nexus = hit.GetComponent<Nexus>();

            if (nexus != null && !nexus.IsDestroyed)
            {
                Vector3 closestPoint = hit.ClosestPoint(transform.position);
                float distance = Vector3.Distance(transform.position, closestPoint);

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
        else
        {
            currentTargetNexus = null;
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
            Collider2D nexusCollider = currentTargetNexus.GetComponent<Collider2D>();
            if (nexusCollider != null)
            {
                Vector3 closestPoint = nexusCollider.ClosestPoint(transform.position);
                return Vector3.Distance(transform.position, closestPoint);
            }
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
            Collider2D nexusCollider = currentTargetNexus.GetComponent<Collider2D>();
            if (nexusCollider != null)
            {
                return nexusCollider.ClosestPoint(transform.position);
            }
            return currentTargetNexus.transform.position;
        }
        return transform.position;
    }

    private void Attack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= unit.Data.AttackSpeed)
        {
            attackTimer = 0f;
            int damage = unit.CurAtk;

            PlayAttackSound();

            if (currentTargetUnit != null)
            {
                currentTargetUnit.TakeDamage(damage);

                if (currentTargetUnit.IsDead)
                {
                    currentTargetUnit = null;
                }
            }
            else if (currentTargetNexus != null)
            {
                currentTargetNexus.TakeDamage(damage);

                if (currentTargetNexus.IsDestroyed)
                {
                    currentTargetNexus = null;
                }
            }
        }
    }

    private void PlayAttackSound()
    {
        if (unit == null) return;

        switch (unit.Data.Type)
        {
            case Enums.UnitType.Warrior:
                GameManager.Instance.AudioManager.WarriorAttackSFX();
                break;

            case Enums.UnitType.Archer:
                GameManager.Instance.AudioManager.ArcherAtttackSFX();
                break;
        }
    }

    private void OnDrawGizmos()
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
            Gizmos.DrawWireSphere(currentTargetUnit.transform.position, 0.3f);
        }
        else if (currentTargetNexus != null)
        {
            Gizmos.color = Color.magenta;

            Collider2D nexusCollider = currentTargetNexus.GetComponent<Collider2D>();
            if (nexusCollider != null)
            {
                Vector3 closestPoint = nexusCollider.ClosestPoint(transform.position);
                Gizmos.DrawLine(transform.position, closestPoint);
                Gizmos.DrawWireSphere(closestPoint, 0.2f);
            }
        }
    }
}