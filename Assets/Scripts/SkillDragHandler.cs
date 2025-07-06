using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(SkillHudSlotUI slot)
    {
        DraggedSkillSlot.draggedSlotUI = null; // sempre limpa antes
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DraggedSkillSlot.draggedSlotUI = GetComponent<SkillHudSlotUI>();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DraggedSkillSlot.draggedSlotUI = null;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = Vector2.zero;
        SkillManager.Instance.skillHUDController.UpdateHUD();
    }

}
