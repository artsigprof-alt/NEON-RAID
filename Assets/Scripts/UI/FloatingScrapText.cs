using TMPro;
using UnityEngine;

public class FloatingScrapText : MonoBehaviour
{
    [SerializeField] private float speed = 1.2f;
    [SerializeField] private float lifetime = 0.8f;

    private TMP_Text text;
    private float timer;

    public void Initialize(int amount)
    {
        text = GetComponent<TMP_Text>();

        if (text != null)
            text.text = $"+{amount} SCRAP";

        timer = lifetime;
    }

    private void Update()
    {
        transform.position +=
            Vector3.up *
            speed *
            Time.deltaTime;

        timer -= Time.deltaTime;

        if (timer <= 0f)
            Destroy(gameObject);
    }
}