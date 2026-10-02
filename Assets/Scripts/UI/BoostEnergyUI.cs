using UnityEngine;
using UnityEngine.UI;

public class BoostEnergyUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Slider slider;

    private void Update()
    {
        if (player == null ||
            slider == null)
            return;

        slider.maxValue =
            player.MaxBoostEnergy;

        slider.value =
            player.BoostEnergy;
    }
}