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

        SetColorByLevel();
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

    /// <summary>
    /// 레벨에 따라 색상 설정 (기획서 기준)
    /// </summary>
    private void SetColorByLevel()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        if (Team == Team.Enemy)
        {
            // 적 유닛
            if (Data.Type == Enums.UnitType.Warrior)
            {
                switch (Data.Level)
                {
                    case 1: spriteRenderer.color = new Color(0.8235295f, 0.8235295f, 0.8235295f); break;  
                    case 2: spriteRenderer.color = new Color(0.2509804f, 0.2509804f, 0.2509804f); break;  
                    case 3: spriteRenderer.color = new Color(0.0509804f, 0.0509804f, 0.0509804f); break; 
                }
            }
            else if (Data.Type == Enums.UnitType.Archer)
            {
                switch (Data.Level)
                {
                    case 1: spriteRenderer.color = new Color(0.9490197f, 0.8156863f, 0.9294118f); break; 
                    case 2: spriteRenderer.color = new Color(0.8470589f, 0.4313726f, 0.8039216f); break;    
                    case 3: spriteRenderer.color = new Color(0.4705883f, 0.1254902f, 0.4313726f); break; 
                }
            }
        }
        else
        {
            // 아군 유닛
            if (Data.Type == Enums.UnitType.Warrior)
            {
                switch (Data.Level)
                {
                    case 1: spriteRenderer.color = new Color(0.9803922f, 0.8901961f, 0.8431373f); break;     
                    case 2: spriteRenderer.color = new Color(0.9490197f, 0.6666667f, 0.5215687f); break;     
                    case 3: spriteRenderer.color = new Color(0.7529413f, 0.3098039f, 0.08235294f); break;  
                }
            }
            else if (Data.Type == Enums.UnitType.Archer)
            {
                switch (Data.Level)
                {
                    case 1: spriteRenderer.color = new Color(0.854902f, 0.9450981f, 0.8156863f); break;  
                    case 2: spriteRenderer.color = new Color(0.5529412f, 0.8509805f, 0.4470589f); break;  
                    case 3: spriteRenderer.color = new Color(0.2352941f, 0.4901961f, 0.1372549f); break;  
                }
            }
        }
    }
}
