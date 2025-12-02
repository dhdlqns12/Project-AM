using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [Header("맵 경계")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void Reset()
    {
        minX = -59.5f;
        maxX = 59.5f;
        minY = -9.5f;
        maxY = 9.5f;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        // 경계선 표시
        Gizmos.color = Color.red;

        // 상단
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(maxX, maxY));
        // 하단
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY));
        // 좌측
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(minX, maxY));
        // 우측
        Gizmos.DrawLine(new Vector3(maxX, minY), new Vector3(maxX, maxY));
    }
}