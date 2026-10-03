using System.Collections;
using TMPro;
using UnityEngine;

public class BossWarningUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text text;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowWarning()
    {
        if (ProceduralAudioManager.Instance != null)
{
    ProceduralAudioManager.Instance.PlaySfx(
        AudioSfxType.BossWarning
    );
}
        StartCoroutine(
            WarningRoutine()
        );
    }

    private IEnumerator WarningRoutine()
    {
        if (panel == null)
            yield break;

        panel.SetActive(true);

        for (int i = 0; i < 6; i++)
        {
            if (text != null)
                text.enabled = !text.enabled;

            yield return
                new WaitForSeconds(
                    0.15f
                );
        }

        if (text != null)
            text.enabled = true;

        yield return
            new WaitForSeconds(
                1.2f
            );

        panel.SetActive(false);
    }
}