using UnityEngine;

public enum Team
{
    Player,
    Enemy
}

public abstract class UnitBase : MonoBehaviour
{
    public Team Team { get; private set; }
    public UnitData Data { get; private set; }

    public int CurHp { get; private set; }
    public int CurAtk { get; private set; }
    public int CurDef { get; private set; }

    public int MaxHp { get; private set; }
    public bool IsDead => CurHp <= 0;

    public virtual void Init(UnitData data)
    {
        Data = data;
        Team = data.Team;

        // 스탯 초기화
        MaxHp = data.HP;
        CurHp = data.HP;
        CurAtk = data.Attack;
        CurDef = data.Defense;

        Debug.Log($"{data.Name} 초기화 (Team: {Team}, Type: {data.Type}, Lv.{data.Level})");

        OnInitialized();
    }

    /// <summary>
    /// 초기화 후 자식 클래스별 추가 작업
    /// </summary>
    protected virtual void OnInitialized() { }

    /// <summary>
    /// 데미지 받기
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (IsDead) return;

        int curDamage = Mathf.Max(1, damage - CurDef);
        CurHp -= curDamage;

        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();
        }

        OnDamaged();
    }

    /// <summary>
    /// 데미지를 받은 후 처리
    /// </summary>
    protected virtual void OnDamaged() { }

    protected virtual void Die()
    {
        Debug.Log($"{Data.Name} 사망");
        OnDeath();
        Destroy(gameObject, 1f);
    }

    /// <summary>
    /// 사망 시 처리
    /// </summary>
    protected virtual void OnDeath() { }

    /// <summary>
    /// 적 유닛인지 판별
    /// </summary>
    public bool IsEnemy(UnitBase other)
    {
        if (other == null) return false;
        return this.Team != other.Team;
    }

    /// <summary>
    /// 아군 유닛인지 판별
    /// </summary>
    public bool IsAlly(UnitBase other)
    {
        if (other == null) return false;
        return this.Team == other.Team;
    }
}
