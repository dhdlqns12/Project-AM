using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager :Singleton<GameManager>
{
    #region Timer 기능 관련 Property 모음

    // Timer 기능에 사용할 시간 값 정의_Property 형식
    private float playTime;
    public float PlayTime       // 외부에서 읽기만 가능
    {
        get { return playTime; }
        private set { playTime = value; }
    }

    // Timer 기능에서 Stage가 실행중인지 판별할 bool 값 정의_Property 형식
    private bool isRunning;
    public bool IsRunning       // 외부에서 읽기 & 쓰기 가능
    {
        get { return isRunning; }
        set { isRunning = value; }
    }

    #endregion

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
        CreateUnitDataManager();
        CreateEnemySpawnerManager();

    }

    private void Start()
    {
        GameStart();
    }

    private void Update()
    {
        GameUpdate();
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
        // Scene이 비활성화 상태임
        isRunning = false;
    }

    public void GameUpdate()
    {
        // Scene이 비활성화 상태일 경우
        if (!isRunning)
            // 반환 (아래 코드 무시)
            return;

        // Timer 기능 동작
        playTime += Time.deltaTime;
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
        playTime = 0f;      // 시간 값 초기화_0(초)
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
