using UnityEngine;

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

    #region 타 Class에서 참조할 Property 생성_SerializeField 적용
    [SerializeField] private GameObject canvasStageSelect;
    public GameObject CanvasStageSelect {  get; }

    [SerializeField] private GameObject canvasPlayGame;
    public GameObject CanvasPlayGame { get; }

    [SerializeField] private GameObject canvasOptions;
    public GameObject CanvasOptions { get; }
    #endregion

    // 해당 Script를 Component로 가진 객체의 정보 정의
    private GameObject thisGO;

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
        // UI Canvas 관련 null 체크 로직
        if (canvasStageSelect == null || canvasPlayGame == null || canvasOptions == null)
        {
            Debug.LogError("UI Canvas가 정상적으로 연동되지 않았습니다!");

            return;
        }
        #endregion

        // 해당 Script를 Component로 가진 객체의 정보 갱신
        thisGO = this.gameObject;
    }

    // 메서드 - 반복문_부모 제외 모두 닫기(SetActive(false))
    private void OffSetActiveInChild(GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // 기능 Test용으로만 사용, 최종 제출 시 삭제 예정
    private void Start()
    {
        // OnCanvasStageSelect();      // 기능 Test용 코드, 최종 제출 시 삭제 예정
    }

    private void OnCanvasStageSelect()
    {
        OffSetActiveInChild(thisGO.gameObject);

        canvasStageSelect.SetActive(true);
    }

    private void OnCanvasPlayGame()
    {
        OffSetActiveInChild(thisGO.gameObject);

        canvasPlayGame.SetActive(true);
    }

    private void OnCanvasOptions()
    {
        canvasOptions.SetActive(true);
    }

    private void OffCanvasOptions()
    {
        canvasOptions.SetActive(false);
    }
}
