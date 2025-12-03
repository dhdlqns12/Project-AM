using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Singleton_GameManager 정보를 참조할 변수 생성
    private GameManager gameManager;

    // Main Game Scene에 위치한 Camera 정보를 담을 변수 생성
    [SerializeField] private GameObject mainGameCam;

    private void Awake()
    {
        // Singleton_GameManager 참조
        gameManager = GameManager.Instance;

        mainGameCam = this.gameObject;

        Debug.Log(mainGameCam);

        // Camera null 체크 로직
        if (mainGameCam == null)
        {
            Debug.LogError("MainGame Scene 내 Camera가 Script에 연동되지 않음");
        }
    }

    void Update()
    {
        // 메서드 호출 - Game 상태에 따른 Camera On/Off
        TurnCam();
    }

    // 메서드 - Game 상태에 따른 Camera On/Off
    public void TurnCam()
    {
        // Stage가 진행중일 경우
        if (gameManager.IsRunning)
        {
            // Camera Off
            mainGameCam.SetActive(false);

            Debug.Log("false 완료");
        }
        // Stage가 진행중이 아닐 경우
        else
        {
            // Camera On
            mainGameCam.SetActive(true);
        }
    }
}
