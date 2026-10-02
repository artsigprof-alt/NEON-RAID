
using UnityEngine;

public class SkrReactorVFX : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Transform reactorPoint;
    [SerializeField] private ParticleSystem engineFX;

    [Header("Glow")]
    [SerializeField] private float glowScale = 1.7f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.15f;

    [Header("Rings")]
    [SerializeField] private float innerRingSize = 1.35f;
    [SerializeField] private float outerRingSize = 1.8f;
    [SerializeField] private float innerRingSpeed = 70f;
    [SerializeField] private float outerRingSpeed = -45f;

    [Header("Particles")]
    [SerializeField] private int particleEmission = 12;
    [SerializeField] private float particleRadius = 0.6f;

    [Header("Colors")]
    [SerializeField] private Color activeCyan =
        new Color(0.1f, 1f, 0.9f, 1f);

    [SerializeField] private Color activePurple =
        new Color(0.7f, 0.25f, 1f, 1f);

    [SerializeField] private Color inactiveColor =
        new Color(1f, 1f, 1f, 1f);

    private GameObject glowObject;
    private SpriteRenderer glowRenderer;

    private GameObject innerRingObject;
    private GameObject outerRingObject;

    private LineRenderer innerRing;
    private LineRenderer outerRing;

    private ParticleSystem particles;

    private Texture2D glowTexture;
    private Sprite glowSprite;

    private float currentAlpha;
    private float targetAlpha;

    private bool wasActive;

    private void Awake()
    {
        if (reactorPoint == null)
            reactorPoint = transform;

        if (playerSprite == null)
            playerSprite =
                GetComponentInChildren<SpriteRenderer>();

        CreateGlow();
        CreateRings();
        CreateParticles();

        SetAlphaInstant(0f);
    }

    private void Update()
    {
        bool active =
            SkrReactorManager.Instance != null &&
            SkrReactorManager.Instance.IsActive;

        targetAlpha =
            active ? 1f : 0f;

        currentAlpha =
            Mathf.MoveTowards(
                currentAlpha,
                targetAlpha,
                Time.unscaledDeltaTime * 3f
            );

        UpdateGlow();
        UpdateRings();
        UpdateParticles();

        if (active && !wasActive)
        {
            ActivateEffect();
        }

        if (!active && wasActive)
        {
            DeactivateEffect();
        }

        wasActive = active;
    }

    // =========================================================
    // GLOW
    // =========================================================

    private void CreateGlow()
    {
        glowObject =
            new GameObject("SKR_Reactor_Glow");

        glowObject.transform.SetParent(
            reactorPoint
        );

        glowObject.transform.localPosition =
            Vector3.zero;

        glowObject.transform.localRotation =
            Quaternion.identity;

        glowObject.transform.localScale =
            Vector3.one * glowScale;

        glowRenderer =
            glowObject.AddComponent<SpriteRenderer>();

        glowRenderer.sortingOrder = -1;

        CreateGlowTexture();

        glowRenderer.sprite =
            glowSprite;
    }

    private void CreateGlowTexture()
    {
        int size = 64;

        glowTexture =
            new Texture2D(
                size,
                size,
                TextureFormat.RGBA32,
                false
            );

        glowTexture.filterMode =
            FilterMode.Bilinear;

        Color[] pixels =
            new Color[size * size];

        Vector2 center =
            new Vector2(
                size * 0.5f,
                size * 0.5f
            );

        float radius =
            size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance =
                    Vector2.Distance(
                        new Vector2(x, y),
                        center
                    );

                float normalized =
                    distance / radius;

                if (normalized >= 1f)
                {
                    pixels[
                        y * size + x
                    ] = Color.clear;

                    continue;
                }

                float alpha =
                    Mathf.Pow(
                        1f - normalized,
                        2.4f
                    );

                pixels[
                    y * size + x
                ] =
                    new Color(
                        1f,
                        1f,
                        1f,
                        alpha
                    );
            }
        }

        glowTexture.SetPixels(pixels);
        glowTexture.Apply();

        glowSprite =
            Sprite.Create(
                glowTexture,
                new Rect(
                    0,
                    0,
                    size,
                    size
                ),
                new Vector2(
                    0.5f,
                    0.5f
                ),
                64f
            );
    }

    private void UpdateGlow()
    {
        if (glowRenderer == null)
            return;

        float pulse =
            1f +
            Mathf.Sin(
                Time.unscaledTime *
                pulseSpeed
            ) *
            pulseAmount;

        glowObject.transform.localScale =
            Vector3.one *
            glowScale *
            pulse;

        Color color =
            Color.Lerp(
                activeCyan,
                activePurple,
                (Mathf.Sin(
                    Time.unscaledTime *
                    1.4f
                ) + 1f) *
                0.5f
            );

        color.a =
            currentAlpha *
            0.55f;

        glowRenderer.color =
            color;
    }

    // =========================================================
    // RINGS
    // =========================================================

    private void CreateRings()
    {
        innerRingObject =
            new GameObject(
                "SKR_Reactor_InnerRing"
            );

        innerRingObject.transform.SetParent(
            reactorPoint
        );

        innerRingObject.transform.localPosition =
            Vector3.zero;

        innerRing =
            CreateRing(
                innerRingObject,
                innerRingSize,
                activeCyan
            );

        outerRingObject =
            new GameObject(
                "SKR_Reactor_OuterRing"
            );

        outerRingObject.transform.SetParent(
            reactorPoint
        );

        outerRingObject.transform.localPosition =
            Vector3.zero;

        outerRing =
            CreateRing(
                outerRingObject,
                outerRingSize,
                activePurple
            );
    }

    private LineRenderer CreateRing(
        GameObject obj,
        float size,
        Color color
    )
    {
        LineRenderer line =
            obj.AddComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.loop = true;

        line.positionCount = 64;

        line.startWidth = 0.025f;
        line.endWidth = 0.025f;

        line.material =
            CreateUnlitMaterial();

        line.startColor =
            color;

        line.endColor =
            color;

        line.sortingOrder = -1;

        for (int i = 0; i < 64; i++)
        {
            float angle =
                i /
                64f *
                Mathf.PI *
                2f;

            float x =
                Mathf.Cos(angle) *
                size;

            float y =
                Mathf.Sin(angle) *
                size;

            line.SetPosition(
                i,
                new Vector3(
                    x,
                    y,
                    0f
                )
            );
        }

        return line;
    }

    private Material CreateUnlitMaterial()
    {
        Shader shader =
            Shader.Find(
                "Sprites/Default"
            );

        if (shader == null)
            shader =
                Shader.Find(
                    "Unlit/Color"
                );

        Material material =
            new Material(shader);

        return material;
    }

    private void UpdateRings()
    {
        if (innerRing == null ||
            outerRing == null)
            return;

        float innerRotation =
            Time.unscaledDeltaTime *
            innerRingSpeed;

        float outerRotation =
            Time.unscaledDeltaTime *
            outerRingSpeed;

        innerRingObject.transform.Rotate(
            0f,
            0f,
            innerRotation
        );

        outerRingObject.transform.Rotate(
            0f,
            0f,
            outerRotation
        );

        float pulse =
            1f +
            Mathf.Sin(
                Time.unscaledTime *
                pulseSpeed
            ) *
            pulseAmount;

        innerRingObject.transform.localScale =
            Vector3.one *
            innerRingSize *
            pulse;

        outerRingObject.transform.localScale =
            Vector3.one *
            outerRingSize *
            pulse;

        Color innerColor =
            activeCyan;

        innerColor.a =
            currentAlpha *
            0.7f;

        Color outerColor =
            activePurple;

        outerColor.a =
            currentAlpha *
            0.5f;

        innerRing.startColor =
            innerColor;

        innerRing.endColor =
            innerColor;

        outerRing.startColor =
            outerColor;

        outerRing.endColor =
            outerColor;

        innerRing.startWidth =
            0.025f *
            currentAlpha;

        innerRing.endWidth =
            innerRing.startWidth;

        outerRing.startWidth =
            0.018f *
            currentAlpha;

        outerRing.endWidth =
            outerRing.startWidth;
    }

    // =========================================================
    // PARTICLES
    // =========================================================

    private void CreateParticles()
    {
        GameObject particleObject =
            new GameObject(
                "SKR_Reactor_Particles"
            );

        particleObject.transform.SetParent(
            reactorPoint
        );

        particleObject.transform.localPosition =
            Vector3.zero;

        particles =
            particleObject.AddComponent<
                ParticleSystem
            >();

        var main =
            particles.main;

        main.loop = true;
        main.playOnAwake = false;

        main.startLifetime =
            new ParticleSystem.MinMaxCurve(
                0.5f,
                1.2f
            );

        main.startSpeed =
            new ParticleSystem.MinMaxCurve(
                0.15f,
                0.45f
            );

        main.startSize =
            new ParticleSystem.MinMaxCurve(
                0.025f,
                0.06f
            );

        main.maxParticles =
            particleEmission * 3;

        main.simulationSpace =
            ParticleSystemSimulationSpace.Local;

        main.startColor =
            activeCyan;

        var emission =
            particles.emission;

        emission.enabled = true;

        emission.rateOverTime =
            particleEmission;

        var shape =
            particles.shape;

        shape.enabled = true;
        shape.shapeType =
            ParticleSystemShapeType.Circle;

        shape.radius =
            particleRadius;

        var velocity =
            particles.velocityOverLifetime;

        velocity.enabled = true;

        velocity.radial =
            new ParticleSystem.MinMaxCurve(
                0.05f,
                0.15f
            );

        var renderer =
            particles.GetComponent<
                ParticleSystemRenderer
            >();

        renderer.sortingOrder = 1;

        particles.Stop();
    }

    private void UpdateParticles()
    {
        if (particles == null)
            return;

        var main =
            particles.main;

        Color color =
            Color.Lerp(
                activeCyan,
                activePurple,
                (Mathf.Sin(
                    Time.unscaledTime *
                    1.4f
                ) + 1f) *
                0.5f
            );

        color.a =
            currentAlpha;

        main.startColor =
            color;

        if (currentAlpha > 0.01f)
        {
            if (!particles.isPlaying)
                particles.Play();
        }
        else
        {
            if (particles.isPlaying)
                particles.Stop();
        }
    }

    // =========================================================
    // EFFECT START / END
    // =========================================================

    private void ActivateEffect()
    {
        Debug.Log(
            "[SKR] Reactor VFX activated."
        );

        if (engineFX != null)
        {
            var emission =
                engineFX.emission;

            emission.enabled = true;

            emission.rateOverTime =
                new ParticleSystem.MinMaxCurve(
                    15f,
                    25f
                );
        }
    }

    private void DeactivateEffect()
    {
        Debug.Log(
            "[SKR] Reactor VFX deactivated."
        );

        if (engineFX != null)
        {
            var emission =
                engineFX.emission;

            emission.rateOverTime =
                new ParticleSystem.MinMaxCurve(
                    5f,
                    10f
                );
        }
    }

    private void SetAlphaInstant(
        float alpha
    )
    {
        currentAlpha =
            alpha;

        targetAlpha =
            alpha;

        if (glowRenderer != null)
        {
            Color color =
                activeCyan;

            color.a =
                alpha;

            glowRenderer.color =
                color;
        }

        if (innerRing != null)
        {
            Color color =
                activeCyan;

            color.a =
                alpha;

            innerRing.startColor =
                color;

            innerRing.endColor =
                color;
        }

        if (outerRing != null)
        {
            Color color =
                activePurple;

            color.a =
                alpha;

            outerRing.startColor =
                color;

            outerRing.endColor =
                color;
        }
    }

    private void OnDestroy()
    {
        if (glowSprite != null)
            Destroy(glowSprite);

        if (glowTexture != null)
            Destroy(glowTexture);

        if (innerRing != null &&
            innerRing.material != null)
        {
            Destroy(
                innerRing.material
            );
        }

        if (outerRing != null &&
            outerRing.material != null)
        {
            Destroy(
                outerRing.material
            );
        }
    }
}

