using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager :Singleton<GameManager>
{
    protected override void Init()
    {
        ResourceManager.Init();
        CreateStageManager();
        CreateAudioManager();
    }

    //상대, 우리편 불값 분리 조정
    private bool isDead;

    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }
    
    private void CreateStageManager()
    {
        GameObject stageManagerObj = new GameObject("StageManager");
        stageManagerObj.AddComponent<StageManager>();
    }

    private void CreateAudioManager()
    {
        GameObject audioManagerObj = new GameObject("AudioManager");
        audioManagerObj.AddComponent<AudioManager>();
        
    }
    
    public void GameStart()
    {
        
    }

    public void GameOver()
    {
        //게임씬 오브젝트로부터 정보를 전달받아 isDead전환
        
        if (isDead)
        {
            //플레이어 패배
        }
        
        //플레이어 승리
        isDead = true;
    }
    
    public void ResetStageData()
    {
        isDead = false;
        StageManager.Instance.ResetGold();
        /*
         * 플레이어 유닛,건물 초기화
         * 상대 유닛, 건물 초기화
         */
    }

    // 시간 정지
    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    // 시간 재개
    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }
}
