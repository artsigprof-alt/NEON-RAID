
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PianoController : MonoBehaviour
{
    private const int MinMidi = 21;   // A0
    private const int MaxMidi = 108;  // C8
    private const int KeysPerOctave = 12;

    [Header("Piano Keys")]
    [SerializeField] private Button[] keyButtons = new Button[12];
    [SerializeField] private TMP_Text[] keyLabels = new TMP_Text[12];

    [Header("Octave Controls")]
    [SerializeField] private Button previousOctaveButton;
    [SerializeField] private Button nextOctaveButton;

    [Header("Info UI")]
    [SerializeField] private TMP_Text noteText;
    [SerializeField] private TMP_Text octaveText;

    [Header("Audio")]
    [SerializeField] private AudioSource[] keyAudioSources = new AudioSource[12];

    [SerializeField, Range(0.2f, 1f)]
    private float volume = 0.55f;

    [SerializeField, Range(22050, 48000)]
    private int sampleRate = 44100;

    [Header("Piano Sound")]
    [SerializeField, Range(0.5f, 2f)]
    private float brightness = 0.72f;

    [SerializeField, Range(0.5f, 3f)]
    private float sustain = 1.5f;

    [SerializeField, Range(0f, 1f)]
    private float hammerStrength = 0.06f;

    [Header("Key Release")]
    [Tooltip("How quickly the sound fades after releasing a key.")]
    [SerializeField, Range(0.02f, 1f)]
    private float normalRelease = 0.25f;

    [Tooltip("Release time for a very quick tap.")]
    [SerializeField, Range(0.01f, 0.15f)]
    private float quickRelease = 0.045f;

    [Tooltip("Maximum fade time when a key was held for a long time.")]
    [SerializeField, Range(0.1f, 2f)]
    private float longRelease = 0.65f;

    [Header("Start")]
    [SerializeField, Range(0, 8)]
    private int startOctave = 4;

    private int currentOctave;

    private readonly string[] noteNames =
    {
        "C", "C#", "D", "D#", "E", "F",
        "F#", "G", "G#", "A", "A#", "B"
    };

    private readonly Dictionary<int, AudioClip> clipCache =
        new Dictionary<int, AudioClip>();

    private readonly float[] pressStartTime = new float[12];
    private readonly bool[] keyHeld = new bool[12];
    private readonly Coroutine[] releaseCoroutines = new Coroutine[12];

    private void Awake()
    {
        currentOctave = Mathf.Clamp(startOctave, 0, 8);

        CreateAudioSources();
        SetupButtons();
        RefreshOctave();

        if (noteText != null)
            noteText.text = "---";
    }

    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    private void CreateAudioSources()
    {
        if (keyAudioSources == null ||
            keyAudioSources.Length != KeysPerOctave)
        {
            keyAudioSources = new AudioSource[KeysPerOctave];
        }

        for (int i = 0; i < KeysPerOctave; i++)
        {
            if (keyAudioSources[i] != null)
                continue;

            GameObject sourceObject = new GameObject("PianoKeyAudio_" + i);
            sourceObject.transform.SetParent(transform);

            AudioSource source = sourceObject.AddComponent<AudioSource>();

            source.playOnAwake = false;
            source.loop = false;
            source.volume = volume;
            source.spatialBlend = 0f;

            keyAudioSources[i] = source;
        }
    }

    // =========================================================
    // BUTTONS
    // =========================================================

    private void SetupButtons()
    {
        if (keyButtons != null)
        {
            for (int i = 0; i < keyButtons.Length && i < KeysPerOctave; i++)
            {
                if (keyButtons[i] == null)
                    continue;

                int index = i;

                // Убираем обычный Click.
                keyButtons[i].onClick.RemoveAllListeners();

                EventTrigger trigger =
                    keyButtons[i].GetComponent<EventTrigger>();

                if (trigger == null)
                    trigger = keyButtons[i].gameObject.AddComponent<EventTrigger>();

                trigger.triggers.Clear();

                AddPointerEvent(
                    trigger,
                    EventTriggerType.PointerDown,
                    () => KeyDown(index)
                );

                AddPointerEvent(
                    trigger,
                    EventTriggerType.PointerUp,
                    () => KeyUp(index)
                );

                AddPointerEvent(
                    trigger,
                    EventTriggerType.PointerExit,
                    () => KeyUp(index)
                );
            }
        }

        if (previousOctaveButton != null)
            previousOctaveButton.onClick.AddListener(PreviousOctave);

        if (nextOctaveButton != null)
            nextOctaveButton.onClick.AddListener(NextOctave);
    }

    private void AddPointerEvent(
        EventTrigger trigger,
        EventTriggerType eventType,
        Action callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = eventType;

        entry.callback.AddListener((eventData) =>
        {
            callback?.Invoke();
        });

        trigger.triggers.Add(entry);
    }

    // =========================================================
    // OCTAVES
    // =========================================================

    private void RefreshOctave()
    {
        if (octaveText != null)
            octaveText.text = "ОКТАВА " + currentOctave;

        for (int i = 0; i < KeysPerOctave; i++)
        {
            int midi = GetMidi(currentOctave, i);

            bool valid =
                midi >= MinMidi &&
                midi <= MaxMidi;

            if (keyButtons != null &&
                i < keyButtons.Length &&
                keyButtons[i] != null)
            {
                keyButtons[i].interactable = valid;
            }

            if (keyLabels != null &&
                i < keyLabels.Length &&
                keyLabels[i] != null)
            {
                keyLabels[i].text =
                    valid ? GetNoteName(midi) : "";
            }
        }

        if (previousOctaveButton != null)
            previousOctaveButton.interactable = currentOctave > 0;

        if (nextOctaveButton != null)
            nextOctaveButton.interactable = currentOctave < 8;
    }

    public void PreviousOctave()
    {
        StopAllKeys();

        if (currentOctave <= 0)
            return;

        currentOctave--;

        RefreshOctave();

        if (noteText != null)
            noteText.text = "---";
    }

    public void NextOctave()
    {
        StopAllKeys();

        if (currentOctave >= 8)
            return;

        currentOctave++;

        RefreshOctave();

        if (noteText != null)
            noteText.text = "---";
    }

    // =========================================================
    // KEY DOWN
    // =========================================================

    private void KeyDown(int index)
    {
        if (index < 0 || index >= KeysPerOctave)
            return;

        int midi = GetMidi(currentOctave, index);

        if (midi < MinMidi || midi > MaxMidi)
            return;

        keyHeld[index] = true;
        pressStartTime[index] = Time.unscaledTime;

        if (releaseCoroutines[index] != null)
        {
            StopCoroutine(releaseCoroutines[index]);
            releaseCoroutines[index] = null;
        }

        AudioSource source = keyAudioSources[index];

        if (source == null)
            return;

        source.Stop();
        source.volume = volume;

        AudioClip clip = GetOrCreateClip(midi);

        source.clip = clip;
        source.Play();

        if (noteText != null)
            noteText.text = GetNoteName(midi);
    }

    // =========================================================
    // KEY UP
    // =========================================================

    private void KeyUp(int index)
    {
        if (index < 0 || index >= KeysPerOctave)
            return;

        if (!keyHeld[index])
            return;

        keyHeld[index] = false;

        float heldTime =
            Time.unscaledTime - pressStartTime[index];

        AudioSource source = keyAudioSources[index];

        if (source == null || !source.isPlaying)
            return;

        /*
         * Чем дольше держали:
         * → тем длиннее естественное затухание.
         *
         * Быстро нажал/отпустил:
         * → короткий звук.
         *
         * Долго держал:
         * → длинный хвост.
         */

        float releaseTime;

        if (heldTime < 0.12f)
        {
            releaseTime = quickRelease;
        }
        else
        {
            float normalized =
                Mathf.InverseLerp(0.12f, 2f, heldTime);

            releaseTime =
                Mathf.Lerp(
                    normalRelease,
                    longRelease,
                    normalized
                );
        }

        releaseCoroutines[index] =
            StartCoroutine(
                FadeOut(source, releaseTime, index)
            );
    }

    private IEnumerator FadeOut(
        AudioSource source,
        float duration,
        int index)
    {
        float startVolume = source.volume;

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(time / duration);

            // Мягкое экспоненциальное затухание.
            float fade =
                1f - Mathf.SmoothStep(0f, 1f, t);

            source.volume =
                startVolume * fade;

            yield return null;
        }

        source.Stop();
        source.volume = volume;

        releaseCoroutines[index] = null;
    }

    private void StopAllKeys()
    {
        for (int i = 0; i < KeysPerOctave; i++)
        {
            keyHeld[i] = false;

            if (releaseCoroutines[i] != null)
            {
                StopCoroutine(releaseCoroutines[i]);
                releaseCoroutines[i] = null;
            }

            if (keyAudioSources[i] != null)
            {
                keyAudioSources[i].Stop();
                keyAudioSources[i].volume = volume;
            }
        }
    }

    // =========================================================
    // PIANO SOUND GENERATION
    // =========================================================

    private AudioClip GetOrCreateClip(int midi)
    {
        if (clipCache.TryGetValue(midi, out AudioClip cached))
            return cached;

        string noteName = GetNoteName(midi);

        float frequency =
            MidiToFrequency(midi);

        // Длинный клип нужен для удержания клавиши.
        float clipDuration = 7f;

        int samples =
            Mathf.CeilToInt(
                clipDuration * sampleRate
            );

        AudioClip clip =
            AudioClip.Create(
                "Piano_" + noteName,
                samples,
                1,
                sampleRate,
                false
            );

        float[] data =
            new float[samples];

        GeneratePianoSound(
            data,
            frequency,
            midi
        );

        clip.SetData(data, 0);

        clipCache.Add(midi, clip);

        return clip;
    }

    private void GeneratePianoSound(
        float[] data,
        float frequency,
        int midi)
    {
        /*
         * Нормализуем диапазон:
         *
         * 0 = самый низ
         * 1 = самый верх
         */
        float pitchPosition =
            Mathf.InverseLerp(
                MinMidi,
                MaxMidi,
                midi
            );

        /*
         * Края пианино делаем мягче:
         *
         * Низ:
         * - меньше верхних гармоник
         * - длиннее decay
         *
         * Верх:
         * - тоже меньше резких гармоник
         * - мягкая атака
         */

        float lowFactor =
            1f - pitchPosition;

        float highFactor =
            pitchPosition;

        float mainDecay =
            Mathf.Lerp(
                3.8f,
                1.7f,
                pitchPosition
            );

        mainDecay *= sustain;

        /*
         * Низкие ноты:
         * фундаментальная частота доминирует.
         */
        float harmonic2 =
            Mathf.Lerp(
                0.25f,
                0.40f,
                1f - highFactor
            );

        /*
         * Высокие ноты:
         * не даём им становиться пищащими.
         */
        float harmonic3 =
            Mathf.Lerp(
                0.07f,
                0.13f,
                1f - highFactor
            );

        float harmonic4 =
            Mathf.Lerp(
                0.018f,
                0.045f,
                1f - highFactor
            );

        float harmonic5 =
            Mathf.Lerp(
                0.004f,
                0.015f,
                1f - highFactor
            );

        harmonic2 *= brightness;
        harmonic3 *= brightness;
        harmonic4 *= brightness;
        harmonic5 *= brightness;

        float previous = 0f;

        for (int i = 0; i < data.Length; i++)
        {
            float t =
                (float)i / sampleRate;

            /*
             * Очень плавная атака.
             * Не должно быть резкого "ПИ".
             */
            float attack =
                1f - Mathf.Exp(-t * 180f);

            /*
             * Основной хвост.
             */
            float envelope =
                Mathf.Exp(
                    -t / mainDecay
                );

            float sample = 0f;

            // Fundamental
            sample +=
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    t
                ) * 1.0f;

            // 2nd harmonic
            sample +=
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    2f *
                    t
                ) *
                harmonic2 *
                Mathf.Exp(
                    -t /
                    (mainDecay * 0.72f)
                );

            // 3rd harmonic
            sample +=
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    3f *
                    t
                ) *
                harmonic3 *
                Mathf.Exp(
                    -t /
                    (mainDecay * 0.48f)
                );

            // 4th harmonic
            sample +=
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    4f *
                    t
                ) *
                harmonic4 *
                Mathf.Exp(
                    -t /
                    (mainDecay * 0.30f)
                );

            // 5th harmonic
            sample +=
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    5f *
                    t
                ) *
                harmonic5 *
                Mathf.Exp(
                    -t /
                    (mainDecay * 0.20f)
                );

            /*
             * Очень мягкий молоточек.
             *
             * Сильнее в середине диапазона,
             * почти отсутствует на самых низких
             * и самых высоких нотах.
             */
            float hammer =
                Mathf.Sin(
                    2f *
                    Mathf.PI *
                    frequency *
                    2.1f *
                    t
                ) *
                Mathf.Exp(-t * 90f) *
                hammerStrength *
                Mathf.Sin(
                    Mathf.PI *
                    Mathf.Clamp01(
                        pitchPosition * 1.4f
                    )
                );

            sample += hammer;

            sample *=
                attack *
                envelope;

            /*
             * Мягкий saturation.
             * Убирает цифровую резкость.
             */
            sample =
                (float)Math.Tanh(
                    sample * 0.72f
                );

            /*
             * Очень простой low-pass.
             * Особенно полезен для верхних нот.
             */
            float smoothing =
                Mathf.Lerp(
                    0.20f,
                    0.075f,
                    highFactor
                );

            previous =
                Mathf.Lerp(
                    previous,
                    sample,
                    smoothing
                );

            /*
             * Низам добавляем немного тела,
             * верхам уменьшаем яркость.
             */
            float finalGain =
                Mathf.Lerp(
                    0.43f,
                    0.35f,
                    highFactor
                );

            data[i] =
                previous * finalGain;
        }
    }

    // =========================================================
    // UTILITIES
    // =========================================================

    private int GetMidi(int octave, int keyIndex)
    {
        return 12 * (octave + 1) + keyIndex;
    }

    private string GetNoteName(int midi)
    {
        int noteIndex = midi % 12;

        int octave =
            (midi / 12) - 1;

        return noteNames[noteIndex] + octave;
    }

    private float MidiToFrequency(int midi)
    {
        // A4 = 440 Hz
        return 440f *
               Mathf.Pow(
                   2f,
                   (midi - 69) / 12f
               );
    }

    private void OnDestroy()
    {
        StopAllKeys();

        foreach (AudioClip clip in clipCache.Values)
        {
            if (clip != null)
                Destroy(clip);
        }

        clipCache.Clear();
    }
}

