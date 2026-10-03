using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralEngineAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;

    [Header("Engine Pitch")]
    [SerializeField] private float minPitch = 0.7f;
    [SerializeField] private float maxPitch = 1.35f;

    [Header("Engine Volume")]
    [SerializeField] private float minVolume = 0.10f;
    [SerializeField] private float maxVolume = 0.32f;

    [Header("Boost")]
    [SerializeField] private float boostPitch = 1.65f;
    [SerializeField] private float boostVolume = 0.48f;

    [Header("Response")]
    [SerializeField] private float pitchSmooth = 3.5f;
    [SerializeField] private float volumeSmooth = 4f;

    [Header("Reactor")]
    [SerializeField] private float reactorFrequency = 68f;
    [SerializeField] private float reactorDepth = 0.22f;

    [Header("Electrical Whine")]
    [SerializeField] private float whineFrequency = 420f;
    [SerializeField] private float whineAmount = 0.16f;

    [Header("Air / Turbine")]
    [SerializeField] private float airflowAmount = 0.055f;

    private AudioSource audioSource;
    private AudioClip engineClip;

    private const int SampleRate = 44100;

    private void Awake()
    {
        audioSource =
            GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError(
                "[EngineAudio] AudioSource missing."
            );

            enabled = false;
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.mute = false;

        CreateEngineClip();

        if (engineClip == null)
        {
            Debug.LogError(
                "[EngineAudio] Failed to create engine clip."
            );

            enabled = false;
            return;
        }

        audioSource.clip = engineClip;

        audioSource.pitch =
            minPitch;

        audioSource.volume =
            minVolume;

        audioSource.Play();

        Debug.Log(
            "[EngineAudio] Sci-fi engine started."
        );
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        UpdateEngine();
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (playerObject != null)
        {
            player =
                playerObject.GetComponent<PlayerController>();
        }
    }

    private void UpdateEngine()
    {
        float maxSpeed = 7f;

        if (PlayerStats.Instance != null)
        {
            maxSpeed =
                Mathf.Max(
                    0.1f,
                    PlayerStats.Instance.MoveSpeed
                );
        }

        float speed =
            player.Velocity.magnitude;

        float speed01 =
            Mathf.Clamp01(
                speed / maxSpeed
            );

        bool boosting =
            player.IsBoosting;

        float targetPitch =
            Mathf.Lerp(
                minPitch,
                maxPitch,
                speed01
            );

        float targetVolume =
            Mathf.Lerp(
                minVolume,
                maxVolume,
                speed01
            );

        if (boosting)
        {
            targetPitch =
                boostPitch;

            targetVolume =
                boostVolume;
        }

        audioSource.pitch =
            Mathf.Lerp(
                audioSource.pitch,
                targetPitch,
                pitchSmooth *
                Time.deltaTime
            );

        audioSource.volume =
            Mathf.Lerp(
                audioSource.volume,
                targetVolume,
                volumeSmooth *
                Time.deltaTime
            );
    }

    private void CreateEngineClip()
    {
        float duration = 2f;

        int sampleCount =
            Mathf.RoundToInt(
                duration *
                SampleRate
            );

        engineClip =
            AudioClip.Create(
                "NEON_RAID_SCI_FI_ENGINE",
                sampleCount,
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[sampleCount];

        float phaseReactor = 0f;
        float phaseSub = 0f;
        float phaseWhine = 0f;
        float phaseWhine2 = 0f;
        float phaseAir = 0f;

        float subFrequency =
            reactorFrequency * 0.5f;

        float whine2Frequency =
            whineFrequency * 2.37f;

        float airFrequency =
            31f;

        float reactorStep =
            reactorFrequency *
            2f *
            Mathf.PI /
            SampleRate;

        float subStep =
            subFrequency *
            2f *
            Mathf.PI /
            SampleRate;

        float whineStep =
            whineFrequency *
            2f *
            Mathf.PI /
            SampleRate;

        float whine2Step =
            whine2Frequency *
            2f *
            Mathf.PI /
            SampleRate;

        float airStep =
            airFrequency *
            2f *
            Mathf.PI /
            SampleRate;

        for (int i = 0; i < sampleCount; i++)
        {
            phaseReactor += reactorStep;
            phaseSub += subStep;
            phaseWhine += whineStep;
            phaseWhine2 += whine2Step;
            phaseAir += airStep;

            if (phaseReactor > Mathf.PI * 2f)
                phaseReactor -= Mathf.PI * 2f;

            if (phaseSub > Mathf.PI * 2f)
                phaseSub -= Mathf.PI * 2f;

            if (phaseWhine > Mathf.PI * 2f)
                phaseWhine -= Mathf.PI * 2f;

            if (phaseWhine2 > Mathf.PI * 2f)
                phaseWhine2 -= Mathf.PI * 2f;

            if (phaseAir > Mathf.PI * 2f)
                phaseAir -= Mathf.PI * 2f;

            // -----------------------------------------
            // REACTOR HUM
            // -----------------------------------------

            float reactor =
                Mathf.Sin(
                    phaseReactor
                );

            float sub =
                Mathf.Sin(
                    phaseSub
                );

            // Лёгкая FM-модуляция.
            float modulation =
                1f +
                Mathf.Sin(
                    phaseReactor * 0.35f
                ) *
                reactorDepth;

            reactor =
                Mathf.Sin(
                    phaseReactor *
                    modulation
                );

            // -----------------------------------------
            // ELECTRICAL WHINE
            // -----------------------------------------

            float whine =
                Mathf.Sin(
                    phaseWhine
                );

            float whine2 =
                Mathf.Sin(
                    phaseWhine2
                );

            // -----------------------------------------
            // AIRFLOW
            // -----------------------------------------

            float airflow =
                Mathf.Sin(
                    phaseAir *
                    1.7f
                ) *
                0.5f;

            airflow +=
                Mathf.Sin(
                    phaseAir *
                    3.13f
                ) *
                0.25f;

            // -----------------------------------------
            // MICRO NOISE
            // -----------------------------------------

            float noise =
                Mathf.Sin(
                    i *
                    0.037f
                ) *
                0.35f;

            // -----------------------------------------
            // MIX
            // -----------------------------------------

            float sample =
                sub * 0.18f +
                reactor * 0.42f +
                whine * whineAmount +
                whine2 * whineAmount * 0.35f +
                airflow * airflowAmount +
                noise * 0.012f;

            data[i] =
                sample *
                0.42f;
        }

        engineClip.SetData(
            data,
            0
        );
    }
}