using UnityEngine;

[ExecuteAlways]
public class ProceduralDroneSprite : MonoBehaviour
{
    [Header("Texture")]
    [SerializeField] private int textureSize = 128;
    [SerializeField] private float pixelsPerUnit = 64f;

    [Header("Position")]
    [SerializeField] private int verticalOffset = 2;

    [Header("Main Colors")]
    [SerializeField] private Color bodyColor =
        new Color(0.07f, 0.10f, 0.16f, 1f);

    [SerializeField] private Color bodyLight =
        new Color(0.20f, 0.27f, 0.36f, 1f);

    [SerializeField] private Color metalColor =
        new Color(0.38f, 0.46f, 0.58f, 1f);

    [SerializeField] private Color darkColor =
        new Color(0.025f, 0.035f, 0.06f, 1f);

    [Header("Neon")]
    [SerializeField] private Color accentColor =
        new Color(0.0f, 0.65f, 1f, 1f);

    [SerializeField] private Color glowColor =
        new Color(0.0f, 0.95f, 1f, 1f);

    [SerializeField] private Color secondaryGlow =
        new Color(0.65f, 0.15f, 1f, 1f);

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
            "ProceduralDroneTexture";

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
            C(113),
            center - 22,
            C(78),
            center - 27,
            C(44),
            darkColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(113),
            center + 22,
            C(78),
            center + 27,
            C(44),
            darkColor
        );

        // =====================================================
        // MAIN BODY
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(108),
            center - 17,
            C(75),
            center - 19,
            C(41),
            bodyColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(108),
            center + 17,
            C(75),
            center + 19,
            C(41),
            bodyColor
        );

        // =====================================================
        // CENTRAL ARMOR PLATE
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(103),
            center - 10,
            C(76),
            center - 11,
            C(51),
            bodyLight
        );

        FillTriangle(
            generatedTexture,
            center,
            C(103),
            center + 10,
            C(76),
            center + 11,
            C(51),
            bodyLight
        );

        // =====================================================
        // LEFT WING
        // =====================================================

        FillTriangle(
            generatedTexture,
            center - 6,
            C(78),
            center - 49,
            C(60),
            center - 29,
            C(38),
            metalColor
        );

        FillTriangle(
            generatedTexture,
            center - 10,
            C(69),
            center - 42,
            C(56),
            center - 25,
            C(47),
            bodyLight
        );

        // =====================================================
        // RIGHT WING
        // =====================================================

        FillTriangle(
            generatedTexture,
            center + 6,
            C(78),
            center + 49,
            C(60),
            center + 29,
            C(38),
            metalColor
        );

        FillTriangle(
            generatedTexture,
            center + 10,
            C(69),
            center + 42,
            C(56),
            center + 25,
            C(47),
            bodyLight
        );

        // =====================================================
        // WING DARK ARMOR
        // =====================================================

        FillTriangle(
            generatedTexture,
            center - 14,
            C(67),
            center - 38,
            C(56),
            center - 27,
            C(49),
            darkColor
        );

        FillTriangle(
            generatedTexture,
            center + 14,
            C(67),
            center + 38,
            C(56),
            center + 27,
            C(49),
            darkColor
        );

        // =====================================================
        // WING NEON STRIPS
        // =====================================================

        DrawThickLine(
            generatedTexture,
            center - 11,
            C(69),
            center - 37,
            C(55),
            2,
            accentColor
        );

        DrawThickLine(
            generatedTexture,
            center + 11,
            C(69),
            center + 37,
            C(55),
            2,
            accentColor
        );

        // =====================================================
        // SECONDARY WING DETAILS
        // =====================================================

        FillTriangle(
            generatedTexture,
            center - 25,
            C(59),
            center - 37,
            C(55),
            center - 28,
            C(51),
            secondaryGlow
        );

        FillTriangle(
            generatedTexture,
            center + 25,
            C(59),
            center + 37,
            C(55),
            center + 28,
            C(51),
            secondaryGlow
        );

        // =====================================================
        // CENTRAL CORE OUTER RING
        // =====================================================

        FillCircle(
            generatedTexture,
            center,
            C(66),
            15,
            darkColor
        );

        FillCircleRing(
            generatedTexture,
            center,
            C(66),
            13,
            3,
            metalColor
        );

        // =====================================================
        // CORE GLOW
        // =====================================================

        FillCircle(
            generatedTexture,
            center,
            C(66),
            9,
            accentColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(66),
            6,
            glowColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(66),
            3,
            Color.white
        );

        // =====================================================
        // CORE CROSS DETAILS
        // =====================================================

        DrawThickLine(
            generatedTexture,
            center - 11,
            C(66),
            center - 7,
            C(66),
            2,
            secondaryGlow
        );

        DrawThickLine(
            generatedTexture,
            center + 7,
            C(66),
            center + 11,
            C(66),
            2,
            secondaryGlow
        );

        DrawThickLine(
            generatedTexture,
            center,
            C(55),
            center,
            C(59),
            2,
            secondaryGlow
        );

        DrawThickLine(
            generatedTexture,
            center,
            C(73),
            center,
            C(77),
            2,
            secondaryGlow
        );

        // =====================================================
        // SIDE ARMOR PANELS
        // =====================================================

        FillTriangle(
            generatedTexture,
            center - 14,
            C(84),
            center - 23,
            C(77),
            center - 17,
            C(67),
            bodyLight
        );

        FillTriangle(
            generatedTexture,
            center + 14,
            C(84),
            center + 23,
            C(77),
            center + 17,
            C(67),
            bodyLight
        );

        // =====================================================
        // FRONT NOSE ARMOR
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(113),
            center - 6,
            C(94),
            center,
            C(88),
            metalColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(113),
            center + 6,
            C(94),
            center,
            C(88),
            metalColor
        );

        // =====================================================
        // NOSE NEON
        // =====================================================

        DrawThickLine(
            generatedTexture,
            center,
            C(108),
            center,
            C(94),
            2,
            glowColor
        );

        // =====================================================
        // LOWER BODY
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(50),
            center - 9,
            C(39),
            center - 8,
            C(27),
            darkColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(50),
            center + 9,
            C(39),
            center + 8,
            C(27),
            darkColor
        );

        // =====================================================
        // ENGINE NOZZLE
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
            accentColor
        );

        FillCircle(
            generatedTexture,
            center,
            C(25),
            3,
            glowColor
        );

        // =====================================================
        // ENGINE LOWER NOZZLE
        // =====================================================

        FillTriangle(
            generatedTexture,
            center,
            C(17),
            center - 6,
            C(24),
            center + 6,
            C(24),
            darkColor
        );

        FillTriangle(
            generatedTexture,
            center,
            C(16),
            center - 3,
            C(21),
            center + 3,
            C(21),
            secondaryGlow
        );

        // =====================================================
        // SMALL SIDE LIGHTS
        // =====================================================

        FillCircle(
            generatedTexture,
            center - 17,
            C(43),
            2,
            glowColor
        );

        FillCircle(
            generatedTexture,
            center + 17,
            C(43),
            2,
            glowColor
        );

        // =====================================================
        // SMALL ARMOR HIGHLIGHTS
        // =====================================================

        DrawThickLine(
            generatedTexture,
            center - 6,
            C(88),
            center - 15,
            C(79),
            2,
            metalColor
        );

        DrawThickLine(
            generatedTexture,
            center + 6,
            C(88),
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
            "ProceduralDroneSprite";

        spriteRenderer.sprite =
            generatedSprite;

        spriteRenderer.sortingOrder =
            10;
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
            Mathf.CeilToInt(distance);

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
                DestroyImmediate(
                    generatedSprite
                );
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
                DestroyImmediate(
                    generatedTexture
                );
            else
                Destroy(generatedTexture);
#else
            Destroy(generatedTexture);
#endif

            generatedTexture = null;
        }
    }
}