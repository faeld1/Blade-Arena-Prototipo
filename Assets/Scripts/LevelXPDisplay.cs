using UnityEngine;
using TMPro;

public class LevelXPDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;

    private void Awake()
    {
        if (displayText == null)
            displayText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameManager.OnXPChanged += UpdateDisplay;
        GameManager.OnLevelUp += LevelUp;
        UpdateDisplay(GameManager.Instance ? GameManager.Instance.playerLevel : 0,
                      GameManager.Instance ? GameManager.Instance.playerXP : 0,
                      GameManager.Instance ? GameManager.Instance.xpToNextLevel[Mathf.Clamp(GameManager.Instance.playerLevel,0,GameManager.Instance.xpToNextLevel.Length-1)] : 0);
    }

    private void OnDisable()
    {
        GameManager.OnXPChanged -= UpdateDisplay;
        GameManager.OnLevelUp -= LevelUp;
    }

    private void LevelUp(int level)
    {
        UpdateDisplay(level,
                      GameManager.Instance.playerXP,
                      level < GameManager.Instance.xpToNextLevel.Length ? GameManager.Instance.xpToNextLevel[level] : 0);
    }

    private void UpdateDisplay(int level, int xp, int toNext)
    {
        if (displayText != null)
        {
            if (toNext <= 0) toNext = 0;
            displayText.text = $"Level {level} — XP: {xp} / {toNext}";
        }
    }
}
