using UnityEngine;

public class Nexus : MonoBehaviour
{
    [Header("넥서스 설정")]
    [SerializeField] private Team team;
    [SerializeField] private int maxHp = 1500;

    private int currentHp;

    public Team Team => team;
    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsDestroyed => currentHp <= 0;

    private void Start()
    {
        currentHp = maxHp;

        // 레이어 설정
        gameObject.layer = team == Team.Player ? LayerMask.NameToLayer("PlayerNexus") : LayerMask.NameToLayer("EnemyNexus");
    }

    /// <summary>
    /// 데미지 받기
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (IsDestroyed) return;

        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        Debug.Log($"{team} 넥서스 피격 (데미지: {damage}, 남은 HP: {currentHp}/{maxHp})");
        if (currentHp <= 0)
        {
            OnDestroyed();
        }
    }

    /// <summary>
    /// 넥서스 파괴
    /// </summary>
    private void OnDestroyed()
    {
        if (team == Team.Player)
        {
            GameManager.Instance.PlayerDead();
        }
        else
        {
            GameManager.Instance.EnemyDead();
        }

        // 넥서스 비활성화
        gameObject.SetActive(false);
    }
}
