using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Singleton 구조 적용_Basic 반 수업 자료

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

    #region SerializeField 변수 생성_타 Class에서 참조할 가능성이 있을 경우 Property 형식 적용할 것

    [Header("Canvases")]
    [SerializeField] private GameObject canvasSelectStage;
    [SerializeField] private GameObject canvasPlayGame;
    [SerializeField] private GameObject canvasOptions;
    [SerializeField] private GameObject canvasGameEnd;

    [Header("Stages")]
    [SerializeField] private GameObject stageSpawnPosition;     // Stage를 생성할 위치
    [SerializeField] private GameObject stage;

    [Header("Options")]
    [SerializeField] private GameObject[] optionCase;           // Option 버튼을 누른 위치 판단

    [Header("GameEnds")]
    [SerializeField] private GameObject gameClear;
    [SerializeField] private GameObject gameOver;

    #endregion

    // 이 Script를 Component로 보유한 객체를 담을 변수 생성 (UIManager)
    private GameObject thisGO;

    // Singleton_GameManager 정보를 참조할 변수 생성
    private GameManager gameManager;

    private void Awake()
    {
        #region Singleton 로직

        if (instance != null & instance != this)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }

        #endregion

        #region null 체크 로직 모음

        // Canvases null 체크 로직
        if (canvasSelectStage == null || canvasPlayGame == null || canvasOptions == null || canvasGameEnd == null)
        {
            Debug.LogError("Canvases가 정상적으로 연동되지 않음");
            return;
        }

        // Stages null 체크 로직
        if (stageSpawnPosition == null || stage == null)
        {
            Debug.LogError("Stages가 정상적으로 연동되지 않음");
            return;
        }

        // Options null 체크 로직
        if (optionCase == null)
        {
            Debug.LogError("Options가 정상적으로 연동되지 않음");
            return;
        }

        // GameEnds null 체크 로직
        if (gameClear == null || gameOver == null)
        {
            Debug.LogError("GameEnds가 정상적으로 연동되지 않음");
            return;
        }

        #endregion

        // 해당 Script를 Component로 가진 객체의 정보 갱신
        thisGO = this.gameObject;
    }
    
    private void Start()
    {
        // OnCanvasStageSelect();      // 기능 Test용 메서드, 최종 제출 시 삭제 예정
    }



    #region 버튼 동작 메서드 모음

    #region     메서드 내에서만 사용할 메서드 모음

    // 메서드 - 부모 Object 제외 모두 닫기 (SetActive(false))
    private void OffSetActiveInChild(GameObject go)
    {
        // 반복문_Object의 자식 Object 비활성화_매개 변수
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // 메서드 - 매개 변수와 동일한 이름을 가진 Scene이 이미 Load되어 있는지 체크 (feat. GPT)
    private bool IsSceneLoaded(string sceneName)
    {
        // 현재 Load된 Scene의 개수 변수 값 생성 및 갱신_일회용
        int count = SceneManager.sceneCount;

        // 반복문 - Scene 이름 판별_매개변수와 이름이 같은가?
        for (int i = 0; i < count; i++)
        {
            // Scene 정보를 담을 변수 생성 및 갱신_일회용
            Scene scene = SceneManager.GetSceneAt(i);

            // 반복문 - Scene 이름이 매개 변수와 같을 경우
            if (scene.name == sceneName)
            {
                return true;
            }
        }

        // 반복문 순환을 모두 통과하였을 경우
        return false;
    }

    #endregion

    #region     Canvas 전환 관련 메서드 모음

    // 메서드 - Canvas 창 전환
    public void SwitchCanvas(GameObject go)
    {
        // 메서드 호출 - 부모 Object 제외 모두 닫기
        OffSetActiveInChild(thisGO.gameObject);

        // Object(= Canvas) 활성화_매개 변수
        go.SetActive(true);
    }

    #endregion

    #region     팝업 On/Off 관련 메서드 모음

    // 메서드 - Option 팝업 활성화
    public void OnCanvasOptions(int i)
    {
        // Singleton_GameManager 참조   <- 필요한 곳에서 직접 참조하라는 피드백 반영 ("Awake, Start 에서 참조하는 것은 불안정하다")
        gameManager = GameManager.Instance;

        // Singleton 메서드 호출 - Unity 시간 정지
        gameManager.StopTime();

        // Option 팝업 활성화
        canvasOptions.SetActive(true);

        // 조건문 - 조건에 따라 함수 실행_매개 변수
        switch (i)
        {
            // 조건이 0일 경우
            case 0:
                // 조건에 대응하는 Object 활성화
                optionCase[0].SetActive(true);
                break;
            // 조건이 1일 경우
            case 1:
                // 조건에 대응하는 Object 활성화
                optionCase[1].SetActive(true);
                break;
        }
    }

    // 메서드 - Option 팝업 비활성화
    public void OffCanvasOptions()
    {
        // Singleton_GameManager 참조   <- 필요한 곳에서 직접 참조하라는 피드백 반영 ("Awake, Start 에서 참조하는 것은 불안정하다")
        gameManager = GameManager.Instance;

        // Option 팝업 내 모든 대응 Object 비활성화
        optionCase[0].SetActive(false);
        optionCase[1].SetActive(false);

        // Option 팝업 비활성화
        canvasOptions.SetActive(false);

        // Singleton 메서드 호출 - Unity 시간 재생
        gameManager.ResumeTime();
    }

    #endregion

    #region     Scene Load/Unload 관련 메서드 모음

    // 메서드 - Scene Load (Async 활용)
    public void LoadSceneAdditive(string sceneName)
    {
        // 조건문 - 매개 변수와 동일한 이름을 가진 Scene이 이미 Load되어 있을 경우
        if (IsSceneLoaded(sceneName))
        {
            // Load되어 있는 Scene 제거
            UnloadScene(sceneName);
        }

        // Scene Load_매개 변수
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    // 메서드 - Scene Unload (Async 활용)
    public void UnloadScene(string sceneName)
    {
        // 조건문 - 매개 변수와 동일한 이름을 가진 Scene이 이미 Load되어 있지 않을 경우
        if (!IsSceneLoaded(sceneName))
        {
            Debug.Log($"'{sceneName}' 은(는) 현재 로드되어 있지 않음");
            return;
        }

        // Scene Unload_매개 변수
        SceneManager.UnloadSceneAsync(sceneName);
    }

    // 메서드 - Scene Load_현재 실행중인 Scene과 동일한 Scene
    public void RestartScene(string sceneName)
    {
        // 메서드 호출 - Scene Unload_매개 변수
        UnloadScene(sceneName);

        // 메서드 호출 - Scene Load_매개 변수
        LoadSceneAdditive(sceneName);
    }

    #endregion

    #endregion
}
