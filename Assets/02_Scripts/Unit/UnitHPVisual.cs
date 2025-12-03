using UnityEngine;

public class UnitHPVisual : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private UnitBase unit;

    [Header("Circle Sprites")]
    [SerializeField] private SpriteRenderer outlineRenderer;  // 외곽선 (고정)
    [SerializeField] private Transform fillTransform;  // Fill (크기 변경)
    [SerializeField] private SpriteRenderer fillRenderer;

    [Header("크기 설정")]
    [SerializeField] private float minFillScale = 0f;  // Fill 최소 크기

    private Vector3 originalFillScale;

    private void Start()
    {
        if (unit == null)
        {
            unit = GetComponent<UnitBase>();
        }

        if (fillTransform != null)
        {
            originalFillScale = fillTransform.localScale;
        }

        StartCoroutine(InitializeColors());
    }

    private void OnDisable()
    {
        StopCoroutine(InitializeColors());
    }

    private System.Collections.IEnumerator InitializeColors()
    {
        yield return new WaitForEndOfFrame();

        SpriteRenderer unitRenderer = GetComponent<SpriteRenderer>();
        if (unitRenderer != null)
        {
            Color unitColor = unitRenderer.color;

            if (outlineRenderer != null)
            {
                outlineRenderer.color = unitColor;
            }

            if (fillRenderer != null)
            {
                fillRenderer.color = unitColor;
            }
            unitRenderer.enabled = false;
        }

        UpdateVisual();
    }

    private void Update()
    {
        if (unit == null || unit.IsDead) return;

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (unit == null || fillTransform == null) return;

        float hpRatio = (float)unit.CurHp / unit.MaxHp;

        float fillScale = Mathf.Lerp(minFillScale, 1f, hpRatio);
        fillTransform.localScale = originalFillScale * fillScale;
    }
}