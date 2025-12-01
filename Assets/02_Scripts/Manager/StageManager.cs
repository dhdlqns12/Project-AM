using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private int gold;

    public int Gold
    {
        get => gold;
        set => gold = value;
    }
    
    public void Init()
    {
        
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
