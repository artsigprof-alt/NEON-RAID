using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralBoostAudio : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    [Header("Boost")]
    [SerializeField] private float normalPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.65f;
    [SerializeField] private float maxVolume = 0.45f;

    [Header("Response")]
    [SerializeField] private float pitchSmooth = 8f;
    [SerializeField] private float volumeSmooth = 10f;

    private AudioSource source;
    private AudioClip boostClip;

    private const int SampleRate = 44100;

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        if (source == null)
        {
            Debug.LogError(
                "[BoostAudio] AudioSource is missing."
            );

            enabled = false;
            return;
        }

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.mute = false;
        source.volume = 0f;

        CreateBoostClip();

        if (boostClip == null)
        {
            Debug.LogError(
                "[BoostAudio] Failed to create boost clip."
            );

            enabled = false;
            return;
        }

        source.clip = boostClip;
        source.Play();
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

        bool boosting = player.IsBoosting;

        float targetVolume =
            boosting
                ? maxVolume
                : 0f;

        float targetPitch =
            boosting
                ? maxPitch
                : normalPitch;

        source.volume =
            Mathf.Lerp(
                source.volume,
                targetVolume,
                volumeSmooth *
                Time.deltaTime
            );

        source.pitch =
            Mathf.Lerp(
                source.pitch,
                targetPitch,
                pitchSmooth *
                Time.deltaTime
            );
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

    private void CreateBoostClip()
    {
        float duration = 2f;

        int sampleCount =
            Mathf.RoundToInt(
                duration * SampleRate
            );

        boostClip =
            AudioClip.Create(
                "ProceduralBoost",
                sampleCount,
                1,
                SampleRate,
                true
            );

        float[] data =
            new float[sampleCount];

        System.Random random =
            new System.Random(48121);

        float phase1 = 0f;
        float phase2 = 0f;

        float frequency1 = 130f;
        float frequency2 = 260f;

        for (int i = 0; i < sampleCount; i++)
        {
            phase1 +=
                2f *
                Mathf.PI *
                frequency1 /
                SampleRate;

            phase2 +=
                2f *
                Mathf.PI *
                frequency2 /
                SampleRate;

            if (phase1 > Mathf.PI * 2f)
                phase1 -= Mathf.PI * 2f;

            if (phase2 > Mathf.PI * 2f)
                phase2 -= Mathf.PI * 2f;

            float rumble =
                Mathf.Sin(phase1);

            float high =
                Mathf.Sin(phase2) *
                0.35f;

            float noise =
                (
                    (float)random.NextDouble() *
                    2f -
                    1f
                ) *
                0.18f;

            data[i] =
                (
                    rumble * 0.45f +
                    high +
                    noise
                ) *
                0.3f;
        }

        // ВАЖНО:
        // streamed = false, потому что используем SetData().
        boostClip =
            AudioClip.Create(
                "ProceduralBoost",
                sampleCount,
                1,
                SampleRate,
                false
            );

        boostClip.SetData(
            data,
            0
        );
    }
}