using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool isActiveSlot;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = DraggedSkillSlot.draggedSlotUI;
        if (dragged == null) return;

        var skill = dragged.GetInstance();
        var wasActive = dragged.IsActive();

        // evita mover para mesmo grupo
        if (wasActive == isActiveSlot) return;

        SkillManager.Instance.MoveSkill(skill, isActiveSlot);
        SkillManager.Instance.skillHUDController.UpdateHUD();
        SkillManager.Instance.ReapplyBonuses();
    }
}
