using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private CharacterStats myStats;
    private Slider slider;
    private Enemy enemy;
    private Player player;

    private TextMeshProUGUI healthText;

    private bool isDead = false;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
        enemy = GetComponentInParent<Enemy>();
        player = GetComponentInParent<Player>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateHealthUI();
    }


    public void UpdateHealthUI()
    {
        slider.maxValue = myStats.maxHealth;
        slider.value = myStats.currentHealth;

        healthText.text = (myStats.currentHealth + "/" + myStats.maxHealth);


        if (myStats.currentHealth <= 0)
        {
            //Destroy(myStats.gameObject);
            if (this.enemy != null && !isDead)
            {

                isDead = true;


                this.slider.gameObject.SetActive(false);


                StartCoroutine(DeathWithDelay());
            }

        }
    }
    private IEnumerator DeathWithDelay()
    {
        Destroy(myStats);
        yield return new WaitForSeconds(1.8f);
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void OnEnable()
    {
        CharacterStats.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        CharacterStats.OnHealthChanged -= UpdateHealthUI;
    }
}
