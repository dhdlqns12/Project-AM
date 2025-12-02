using UnityEngine;

public class GameManager :Singleton<GameManager>
{
    [SerializeField] private AudioManager audioManager;

    public AudioManager AudioManager
    {
        get { return audioManager; }
        set { audioManager = value; }
    }
    
    protected override void Init()
    {
        ResourceManager.Init();
        CreateStageManager();
        audioManager.Init();
    }

    //상대, 우리편 불값 분리 조정
    private bool isPlayerDead;

    public bool IsPlayerDead
    {
        get => isPlayerDead;
        set => isPlayerDead = value; 
    }

    private bool isEnemyDead;

    public bool IsEnemyDead
    {
        get => isEnemyDead;
        set => isEnemyDead = value;
    }

    private void CreateStageManager()
    {
        GameObject stageManagerObj = new GameObject("StageManager");
        stageManagerObj.AddComponent<StageManager>();
    }
    
    public void GameStart()
    {
        
    }

    public void PlayerDead()
    {
        isPlayerDead = true;
        GameOver();
    }

    public void EnemyDead()
    {
        isEnemyDead = true;
        GameOver();
    }

    private void GameOver()
    {
        if (isEnemyDead)
        {
            Debug.Log("플레이어 승리");
            //게임 정지 , 플레이어 승리 UI
        }
        else if (isPlayerDead)
        {
            Debug.Log("플레이어 패배");
            //게임 정지 , 플레이어 패배 UI
        }
        else
        {
            Debug.Log("승패 판정 불가");
        }
    }
    
    public void ResetStageData()
    {
        isPlayerDead = false;
        isEnemyDead  = false;
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
