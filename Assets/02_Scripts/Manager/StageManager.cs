using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    private int gold;

    public int Gold
    {
        get => gold;
        set => gold = value;
    }

    protected override void Init()
    {
        Debug.Log("스테이지 매니저 초기화");
        ResetGold();
    }

    public void SetStartGold()
    {
        gold = 1000;
    }

    public void ResetGold()
    {
        gold = 0;
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
    
    //ui랑 연결 방식 고민
    
}
