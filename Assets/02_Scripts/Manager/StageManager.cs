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
    }

    public void PlusGold(int amount)
    {
        if(amount <=0) return;
        gold += amount;
    }

    public void MinusGold(int amount)
    {
        if(amount <=0) return;
        gold -= amount;
    }

    public void ClearGold()
    {
        gold = 0;
    }
    
}
