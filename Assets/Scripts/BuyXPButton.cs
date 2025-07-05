using UnityEngine;
using UnityEngine.UI;

public class BuyXPButton : MonoBehaviour
{
    [SerializeField] private int xpAmount = 1;
    [SerializeField] private int goldCost = 1;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(Buy);
    }

    private void Buy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BuyXP(xpAmount, goldCost);
        }
    }
}
