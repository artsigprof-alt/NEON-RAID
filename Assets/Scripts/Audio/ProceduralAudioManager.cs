using System.Collections.Generic;
using UnityEngine;

public enum AudioSfxType
{
    UIHover,
    UIClick,
    Upgrade,

    Shot,
    Hit,
    EnemyDeath,
    PlayerDamage,

    EngineThrust,
    BoostStart,

    EnemyEngine,
    FastEnemyEngine,
    TankEngine,

    BossEngine,
    BossShot,
    BossWarning,
    BossDeath,

    Victory,
    Defeat
}

public class ProceduralAudioManager : MonoBehaviour
{
    public static ProceduralAudioManager Instance { get; private set; }

    [Header("Volumes")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float sfxVolume = 0.8f;
    [SerializeField] private float musicVolume = 0.25f;

    [Header("Sources")]
    [SerializeField] private int sourcePoolSize = 12;

    private readonly List<AudioSource> sources =
        new List<AudioSource>();

    private Dictionary<AudioSfxType, AudioClip> clips;

    private AudioSource musicSource;
    private AudioClip musicClip;

    private const int SampleRate = 44100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        CreateSources();
        GenerateAllAudio();
       
    }

    private void CreateSources()
    {
        for (int i = 0; i < sourcePoolSize; i++)
        {
            GameObject sourceObject =
                new GameObject($"SFX_Source_{i}");

            sourceObject.transform.SetParent(transform);

            AudioSource source =
                sourceObject.AddComponent<AudioSource>();

            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;

            sources.Add(source);
        }

        GameObject musicObject =
            new GameObject("MusicSource");

        musicObject.transform.SetParent(transform);

        musicSource =
            musicObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume =
            musicVolume * masterVolume;
    }

    private void GenerateAllAudio()
    {
        clips =
            new Dictionary<AudioSfxType, AudioClip>();

        clips[AudioSfxType.UIHover] =
            CreateSweep(
                "UI_Hover",
                0.06f,
                900f,
                1450f,
                0.16f,
                WaveType.Sine
            );

        clips[AudioSfxType.UIClick] =
            CreateSweep(
                "UI_Click",
                0.09f,
                700f,
                1100f,
                0.28f,
                WaveType.Sine
            );

        clips[AudioSfxType.Upgrade] =
            CreateUpgradeSound();

        clips[AudioSfxType.Shot] =
            CreateShot();

        clips[AudioSfxType.Hit] =
            CreateHit();

        clips[AudioSfxType.EnemyDeath] =
            CreateExplosion(
                "EnemyDeath",
                0.32f,
                0.45f
            );

        clips[AudioSfxType.PlayerDamage] =
            CreateSweep(
                "PlayerDamage",
                0.18f,
                180f,
                70f,
                0.35f,
                WaveType.Saw
            );

        clips[AudioSfxType.BoostStart] =
            CreateBoost();

            clips[AudioSfxType.EngineThrust] =
    CreateEngineThrust();

clips[AudioSfxType.EnemyEngine] =
    CreateEnemyEngine();

clips[AudioSfxType.FastEnemyEngine] =
    CreateFastEnemyEngine();

clips[AudioSfxType.TankEngine] =
    CreateTankEngine();

clips[AudioSfxType.BossEngine] =
    CreateBossEngine();

        clips[AudioSfxType.BossShot] =
            CreateBossShot();

        clips[AudioSfxType.BossWarning] =
            CreateBossWarning();

        clips[AudioSfxType.BossDeath] =
            CreateExplosion(
                "BossDeath",
                1.1f,
                0.9f
            );

        clips[AudioSfxType.Victory] =
            CreateVictory();

        clips[AudioSfxType.Defeat] =
            CreateDefeat();

        musicClip =
            CreateMusicLoop();
    }

    // =========================================================
    // PUBLIC
    // =========================================================

    public void PlaySfx(AudioSfxType type, float volumeMultiplier = 1f)
    {
        if (!SettingsController.IsSfxEnabled())
    return;
        if (clips == null)
            return;

        if (!clips.TryGetValue(type, out AudioClip clip))
            return;

        AudioSource source = GetFreeSource();

        if (source == null)
            return;

        source.volume =
            sfxVolume *
            masterVolume *
            volumeMultiplier;

        source.pitch = 1f;

        source.PlayOneShot(clip);
    }

    private AudioSource GetFreeSource()
    {
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
                return source;
        }

        return sources.Count > 0
            ? sources[0]
            : null;
    }

    private void PlayMusic()
    {
        if (musicSource == null ||
            musicClip == null)
            return;

        musicSource.clip =
            musicClip;

        musicSource.volume =
            musicVolume *
            masterVolume;

        musicSource.Play();
    }

    // =========================================================
    // SHOT
    // =========================================================

    private AudioClip CreateShot()
    {
        return CreateSweep(
            "Shot",
            0.11f,
            950f,
            180f,
            0.32f,
            WaveType.Saw,
            0.01f,
            0.10f,
            0.18f
        );
    }

    private AudioClip CreateHit()
    {
        return CreateSweep(
            "Hit",
            0.07f,
            1800f,
            500f,
            0.24f,
            WaveType.Square,
            0.003f,
            0.06f,
            0.08f
        );
    }

    // =========================================================
    // BOOST
    // =========================================================

    private AudioClip CreateBoost()
    {
        return CreateSweep(
            "Boost",
            0.45f,
            140f,
            1200f,
            0.3f,
            WaveType.Saw,
            0.04f,
            0.4f,
            0.1f
        );
    }

    // =========================================================
    // BOSS SHOT
    // =========================================================

    private AudioClip CreateBossShot()
    {
        return CreateSweep(
            "BossShot",
            0.3f,
            100f,
            55f,
            0.42f,
            WaveType.Square,
            0.015f,
            0.28f,
            0.2f
        );
    }

    private AudioClip CreateEngineThrust()
{
    return CreateSweep(
        "EngineThrust",
        0.16f,
        90f,
        240f,
        0.24f,
        WaveType.Saw,
        0.01f,
        0.12f,
        0.08f
    );
}

private AudioClip CreateEnemyEngine()
{
    return CreateSweep(
        "EnemyEngine",
        0.18f,
        220f,
        360f,
        0.20f,
        WaveType.Square,
        0.005f,
        0.12f,
        0.06f
    );
}

private AudioClip CreateFastEnemyEngine()
{
    return CreateSweep(
        "FastEnemyEngine",
        0.14f,
        500f,
        1100f,
        0.18f,
        WaveType.Sine,
        0.003f,
        0.09f,
        0.04f
    );
}

private AudioClip CreateTankEngine()
{
    return CreateSweep(
        "TankEngine",
        0.30f,
        55f,
        90f,
        0.35f,
        WaveType.Saw,
        0.02f,
        0.22f,
        0.12f
    );
}

private AudioClip CreateBossEngine()
{
    return CreateSweep(
        "BossEngine",
        0.50f,
        38f,
        75f,
        0.42f,
        WaveType.Saw,
        0.04f,
        0.35f,
        0.16f
    );
}

    // =========================================================
    // BOSS WARNING
    // =========================================================

    private AudioClip CreateBossWarning()
    {
        float duration = 1.5f;

        AudioClip clip =
            AudioClip.Create(
                "BossWarning",
                Mathf.RoundToInt(
                    SampleRate * duration
                ),
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[
                Mathf.RoundToInt(
                    SampleRate * duration
                )
            ];

        for (int i = 0; i < data.Length; i++)
        {
            float time =
                i / (float)SampleRate;

            float pulse =
                Mathf.Sin(
                    time *
                    Mathf.PI *
                    2f *
                    440f
                );

            float envelope =
                Mathf.Exp(
                    -8f *
                    (time %
                    0.32f)
                );

            float section =
                time < 0.75f
                    ? 1f
                    : 0.65f;

            data[i] =
                pulse *
                envelope *
                section *
                0.3f;
        }

        clip.SetData(
            data,
            0
        );

        return clip;
    }

    // =========================================================
    // EXPLOSION
    // =========================================================

    private AudioClip CreateExplosion(
        string name,
        float duration,
        float strength)
    {
        AudioClip clip =
            AudioClip.Create(
                name,
                Mathf.RoundToInt(
                    SampleRate * duration
                ),
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[
                Mathf.RoundToInt(
                    SampleRate * duration
                )
            ];

        System.Random random =
            new System.Random(
                name.GetHashCode()
            );

        for (int i = 0; i < data.Length; i++)
        {
            float time =
                i / (float)SampleRate;

            float progress =
                time / duration;

            float envelope =
                Mathf.Pow(
                    1f - progress,
                    2.2f
                );

            float low =
                Mathf.Sin(
                    time *
                    Mathf.PI *
                    2f *
                    Mathf.Lerp(
                        90f,
                        35f,
                        progress
                    )
                );

            float noise =
                ((float)random.NextDouble() *
                 2f - 1f);

            data[i] =
                (
                    low * 0.45f +
                    noise * 0.75f
                ) *
                envelope *
                strength;
        }

        clip.SetData(
            data,
            0
        );

        return clip;
    }

    // =========================================================
    // UI / SPECIAL
    // =========================================================

    private AudioClip CreateUpgradeSound()
    {
        float duration = 0.45f;

        AudioClip clip =
            AudioClip.Create(
                "Upgrade",
                Mathf.RoundToInt(
                    SampleRate * duration
                ),
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[
                Mathf.RoundToInt(
                    SampleRate * duration
                )
            ];

        float[] notes =
        {
            523.25f,
            659.25f,
            783.99f
        };

        for (int i = 0; i < data.Length; i++)
        {
            float time =
                i / (float)SampleRate;

            float noteLength =
                0.15f;

            int noteIndex =
                Mathf.Min(
                    2,
                    Mathf.FloorToInt(
                        time / noteLength
                    )
                );

            float localTime =
                time -
                noteIndex *
                noteLength;

            float envelope =
                Mathf.Exp(
                    -10f *
                    localTime
                );

            data[i] =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    notes[noteIndex] *
                    time
                ) *
                envelope *
                0.3f;
        }

        clip.SetData(
            data,
            0
        );

        return clip;
    }

    private AudioClip CreateVictory()
    {
        return CreateSequence(
            "Victory",
            new[]
            {
                523.25f,
                659.25f,
                783.99f,
                1046.5f
            },
            0.16f
        );
    }

    private AudioClip CreateDefeat()
    {
        return CreateSequence(
            "Defeat",
            new[]
            {
                440f,
                330f,
                220f
            },
            0.24f
        );
    }

    // =========================================================
    // GENERATORS
    // =========================================================

    private enum WaveType
    {
        Sine,
        Square,
        Saw,
        Triangle
    }

    private AudioClip CreateSweep(
        string name,
        float duration,
        float startFrequency,
        float endFrequency,
        float volume,
        WaveType wave,
        float attack = 0.005f,
        float decay = -1f,
        float noiseMix = 0f)
    {
        if (decay <= 0f)
            decay = duration;

        int sampleCount =
            Mathf.RoundToInt(
                duration *
                SampleRate
            );

        AudioClip clip =
            AudioClip.Create(
                name,
                sampleCount,
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[sampleCount];

        System.Random random =
            new System.Random(
                name.GetHashCode()
            );

        for (int i = 0; i < sampleCount; i++)
        {
            float t =
                i / (float)SampleRate;

            float normalized =
                i / (float)sampleCount;

            float frequency =
                Mathf.Lerp(
                    startFrequency,
                    endFrequency,
                    normalized
                );

            float phase =
                t *
                frequency;

            float waveValue =
                GenerateWave(
                    phase,
                    wave
                );

            float noise =
                ((float)random.NextDouble() *
                 2f - 1f);

            float envelope;

            if (t < attack)
            {
                envelope =
                    Mathf.Clamp01(
                        t / attack
                    );
            }
            else
            {
                envelope =
                    Mathf.Exp(
                        -8f *
                        (t / decay)
                    );
            }

            data[i] =
                (
                    waveValue *
                    (1f - noiseMix) +
                    noise *
                    noiseMix
                ) *
                envelope *
                volume;
        }

        clip.SetData(
            data,
            0
        );

        return clip;
    }

    private float GenerateWave(
        float phase,
        WaveType type)
    {
        phase =
            phase -
            Mathf.Floor(
                phase
            );

        switch (type)
        {
            case WaveType.Sine:
                return Mathf.Sin(
                    phase *
                    Mathf.PI *
                    2f
                );

            case WaveType.Square:
                return phase < 0.5f
                    ? 1f
                    : -1f;

            case WaveType.Saw:
                return phase *
                    2f -
                    1f;

            case WaveType.Triangle:
                return 1f -
                    4f *
                    Mathf.Abs(
                        phase -
                        0.5f
                    );

            default:
                return 0f;
        }
    }

    private AudioClip CreateSequence(
        string name,
        float[] frequencies,
        float noteLength)
    {
        float duration =
            frequencies.Length *
            noteLength;

        int sampleCount =
            Mathf.RoundToInt(
                duration *
                SampleRate
            );

        AudioClip clip =
            AudioClip.Create(
                name,
                sampleCount,
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[sampleCount];

        for (int i = 0; i < data.Length; i++)
        {
            float time =
                i / (float)SampleRate;

            int note =
                Mathf.Min(
                    frequencies.Length - 1,
                    Mathf.FloorToInt(
                        time / noteLength
                    )
                );

            float localTime =
                time -
                note * noteLength;

            float envelope =
                Mathf.Exp(
                    -8f *
                    localTime
                );

            float fundamental =
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequencies[note] *
                    time
                );

            float harmonic =
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequencies[note] *
                    2f *
                    time
                ) *
                0.25f;

            data[i] =
                (
                    fundamental +
                    harmonic
                ) *
                envelope *
                0.25f;
        }

        clip.SetData(
            data,
            0
        );

        return clip;
    }

    private AudioClip CreateMusicLoop()
    {
        float duration = 8f;

        int sampleCount =
            Mathf.RoundToInt(
                duration *
                SampleRate
            );

        AudioClip clip =
            AudioClip.Create(
                "NeonRaid_Music",
                sampleCount,
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[sampleCount];

        float[] notes =
        {
            55f,
            65.41f,
            73.42f,
            82.41f
        };

        for (int i = 0; i < data.Length; i++)
        {
            float time =
                i / (float)SampleRate;

            int note =
                Mathf.FloorToInt(
                    time * 0.75f
                ) %
                notes.Length;

            float beat =
                Mathf.Sin(
                    time *
                    Mathf.PI *
                    2f *
                    1.5f
                );

            float bass =
                Mathf.Sin(
                    time *
                    Mathf.PI *
                    2f *
                    notes[note]
                );

            float pad =
                Mathf.Sin(
                    time *
                    Mathf.PI *
                    2f *
                    notes[note] *
                    2f
                ) *
                0.35f;

            float modulation =
                0.65f +
                0.35f *
                beat;

            data[i] =
                (
                    bass * 0.11f +
                    pad * 0.05f
                ) *
                modulation;
        }

        clip.SetData(
            data,
            0
        );

        return clip;
    }
}