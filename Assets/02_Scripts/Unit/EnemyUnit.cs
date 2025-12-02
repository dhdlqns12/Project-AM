using UnityEngine;

public class EnemyUnit : UnitBase
{
    private UnitMovement movement;
    private UnitCombat combat;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        combat = GetComponent<UnitCombat>();
    }

    protected override void OnInitialized()
    {
        movement?.Init(this);
        combat?.Init(this,movement);

        Debug.Log($"{Data.Name} 생성 (Lv.{Data.Level})");
    }

    protected override void OnDamaged()
    {
        Debug.Log($"{Data.Name} 피격 (남은 체력: {CurHp}/{MaxHp})");
    }

    protected override void OnDeath()
    {
        Debug.Log($"{Data.Name} 사망!");

        movement?.Stop();
        

        // TODO: 골드/아이템 드롭
    }
}