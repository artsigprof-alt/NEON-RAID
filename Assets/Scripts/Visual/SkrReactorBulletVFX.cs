
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SkrReactorBulletVFX : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private SpriteRenderer bulletRenderer;

    [Header("Trail")]
    [SerializeField] private bool useTrail = true;
    [SerializeField] private float trailTimeTier1 = 0.08f;
    [SerializeField] private float trailTimeTier2 = 0.12f;
    [SerializeField] private float trailTimeTier3 = 0.18f;

    [SerializeField] private float trailStartWidth = 0.12f;
    [SerializeField] private float trailEndWidth = 0.01f;

    [Header("Color")]
    [SerializeField] private Color normalColor = Color.white;

    [SerializeField] private Color reactorCyan =
        new Color(
            0.1f,
            1f,
            0.9f,
            1f
        );

    [SerializeField] private Color reactorPurple =
        new Color(
            0.7f,
            0.25f,
            1f,
            1f
        );

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 8f;
    [SerializeField] private float pulseAmount = 0.18f;

    [Header("Sorting")]
    [SerializeField] private int trailSortingOrder = -1;

    private TrailRenderer trail;

    private Color originalColor;

    private bool lastReactorState;

    private void Awake()
    {
        if (bulletRenderer == null)
            bulletRenderer =
                GetComponent<SpriteRenderer>();

        originalColor =
            bulletRenderer.color;

        if (originalColor == Color.clear)
            originalColor = normalColor;

        if (useTrail)
            CreateTrail();
    }

    private void Start()
    {
        UpdateReactorState();
    }

    private void Update()
    {
        UpdateReactorState();

        if (
            SkrReactorManager.Instance == null ||
            !SkrReactorManager.Instance.IsActive
        )
        {
            RestoreNormal();
            return;
        }

        int tier =
            SkrReactorManager.Instance.CurrentTier;

        UpdateReactorColor(tier);
        UpdateTrail(tier);
    }

    // =========================================================
    // TRAIL
    // =========================================================

    private void CreateTrail()
    {
        GameObject trailObject =
            new GameObject(
                "SKR_Bullet_Trail"
            );

        trailObject.transform.SetParent(
            transform
        );

        trailObject.transform.localPosition =
            Vector3.zero;

        trail =
            trailObject.AddComponent<
                TrailRenderer
            >();

        trail.time =
            trailTimeTier1;

        trail.startWidth =
            trailStartWidth;

        trail.endWidth =
            trailEndWidth;

        trail.minVertexDistance =
            0.02f;

        trail.numCapVertices =
            2;

        trail.numCornerVertices =
            2;

        trail.sortingOrder =
            trailSortingOrder;

        Shader shader =
            Shader.Find(
                "Sprites/Default"
            );

        if (shader == null)
            shader =
                Shader.Find(
                    "Unlit/Color"
                );

        trail.material =
            new Material(shader);

        trail.startColor =
            reactorCyan;

        trail.endColor =
            new Color(
                reactorPurple.r,
                reactorPurple.g,
                reactorPurple.b,
                0f
            );

        trail.emitting = false;
    }

    private void UpdateTrail(int tier)
    {
        if (trail == null)
            return;

        trail.emitting = true;

        switch (tier)
        {
            case 1:
                trail.time =
                    trailTimeTier1;
                break;

            case 2:
                trail.time =
                    trailTimeTier2;
                break;

            case 3:
                trail.time =
                    trailTimeTier3;
                break;

            default:
                trail.time =
                    trailTimeTier1;
                break;
        }

        float pulse =
            1f +
            Mathf.Sin(
                Time.unscaledTime *
                pulseSpeed
            ) *
            pulseAmount;

        trail.startWidth =
            trailStartWidth *
            pulse;

        trail.endWidth =
            trailEndWidth;

        Color start =
            Color.Lerp(
                reactorCyan,
                reactorPurple,
                (
                    Mathf.Sin(
                        Time.unscaledTime *
                        2f
                    ) + 1f
                ) * 0.5f
            );

        start.a = 0.9f;

        Color end =
            start;

        end.a = 0f;

        trail.startColor =
            start;

        trail.endColor =
            end;
    }

    // =========================================================
    // COLOR
    // =========================================================

    private void UpdateReactorColor(int tier)
    {
        if (bulletRenderer == null)
            return;

        float pulse =
            1f +
            Mathf.Sin(
                Time.unscaledTime *
                pulseSpeed
            ) *
            pulseAmount;

        Color color =
            Color.Lerp(
                reactorCyan,
                reactorPurple,
                (
                    Mathf.Sin(
                        Time.unscaledTime *
                        2f
                    ) + 1f
                ) * 0.5f
            );

        float intensity =
            1f;

        switch (tier)
        {
            case 1:
                intensity = 1.05f;
                break;

            case 2:
                intensity = 1.15f;
                break;

            case 3:
                intensity = 1.3f;
                break;
        }

        color *=
            intensity *
            pulse;

        color.a = 1f;

        bulletRenderer.color =
            color;
    }

    // =========================================================
    // STATE
    // =========================================================

    private void UpdateReactorState()
    {
        bool active =
            SkrReactorManager.Instance != null &&
            SkrReactorManager.Instance.IsActive;

        if (active != lastReactorState)
        {
            if (active)
                Activate();

            else
                RestoreNormal();

            lastReactorState =
                active;
        }
    }

    private void Activate()
    {
        if (bulletRenderer != null)
        {
            bulletRenderer.color =
                reactorCyan;
        }

        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }

    private void RestoreNormal()
    {
        if (bulletRenderer != null)
        {
            bulletRenderer.color =
                originalColor;
        }

        if (trail != null)
        {
            trail.emitting = false;
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (trail != null &&
            trail.material != null)
        {
            Destroy(
                trail.material
            );
        }
    }
}

