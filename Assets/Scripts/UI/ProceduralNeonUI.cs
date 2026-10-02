using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class ProceduralNeonUI : MonoBehaviour
{
    public enum ElementType
    {
        Panel,
        Button,
        ButtonSmall,
        JoystickBackground,
        JoystickHandle,
        BoostButton,
        BarBackground,
        BarFill,
        BossBarBackground,
        BossBarFill
    }

    [Header("Element")]
    [SerializeField] private ElementType elementType =
        ElementType.Button;

    [Header("Texture")]
    [SerializeField] private int textureWidth = 256;
    [SerializeField] private int textureHeight = 96;
    [SerializeField] private float pixelsPerUnit = 100f;

    [Header("Style")]
    [SerializeField] private Color baseColor =
        new Color(0.025f, 0.045f, 0.075f, 0.96f);

    [SerializeField] private Color borderColor =
        new Color(0f, 0.75f, 1f, 1f);

    [SerializeField] private Color glowColor =
        new Color(0f, 0.95f, 1f, 1f);

    [SerializeField] private Color secondaryColor =
        new Color(0.55f, 0.05f, 1f, 1f);

    [Header("Shape")]
    [SerializeField] private int cornerRadius = 14;
    [SerializeField] private int borderThickness = 3;

    [Header("Glow")]
    [SerializeField] private int glowSize = 5;

    private Image image;

    private Texture2D generatedTexture;
    private Sprite generatedSprite;

    private void Awake()
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

    public void Generate()
    {
        image = GetComponent<Image>();

        if (image == null)
            return;

        Cleanup();

        switch (elementType)
        {
            case ElementType.JoystickBackground:
                textureWidth = 256;
                textureHeight = 256;
                break;

            case ElementType.JoystickHandle:
                textureWidth = 128;
                textureHeight = 128;
                break;

            case ElementType.BoostButton:
                textureWidth = 180;
                textureHeight = 180;
                break;

            case ElementType.BarBackground:
            case ElementType.BarFill:
            case ElementType.BossBarBackground:
            case ElementType.BossBarFill:
                textureWidth = 256;
                textureHeight = 32;
                break;

            case ElementType.ButtonSmall:
                textureWidth = 180;
                textureHeight = 70;
                break;
        }

        generatedTexture =
            new Texture2D(
                textureWidth,
                textureHeight,
                TextureFormat.RGBA32,
                false
            );

        generatedTexture.filterMode =
            FilterMode.Bilinear;

        generatedTexture.wrapMode =
            TextureWrapMode.Clamp;

        Clear();

        switch (elementType)
        {
            case ElementType.Panel:
                DrawPanel();
                break;

            case ElementType.Button:
                DrawButton(false);
                break;

            case ElementType.ButtonSmall:
                DrawButton(true);
                break;

            case ElementType.JoystickBackground:
                DrawJoystickBackground();
                break;

            case ElementType.JoystickHandle:
                DrawJoystickHandle();
                break;

            case ElementType.BoostButton:
                DrawBoostButton();
                break;

            case ElementType.BarBackground:
                DrawBarBackground();
                break;

            case ElementType.BarFill:
                DrawBarFill();
                break;

            case ElementType.BossBarBackground:
                DrawBossBarBackground();
                break;

            case ElementType.BossBarFill:
                DrawBossBarFill();
                break;
        }

        generatedTexture.Apply();

        generatedSprite =
            Sprite.Create(
                generatedTexture,
                new Rect(
                    0,
                    0,
                    textureWidth,
                    textureHeight
                ),
                new Vector2(
                    0.5f,
                    0.5f
                ),
                pixelsPerUnit,
                0,
                SpriteMeshType.FullRect
            );

        generatedSprite.name =
            "ProceduralNeonUI";

        image.sprite =
            generatedSprite;

        image.type =
            Image.Type.Simple;

        image.preserveAspect = false;
    }

    // =========================================================
    // PANEL
    // =========================================================

    private void DrawPanel()
    {
        DrawRoundedRect(
            baseColor,
            borderColor,
            cornerRadius,
            borderThickness
        );

        DrawCornerAccents();
    }

    // =========================================================
    // BUTTON
    // =========================================================

    private void DrawButton(bool small)
    {
        DrawRoundedRect(
            baseColor,
            borderColor,
            small ? 10 : cornerRadius,
            borderThickness
        );

        DrawGlowLine(
            8,
            8,
            textureWidth - 9,
            8,
            glowColor,
            2
        );

        DrawCornerAccents();
    }

    // =========================================================
    // JOYSTICK
    // =========================================================

    private void DrawJoystickBackground()
    {
        int centerX =
            textureWidth / 2;

        int centerY =
            textureHeight / 2;

        int outerRadius =
            Mathf.Min(
                textureWidth,
                textureHeight
            ) / 2 - 5;

        DrawCircle(
            centerX,
            centerY,
            outerRadius,
            new Color(
                0.02f,
                0.05f,
                0.09f,
                0.86f
            )
        );

        DrawCircleRing(
            centerX,
            centerY,
            outerRadius - 3,
            4,
            borderColor
        );

        DrawCircleRing(
            centerX,
            centerY,
            outerRadius - 16,
            1,
            new Color(
                glowColor.r,
                glowColor.g,
                glowColor.b,
                0.25f
            )
        );

        DrawCircleRing(
            centerX,
            centerY,
            outerRadius - 30,
            1,
            new Color(
                secondaryColor.r,
                secondaryColor.g,
                secondaryColor.b,
                0.25f
            )
        );
    }

    private void DrawJoystickHandle()
    {
        int centerX =
            textureWidth / 2;

        int centerY =
            textureHeight / 2;

        int radius =
            textureWidth / 2 - 8;

        DrawCircle(
            centerX,
            centerY,
            radius,
            new Color(
                0.04f,
                0.10f,
                0.15f,
                0.98f
            )
        );

        DrawCircleRing(
            centerX,
            centerY,
            radius - 3,
            4,
            glowColor
        );

        DrawCircle(
            centerX,
            centerY,
            radius - 15,
            new Color(
                0.08f,
                0.20f,
                0.28f,
                1f
            )
        );

        DrawCircle(
            centerX,
            centerY,
            12,
            glowColor
        );
    }

    // =========================================================
    // BOOST
    // =========================================================

    private void DrawBoostButton()
    {
        int centerX =
            textureWidth / 2;

        int centerY =
            textureHeight / 2;

        int radius =
            textureWidth / 2 - 8;

        DrawCircle(
            centerX,
            centerY,
            radius,
            new Color(
                0.08f,
                0.025f,
                0.12f,
                0.95f
            )
        );

        DrawCircleRing(
            centerX,
            centerY,
            radius - 3,
            5,
            secondaryColor
        );

        DrawCircleRing(
            centerX,
            centerY,
            radius - 14,
            2,
            glowColor
        );

        // Центральный символ ускорения
        FillTriangle(
            centerX,
            centerY + 35,
            centerX - 22,
            centerY - 15,
            centerX,
            centerY - 5,
            glowColor
        );

        FillTriangle(
            centerX,
            centerY + 35,
            centerX + 22,
            centerY - 15,
            centerX,
            centerY - 5,
            secondaryColor
        );
    }

    // =========================================================
    // BARS
    // =========================================================

    private void DrawBarBackground()
    {
        DrawRoundedRect(
            new Color(
                0.015f,
                0.025f,
                0.045f,
                0.95f
            ),
            new Color(
                0.1f,
                0.35f,
                0.45f,
                1f
            ),
            8,
            2
        );
    }

    private void DrawBarFill()
    {
        DrawRoundedRect(
            glowColor,
            glowColor,
            8,
            2
        );

        DrawGlowLine(
            6,
            textureHeight - 7,
            textureWidth - 7,
            textureHeight - 7,
            Color.white,
            1
        );
    }

    private void DrawBossBarBackground()
    {
        DrawRoundedRect(
            new Color(
                0.025f,
                0.01f,
                0.035f,
                0.96f
            ),
            secondaryColor,
            8,
            3
        );
    }

    private void DrawBossBarFill()
    {
        DrawRoundedRect(
            new Color(
                0.95f,
                0.04f,
                0.18f,
                1f
            ),
            secondaryColor,
            8,
            2
        );

        DrawGlowLine(
            5,
            textureHeight - 6,
            textureWidth - 6,
            textureHeight - 6,
            Color.white,
            1
        );
    }

    // =========================================================
    // ROUNDED RECT
    // =========================================================

    private void DrawRoundedRect(
        Color fill,
        Color border,
        int radius,
        int thickness)
    {
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                float distance =
                    RoundedBoxDistance(
                        x,
                        y,
                        textureWidth,
                        textureHeight,
                        radius
                    );

                if (distance <= 0f)
                {
                    generatedTexture.SetPixel(
                        x,
                        y,
                        fill
                    );
                }
                else if (distance <= thickness)
                {
                    generatedTexture.SetPixel(
                        x,
                        y,
                        border
                    );
                }
            }
        }
    }

    private float RoundedBoxDistance(
        float x,
        float y,
        float width,
        float height,
        float radius)
    {
        float cx =
            Mathf.Clamp(
                x,
                radius,
                width - radius
            );

        float cy =
            Mathf.Clamp(
                y,
                radius,
                height - radius
            );

        float dx =
            x - cx;

        float dy =
            y - cy;

        float distance =
            Mathf.Sqrt(
                dx * dx +
                dy * dy
            );

        if (x >= radius &&
            x <= width - radius)
        {
            if (y >= radius &&
                y <= height - radius)
            {
                return -1f;
            }
        }

        if (y >= radius &&
            y <= height - radius)
        {
            return distance -
                   radius;
        }

        if (x >= radius &&
            x <= width - radius)
        {
            return distance -
                   radius;
        }

        return distance -
               radius;
    }

    // =========================================================
    // CIRCLES
    // =========================================================

    private void DrawCircle(
        int cx,
        int cy,
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
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    private void DrawCircleRing(
        int cx,
        int cy,
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
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    // =========================================================
    // DETAILS
    // =========================================================

    private void DrawCornerAccents()
    {
        Color color =
            glowColor;

        DrawGlowLine(
            8,
            8,
            30,
            8,
            color,
            3
        );

        DrawGlowLine(
            textureWidth - 31,
            textureHeight - 8,
            textureWidth - 9,
            textureHeight - 8,
            secondaryColor,
            3
        );
    }

    private void DrawGlowLine(
        int x1,
        int y1,
        int x2,
        int y2,
        Color color,
        int thickness)
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

            DrawCircle(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(y),
                thickness * 0.5f,
                color
            );
        }
    }

    private void FillTriangle(
        int x1,
        int y1,
        int x2,
        int y2,
        int x3,
        int y3,
        Color color)
    {
        int minX =
            Mathf.Min(
                x1,
                Mathf.Min(x2, x3)
            );

        int maxX =
            Mathf.Max(
                x1,
                Mathf.Max(x2, x3)
            );

        int minY =
            Mathf.Min(
                y1,
                Mathf.Min(y2, y3)
            );

        int maxY =
            Mathf.Max(
                y1,
                Mathf.Max(y2, y3)
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
        int x,
        int y,
        Color color)
    {
        if (x < 0 ||
            y < 0 ||
            x >= textureWidth ||
            y >= textureHeight)
            return;

        generatedTexture.SetPixel(
            x,
            y,
            color
        );
    }

    private void Clear()
    {
        Color[] pixels =
            new Color[
                textureWidth *
                textureHeight
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

        generatedTexture.SetPixels(
            pixels
        );
    }

    private void Cleanup()
    {
        if (generatedSprite != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(
                    generatedSprite
                );
            else
                Destroy(
                    generatedSprite
                );
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
                Destroy(
                    generatedTexture
                );
#else
            Destroy(generatedTexture);
#endif

            generatedTexture = null;
        }
    }
}