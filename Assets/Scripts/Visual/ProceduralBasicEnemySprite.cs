using UnityEngine;

[ExecuteAlways]
public class ProceduralBasicEnemySprite : MonoBehaviour
{
    [Header("Texture")]
    [SerializeField] private int textureSize = 128;
    [SerializeField] private float pixelsPerUnit = 64f;

    [Header("Position")]
    [SerializeField] private int verticalOffset = 0;

    [Header("Body")]
    [SerializeField] private Color bodyColor =
        new Color(0.16f, 0.08f, 0.10f, 1f);

    [SerializeField] private Color armorColor =
        new Color(0.34f, 0.19f, 0.22f, 1f);

    [SerializeField] private Color metalColor =
        new Color(0.48f, 0.32f, 0.36f, 1f);

    [SerializeField] private Color darkColor =
        new Color(0.04f, 0.025f, 0.04f, 1f);

    [Header("Enemy Glow")]
    [SerializeField] private Color accentColor =
        new Color(1f, 0.08f, 0.12f, 1f);

    [SerializeField] private Color glowColor =
        new Color(1f, 0.25f, 0.08f, 1f);

    [SerializeField] private Color secondaryGlow =
        new Color(1f, 0.02f, 0.35f, 1f);

    private SpriteRenderer spriteRenderer;

    private Texture2D generatedTexture;
    private Sprite generatedSprite;

    private int C(float value)
    {
        return Mathf.RoundToInt(
            value + verticalOffset
        );
    }

    private void OnEnable()
    {
        Generate();
    }

    private void Start()
    {
        Generate();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            Generate();
        }
    }
#endif

    private void Generate()
    {
        if (textureSize < 32)
            textureSize = 32;

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer =
                gameObject.AddComponent<SpriteRenderer>();
        }

        CleanupOldSprite();

        generatedTexture =
            new Texture2D(
                textureSize,
                textureSize,
                TextureFormat.RGBA32,
                false
            );

        generatedTexture.name =
            "ProceduralBasicEnemyTexture";

        generatedTexture.filterMode =
            FilterMode.Point;

        generatedTexture.wrapMode =
            TextureWrapMode.Clamp;

        Clear(generatedTexture);

        float center =
            textureSize * 0.5f;

        // =====================================================
        // OUTER SILHOUETTE
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(108),
            center - 24,
            C(72),
            center - 37,
            C(48),
            darkColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(108),
            center + 24,
            C(72),
            center + 37,
            C(48),
            darkColor
        );

        // =====================================================
        // MAIN BODY
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(101),
            center - 17,
            C(71),
            center - 22,
            C(48),
            bodyColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(101),
            center + 17,
            C(71),
            center + 22,
            C(48),
            bodyColor
        );

        // =====================================================
        // CENTRAL ARMOR
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(95),
            center - 11,
            C(73),
            center - 12,
            C(54),
            armorColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(95),
            center + 11,
            C(73),
            center + 12,
            C(54),
            armorColor
        );

        // =====================================================
        // LEFT WEAPON ARM
        // =====================================================

        FillTriangle(
            generatedTexture,
            center - 10,
            C(75),
            center - 45,
            C(61),
            center - 35,
            C(39),
            metalColor
        );

        FillTriangle(
            generatedTexture,
            center - 15,
            C(66),
            center - 39,
            C(56),
            center - 30,
            C(46),
            armorColor
        );

        // =====================================================
        // RIGHT WEAPON ARM
        // =====================================================

        FillTriangle(
            generatedTexture,
            center + 10,
            C(75),
            center + 45,
            C(61),
            center + 35,
            C(39),
            metalColor
        );

        FillTriangle(
            generatedTexture,
            center + 15,
            C(66),
            center + 39,
            C(56),
            center + 30,
            C(46),
            armorColor
        );

        // =====================================================
        // ARM DARK PANELS
        // =====================================================

        FillTriangle(
            generatedTexture,
            center - 16,
            C(67),
            center - 36,
            C(56),
            center - 29,
            C(49),
            darkColor
        );

        FillTriangle(
            generatedTexture,
            center + 16,
            C(67),
            center + 36,
            C(56),
            center + 29,
            C(49),
            darkColor
        );

        // =====================================================
        // NEON ARM STRIPS
        // =====================================================

        DrawThickLine(
            generatedTexture,
            center - 15,
            C(65),
            center - 37,
            C(54),
            2,
            accentColor
        );

        DrawThickLine(
            generatedTexture,
            center + 15,
            C(65),
            center + 37,
            C(54),
            2,
            accentColor
        );

        // =====================================================
        // CENTRAL CORE
        // =====================================================

        FillCircle(
            generatedTexture,
            center,
            C(68),
            16,
            darkColor
        );

        FillCircleRing(
            generatedTexture,
            center,
            C(68),
            13,
            3,
            metalColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(68),
            9,
            accentColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(68),
            6,
            glowColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(68),
            3,
            Color.white
        );

        // =====================================================
        // CORE CROSS
        // =====================================================

        DrawThickLine(
            generatedTexture,
            center - 10,
            C(68),
            center - 6,
            C(68),
            2,
            secondaryGlow
        );

        DrawThickLine(
            generatedTexture,
            center + 6,
            C(68),
            center + 10,
            C(68),
            2,
            secondaryGlow
        );

        // =====================================================
        // UPPER SENSOR
        // =====================================================

        FillCircle(
            generatedTexture,
            center,
            C(91),
            5,
            darkColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(91),
            2,
            glowColor
        );

        // =====================================================
        // SIDE SENSORS
        // =====================================================

        FillCircle(
            generatedTexture,
            center - 18,
            C(82),
            3,
            darkColor
        );

        FillCircle(
            generatedTexture,
            center + 18,
            C(82),
            3,
            darkColor
        );

        FillCircle(
            generatedTexture,
            center - 18,
            C(82),
            1.5f,
            accentColor
        );

        FillCircle(
            generatedTexture,
            center + 18,
            C(82),
            1.5f,
            accentColor
        );

        // =====================================================
        // FRONT ARMOR
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(108),
            center - 6,
            C(91),
            center + 6,
            C(91),
            armorColor
        );

        DrawThickLine(
            generatedTexture,
            center,
            C(103),
            center,
            C(93),
            2,
            glowColor
        );

        // =====================================================
        // LOWER ENGINE HOUSING
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(51),
            center - 12,
            C(35),
            center - 10,
            C(22),
            darkColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(51),
            center + 12,
            C(35),
            center + 10,
            C(22),
            darkColor
        );

        // =====================================================
        // ENGINE RING
        // =====================================================

        FillCircle(
            generatedTexture,
            center,
            C(25),
            10,
            darkColor
        );

        FillCircleRing(
            generatedTexture,
            center,
            C(25),
            8,
            2,
            metalColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(25),
            5,
            secondaryGlow
        );

        FillCircle(
            generatedTexture,
            center,
            C(25),
            2,
            glowColor
        );

        // =====================================================
        // SMALL SIDE LIGHTS
        // =====================================================

        FillCircle(
            generatedTexture,
            center - 27,
            C(43),
            2,
            secondaryGlow
        );

        FillCircle(
            generatedTexture,
            center + 27,
            C(43),
            2,
            secondaryGlow
        );

        // =====================================================
        // ARMOR HIGHLIGHTS
        // =====================================================

        DrawThickLine(
            generatedTexture,
            center - 7,
            C(89),
            center - 15,
            C(79),
            2,
            metalColor
        );

        DrawThickLine(
            generatedTexture,
            center + 7,
            C(89),
            center + 15,
            C(79),
            2,
            metalColor
        );

        generatedTexture.Apply();

        generatedSprite =
            Sprite.Create(
                generatedTexture,
                new Rect(
                    0,
                    0,
                    textureSize,
                    textureSize
                ),
                new Vector2(
                    0.5f,
                    0.5f
                ),
                pixelsPerUnit
            );

        generatedSprite.name =
            "ProceduralBasicEnemySprite";

        spriteRenderer.sprite =
            generatedSprite;

        spriteRenderer.sortingOrder =
            9;
    }

    private void Clear(Texture2D texture)
    {
        Color[] pixels =
            new Color[
                texture.width *
                texture.height
            ];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] =
                new Color(
                    0f,
                    0f,
                    0f,
                    0f
                );
        }

        texture.SetPixels(pixels);
    }

    private void FillCircle(
        Texture2D texture,
        float cx,
        float cy,
        float radius,
        Color color)
    {
        int minX =
            Mathf.FloorToInt(
                cx - radius
            );

        int maxX =
            Mathf.CeilToInt(
                cx + radius
            );

        int minY =
            Mathf.FloorToInt(
                cy - radius
            );

        int maxY =
            Mathf.CeilToInt(
                cy + radius
            );

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                float dx =
                    x - cx;

                float dy =
                    y - cy;

                if (dx * dx +
                    dy * dy <=
                    radius * radius)
                {
                    SetPixelSafe(
                        texture,
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    private void FillCircleRing(
        Texture2D texture,
        float cx,
        float cy,
        float radius,
        float thickness,
        Color color)
    {
        float outer =
            radius;

        float inner =
            radius - thickness;

        int minX =
            Mathf.FloorToInt(
                cx - outer
            );

        int maxX =
            Mathf.CeilToInt(
                cx + outer
            );

        int minY =
            Mathf.FloorToInt(
                cy - outer
            );

        int maxY =
            Mathf.CeilToInt(
                cy + outer
            );

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                float dx =
                    x - cx;

                float dy =
                    y - cy;

                float distance =
                    Mathf.Sqrt(
                        dx * dx +
                        dy * dy
                    );

                if (distance <= outer &&
                    distance >= inner)
                {
                    SetPixelSafe(
                        texture,
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    private void DrawThickLine(
        Texture2D texture,
        float x1,
        float y1,
        float x2,
        float y2,
        int thickness,
        Color color)
    {
        float dx =
            x2 - x1;

        float dy =
            y2 - y1;

        float distance =
            Mathf.Sqrt(
                dx * dx +
                dy * dy
            );

        int steps =
            Mathf.CeilToInt(
                distance
            );

        for (int i = 0; i <= steps; i++)
        {
            float t =
                steps == 0
                    ? 0f
                    : i / (float)steps;

            float x =
                Mathf.Lerp(
                    x1,
                    x2,
                    t
                );

            float y =
                Mathf.Lerp(
                    y1,
                    y2,
                    t
                );

            FillCircle(
                texture,
                x,
                y,
                thickness * 0.5f,
                color
            );
        }
    }

    private void FillTriangle(
        Texture2D texture,
        float x1,
        float y1,
        float x2,
        float y2,
        float x3,
        float y3,
        Color color)
    {
        int minX =
            Mathf.FloorToInt(
                Mathf.Min(
                    x1,
                    Mathf.Min(x2, x3)
                )
            );

        int maxX =
            Mathf.CeilToInt(
                Mathf.Max(
                    x1,
                    Mathf.Max(x2, x3)
                )
            );

        int minY =
            Mathf.FloorToInt(
                Mathf.Min(
                    y1,
                    Mathf.Min(y2, y3)
                )
            );

        int maxY =
            Mathf.CeilToInt(
                Mathf.Max(
                    y1,
                    Mathf.Max(y2, y3)
                )
            );

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (PointInTriangle(
                    x,
                    y,
                    x1,
                    y1,
                    x2,
                    y2,
                    x3,
                    y3))
                {
                    SetPixelSafe(
                        texture,
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    private bool PointInTriangle(
        float px,
        float py,
        float ax,
        float ay,
        float bx,
        float by,
        float cx,
        float cy)
    {
        float d1 =
            Sign(
                px,
                py,
                ax,
                ay,
                bx,
                by
            );

        float d2 =
            Sign(
                px,
                py,
                bx,
                by,
                cx,
                cy
            );

        float d3 =
            Sign(
                px,
                py,
                cx,
                cy,
                ax,
                ay
            );

        bool hasNegative =
            d1 < 0 ||
            d2 < 0 ||
            d3 < 0;

        bool hasPositive =
            d1 > 0 ||
            d2 > 0 ||
            d3 > 0;

        return !(hasNegative &&
                 hasPositive);
    }

    private float Sign(
        float px,
        float py,
        float ax,
        float ay,
        float bx,
        float by)
    {
        return
            (px - bx) *
            (ay - by) -
            (ax - bx) *
            (py - by);
    }

    private void SetPixelSafe(
        Texture2D texture,
        int x,
        int y,
        Color color)
    {
        if (x < 0 ||
            y < 0 ||
            x >= texture.width ||
            y >= texture.height)
            return;

        texture.SetPixel(
            x,
            y,
            color
        );
    }

    private void CleanupOldSprite()
    {
        if (generatedSprite != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(generatedSprite);
            else
                Destroy(generatedSprite);
#else
            Destroy(generatedSprite);
#endif

            generatedSprite = null;
        }

        if (generatedTexture != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(generatedTexture);
            else
                Destroy(generatedTexture);
#else
            Destroy(generatedTexture);
#endif

            generatedTexture = null;
        }
    }
}