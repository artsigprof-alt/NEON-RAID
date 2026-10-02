using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CryptoEvent
{
    public string date;
    public string title;
    public string description;
    public string category;
    public string source;
}

[Serializable]
public class CryptoEventDatabase
{
    public List<CryptoEvent> events;
}

public class CryptoHistoryRandomizer : MonoBehaviour
{
    [Header("JSON-файл с событиями")]
    [SerializeField] private TextAsset eventsJson;

    [Header("UI")]
    [SerializeField] private TMP_Text eventText;
    [SerializeField] private Button nextButton;

    [Header("Настройки")]
    [SerializeField] private bool avoidRepeats = true;

    private List<CryptoEvent> events = new List<CryptoEvent>();
    private List<int> availableIndexes = new List<int>();
    private int currentEventIndex = -1;

    private void Start()
    {
        LoadEvents();

        if (nextButton != null)
            nextButton.onClick.AddListener(ShowRandomEvent);

        ShowRandomEvent();
    }

    private void LoadEvents()
    {
        if (eventsJson == null)
        {
            Debug.LogError("JSON-файл с событиями не назначен.");
            return;
        }

        try
        {
            CryptoEventDatabase database =
                JsonUtility.FromJson<CryptoEventDatabase>(eventsJson.text);

            if (database == null || database.events == null)
            {
                Debug.LogError("Не удалось прочитать список событий.");
                return;
            }

            events = database.events;

            for (int i = 0; i < events.Count; i++)
                availableIndexes.Add(i);

            Debug.Log($"Загружено исторических событий: {events.Count}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Ошибка чтения JSON: {exception.Message}");
        }
    }

   public void ShowRandomEvent()
{
    if (events == null || events.Count == 0)
    {
        if (eventText != null)
            eventText.text = "The event database is empty.";

        return;
    }

    if (avoidRepeats)
    {
        if (availableIndexes.Count == 0)
        {
            for (int i = 0; i < events.Count; i++)
                availableIndexes.Add(i);
        }

        int randomPosition =
            UnityEngine.Random.Range(0, availableIndexes.Count);

        currentEventIndex = availableIndexes[randomPosition];
        availableIndexes.RemoveAt(randomPosition);
    }
    else
    {
        currentEventIndex =
            UnityEngine.Random.Range(0, events.Count);
    }

    CryptoEvent cryptoEvent = events[currentEventIndex];

    eventText.text =
        $"<b>{cryptoEvent.date}</b>\n\n" +
        $"<size=120%><b>{cryptoEvent.title}</b></size>\n\n" +
        $"{cryptoEvent.description}";
}

   public void CopyCurrentEventToClipboard()
{
    if (currentEventIndex < 0 ||
        currentEventIndex >= events.Count)
    {
        return;
    }

    CryptoEvent cryptoEvent = events[currentEventIndex];

    GUIUtility.systemCopyBuffer =
        $"{cryptoEvent.date}\n" +
        $"{cryptoEvent.title}\n\n" +
        $"{cryptoEvent.description}";
}
}