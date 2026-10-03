using System.Collections;
using UnityEngine;

public class DynamicMusicController : MonoBehaviour
{
    public static DynamicMusicController Instance { get; private set; }

   public enum MusicState
{
    Menu,
    Normal,
    Combat,
    Wave3,
    Wave4,
    Boss,
    Victory,
    Defeat
}

    [Header("Volume")]
    [SerializeField] private float musicVolume = 0.22f;

    [Header("Crossfade")]
    [SerializeField] private float crossfadeDuration = 1.2f;

    [Header("Music")]
    [SerializeField] private int musicLengthSeconds = 12;

    private AudioSource sourceA;
    private AudioSource sourceB;

   private AudioClip menuClip;
private AudioClip normalClip;
private AudioClip combatClip;
private AudioClip wave3Clip;
private AudioClip wave4Clip;
private AudioClip bossClip;
private AudioClip victoryClip;
private AudioClip defeatClip;

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    private MusicState currentState;

    private Coroutine transitionCoroutine;

    private const int SampleRate = 44100;
public void SetMusicEnabled(bool enabled)
{
    if (!enabled)
    {
        StopMusic();
        return;
    }

    PlayState(
        currentState,
        true
    );
}
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        CreateAudioSources();
        GenerateMusic();

        PlayState(
    MusicState.Menu,
    true
);
    }

    private void CreateAudioSources()
    {
        sourceA =
            CreateSource(
                "MusicSource_A"
            );

        sourceB =
            CreateSource(
                "MusicSource_B"
            );

        activeSource = sourceA;
        inactiveSource = sourceB;
    }

    private AudioSource CreateSource(
        string objectName)
    {
        GameObject obj =
            new GameObject(
                objectName
            );

        obj.transform.SetParent(
            transform
        );

        AudioSource source =
            obj.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = 0f;

        return source;
    }

    private void GenerateMusic()
{
    menuClip = CreateMenuMusic();

    normalClip = CreateNormalMusic();
    combatClip = CreateCombatMusic();
    wave3Clip = CreateWave3Music();
    wave4Clip = CreateWave4Music();
    bossClip = CreateBossMusic();

    victoryClip = CreateVictoryMusic();
    defeatClip = CreateDefeatMusic();
}

    // =========================================================
    // PUBLIC
    // =========================================================
public void SetMenu()
{
    PlayState(
        MusicState.Menu
    );
}
    public void SetWave(int wave)
    {
        if (wave >= 5)
        {
            PlayState(
                MusicState.Boss
            );
        }
        else if (wave == 4)
        {
            PlayState(
                MusicState.Wave4
            );
        }
        else if (wave == 3)
        {
            PlayState(
                MusicState.Wave3
            );
        }
        else
        {
            PlayState(
                MusicState.Normal
            );
        }
    }

    public void PlayVictory()
    {
        PlayState(
            MusicState.Victory
        );
    }

    public void PlayDefeat()
    {
        PlayState(
            MusicState.Defeat
        );
    }

    public void StopMusic()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );
        }

        sourceA.Stop();
        sourceB.Stop();

        sourceA.volume = 0f;
        sourceB.volume = 0f;
    }

    // =========================================================
    // STATE
    // =========================================================

    private void PlayState(
        MusicState state,
        bool instant = false)
    {
        if (currentState == state &&
            activeSource.isPlaying)
            return;

        currentState =
            state;

        AudioClip clip =
            GetClip(state);

        if (clip == null)
            return;

        if (state == MusicState.Victory ||
            state == MusicState.Defeat)
        {
            PlayOneShotMusic(
                clip
            );

            return;
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );
        }

        transitionCoroutine =
            StartCoroutine(
                Crossfade(
                    clip,
                    instant
                )
            );
    }

    private AudioClip GetClip(
    MusicState state)
{
    switch (state)
    {
        case MusicState.Menu:
            return menuClip;

        case MusicState.Combat:
            return combatClip;

        case MusicState.Wave3:
            return wave3Clip;

        case MusicState.Wave4:
            return wave4Clip;

        case MusicState.Boss:
            return bossClip;

        case MusicState.Victory:
            return victoryClip;

        case MusicState.Defeat:
            return defeatClip;

        default:
            return normalClip;
    }
}

    private IEnumerator Crossfade(
        AudioClip newClip,
        bool instant)
    {
        inactiveSource.clip =
            newClip;

        inactiveSource.loop =
            true;

        inactiveSource.volume =
            0f;

        inactiveSource.Play();

        if (instant)
        {
            activeSource.Stop();

            inactiveSource.volume =
                musicVolume;

            SwapSources();

            yield break;
        }

        float timer = 0f;

        float oldVolume =
            activeSource.isPlaying
                ? activeSource.volume
                : 0f;

        while (timer <
               crossfadeDuration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    crossfadeDuration
                );

            float smooth =
                t * t *
                (3f - 2f * t);

            if (activeSource.isPlaying)
            {
                activeSource.volume =
                    Mathf.Lerp(
                        oldVolume,
                        0f,
                        smooth
                    );
            }

            inactiveSource.volume =
                Mathf.Lerp(
                    0f,
                    musicVolume,
                    smooth
                );

            yield return null;
        }

        activeSource.Stop();

        inactiveSource.volume =
            musicVolume;

        SwapSources();

        transitionCoroutine =
            null;
    }

    private void SwapSources()
    {
        AudioSource temp =
            activeSource;

        activeSource =
            inactiveSource;

        inactiveSource =
            temp;
    }

    private void PlayOneShotMusic(
        AudioClip clip)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );
        }

        StartCoroutine(
            SpecialMusicRoutine(
                clip
            )
        );
    }

    private IEnumerator SpecialMusicRoutine(
        AudioClip clip)
    {
        float fadeDuration = 0.8f;

        float startVolume =
            activeSource.volume;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    fadeDuration
                );

            activeSource.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    t
                );

            yield return null;
        }

        activeSource.Stop();

        inactiveSource.clip =
            clip;

        inactiveSource.loop =
            false;

        inactiveSource.volume =
            musicVolume;

        inactiveSource.Play();

        yield return new WaitForSecondsRealtime(
            clip.length
        );

        inactiveSource.Stop();

        inactiveSource.volume =
            0f;

        activeSource =
            inactiveSource;
    }

    // =========================================================
    // NORMAL MUSIC
    // =========================================================
private AudioClip CreateMenuMusic()
{
    const int lengthSeconds = 24;
    const float bpm = 78f;

    int sampleCount =
        lengthSeconds * SampleRate;

    AudioClip clip =
        AudioClip.Create(
            "NEON_RAID_MENU_MELODY",
            sampleCount,
            1,
            SampleRate,
            false
        );

    float[] data =
        new float[sampleCount];

    float secondsPerBeat =
        60f / bpm;

    float secondsPerBar =
        secondsPerBeat * 4f;

    // ---------------------------------------------------------
    // Chord progression:
    //
    // Am → F → C → G
    //
    // Спокойная sci-fi гармония.
    // ---------------------------------------------------------

    float[] chordRoots =
    {
        220f,       // A
        174.61f,    // F
        130.81f,    // C
        196f        // G
    };

    // ---------------------------------------------------------
    // Основная мелодия.
    //
    // 16 нот на 4 такта.
    // 0 = пауза.
    // ---------------------------------------------------------

    float[] melody =
    {
        440f, 523.25f, 659.25f, 523.25f,
        392f, 523.25f, 659.25f, 783.99f,

        523.25f, 659.25f, 783.99f, 659.25f,
        493.88f, 587.33f, 659.25f, 783.99f,

        659.25f, 783.99f, 880f, 783.99f,
        659.25f, 587.33f, 523.25f, 440f,

        493.88f, 523.25f, 587.33f, 659.25f,
        783.99f, 659.25f, 523.25f, 440f
    };

    int melodySteps =
        melody.Length;

    float melodyStep =
        secondsPerBeat * 0.5f;

    for (int i = 0; i < sampleCount; i++)
    {
        float time =
            i /
            (float)SampleRate;

        // =====================================================
        // BAR
        // =====================================================

        int barIndex =
            Mathf.FloorToInt(
                time / secondsPerBar
            );

        int chordIndex =
            barIndex %
            chordRoots.Length;

        float root =
            chordRoots[chordIndex];

        float fifth =
            root * 1.5f;

        float octave =
            root * 2f;

        // =====================================================
        // SOFT BASS
        // =====================================================

        float bass =
            Mathf.Sin(
                2f *
                Mathf.PI *
                root *
                0.5f *
                time
            ) *
            0.16f;

        // =====================================================
        // PAD / CHORD
        // =====================================================

        float chord =
            Mathf.Sin(
                2f *
                Mathf.PI *
                root *
                time
            ) *
            0.045f;

        chord +=
            Mathf.Sin(
                2f *
                Mathf.PI *
                fifth *
                time
            ) *
            0.035f;

        chord +=
            Mathf.Sin(
                2f *
                Mathf.PI *
                octave *
                time
            ) *
            0.02f;

        // Медленное дыхание pad.
        float padMovement =
            0.75f +
            0.25f *
            Mathf.Sin(
                time * 0.5f
            );

        chord *=
            padMovement;

        // =====================================================
        // ARPEGGIO
        // =====================================================

        float beat =
            time /
            secondsPerBeat;

        int arpStep =
            Mathf.FloorToInt(
                beat * 2f
            ) % 8;

        float[] arpRatios =
        {
            2f,
            2.5f,
            3f,
            2.5f,
            4f,
            3f,
            2.5f,
            3f
        };

        float arpFrequency =
            root *
            arpRatios[arpStep];

        float localArp =
            (
                beat * 2f
            ) -
            Mathf.Floor(
                beat * 2f
            );

        float arpEnvelope =
            Mathf.Exp(
                -12f *
                localArp
            );

        float arpeggio =
            Mathf.Sin(
                2f *
                Mathf.PI *
                arpFrequency *
                time
            ) *
            arpEnvelope *
            0.035f;

        // =====================================================
        // MAIN MELODY
        // =====================================================

        float melodyPosition =
            time /
            melodyStep;

        int melodyIndex =
            Mathf.FloorToInt(
                melodyPosition
            ) %
            melodySteps;

        float melodyFrequency =
            melody[melodyIndex];

        float localMelodyTime =
            melodyPosition -
            Mathf.Floor(
                melodyPosition
            );

        float melodyEnvelope =
            Mathf.Exp(
                -7f *
                localMelodyTime
            );

        float melodyTone =
            Mathf.Sin(
                2f *
                Mathf.PI *
                melodyFrequency *
                time
            );

        // Вторая гармоника даёт synth-характер.
        melodyTone +=
            Mathf.Sin(
                2f *
                Mathf.PI *
                melodyFrequency *
                2f *
                time
            ) *
            0.22f;

        // Ещё одна очень тихая гармоника.
        melodyTone +=
            Mathf.Sin(
                2f *
                Mathf.PI *
                melodyFrequency *
                3f *
                time
            ) *
            0.07f;

        float melodyLayer =
            melodyTone *
            melodyEnvelope *
            0.15f;

        // =====================================================
        // SOFT ELECTRONIC PULSE
        // =====================================================

        float beatPosition =
            time /
            secondsPerBeat;

        float localBeat =
            beatPosition -
            Mathf.Floor(
                beatPosition
            );

        float pulseEnvelope =
            Mathf.Exp(
                -20f *
                localBeat
            );

        float pulse =
            Mathf.Sin(
                2f *
                Mathf.PI *
                82f *
                time
            ) *
            pulseEnvelope *
            0.035f;

        // =====================================================
        // SMALL HIGH ATMOSPHERE
        // =====================================================

        float atmosphere =
            Mathf.Sin(
                2f *
                Mathf.PI *
                987.77f *
                time
            ) *
            0.006f;

        atmosphere *=
            0.5f +
            0.5f *
            Mathf.Sin(
                time *
                0.32f
            );

        // =====================================================
        // FINAL MIX
        // =====================================================

        float sample =
            bass +
            chord +
            arpeggio +
            melodyLayer +
            pulse +
            atmosphere;

        data[i] =
            Mathf.Clamp(
                sample,
                -0.95f,
                0.95f
            );
    }

    clip.SetData(
        data,
        0
    );

    return clip;
}
private AudioClip CreateNormalMusic()
{
    const int lengthSeconds = 24;
    const float bpm = 104f;

    int sampleCount =
        lengthSeconds * SampleRate;

    AudioClip clip =
        AudioClip.Create(
            "NEON_RAID_MAIN_THEME",
            sampleCount,
            1,
            SampleRate,
            false
        );

    float[] data =
        new float[sampleCount];

    float beatLength =
        60f / bpm;

    float barLength =
        beatLength * 4f;

    // Главный мотив NEON RAID.
    float[] melody =
    {
        261.63f, 329.63f, 392f, 329.63f,
        293.66f, 349.23f, 440f, 349.23f,

        329.63f, 392f, 493.88f, 392f,
        293.66f, 349.23f, 392f, 523.25f,

        392f, 493.88f, 587.33f, 493.88f,
        440f, 392f, 349.23f, 293.66f,

        329.63f, 392f, 493.88f, 392f,
        349.23f, 329.63f, 293.66f, 261.63f
    };

    float[] roots =
    {
        130.81f, // C
        174.61f, // F
        196f,    // G
        146.83f  // D
    };

    for (int i = 0; i < sampleCount; i++)
    {
        float time =
            i / (float)SampleRate;

        float beat =
            time / beatLength;

        int barIndex =
            Mathf.FloorToInt(
                time / barLength
            );

        int chordIndex =
            barIndex % roots.Length;

        float root =
            roots[chordIndex];

        float fifth =
            root * 1.5f;

        float octave =
            root * 2f;

        // =================================================
        // BASS
        // =================================================

        float bass =
            Mathf.Sin(
                Mathf.PI *
                2f *
                root *
                0.5f *
                time
            ) *
            0.16f;

        float bass2 =
            Mathf.Sin(
                Mathf.PI *
                2f *
                root *
                time
            ) *
            0.045f;

        // =================================================
        // SYNTH PAD
        // =================================================

        float pad =
            Mathf.Sin(
                Mathf.PI *
                2f *
                fifth *
                time
            ) *
            0.035f;

        pad +=
            Mathf.Sin(
                Mathf.PI *
                2f *
                octave *
                time
            ) *
            0.025f;

        float padMod =
            0.7f +
            0.3f *
            Mathf.Sin(
                time * 0.55f
            );

        pad *= padMod;

        // =================================================
        // ARPEGGIO
        // =================================================

        int arpStep =
            Mathf.FloorToInt(
                beat * 2f
            ) % 8;

        float[] arpRatios =
        {
            2f,
            2.5f,
            3f,
            4f,
            3f,
            2.5f,
            2f,
            3f
        };

        float arpFrequency =
            root *
            arpRatios[arpStep];

        float localArp =
            beat * 2f -
            Mathf.Floor(
                beat * 2f
            );

        float arpEnvelope =
            Mathf.Exp(
                -11f *
                localArp
            );

        float arp =
            Mathf.Sin(
                Mathf.PI *
                2f *
                arpFrequency *
                time
            ) *
            arpEnvelope *
            0.028f;

        // =================================================
        // MAIN MELODY
        // =================================================

        float melodyPosition =
            beat * 2f;

        int melodyIndex =
            Mathf.FloorToInt(
                melodyPosition
            ) % melody.Length;

        float melodyFrequency =
            melody[melodyIndex];

        float localMelody =
            melodyPosition -
            Mathf.Floor(
                melodyPosition
            );

        float melodyEnvelope =
            Mathf.Exp(
                -5.5f *
                localMelody
            );

        float melodyTone =
            Mathf.Sin(
                Mathf.PI *
                2f *
                melodyFrequency *
                time
            );

        // Более яркий digital-synth тембр.
        melodyTone +=
            Mathf.Sin(
                Mathf.PI *
                2f *
                melodyFrequency *
                2f *
                time
            ) *
            0.18f;

        melodyTone +=
            Mathf.Sin(
                Mathf.PI *
                2f *
                melodyFrequency *
                3f *
                time
            ) *
            0.05f;

        float melodyLayer =
            melodyTone *
            melodyEnvelope *
            0.14f;

        // =================================================
        // RHYTHM
        // =================================================

        float localBeat =
            beat -
            Mathf.Floor(beat);

        float kick =
            0f;

        if (localBeat < 0.07f)
        {
            float envelope =
                Mathf.Exp(
                    -35f *
                    localBeat
                );

            kick =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    65f *
                    time
                ) *
                envelope *
                0.08f;
        }

        // =================================================
        // ATMOSPHERE
        // =================================================

        float atmosphere =
            Mathf.Sin(
                time * 0.31f
            ) *
            0.009f;

        float sample =
            bass +
            bass2 +
            pad +
            arp +
            melodyLayer +
            kick +
            atmosphere;

        data[i] =
            Mathf.Clamp(
                sample * 1.05f,
                -0.95f,
                0.95f
            );
    }

    clip.SetData(
        data,
        0
    );

    return clip;
}


    // =========================================================
    // WAVE 3 MUSIC
    // =========================================================

    private AudioClip CreateWave3Music()
    {
        const int lengthSeconds = 24;
        const float bpm = 94f;

        int sampleCount =
            lengthSeconds * SampleRate;

        AudioClip clip =
            AudioClip.Create(
                "NEON_RAID_WAVE3_THEME",
                sampleCount,
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[sampleCount];

        float beatLength =
            60f / bpm;

        float barLength =
            beatLength * 4f;

        // D minor / F / C / G
        float[] roots =
        {
            73.42f, // D2
            87.31f, // F2
            65.41f, // C2
            98.00f  // G2
        };

        // 32-step melody, 1/8 notes.
        float[] melody =
        {
            293.66f, 349.23f, 392.00f, 349.23f,
            293.66f, 261.63f, 293.66f, 349.23f,

            349.23f, 392.00f, 440.00f, 392.00f,
            349.23f, 293.66f, 261.63f, 293.66f,

            261.63f, 293.66f, 349.23f, 392.00f,
            440.00f, 392.00f, 349.23f, 293.66f,

            246.94f, 261.63f, 293.66f, 349.23f,
            392.00f, 349.23f, 293.66f, 246.94f
        };

        int melodyLength = melody.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            float time =
                i / (float)SampleRate;

            float beat =
                time / beatLength;

            int barIndex =
                Mathf.FloorToInt(
                    time / barLength
                );

            int chordIndex =
                barIndex % roots.Length;

            float root =
                roots[chordIndex];

            // -------------------------------------------------
            // Deep bass
            // -------------------------------------------------

            float bass =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    root *
                    0.5f *
                    time
                ) *
                0.18f;

            bass +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    root *
                    time
                ) *
                0.055f;

            // -------------------------------------------------
            // Chord pad
            // -------------------------------------------------

            float fifth =
                root * 1.5f;

            float octave =
                root * 2f;

            float pad =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    root *
                    time
                ) *
                0.025f;

            pad +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    fifth *
                    time
                ) *
                0.025f;

            pad +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    octave *
                    time
                ) *
                0.015f;

            pad *=
                0.75f +
                0.25f *
                Mathf.Sin(time * 0.6f);

            // -------------------------------------------------
            // Main melody
            // -------------------------------------------------

            float melodyPosition =
                beat * 2f;

            int melodyIndex =
                Mathf.FloorToInt(
                    melodyPosition
                ) % melodyLength;

            float frequency =
                melody[melodyIndex];

            float localMelody =
                melodyPosition -
                Mathf.Floor(melodyPosition);

            float melodyEnvelope =
                Mathf.Exp(
                    -6.5f *
                    localMelody
                );

            float melodyTone =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    time
                );

            melodyTone +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    2f *
                    time
                ) *
                0.12f;

            melodyTone +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    3f *
                    time
                ) *
                0.025f;

            float melodyLayer =
                melodyTone *
                melodyEnvelope *
                0.15f;

            // -------------------------------------------------
            // Arpeggio
            // -------------------------------------------------

            int arpStep =
                Mathf.FloorToInt(
                    beat * 4f
                ) % 8;

            float[] arpNotes =
            {
                root * 2f,
                root * 2.5f,
                root * 3f,
                root * 4f,
                root * 3f,
                root * 2.5f,
                root * 2f,
                root * 3f
            };

            float arpFrequency =
                arpNotes[arpStep];

            float localArp =
                beat * 4f -
                Mathf.Floor(
                    beat * 4f
                );

            float arpEnvelope =
                Mathf.Exp(
                    -15f *
                    localArp
                );

            float arpeggio =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    arpFrequency *
                    time
                ) *
                arpEnvelope *
                0.035f;

            // -------------------------------------------------
            // Pulse
            // -------------------------------------------------

            float localBeat =
                beat -
                Mathf.Floor(beat);

            float pulseEnvelope =
                Mathf.Exp(
                    -20f *
                    localBeat
                );

            float pulse =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    62f *
                    time
                ) *
                pulseEnvelope *
                0.055f;

            // -------------------------------------------------
            // Final
            // -------------------------------------------------

            float sample =
                bass +
                pad +
                melodyLayer +
                arpeggio +
                pulse;

            data[i] =
                Mathf.Clamp(
                    sample,
                    -0.8f,
                    0.8f
                );
        }

        clip.SetData(data, 0);

        return clip;
    }

    // =========================================================
    // WAVE 4 MUSIC
    // =========================================================

    private AudioClip CreateWave4Music()
    {
        const int lengthSeconds = 24;
        const float bpm = 112f;

        int sampleCount =
            lengthSeconds * SampleRate;

        AudioClip clip =
            AudioClip.Create(
                "NEON_RAID_WAVE4_THEME",
                sampleCount,
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[sampleCount];

        float beatLength =
            60f / bpm;

        float barLength =
            beatLength * 4f;

        // E minor / G / D / A
        float[] roots =
        {
            82.41f,  // E2
            98.00f,  // G2
            73.42f,  // D2
            110.00f  // A2
        };

        // Faster, more aggressive 1/8-note melody.
        float[] melody =
        {
            329.63f, 392.00f, 440.00f, 392.00f,
            329.63f, 293.66f, 329.63f, 392.00f,

            392.00f, 440.00f, 493.88f, 440.00f,
            392.00f, 329.63f, 293.66f, 329.63f,

            293.66f, 329.63f, 392.00f, 440.00f,
            493.88f, 440.00f, 392.00f, 329.63f,

            349.23f, 392.00f, 440.00f, 493.88f,
            523.25f, 493.88f, 440.00f, 392.00f
        };

        int melodyLength = melody.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            float time =
                i / (float)SampleRate;

            float beat =
                time / beatLength;

            int barIndex =
                Mathf.FloorToInt(
                    time / barLength
                );

            int chordIndex =
                barIndex % roots.Length;

            float root =
                roots[chordIndex];

            // -------------------------------------------------
            // Strong bass
            // -------------------------------------------------

            float bass =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    root *
                    0.5f *
                    time
                ) *
                0.20f;

            bass +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    root *
                    time
                ) *
                0.07f;

            // -------------------------------------------------
            // Chord layer
            // -------------------------------------------------

            float fifth =
                root * 1.5f;

            float octave =
                root * 2f;

            float chord =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    root *
                    time
                ) *
                0.028f;

            chord +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    fifth *
                    time
                ) *
                0.024f;

            chord +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    octave *
                    time
                ) *
                0.015f;

            // -------------------------------------------------
            // Main lead
            // -------------------------------------------------

            float melodyPosition =
                beat * 2f;

            int melodyIndex =
                Mathf.FloorToInt(
                    melodyPosition
                ) % melodyLength;

            float frequency =
                melody[melodyIndex];

            float localMelody =
                melodyPosition -
                Mathf.Floor(melodyPosition);

            float melodyEnvelope =
                Mathf.Exp(
                    -9f *
                    localMelody
                );

            float lead =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    time
                );

            lead +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    2f *
                    time
                ) *
                0.16f;

            lead +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    3f *
                    time
                ) *
                0.035f;

            float melodyLayer =
                lead *
                melodyEnvelope *
                0.17f;

            // -------------------------------------------------
            // Fast arpeggio
            // -------------------------------------------------

            int arpStep =
                Mathf.FloorToInt(
                    beat * 4f
                ) % 8;

            float[] arpNotes =
            {
                root * 2f,
                root * 3f,
                root * 4f,
                root * 3f,
                root * 5f,
                root * 4f,
                root * 3f,
                root * 2f
            };

            float arpFrequency =
                arpNotes[arpStep];

            float localArp =
                beat * 4f -
                Mathf.Floor(
                    beat * 4f
                );

            float arpEnvelope =
                Mathf.Exp(
                    -18f *
                    localArp
                );

            float arpeggio =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    arpFrequency *
                    time
                ) *
                arpEnvelope *
                0.045f;

            // -------------------------------------------------
            // Kick / pulse
            // -------------------------------------------------

            float localBeat =
                beat -
                Mathf.Floor(beat);

            float kickEnvelope =
                Mathf.Exp(
                    -35f *
                    localBeat
                );

            float kick =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    65f *
                    time
                ) *
                kickEnvelope *
                0.10f;

            // Off-beat pulse makes wave 4 feel faster.
            float halfBeat =
                beat * 2f;

            float localHalfBeat =
                halfBeat -
                Mathf.Floor(halfBeat);

            float secondPulseEnvelope =
                Mathf.Exp(
                    -22f *
                    localHalfBeat
                );

            float secondPulse =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    120f *
                    time
                ) *
                secondPulseEnvelope *
                0.025f;

            // -------------------------------------------------
            // Small digital texture
            // -------------------------------------------------

            float texture =
                Mathf.Sin(
                    time * 1.7f
                ) *
                0.006f;

            // -------------------------------------------------
            // Final
            // -------------------------------------------------

            float sample =
                bass +
                chord +
                melodyLayer +
                arpeggio +
                kick +
                secondPulse +
                texture;

            data[i] =
                Mathf.Clamp(
                    sample * 0.95f,
                    -0.85f,
                    0.85f
                );
        }

        clip.SetData(data, 0);

        return clip;
    }

    // =========================================================
    // COMBAT MUSIC
    // =========================================================

    private AudioClip CreateCombatMusic()
    {
        float bpm = 108f;

        return CreateMusicClip(
            "NEON_RAID_COMBAT",
            musicLengthSeconds,
            bpm,
            MusicStyle.Combat
        );
    }

    // =========================================================
    // BOSS MUSIC
    // =========================================================
private AudioClip CreateBossMusic()
{
    const int lengthSeconds = 24;
    const float bpm = 78f;

    int sampleCount =
        lengthSeconds * SampleRate;

    AudioClip clip =
        AudioClip.Create(
            "NEON_RAID_BOSS_THEME",
            sampleCount,
            1,
            SampleRate,
            false
        );

    float[] data =
        new float[sampleCount];

    float beatLength =
        60f / bpm;

    float barLength =
        beatLength * 4f;

    // Тот же музыкальный DNA,
    // но темнее и ниже.
    float[] melody =
    {
        196f, 246.94f, 293.66f, 246.94f,
        174.61f, 233.08f, 261.63f, 233.08f,

        196f, 246.94f, 293.66f, 349.23f,
        293.66f, 246.94f, 233.08f, 196f,

        146.83f, 196f, 233.08f, 196f,
        174.61f, 146.83f, 130.81f, 123.47f,

        146.83f, 174.61f, 220f, 246.94f,
        220f, 196f, 174.61f, 146.83f
    };

    float[] roots =
    {
        98f,
        87.31f,
        73.42f,
        82.41f
    };

    for (int i = 0; i < sampleCount; i++)
    {
        float time =
            i /
            (float)SampleRate;

        float beat =
            time /
            beatLength;

        int barIndex =
            Mathf.FloorToInt(
                time /
                barLength
            );

        int chordIndex =
            barIndex %
            roots.Length;

        float root =
            roots[chordIndex];

        float fifth =
            root *
            1.5f;

        float octave =
            root *
            2f;

        // =================================================
        // DEEP REACTOR BASS
        // =================================================

        float bass =
            Mathf.Sin(
                Mathf.PI *
                2f *
                root *
                time
            ) *
            0.22f;

        float sub =
            Mathf.Sin(
                Mathf.PI *
                2f *
                root *
                0.5f *
                time
            ) *
            0.13f;

        // =================================================
        // DARK PAD
        // =================================================

        float pad =
            Mathf.Sin(
                Mathf.PI *
                2f *
                fifth *
                time
            ) *
            0.035f;

        pad +=
            Mathf.Sin(
                Mathf.PI *
                2f *
                octave *
                time
            ) *
            0.025f;

        float slow =
            0.65f +
            0.35f *
            Mathf.Sin(
                time *
                0.22f
            );

        pad *= slow;

        // =================================================
        // BOSS ARPEGGIO
        // =================================================

        int arpStep =
            Mathf.FloorToInt(
                beat * 2f
            ) % 8;

        float[] ratios =
        {
            2f,
            3f,
            2.5f,
            4f,
            3f,
            2.5f,
            2f,
            1.5f
        };

        float arpFrequency =
            root *
            ratios[arpStep];

        float localArp =
            beat * 2f -
            Mathf.Floor(
                beat * 2f
            );

        float arpEnvelope =
            Mathf.Exp(
                -9f *
                localArp
            );

        float arp =
            Mathf.Sin(
                Mathf.PI *
                2f *
                arpFrequency *
                time
            ) *
            arpEnvelope *
            0.042f;

        // =================================================
        // BOSS MELODY
        // =================================================

        float melodyPosition =
            beat * 2f;

        int melodyIndex =
            Mathf.FloorToInt(
                melodyPosition
            ) % melody.Length;

        float frequency =
            melody[melodyIndex];

        float localMelody =
            melodyPosition -
            Mathf.Floor(
                melodyPosition
            );

        float envelope =
            Mathf.Exp(
                -4.2f *
                localMelody
            );

        float tone =
            Mathf.Sin(
                Mathf.PI *
                2f *
                frequency *
                time
            );

        tone +=
            Mathf.Sin(
                Mathf.PI *
                2f *
                frequency *
                2f *
                time
            ) *
            0.24f;

        tone +=
            Mathf.Sin(
                Mathf.PI *
                2f *
                frequency *
                3f *
                time
            ) *
            0.08f;

        float melodyLayer =
            tone *
            envelope *
            0.13f;

        // =================================================
        // HEAVY PULSE
        // =================================================

        float localBeat =
            beat -
            Mathf.Floor(beat);

        float pulseEnvelope =
            Mathf.Exp(
                -16f *
                localBeat
            );

        float pulse =
            Mathf.Sin(
                Mathf.PI *
                2f *
                55f *
                time
            ) *
            pulseEnvelope *
            0.13f;

        // =================================================
        // SECOND PULSE
        // =================================================

        float halfBeat =
            beat * 2f;

        float localHalfBeat =
            halfBeat -
            Mathf.Floor(
                halfBeat
            );

        float secondPulse =
            Mathf.Sin(
                Mathf.PI *
                2f *
                110f *
                time
            ) *
            Mathf.Exp(
                -13f *
                localHalfBeat
            ) *
            0.035f;

        // =================================================
        // DARK ATMOSPHERE
        // =================================================

        float atmosphere =
            Mathf.Sin(
                time *
                0.16f
            ) *
            0.018f;

        atmosphere +=
            Mathf.Sin(
                time *
                0.07f
            ) *
            0.012f;

        // =================================================
        // FINAL MIX
        // =================================================

        float sample =
            bass +
            sub +
            pad +
            arp +
            melodyLayer +
            pulse +
            secondPulse +
            atmosphere;

        data[i] =
            Mathf.Clamp(
                sample * 1.1f,
                -0.95f,
                0.95f
            );
    }

    clip.SetData(
        data,
        0
    );

    return clip;
}

   private enum MusicStyle
{
    Menu,
    Normal,
    Combat,
    Boss
}

   private AudioClip CreateMusicClip(
    string clipName,
    int lengthSeconds,
    float bpm,
    MusicStyle style)
{
    int sampleCount =
        lengthSeconds * SampleRate;

    // ВАЖНО:
    // false = не streaming, потому что ниже используется SetData().
    AudioClip clip =
        AudioClip.Create(
            clipName,
            sampleCount,
            1,
            SampleRate,
            false
        );

    float[] data =
        new float[sampleCount];

    float secondsPerBeat =
        60f / bpm;

    float barLength =
        secondsPerBeat * 4f;

    float root =
        GetRootFrequency(style);

    float fifth =
        root * 1.5f;

    float octave =
        root * 2f;

    for (int i = 0; i < sampleCount; i++)
    {
        float time =
            i / (float)SampleRate;

        float beat =
            time / secondsPerBeat;

        float bar =
            time / barLength;

        float localBeat =
            beat - Mathf.Floor(beat);

        float localBar =
            bar - Mathf.Floor(bar);

        // =====================================================
        // BASS
        // =====================================================

        float bass =
            Mathf.Sin(
                Mathf.PI *
                2f *
                root *
                time
            ) *
            0.16f;

        // =====================================================
        // PAD
        // =====================================================

        float pad =
            Mathf.Sin(
                Mathf.PI *
                2f *
                fifth *
                time
            ) *
            0.05f;

        pad +=
            Mathf.Sin(
                Mathf.PI *
                2f *
                octave *
                time
            ) *
            0.025f;

        // =====================================================
        // COMBAT / BOSS PULSE
        // =====================================================

        float pulse = 0f;

        if (style == MusicStyle.Combat ||
            style == MusicStyle.Boss)
        {
            float pulseEnvelope =
                Mathf.Exp(
                    -18f *
                    localBeat
                );

            pulse =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    110f *
                    time
                ) *
                pulseEnvelope *
                (
                    style == MusicStyle.Boss
                        ? 0.16f
                        : 0.09f
                );
        }

        // =====================================================
        // ARPEGGIO
        // =====================================================

        float arp = 0f;

        if (style == MusicStyle.Combat)
        {
            int step =
                Mathf.FloorToInt(
                    beat * 2f
                ) % 8;

            float frequency =
                GetCombatNote(step);

            float stepPosition =
                beat * 2f;

            float localStep =
                stepPosition -
                Mathf.Floor(stepPosition);

            float envelope =
                Mathf.Exp(
                    -14f *
                    localStep
                );

            arp =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    time
                ) *
                envelope *
                0.035f;
        }

        // =====================================================
        // BOSS ARPEGGIO
        // =====================================================

        if (style == MusicStyle.Boss)
        {
            int step =
                Mathf.FloorToInt(
                    beat
                ) % 4;

            float frequency =
                step == 0
                    ? root * 2f
                    : step == 2
                        ? root * 1.25f
                        : root;

            float envelope =
                Mathf.Exp(
                    -10f *
                    localBeat
                );

            arp =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    frequency *
                    time
                ) *
                envelope *
                0.055f;
        }

        // =====================================================
        // DRUMS
        // =====================================================

        float drums = 0f;

        if (style == MusicStyle.Combat ||
            style == MusicStyle.Boss)
        {
            bool kickBeat =
                localBeat < 0.08f;

            if (kickBeat)
            {
                float kickEnvelope =
                    Mathf.Exp(
                        -35f *
                        localBeat
                    );

                drums +=
                    Mathf.Sin(
                        Mathf.PI *
                        2f *
                        65f *
                        time
                    ) *
                    kickEnvelope *
                    (
                        style == MusicStyle.Boss
                            ? 0.16f
                            : 0.10f
                    );
            }

            bool snareBeat =
                localBar > 0.48f &&
                localBar < 0.56f;

            if (snareBeat)
            {
                float snareEnvelope =
                    Mathf.Exp(
                        -25f *
                        (localBar - 0.48f)
                    );

                float snareNoise =
                    Mathf.Sin(
                        time * 19371f
                    );

                drums +=
                    snareNoise *
                    snareEnvelope *
                    0.035f;
            }
        }

        // =====================================================
        // ATMOSPHERE
        // =====================================================

        float atmosphere =
            Mathf.Sin(
                time * 0.37f
            ) *
            0.012f;

        // =====================================================
        // MENU MUSIC
        // =====================================================

        if (style == MusicStyle.Menu)
        {
            float menuPad =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    246.94f *
                    time
                ) *
                0.025f;

            menuPad +=
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    369.99f *
                    time
                ) *
                0.018f;

            float slowModulation =
                0.5f +
                0.5f *
                Mathf.Sin(
                    time * 0.45f
                );

            menuPad *=
                slowModulation;

            atmosphere +=
                menuPad;
        }

        // =====================================================
        // BOSS ATMOSPHERE
        // =====================================================

        if (style == MusicStyle.Boss)
        {
            atmosphere +=
                Mathf.Sin(
                    time * 0.17f
                ) *
                0.025f;
        }

        // =====================================================
        // FINAL MIX
        // =====================================================

        float sample =
            bass +
            pad +
            pulse +
            arp +
            drums +
            atmosphere;

        data[i] =
            sample * 0.8f;
    }

    // Передаём сгенерированный PCM в clip.
    clip.SetData(
        data,
        0
    );

    return clip;
}

    private float GetRootFrequency(
    MusicStyle style)
{
    switch (style)
    {
        case MusicStyle.Boss:
            return 41.2f;

        case MusicStyle.Combat:
            return 55f;

        case MusicStyle.Menu:
            return 61.74f;

        default:
            return 65.41f;
    }
}

    private float GetCombatNote(
        int step)
    {
        switch (step)
        {
            case 0:
                return 220f;

            case 1:
                return 261.63f;

            case 2:
                return 293.66f;

            case 3:
                return 329.63f;

            case 4:
                return 261.63f;

            case 5:
                return 392f;

            case 6:
                return 329.63f;

            default:
                return 293.66f;
        }
    }

    // =========================================================
    // VICTORY
    // =========================================================

    private AudioClip CreateVictoryMusic()
    {
        return CreateMelody(
            "NEON_RAID_VICTORY",
            new float[]
            {
                523.25f,
                659.25f,
                783.99f,
                1046.5f,
                783.99f,
                1046.5f
            },
            0.28f
        );
    }

    // =========================================================
    // DEFEAT
    // =========================================================

    private AudioClip CreateDefeatMusic()
    {
        return CreateMelody(
            "NEON_RAID_DEFEAT",
            new float[]
            {
                220f,
                196f,
                174.61f,
                146.83f
            },
            0.38f
        );
    }

    private AudioClip CreateMelody(
        string clipName,
        float[] notes,
        float noteLength)
    {
        float duration =
            notes.Length *
            noteLength;

        int sampleCount =
            Mathf.RoundToInt(
                duration *
                SampleRate
            );

        AudioClip clip =
            AudioClip.Create(
                clipName,
                sampleCount,
                1,
                SampleRate,
                false
            );

        float[] data =
            new float[sampleCount];

        for (int i = 0;
             i < sampleCount;
             i++)
        {
            float time =
                i /
                (float)SampleRate;

            int noteIndex =
                Mathf.Min(
                    notes.Length - 1,
                    Mathf.FloorToInt(
                        time /
                        noteLength
                    )
                );

            float localTime =
                time -
                noteIndex *
                noteLength;

            float envelope =
                Mathf.Exp(
                    -4.5f *
                    localTime
                );

            float note =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    notes[noteIndex] *
                    time
                );

            float harmonic =
                Mathf.Sin(
                    Mathf.PI *
                    2f *
                    notes[noteIndex] *
                    2f *
                    time
                ) *
                0.2f;

            data[i] =
                (
                    note +
                    harmonic
                ) *
                envelope *
                0.28f;
        }

        clip.SetData(
            data,
            0
        );

        return clip;
    }
} 