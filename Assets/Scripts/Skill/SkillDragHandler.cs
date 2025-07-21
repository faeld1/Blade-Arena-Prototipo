using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private SkillHudSlotUI slot;
    private bool canDrag;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(SkillHudSlotUI slot)
    {
        this.slot = slot;
        DraggedSkillSlot.draggedSlotUI = null; // sempre limpa antes
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot != null && slot.IsOnCooldown())
        {
            canDrag = false;
            return;
        }

        canDrag = true;
        DraggedSkillSlot.draggedSlotUI = slot;
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        DraggedSkillSlot.draggedSlotUI = null;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
        SkillManager.Instance.skillHUDController.UpdateHUD();
    }

}
