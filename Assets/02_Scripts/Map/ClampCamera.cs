using UnityEngine;

public class ClampCamera : MonoBehaviour
{
    [Header("맵 설정")]
    [SerializeField] private float mapWidth; // 맵 전체 너비

    [Header("카메라 설정")]
    [SerializeField] private Camera cam;
    [SerializeField] private float dragSpeed; // 드래그 민감도

    private float minCameraX;
    private float maxCameraX;
    private Vector3 drag;
    private bool isDragging;

    private void Reset()
    {
        mapWidth = 144f;
        dragSpeed = 1f;
    }

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        Init();
        CalculateCameraBounds();
    }

    private void  LateUpdate()
    {
        HandleDragInput();
    }

    private void Init()
    {
        mapWidth = 144f;
        dragSpeed = 1f;
    }

    private void CalculateCameraBounds()
    {
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;

        float mapHalfWidth = mapWidth / 2f;

        minCameraX = -mapHalfWidth + cameraHalfWidth;  // -72 + 17.78 = -54.22
        maxCameraX = mapHalfWidth - cameraHalfWidth;   //  72 - 17.78 =  54.22

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
}
