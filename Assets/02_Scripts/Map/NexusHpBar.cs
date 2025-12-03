using UnityEngine;

public class NexusHPBar : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Nexus nexus;

    [Header("HP바 오브젝트")]
    [SerializeField] private Transform hpFill;  

    [Header("설정")]
    [SerializeField] private float barWidth = 2f; 

    private Vector3 originalScale;

    private void Start()
    {
        if (nexus == null)
        {
            nexus = GetComponentInParent<Nexus>();
        }

        if (hpFill != null)
        {
            originalScale = hpFill.localScale;
        }

        UpdateHPBar();
    }

    private void Update()
    {
        UpdateHPBar();
    }


    private void UpdateHPBar()
    {
        if (nexus == null || hpFill == null) return;

        float hpRatio = (float)nexus.CurrentHp / nexus.MaxHp;

        Vector3 newScale = originalScale;
        newScale.x = originalScale.x * hpRatio;
        hpFill.localScale = newScale;

        float offset = (barWidth * (1f - hpRatio)) * 0.5f;
        hpFill.localPosition = new Vector3(-offset, 0, 0);
    }
}