using UnityEngine;
using System.Collections.Generic;

public class Player_Skills : MonoBehaviour
{
    [SerializeField] private ActiveSkill[] skillReferences;
    private readonly Dictionary<SkillData, ActiveSkill> skillLookup = new();

    private Player player;
    private ActiveSkill pendingSkill;

    private void Awake()
    {
        player = GetComponent<Player>();
        foreach (var skill in skillReferences)
        {
            if (skill == null)
                continue;

            skill.SetOwner(player);

            if (skill.Data == null || skillLookup.ContainsKey(skill.Data))
                continue;

            skillLookup.Add(skill.Data, skill);
        }
    }

    public bool TryUseNextActiveSkill(Enemy target, float defaultRange)
    {
        if (SkillManager.Instance == null || target == null) return false;
        Debug.Log("Trying to use next active skill on target: " + target.name);
        foreach (var instance in SkillManager.Instance.activeSkills)
        {
            if (!skillLookup.TryGetValue(instance.data, out var skill))
                continue;

            if (skill.IsOnCooldown)
                continue;

            float range = skill.Range > 0 ? skill.Range : defaultRange;

            if (Vector3.Distance(transform.position, target.transform.position) > range)
                continue;

            pendingSkill = skill;

            if (!string.IsNullOrEmpty(skill.AnimationTrigger))
                player.animator.SetTrigger(skill.AnimationTrigger);
            else
                pendingSkill.TryUse();

            return true;
        }

        return false;
    }

    public void ActivatePendingSkill()
    {
        pendingSkill?.TryUse();
        pendingSkill = null;
    }

    public ActiveSkill GetActiveSkill(SkillData data)
    {
        skillLookup.TryGetValue(data, out var skill);
        return skill;
    }
}
