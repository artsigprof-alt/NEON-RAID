using UnityEngine;
using TMPro;

public class HardCurrencyDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text hardCurrencyText;

    private int lastValue = -1;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        int currentValue =
            HardModeController.GetHardCurrency();

        if (currentValue != lastValue)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (hardCurrencyText == null)
            return;

        int currentValue =
            HardModeController.GetHardCurrency();

        hardCurrencyText.text =
            "HARD COINS: " + currentValue;

        lastValue = currentValue;
    }
}