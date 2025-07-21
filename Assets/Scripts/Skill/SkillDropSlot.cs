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

        // se soltar no mesmo container, move para o final da lista
        if (wasActive == isActiveSlot)
        {
            var list = wasActive ? SkillManager.Instance.activeSkills : SkillManager.Instance.reservedSkills;
            if (list.Remove(skill))
                list.Add(skill);

            SkillManager.Instance.skillHUDController.UpdateHUD();
            return;
        }

        // MoveSkill já atualiza HUD e stats
        SkillManager.Instance.MoveSkill(skill, isActiveSlot);
    }
}
