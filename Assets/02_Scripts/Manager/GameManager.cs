using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager :Singleton<GameManager>
{
    [SerializeField] private StageManager stageManager;
    
    protected override void Init()
    {
        ResourceManager.Init();
        UnitDataManager.Instance.Init();
    }

    private bool isDead;

    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }
    private void CreateStageManager()
    {
        GameObject stageManagerObj = new GameObject("StageManager");
        stageManager = stageManagerObj.AddComponent<StageManager>();
    }
    
    public void GameStart()
    {
        
    }

    public void GameOver()
    {
        if (isDead)
        {
            //플레이어 패배
        }
        
        //플레이어 승리
    }
    
    public void ResetStageData()
    {
        isDead = false;
        StageManager.Instance.ClearGold();
        /*
         * 플레이어 유닛,건물 초기화
         * 상대 유닛, 건물 초기화
         */
    }
    
    
}
