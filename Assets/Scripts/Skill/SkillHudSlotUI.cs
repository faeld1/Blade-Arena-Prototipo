using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillHudSlotUI : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject cooldownGameObject;
    private TextMeshProUGUI cooldownText;
    private ActiveSkill activeSkill;

    [SerializeField] private GameObject[] stars;

    private SkillInstance instance;
    private bool isActive;

    public void Setup(SkillInstance _instance, bool _isActive)
    {
        instance = _instance;
        isActive = _isActive;

        if (cooldownGameObject != null)
            cooldownText = cooldownGameObject.GetComponentInChildren<TextMeshProUGUI>();

        activeSkill = null;

        bool showCooldown = instance.data.type == SkillType.Active;
        if (cooldownGameObject != null)
            cooldownGameObject.SetActive(showCooldown);

        if (showCooldown && GameManager.Instance != null && GameManager.Instance.player != null)
        {
            var skills = GameManager.Instance.player.GetComponent<Player_Skills>();
            if (skills != null)
                activeSkill = skills.GetActiveSkill(instance.data);
        }

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

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    stars[i].SetActive(i < instance.level);
                    if (instance.level == stars.Length)
                    {
                        stars[i].GetComponent<Image>().color = new Color (1f,0.85f,0.05f); // Highlight last star if max level
                    }
                    else
                    {
                        stars[i].GetComponent<Image>().color = Color.white; // Default color for other stars
                    }
                }
            }
        }

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

    public void OnPointerDown(PointerEventData eventData)
    {
        SkillDetailUI.Instance?.Show(instance);
        Debug.Log("Clicked on skill: " + instance.data.skillName);
    }

    private void Update()
    {
        if (activeSkill == null || cooldownText == null || cooldownGameObject == null)
            return;

        float remaining = activeSkill.CooldownRemaining;
        if (remaining > 0f)
        {
            if (!cooldownGameObject.activeSelf)
                cooldownGameObject.SetActive(true);
            cooldownText.text = Mathf.CeilToInt(remaining).ToString();
            if (iconImage != null)
                iconImage.color = Color.gray;
        }
        else
        {
            if (cooldownGameObject.activeSelf)
                cooldownGameObject.SetActive(false);
            if (iconImage != null)
                iconImage.color = Color.white;
        }
    }
}
