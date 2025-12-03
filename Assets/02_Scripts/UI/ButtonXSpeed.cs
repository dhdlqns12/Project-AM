using UnityEngine;
using TMPro;

public class ButtonXSpeed : MonoBehaviour
{
    private GameManager gameManager;

    private TextMeshProUGUI buttonXSpeed;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        buttonXSpeed = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonXSpeed == null)
        {
            Debug.LogWarning("배속 Text가 연동되지 않음");
        }
    }

    public void UpdateXSpeed()
    {
        if (Time.timeScale == 1)
        {
            gameManager.AdjustTime(2);

            buttonXSpeed.text = $"{Time.timeScale}";
        }
        else if (Time.timeScale == 2)
        {
            gameManager.AdjustTime(1);

            buttonXSpeed.text = $"{Time.timeScale}";
        }
    }
}
