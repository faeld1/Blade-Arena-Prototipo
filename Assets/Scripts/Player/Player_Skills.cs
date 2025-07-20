using UnityEngine;

public class Player_Skills : MonoBehaviour
{
    [SerializeField] private SwordSlashSkill slashSkill;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
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
        if (slashSkill == null || slashSkill.IsOnCooldown)
            return;

        if (player != null)
            player.animator.SetTrigger("SkillSlash01");
    }

    public void ActivateSlashSkill()
    {
        slashSkill?.TryUse();
    }
}
