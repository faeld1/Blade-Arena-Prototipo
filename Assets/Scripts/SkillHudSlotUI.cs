using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillHudSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;

    private SkillInstance instance;
    private bool isActive;

    public void Setup(SkillInstance _instance, bool _isActive)
    {
        instance = _instance;
        isActive = _isActive;

        if (iconImage) iconImage.sprite = instance.data.icon;
        if (bgImage) bgImage.color = SkillUIColor.GetColor(instance.data.rarity);
        if (nameText) nameText.text = instance.data.skillName;
        if (levelText) levelText.text = "Lv. " + instance.level;

        GetComponent<SkillDragHandler>().Initialize(this);
    }

    public SkillInstance GetInstance() => instance;
    public bool IsActive() => isActive;
}
