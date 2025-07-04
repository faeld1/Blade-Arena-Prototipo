using UnityEngine;
using TMPro;

public class GoldDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        if (goldText == null)
            goldText = GetComponent<TextMeshProUGUI>();
        UpdateGold(GameManager.Instance ? GameManager.Instance.GetCurrentGold() : 0);
    }

    private void OnEnable()
    {
        GameManager.OnGoldChanged += UpdateGold;
    }

    private void OnDisable()
    {
        GameManager.OnGoldChanged -= UpdateGold;
    }

    private void UpdateGold(int value)
    {
        if (goldText != null)
            goldText.text = value.ToString();
    }
}
