using UnityEngine;

public class SkillHUDController : MonoBehaviour
{
    [SerializeField] private Transform activeContainer;
    [SerializeField] private Transform reserveContainer;
    [SerializeField] private SkillHudSlotUI skillSlotPrefab;

    public void UpdateHUD()
    {
        ClearContainer(activeContainer);
        ClearContainer(reserveContainer);

        foreach (var skill in SkillManager.Instance.activeSkills)
        {
            CreateSlot(skill, activeContainer, true);
        }

        foreach (var skill in SkillManager.Instance.reservedSkills)
        {
            CreateSlot(skill, reserveContainer, false);
        }
    }

    private void CreateSlot(SkillInstance instance, Transform container, bool isActive)
    {
        var slot = Instantiate(skillSlotPrefab, container);
        slot.Setup(instance, isActive);
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
