using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    private int gold;

    private const int START_GOLD = 500;

    public int Gold
    {
        get => gold;
        set => gold = value;
    }

    private int enemyKillCount;

    private const int GOLD_PER_KILL = 10;

    private const int BASE_KILL = 4;

    public int EnemyKillCount
    {
        get => enemyKillCount;
        set => enemyKillCount = value;
    }

    protected override void Init()
    {
        Debug.Log("스테이지 매니저 초기화");
        ResetGold();
        ResetKillCount();
    }

    // public void SetStartGold()
    // {
    //     gold = 1000;
    // }

    #region 골드 관련 기본 로직

    public void ResetGold()
    {
        gold = START_GOLD;
    }
    public void IncreaseGold(int amount)
    {
        if(amount <=0) return;
        gold += amount;
    }

    public void ConsumeGold(int amount)
    {
        if(amount <=0) return;
        gold -= amount;
    }
    
    #endregion

    #region 킬카운트 관련 로직

    private void ResetKillCount()
    {
        enemyKillCount = 0;
    }
    
    public void PlusKillCount()
    {
        enemyKillCount++;
        Debug.Log("현재 킬 수: "+enemyKillCount);
        if (enemyKillCount >= BASE_KILL)
        {
            RewardKillCount();
            enemyKillCount -= BASE_KILL;
        }
    }

    private void RewardKillCount()
    {
        gold += GOLD_PER_KILL;
    }
    
    #endregion
    
    //ui랑 연결 방식 고민
    //enemy킬카운트 필드 가지고 퍼블릭으로, 리셋할 때마다 초기화시키고, n킬마다 골드지급하는 로직
}
