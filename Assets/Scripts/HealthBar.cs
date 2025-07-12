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

    private void Awake()
    {
        myStats = GetComponentInParent<CharacterStats>();
        slider = GetComponentInChildren<Slider>();

        enemy = GetComponentInParent<Enemy>();
        player = GetComponentInParent<Player>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();

    }

    private void Start()
    {

        UpdateHealthUI();
    }


    public void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealth();
        slider.value = myStats.currentHealth;

        healthText.text = (myStats.currentHealth + "/" + myStats.GetMaxHealth());


        if (myStats.currentHealth <= 0)
        {
            //Destroy(myStats.gameObject);
            if (this.enemy != null && !isDead)
            {

                isDead = true;


                this.slider.gameObject.SetActive(false);


                StartCoroutine(DeathWithDelay());
            }

            this.gameObject.SetActive(false);

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
