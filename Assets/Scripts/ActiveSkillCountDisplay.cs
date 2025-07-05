using UnityEngine;
using TMPro;

public class ActiveSkillCountDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;

    private void Awake()
    {
        if (countText == null)
            countText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (SkillManager.Instance == null || countText == null) return;
        countText.text = $"{SkillManager.Instance.activeSkills.Count} / {SkillManager.Instance.maxActiveSlots}";
    }
}
