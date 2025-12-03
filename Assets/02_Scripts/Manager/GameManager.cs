using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager :Singleton<GameManager>
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Button _button;
    [SerializeField] private Button _button2;
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
        CreateUnitDataManager();
        CreateEnemySpawnerManager();

    }

    private void Start()
    {
        _button.onClick.AddListener(PlayerDead);
        _button2.onClick.AddListener(EnemyDead);
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
    
    private void CreateUnitDataManager()
    {
        GameObject unitDataManagerObj = new GameObject("UnitDataManager");
        unitDataManagerObj.AddComponent<UnitDataManager>();
    }

    private void CreateEnemySpawnerManager()
    {
        GameObject EnemySpawnManagerObj = new GameObject("EnemySpawnerDataManager");
        EnemySpawnManagerObj.AddComponent<EnemySpawnerDataManager>();
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
            UIManager.Instance.OnGameResultUI(true);
            //게임 정지 , 플레이어 승리 UI
        }
        else if (isPlayerDead)
        {
            Debug.Log("플레이어 패배");
            UIManager.Instance.OnGameResultUI(false);
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

    /// <summary>
    /// Unity 시간 배속 조정 메서드
    /// 매개변수 내 값에 의거하여 시간 배속이 조정됩니다.
    /// 0 => 시간 정지, 1 => 시간 정상화, x => x배속
    /// </summary>
    public void AdjustTime(float speed)
    {
        Time.timeScale = speed;
    }

    /// <summary>
    /// 게임 종료 메서드    feat. Chat GPT
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("게임 종료 (Unity Test 환경)");

#if UNITY_EDITOR        // && !UNITY_INCLUDE_TESTS
        // 에디터에서 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임 종료
        Application.Quit();
#endif
    }
}
