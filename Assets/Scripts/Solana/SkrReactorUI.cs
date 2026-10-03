
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkrReactorUI : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private GameObject panel;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text tierText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text effectsText;

    [Header("Button")]
    [SerializeField] private Button activateButton;

    [Header("Optional")]
    [SerializeField] private TMP_Text buttonText;

    private void Start()
    {
        if (activateButton != null)
        {
            activateButton.onClick.AddListener(
                OnActivateClicked
            );
        }

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (SkrReactorManager.Instance == null)
            return;

        SkrReactorManager reactor =
            SkrReactorManager.Instance;

        // =========================
        // TITLE
        // =========================

        if (titleText != null)
            titleText.text = "SKR REACTOR";

        // =========================
        // BALANCE
        // =========================

        if (balanceText != null)
        {
            balanceText.text =
                "SKR: " +
                reactor.CurrentSkrBalance
                    .ToString("0.######");
        }

        // =========================
        // TIER
        // =========================

        if (tierText != null)
        {
            tierText.text =
                reactor.GetTierName();
        }

        // =========================
        // EFFECTS
        // =========================

        if (effectsText != null)
        {
            effectsText.text =
                reactor.GetEffectDescription();
        }

        // =========================
        // STATUS
        // =========================

        if (statusText != null)
        {
            if (reactor.CurrentTier <= 0)
            {
                statusText.text =
                    "● LOCKED";
            }
            else if (reactor.IsActive)
            {
                statusText.text =
                    "● OVERDRIVE ONLINE";
            }
            else
            {
                statusText.text =
                    "● READY";
            }
        }

        // =========================
        // BUTTON
        // =========================

        if (activateButton != null)
        {
            activateButton.interactable =
                reactor.CurrentTier > 0;
        }

        if (buttonText != null)
        {
            if (reactor.IsActive)
                buttonText.text = "DEACTIVATE";
            else if (reactor.CurrentTier > 0)
                buttonText.text = "ACTIVATE";
            else
                buttonText.text = "LOCKED";
        }
    }

    private void OnActivateClicked()
    {
        if (SkrReactorManager.Instance == null)
            return;

        SkrReactorManager reactor =
            SkrReactorManager.Instance;

        if (reactor.IsActive)
        {
            reactor.Deactivate();
        }
        else
        {
            reactor.Activate();
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (activateButton != null)
        {
            activateButton.onClick.RemoveListener(
                OnActivateClicked
            );
        }
    }
}

