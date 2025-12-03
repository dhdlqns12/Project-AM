using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Singleton 구조 적용     <<- Basic 반 수업 자료

    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }

            return instance;
        }
    }

    private static void SetupInstance()
    {
        instance = FindAnyObjectByType<UIManager>();

        if (instance != null)
        {
            instance = new GameObject(typeof(UIManager).Name).AddComponent<UIManager>();

            DontDestroyOnLoad(instance.gameObject);
        }
    }

    #endregion

    #region SerializeField 변수 생성 (타 Class에서 참조할 가능성이 있을 경우 Property 형식 적용할 것)

    // Canvas 전환에 사용할 Object 모음
    [Header("Canvases")]
    [SerializeField] private GameObject canvasSelectStage;
    [SerializeField] private GameObject canvasPlayGame;
    [SerializeField] private GameObject canvasOptions;
    [SerializeField] private GameObject canvasGameResult;

    // Option 팝업에 사용할 Object 모음
    [Header("Options")]
    [SerializeField] private GameObject optionCase1;
    [SerializeField] private GameObject optionCase2;

    // GameResult 상황에 사용할 Object 모음
    [Header("GameResults")]
    [SerializeField] private GameObject gameClear;
    [SerializeField] private GameObject gameOver;

    #endregion

    // 이 Script를 Component로 보유한 객체를 담을 변수 생성 (UIManager)
    private GameObject thisGO;

    // Singleton_GameManager 정보를 참조할 변수 생성
    private GameManager gameManager;

    // 선택 중인 Scene을 판별할 변수 정의
    private string selectedScene;

    private void Awake()
    {
        #region Singleton 로직    <<- Basic 반 수업 자료

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        thisGO = this.gameObject;

        DontDestroyOnLoad(this.gameObject);

        #endregion

        #region null 체크 로직 모음

        // Canvases null 체크 로직
        if (canvasSelectStage == null || canvasPlayGame == null || canvasOptions == null || canvasGameResult == null)
        {
            Debug.LogError("Canvases가 정상적으로 연동되지 않음");
            return;
        }

        // Options null 체크 로직
        if (optionCase1 == null || optionCase2 == null)
        {
            Debug.LogError("Options가 정상적으로 연동되지 않음");
            return;
        }

        // GameResults null 체크 로직
        if (gameClear == null || gameOver == null)
        {
            Debug.LogError("GameResults가 정상적으로 연동되지 않음");
            return;
        }

        #endregion
    }
    
    private void Start()
    {
        // 선택 중인 Scene 값 갱신_빈 값     <<- Scene 선택을 누르지 않아도 게임 시작 버튼이 눌리지 않도록 의도했습니다.
        selectedScene = "";
    }


    #region 버튼 동작 메서드 모음


    /// <summary>
    /// 메서드 내에서만 사용할 메서드 모음
    /// </summary>
    
    // 메서드 - 매개 변수 Object의 자식 Object 모두 비활성화
    private void OffSetActiveInChild(GameObject go)
    {
        // 반복문_매개 변수 Object의 자식 Object 비활성화
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // 메서드 - 선택 중인 Scene 값과 동일한 이름을 가진 Scene이 이미 Load되어 있는지 체크 (feat. GPT)
    private bool IsSceneLoaded()
    {
        // 현재 Load된 Scene의 개수 변수 값 생성 및 갱신_일회용
        int count = SceneManager.sceneCount;

        // 반복문 - Scene 이름 판별_선택 중인 Scene 값과 이름이 같은가?
        for (int i = 0; i < count; i++)
        {
            // Scene 정보를 담을 변수 생성 및 갱신_일회용
            Scene scene = SceneManager.GetSceneAt(i);

            // 조건문 - Scene 이름이 선택 중인 Scene 값과 같을 경우
            if (scene.name == selectedScene)
            {
                // bool 값 반환_true
                return true;
            }
        }

        // 반복문 순환을 모두 통과하였을 경우
        // bool 값 반환_false
        return false;
    }


    /// <summary>
    /// Canvas 전환 관련 메서드
    /// </summary>
    
    // 메서드 - Canvas 창 전환
    public void SwitchCanvas(GameObject go)
    {
        if (thisGO == null)
        {
            Debug.Log("thisGO null 상태, 초기화 실행");
            thisGO = gameObject;
        }

        // 선택 중인 Scene이 없을 경우
        if (selectedScene == "")
        {
            // LogWarning 출력
            Debug.LogWarning("Scene가 선택되지 않음");
            // 메서드 종료
            return;
        }

        // 메서드 호출 - 부모 Object 제외 모두 닫기
        OffSetActiveInChild(thisGO.gameObject);

        // Object(= Canvas) 활성화_매개 변수
        go.SetActive(true);

        // 조건문 - 매개 변수로 지정된 Object가 'canvasSelectStage'일 경우
        if (go == canvasSelectStage)
        {
            // 선택 중인 Scene 값 갱신_빈 값
            selectedScene = "";
        }
    }
   

    /// <summary>
    /// 팝업 On/Off 관련 메서드 모음
    /// </summary>
    
    // 메서드 - Option 팝업 활성화
    public void OnCanvasOptions(int i)
    {
        // Singleton_GameManager 참조           <<- 필요한 곳에서 직접 참조하라는 피드백 반영 ("Awake, Start 에서 참조하는 것은 불안정하다")
        gameManager = GameManager.Instance;

        Debug.Log(gameManager);

        // Singleton 메서드 호출 - Unity 시간 배속 조정_시간 정지
        gameManager.AdjustTime(0);

        // Option 팝업 활성화
        canvasOptions.SetActive(true);

        // 조건문 - 조건에 따라 함수 실행_매개 변수
        switch (i)
        {
            // 조건이 1일 경우
            case 1:
                // 조건에 대응하는 Object 활성화
                optionCase1.SetActive(true);
                break;
            // 조건이 2일 경우
            case 2:
                // 조건에 대응하는 Object 활성화
                optionCase2.SetActive(true);
                break;
        }
    }

    // 메서드 - Option 팝업 비활성화
    public void OffCanvasOptions()
    {
        // Singleton_GameManager 참조         <<- 필요한 곳에서 직접 참조하라는 피드백 반영 ("Awake, Start 에서 참조하는 것은 불안정하다")
        gameManager = GameManager.Instance;

        // Option 팝업 내 모든 대응 Object 비활성화
        optionCase1.SetActive(false);
        optionCase2.SetActive(false);

        // Option 팝업 비활성화
        canvasOptions.SetActive(false);

        // Singleton 메서드 호출 - Unity 시간 배속 조정_시간 정상화
        gameManager.AdjustTime(1);
    }


    /// <summary>
    /// Scene 선택 관련 메서드
    /// </summary>
    
    // 메서드 - 선택 중인 Scene 값 갱신
    public void SelectScene(string selectScene)
    {
        // 선택 중인 Scene 값 갱신_매개 변수
        selectedScene = selectScene;

        Debug.Log($"Stage 선택: {selectedScene}");
    }


    /// <summary>
    /// Scene Load/Unload 관련 메서드 모음    <<- GameManager로 이전해야 할까?
    /// </summary>

    // 메서드 - Scene Load (Async 활용)
    public void LoadScene()
    {
        // Scene Load_선택 중인 Scene 값
        SceneManager.LoadSceneAsync(selectedScene, LoadSceneMode.Single);
    }
    
    // 메서드 - Scene Load_Additive (Async 활용)
    public void LoadSceneAdditive()
    {
        // 선택 중인 Scene이 없을 경우
        if (selectedScene == "")
        {
            // LogWarning 출력
            Debug.LogWarning("Scene가 선택되지 않음");
            // 메서드 종료
            return;
        }

        // 조건문 - 선택 중인 Scene과 동일한 이름을 가진 Scene이 이미 Load되어 있을 경우
        if (IsSceneLoaded())
        {
            // Load되어 있는 Scene 제거
            UnloadScene();
        }

        // Scene Load_선택 중인 Scene 값, 현재 Scene을 유지한 채 아래에 Load
        SceneManager.LoadSceneAsync(selectedScene, LoadSceneMode.Additive);
    }

    // 메서드 - Scene Unload (Async 활용)
    public void UnloadScene()
    {
        // 조건문 - 선택 중인 Scene과 동일한 이름을 가진 Scene이 Load되어 있지 않을 경우
        if (!IsSceneLoaded())
        {
            Debug.Log($"'{selectedScene}' 은(는) 현재 로드되어 있지 않음");
            return;
        }

        // Scene Unload_선택 중인 Scene
        SceneManager.UnloadSceneAsync(selectedScene);
    }

    // 메서드 - Scene Reload_현재 실행중인 Scene
    public void ReloadScene()
    {
        // 메서드 호출 - Scene Unload
        UnloadScene();

        // 메서드 호출 - Scene Load_Additive
        LoadSceneAdditive();

        // 메서드 호출 - 현재 실행중인 Scene Reload        <<- Unity 입문 강의 내 코드 응용
        // SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// GameResult 관련 메서드 모음
    /// </summary>

    // 메서드 - Game Result 판별 후 UI 활성화 (GameManager 연동 대기)
    public void OnGameReusltUI(bool gameState)
    {
        // 게임에서 승리했을 경우
        if (gameState)
        {
            // GameClear 호출
            gameClear.SetActive(true);
        }
        // 게임에서 패배했을 경우
        else if(gameState)
        {
            // GameOver 호출
            gameOver.SetActive(true);
        }
    }

    // 메서드 - Game Result UI 비활성화
    public void OffGameResultUI()
    {
        gameClear.SetActive(false);
        gameOver.SetActive(false);
    }


    /// <summary>
    /// GameManager 연관 메서드
    /// </summary>

    // 메서드 - Game 종료
    public void QuitGame()
    {
        // Singleton_GameManager 참조         <<- 필요한 곳에서 직접 참조하라는 피드백 반영 ("Awake, Start 에서 참조하는 것은 불안정하다")
        gameManager = GameManager.Instance;

        gameManager.QuitGame();
    }


    #endregion
}
