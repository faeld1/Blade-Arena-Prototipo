using UnityEngine;
using System.Collections.Generic;

public class Player_Skills : MonoBehaviour
{
    [SerializeField] private SkillData slashSkillData;
    [SerializeField] private ActiveSkill[] skillReferences;
    private readonly Dictionary<SkillData, ActiveSkill> skillLookup = new();

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        foreach (var skill in skillReferences)
        {
            if (skill != null && skill.Data != null && !skillLookup.ContainsKey(skill.Data))
                skillLookup.Add(skill.Data, skill);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TryUseSlashSkill();
        }
    }

    private void TryUseSlashSkill()
    {
        if (!skillLookup.TryGetValue(slashSkillData, out var skill) || skill.IsOnCooldown)
            return;

        if (player != null)
            player.animator.SetTrigger("SkillSlash01");
    }

    public void ActivateSlashSkill()
    {
        if (skillLookup.TryGetValue(slashSkillData, out var skill))
            skill.TryUse();
    }
}
