using System.Collections;
using UnityEngine;

public class ClampCamera : MonoBehaviour
{
    [Header("맵 설정")]
    [SerializeField] private float mapWidth; // 맵 전체 너비

    [Header("카메라 설정")]
    [SerializeField] private Camera cam;
    [SerializeField] private float dragSpeed; // 드래그 민감도

    [Header("부드러운 이동 설정")]
    [SerializeField] private float smoothTime;  // 부드러운 이동 속도 (낮을수록 빠름)

    [Header("더블 탭 설정")]
    [SerializeField] private float doubleTapTime;  // 더블 탭 인식 시간
    [SerializeField] private float centerMoveSpeed;  // 중앙 이동 속도

    private float minCameraX;
    private float maxCameraX;
    private Vector3 drag;
    private bool isDragging;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private float lastTapTime = 0f;
    private bool isMovingToCenter = false;

    private void Reset()
    {
        mapWidth = 120;
        dragSpeed = 1f;
        smoothTime = 0.15f;
        doubleTapTime = 0.3f;
        centerMoveSpeed = 30f;
        cam = Camera.main;
    }

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        CalculateCameraBounds();
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        HandleDoubleTap();

        if (!isMovingToCenter)
        {
            HandleDragInput();
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
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

    private void HandleDoubleTap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastTap = Time.time - lastTapTime;

            if (timeSinceLastTap <= doubleTapTime)
            {
                MoveToCenter();
            }

            lastTapTime = Time.time;
        }
    }

    /// <summary>
    /// 드래그 입력 처리
    /// </summary>
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

            // targetPosition 업데이트 (실제 이동은 SmoothDamp가 처리)
            Vector3 newPos = targetPosition + new Vector3(difference.x * dragSpeed, 0, 0);
            newPos.x = Mathf.Clamp(newPos.x, minCameraX, maxCameraX);
            newPos.y = 0;
            newPos.z = -10;

            targetPosition = newPos;
            drag = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    /// <summary>
    /// 중앙(0, 0)으로 부드럽게 이동
    /// </summary>
    private void MoveToCenter()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToCenterCoroutine());
    }

    private IEnumerator MoveToCenterCoroutine()
    {
        isMovingToCenter = true;
        isDragging = false;

        Vector3 centerPosition = new Vector3(0, 0, -10);

        // 경계 체크
        centerPosition.x = Mathf.Clamp(centerPosition.x, minCameraX, maxCameraX);

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, centerPosition);
        float duration = distance / centerMoveSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // EaseInOut으로 부드럽게
            t = t * t * (3f - 2f * t);

            targetPosition = Vector3.Lerp(startPosition, centerPosition, t);

            yield return null;
        }

        targetPosition = centerPosition;
        isMovingToCenter = false;

        Debug.Log("중앙 이동 완료!");
    }
}