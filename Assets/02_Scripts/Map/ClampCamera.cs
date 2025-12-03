using UnityEngine;

public class ClampCamera : MonoBehaviour
{
    [Header("맵 설정")]
    [SerializeField] private float mapWidth; // 맵 전체 너비

    [Header("카메라 설정")]
    [SerializeField] private Camera cam;
    [SerializeField] private float dragSpeed; // 드래그 민감도
    [SerializeField] private float smoothTime; // 부드러운 이동 시간

    private float minCameraX;
    private float maxCameraX;
    private Vector3 drag;
    private bool isDragging;

    private Vector3 targetPos;
    private Vector3 smoothVelocity;

    // 더블 탭 체크
    private float lastTapTime = 0f;
    private const float doubleTapDelay = 0.25f;

    private void Reset()
    {
        mapWidth = 120;
        dragSpeed = 1f;
        smoothTime = 0.2f;
        cam = Camera.main;
    }

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        CalculateCameraBounds();
        targetPos = transform.position;
    }

    private void LateUpdate()
    {
        HandleDragInput();
        HandleDoubleTap();

        // 카메라 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref smoothVelocity, smoothTime);
    }

    private void CalculateCameraBounds()
    {
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;

        float mapHalfWidth = mapWidth / 2f;

        minCameraX = -mapHalfWidth + cameraHalfWidth;
        maxCameraX = mapHalfWidth - cameraHalfWidth;

        cam.transform.position = new Vector3(minCameraX, 0, -10);

        Debug.Log($"Camera Bounds - Min: {minCameraX}, Max: {maxCameraX}");
    }

    private void HandleDragInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            drag = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = drag - currentPos;

            Vector3 newPos = transform.position + new Vector3(difference.x * dragSpeed, 0, 0);

            newPos.x = Mathf.Clamp(newPos.x, minCameraX, maxCameraX);

            transform.position = newPos;

            drag = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void HandleDoubleTap()
    {
        bool tapped = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        // PC: 마우스 더블클릭
        tapped = Input.GetMouseButtonDown(0);
#else
        // 모바일: 터치 더블탭
        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began)
            tapped = true;
#endif

        if (!tapped) return;

        if (Time.time - lastTapTime < doubleTapDelay)
        {
            //  더블탭 감지됨 (0,0)으로 스무스 이동
            MoveCenterSmooth();
        }

        lastTapTime = Time.time;
    }

    private void MoveCenterSmooth()
    {
        float centerX = Mathf.Clamp(0, minCameraX, maxCameraX);
        targetPos = new Vector3(centerX, 0, -10);
    }
}
