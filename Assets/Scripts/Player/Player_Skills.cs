using System.Collections;
using UnityEngine;

public class Player_Skills : MonoBehaviour
{
    [SerializeField] private GameObject slashSkill;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            ActivateSlashSkill();
        }
    }

    private void ActiveOrDesactiveSkill(GameObject skill, bool activeOrDesactive)
    {
        if(skill == null)
        {
            Debug.LogWarning("Skill is null, cannot activate.");
            return;
        }

        skill.SetActive(activeOrDesactive);
    }

    public void ActivateSlashSkill()
    {
        StartCoroutine(ActiveSlashSkillCo());
    }

    private IEnumerator ActiveSlashSkillCo()
    {
        ActiveOrDesactiveSkill(slashSkill, true);
        yield return new WaitForSeconds(1f); // Adjust the duration as needed
        ActiveOrDesactiveSkill(slashSkill, false);
    }

}
