using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillHudSlotUI : MonoBehaviour, IDropHandler
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
