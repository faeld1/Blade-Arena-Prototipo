using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillHudSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject[] stars;

    private SkillInstance instance;
    private bool isActive;

    public void Setup(SkillInstance _instance, bool _isActive)
    {
        instance = _instance;
        isActive = _isActive;

        int skillValue = 0;

        if (instance.data.attackBonus != 0)
            skillValue = instance.data.attackBonus * instance.level;
        else if (instance.data.defenseBonus != 0)
            skillValue = instance.data.defenseBonus * instance.level;
        else if (instance.data.speedBonus != 0)
            skillValue = instance.data.speedBonus * instance.level;
        else if (instance.data.healthBonus != 0)
            skillValue = instance.data.healthBonus * instance.level;

            if (iconImage) iconImage.sprite = instance.data.icon;
        if (bgImage) bgImage.color = SkillUIColor.GetColor(instance.data.rarity);
        if (nameText) nameText.text = instance.data.skillName;
        if (levelText) levelText.text = "Lv. " + instance.level;
        if (descriptionText) descriptionText.text = $"Increases {instance.data.description} by {skillValue}";

        GetComponent<SkillDragHandler>().Initialize(this);
    }

    public SkillInstance GetInstance() => instance;
    public bool IsActive() => isActive;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = DraggedSkillSlot.draggedSlotUI;
        if (dragged == null || dragged == this) return;

        if (dragged.IsActive() != IsActive())
        {
            SkillManager.Instance.SwapSkills(dragged.GetInstance(), instance);
        }
    }
}
