using UnityEngine;

public class EnemyAudioEmitter : MonoBehaviour
{
    public enum EnemyType
    {
        Basic,
        Fast,
        Tank,
        Boss
    }

    [Header("Type")]
    [SerializeField] private EnemyType enemyType;

    [Header("Timing")]
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 3f;

    [Header("Volume")]
    [SerializeField] private float volume = 0.25f;

    private float timer;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        PlayEngineSound();

        ResetTimer();
    }

    private void ResetTimer()
    {
        timer =
            Random.Range(
                minInterval,
                maxInterval
            );
    }

    private void PlayEngineSound()
    {
        if (ProceduralAudioManager.Instance == null)
            return;

        AudioSfxType type;

        switch (enemyType)
        {
            case EnemyType.Fast:
                type =
                    AudioSfxType.FastEnemyEngine;
                break;

            case EnemyType.Tank:
                type =
                    AudioSfxType.TankEngine;
                break;

            case EnemyType.Boss:
                type =
                    AudioSfxType.BossEngine;
                break;

            default:
                type =
                    AudioSfxType.EnemyEngine;
                break;
        }

        ProceduralAudioManager.Instance.PlaySfx(
            type,
            volume
        );
    }
}