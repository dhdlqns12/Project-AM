using UnityEngine;
using TMPro;

public class TextTimer : MonoBehaviour
{
    private GameManager gameManager;

    private TextMeshProUGUI textTimer;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        textTimer = GetComponent<TextMeshProUGUI>();

        if (textTimer == null)
        {
            Debug.LogWarning("Timer Text가 연동되지 않음");
        }
    }

    void Update()
    {
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        textTimer.text = gameManager.PlayTime.ToString("F2");       // F2: 소수점 2자리까지 반영
    }
}
