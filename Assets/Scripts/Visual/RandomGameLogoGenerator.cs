using System;
using UnityEngine;
using UnityEngine.UI;

public class RandomGameLogoGenerator : MonoBehaviour
{
    private enum FighterStyle
    {
        Interceptor,
        Phantom,
        Striker,
        Spear,
        HeavyWing,
        SolanaViper
    }

    private enum BackgroundStyle
    {
        DeepSpace,
        Nebula,
        PlanetOrbit,
        CyberGrid,
        SolarStorm
    }

    [Header("Target")]
    [SerializeField] private RawImage targetImage;

    [Header("Texture")]
    [SerializeField] private int textureWidth = 1024;
    [SerializeField] private int textureHeight = 1024;
    [SerializeField] private bool generateOnEnable = true;

    [Header("Generation")]
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private int seed = 12345;

    [Header("Regeneration")]
    [SerializeField] private bool regenerateOnApplicationFocus = false;

    [Header("Colors")]
    [SerializeField] private Color spaceColor = new Color(0.004f, 0.008f, 0.025f, 1f);
    [SerializeField] private Color solanaGreen = new Color(0.10f, 0.95f, 0.88f, 1f);
    [SerializeField] private Color solanaPurple = new Color(0.65f, 0.30f, 1f, 1f);
    [SerializeField] private Color electricBlue = new Color(0.12f, 0.42f, 1f, 1f);
    [SerializeField] private Color solarOrange = new Color(1f, 0.30f, 0.08f, 1f);
    [SerializeField] private Color whiteBlue = new Color(0.78f, 0.94f, 1f, 1f);

    private Texture2D texture;
    private System.Random random;

    private FighterStyle fighterStyle;
    private BackgroundStyle backgroundStyle;

    private Color fighterMainColor;
    private Color fighterSecondaryColor;
    private Color fighterGlowColor;

    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        if (generateOnEnable)
            Generate();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (
            hasFocus &&
            Application.isPlaying &&
            regenerateOnApplicationFocus
        )
        {
            Generate();
        }
    }

    public void Generate()
    {
        if (randomizeSeed)
            seed = Guid.NewGuid().GetHashCode();

        random = new System.Random(seed);

        fighterStyle = (FighterStyle)random.Next(
            0,
            Enum.GetValues(typeof(FighterStyle)).Length
        );

        backgroundStyle = (BackgroundStyle)random.Next(
            0,
            Enum.GetValues(typeof(BackgroundStyle)).Length
        );

        SelectFighterColors();

        DestroyTexture();

        texture = new Texture2D(
            textureWidth,
            textureHeight,
            TextureFormat.RGBA32,
            false
        );

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        DrawBackground();
        DrawStars(random.Next(100, 240));
        DrawDistantObjects();

        float centerX = textureWidth * 0.5f;
        float centerY = textureHeight * RandomFloat(0.47f, 0.56f);

        DrawAtmosphericEffects(centerX, centerY);
        DrawFighter(centerX, centerY);
        DrawHudOverlay();
        DrawFrame();

        texture.Apply();

        if (targetImage != null)
            targetImage.texture = texture;
    }

    // =========================================================
    // RANDOM
    // =========================================================

    private float RandomFloat(float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    private bool RandomBool()
    {
        return random.Next(0, 2) == 0;
    }

    private Color RandomNeonColor(float alpha = 1f)
    {
        Color color;

        switch (random.Next(0, 5))
        {
            case 0:
                color = solanaGreen;
                break;

            case 1:
                color = solanaPurple;
                break;

            case 2:
                color = electricBlue;
                break;

            case 3:
                color = solarOrange;
                break;

            default:
                color = whiteBlue;
                break;
        }

        color.a = alpha;
        return color;
    }

    private void SelectFighterColors()
    {
        switch (random.Next(0, 5))
        {
            case 0:
                fighterMainColor = new Color(0.025f, 0.06f, 0.14f, 1f);
                fighterSecondaryColor = solanaGreen;
                fighterGlowColor = solanaGreen;
                break;

            case 1:
                fighterMainColor = new Color(0.08f, 0.025f, 0.16f, 1f);
                fighterSecondaryColor = solanaPurple;
                fighterGlowColor = electricBlue;
                break;

            case 2:
                fighterMainColor = new Color(0.025f, 0.065f, 0.10f, 1f);
                fighterSecondaryColor = electricBlue;
                fighterGlowColor = solanaGreen;
                break;

            case 3:
                fighterMainColor = new Color(0.14f, 0.035f, 0.015f, 1f);
                fighterSecondaryColor = solarOrange;
                fighterGlowColor = solarOrange;
                break;

            default:
                fighterMainColor = new Color(0.10f, 0.12f, 0.18f, 1f);
                fighterSecondaryColor = whiteBlue;
                fighterGlowColor = solanaPurple;
                break;
        }
    }

    // =========================================================
    // BACKGROUND
    // =========================================================

    private void DrawBackground()
    {
        Color[] pixels = new Color[textureWidth * textureHeight];

        Color topColor = Color.Lerp(
            spaceColor,
            RandomNeonColor(),
            RandomFloat(0.03f, 0.13f)
        );

        Color bottomColor = Color.Lerp(
            spaceColor * RandomFloat(0.45f, 0.85f),
            RandomNeonColor(),
            RandomFloat(0.02f, 0.10f)
        );

        for (int y = 0; y < textureHeight; y++)
        {
            float t = y / (float)(textureHeight - 1);
            Color gradient = Color.Lerp(bottomColor, topColor, t);

            for (int x = 0; x < textureWidth; x++)
            {
                float noise = RandomFloat(-0.008f, 0.008f);

                pixels[y * textureWidth + x] = new Color(
                    Mathf.Clamp01(gradient.r + noise),
                    Mathf.Clamp01(gradient.g + noise),
                    Mathf.Clamp01(gradient.b + noise),
                    1f
                );
            }
        }

        texture.SetPixels(pixels);

        int glowCount = random.Next(3, 8);

        for (int i = 0; i < glowCount; i++)
        {
            AddGlow(
                RandomFloat(0f, textureWidth),
                RandomFloat(0f, textureHeight),
                RandomFloat(textureWidth * 0.12f, textureWidth * 0.45f),
                RandomFloat(textureHeight * 0.12f, textureHeight * 0.45f),
                RandomNeonColor(RandomFloat(0.035f, 0.12f))
            );
        }
    }

    private void AddGlow(
        float centerX,
        float centerY,
        float radiusX,
        float radiusY,
        Color glow
    )
    {
        int minX = Mathf.Max(
            0,
            Mathf.FloorToInt(centerX - radiusX)
        );

        int maxX = Mathf.Min(
            textureWidth - 1,
            Mathf.CeilToInt(centerX + radiusX)
        );

        int minY = Mathf.Max(
            0,
            Mathf.FloorToInt(centerY - radiusY)
        );

        int maxY = Mathf.Min(
            textureHeight - 1,
            Mathf.CeilToInt(centerY + radiusY)
        );

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = (x - centerX) / radiusX;
                float dy = (y - centerY) / radiusY;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                if (distance > 1f)
                    continue;

                float alpha = Mathf.Pow(1f - distance, 2f) * glow.a;

                Color oldColor = texture.GetPixel(x, y);

                Color result = Color.Lerp(
                    oldColor,
                    new Color(glow.r, glow.g, glow.b, 1f),
                    alpha
                );

                result.a = 1f;
                texture.SetPixel(x, y, result);
            }
        }
    }

    // =========================================================
    // SPACE
    // =========================================================

    private void DrawStars(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int x = random.Next(0, textureWidth);
            int y = random.Next(0, textureHeight);
            int radius = random.Next(1, 5);

            Color color = RandomNeonColor(
                RandomFloat(0.25f, 0.9f)
            );

            DrawCircle(x, y, radius, color);

            if (random.Next(0, 7) == 0)
            {
                DrawLine(
                    x - radius * 3,
                    y,
                    x + radius * 3,
                    y,
                    color
                );

                DrawLine(
                    x,
                    y - radius * 3,
                    x,
                    y + radius * 3,
                    color
                );
            }
        }
    }

    private void DrawDistantObjects()
    {
        switch (backgroundStyle)
        {
            case BackgroundStyle.DeepSpace:
                DrawDeepSpaceObjects();
                break;

            case BackgroundStyle.Nebula:
                DrawNebula();
                break;

            case BackgroundStyle.PlanetOrbit:
                DrawPlanet();
                break;

            case BackgroundStyle.CyberGrid:
                DrawSpaceGrid();
                break;

            case BackgroundStyle.SolarStorm:
                DrawSolarStorm();
                break;
        }
    }

    private void DrawDeepSpaceObjects()
    {
        int count = random.Next(3, 8);

        for (int i = 0; i < count; i++)
        {
            int x = random.Next(50, textureWidth - 50);
            int y = random.Next(50, textureHeight - 50);
            int radius = random.Next(10, 36);

            DrawCircle(
                x,
                y,
                radius,
                new Color(
                    RandomFloat(0.01f, 0.07f),
                    RandomFloat(0.03f, 0.12f),
                    RandomFloat(0.08f, 0.22f),
                    1f
                )
            );

            DrawCircle(
                x - radius / 3,
                y + radius / 4,
                Mathf.Max(2, radius / 7),
                RandomNeonColor(0.7f)
            );
        }
    }

    private void DrawNebula()
    {
        for (int i = 0; i < random.Next(4, 10); i++)
        {
            AddGlow(
                RandomFloat(0, textureWidth),
                RandomFloat(0, textureHeight),
                RandomFloat(120f, 430f),
                RandomFloat(70f, 260f),
                RandomNeonColor(RandomFloat(0.035f, 0.10f))
            );
        }

        for (int i = 0; i < 12; i++)
        {
            float x1 = RandomFloat(0, textureWidth);
            float y1 = RandomFloat(0, textureHeight);

            DrawThickLine(
                new Vector2(x1, y1),
                new Vector2(
                    x1 + RandomFloat(-260f, 260f),
                    y1 + RandomFloat(-100f, 100f)
                ),
                random.Next(2, 7),
                RandomNeonColor(0.08f)
            );
        }
    }

    private void DrawPlanet()
    {
        int planetX = random.Next(
            textureWidth / 7,
            textureWidth - textureWidth / 7
        );

        int planetY = random.Next(
            textureHeight / 8,
            textureHeight / 2
        );

        int radius = random.Next(
            textureWidth / 10,
            textureWidth / 4
        );

        Color planetColor = RandomNeonColor(0.75f);

        DrawCircle(
            planetX,
            planetY,
            radius + 25,
            new Color(
                planetColor.r,
                planetColor.g,
                planetColor.b,
                0.08f
            )
        );

        DrawCircle(
            planetX,
            planetY,
            radius,
            new Color(
                planetColor.r * 0.25f,
                planetColor.g * 0.25f,
                planetColor.b * 0.35f,
                1f
            )
        );

        for (int i = 0; i < 25; i++)
        {
            int x = planetX + random.Next(-radius, radius);
            int y = planetY + random.Next(-radius, radius);

            if (
                Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(planetX, planetY)
                ) <= radius
            )
            {
                DrawCircle(
                    x,
                    y,
                    random.Next(2, 11),
                    new Color(
                        planetColor.r,
                        planetColor.g,
                        planetColor.b,
                        RandomFloat(0.1f, 0.35f)
                    )
                );
            }
        }

        DrawOrbit(
            planetX,
            planetY,
            radius + random.Next(30, 85),
            random.Next(12, 28),
            new Color(
                planetColor.r,
                planetColor.g,
                planetColor.b,
                0.3f
            )
        );
    }

    private void DrawSpaceGrid()
    {
        int horizon = Mathf.RoundToInt(textureHeight * 0.82f);

        for (int i = -14; i <= 14; i++)
        {
            DrawLine(
                textureWidth / 2,
                horizon,
                textureWidth / 2 + i * 100,
                textureHeight,
                new Color(
                    solanaGreen.r,
                    solanaGreen.g,
                    solanaGreen.b,
                    0.12f
                )
            );
        }

        for (int i = 0; i < 10; i++)
        {
            float t = i / 10f;

            int y = Mathf.RoundToInt(
                Mathf.Lerp(horizon, textureHeight, t * t)
            );

            DrawLine(
                0,
                y,
                textureWidth,
                y,
                new Color(
                    solanaPurple.r,
                    solanaPurple.g,
                    solanaPurple.b,
                    0.12f
                )
            );
        }
    }

    private void DrawSolarStorm()
    {
        for (int i = 0; i < random.Next(15, 30); i++)
        {
            float angle = RandomFloat(-1.2f, 1.2f);

            float startX = RandomFloat(0, textureWidth);
            float startY = RandomFloat(0, textureHeight * 0.5f);
            float length = RandomFloat(100f, 360f);

            DrawThickLine(
                new Vector2(startX, startY),
                new Vector2(
                    startX + Mathf.Cos(angle) * length,
                    startY + Mathf.Sin(angle) * length
                ),
                random.Next(1, 5),
                new Color(
                    solarOrange.r,
                    solarOrange.g,
                    solarOrange.b,
                    RandomFloat(0.08f, 0.3f)
                )
            );
        }
    }

    // =========================================================
    // FIGHTER
    // =========================================================

    private void DrawFighter(float cx, float cy)
    {
        float scale = RandomFloat(0.82f, 1.05f);

        switch (fighterStyle)
        {
            case FighterStyle.Interceptor:
                DrawDetailedFighter(cx, cy, scale, 0);
                break;

            case FighterStyle.Phantom:
                DrawDetailedFighter(cx, cy, scale, 1);
                break;

            case FighterStyle.Striker:
                DrawDetailedFighter(cx, cy, scale, 2);
                break;

            case FighterStyle.Spear:
                DrawDetailedFighter(cx, cy, scale, 3);
                break;

            case FighterStyle.HeavyWing:
                DrawDetailedFighter(cx, cy, scale, 4);
                break;

            case FighterStyle.SolanaViper:
                DrawDetailedFighter(cx, cy, scale, 5);
                break;
        }
    }

    private void DrawDetailedFighter(
        float cx,
        float cy,
        float scale,
        int variant
    )
    {
        float length;
        float bodyWidth;
        float wingWidth;

        switch (variant)
        {
            case 1:
                length = 560f * scale;
                bodyWidth = 86f * scale;
                wingWidth = 265f * scale;
                break;

            case 2:
                length = 500f * scale;
                bodyWidth = 120f * scale;
                wingWidth = 300f * scale;
                break;

            case 3:
                length = 610f * scale;
                bodyWidth = 72f * scale;
                wingWidth = 190f * scale;
                break;

            case 4:
                length = 470f * scale;
                bodyWidth = 145f * scale;
                wingWidth = 330f * scale;
                break;

            case 5:
                length = 540f * scale;
                bodyWidth = 100f * scale;
                wingWidth = 250f * scale;
                break;

            default:
                length = 530f * scale;
                bodyWidth = 100f * scale;
                wingWidth = 235f * scale;
                break;
        }

        float noseX = cx + length * 0.58f;
        float tailX = cx - length * 0.48f;

        DrawGlowLine(
            new Vector2(tailX - 45f * scale, cy),
            new Vector2(noseX + 25f * scale, cy),
            60f * scale,
            fighterGlowColor
        );

        // -----------------------------------------------------
        // Тёмная внешняя тень корпуса
        // -----------------------------------------------------

        Vector2[] outerBody =
        {
            new Vector2(tailX - 20f * scale, cy),

            new Vector2(cx - length * 0.30f, cy - bodyWidth * 0.72f),
            new Vector2(cx + length * 0.20f, cy - bodyWidth * 0.55f),

            new Vector2(noseX + 18f * scale, cy),

            new Vector2(cx + length * 0.20f, cy + bodyWidth * 0.55f),
            new Vector2(cx - length * 0.30f, cy + bodyWidth * 0.72f)
        };

        DrawPolygonFilled(
            outerBody,
            new Color(0.003f, 0.008f, 0.025f, 1f)
        );

        // -----------------------------------------------------
        // Основной корпус
        // -----------------------------------------------------

        Vector2[] body =
        {
            new Vector2(tailX, cy),

            new Vector2(cx - length * 0.30f, cy - bodyWidth * 0.48f),
            new Vector2(cx + length * 0.15f, cy - bodyWidth * 0.40f),
            new Vector2(cx + length * 0.42f, cy - bodyWidth * 0.22f),

            new Vector2(noseX, cy),

            new Vector2(cx + length * 0.42f, cy + bodyWidth * 0.22f),
            new Vector2(cx + length * 0.15f, cy + bodyWidth * 0.40f),
            new Vector2(cx - length * 0.30f, cy + bodyWidth * 0.48f)
        };

        DrawPolygonFilled(body, fighterMainColor);

        DrawPolygonOutline(
            body,
            new Color(
                fighterSecondaryColor.r,
                fighterSecondaryColor.g,
                fighterSecondaryColor.b,
                0.95f
            ),
            Mathf.Max(3, Mathf.RoundToInt(5f * scale))
        );

        // -----------------------------------------------------
        // Верхнее крыло
        // -----------------------------------------------------

        Vector2[] topWing =
        {
            new Vector2(cx - length * 0.22f, cy - bodyWidth * 0.08f),
            new Vector2(cx - length * 0.01f, cy - bodyWidth * 0.38f),
            new Vector2(cx + length * 0.30f, cy - wingWidth * 0.86f),
            new Vector2(cx + length * 0.43f, cy - wingWidth),
            new Vector2(cx + length * 0.35f, cy - bodyWidth * 0.18f),
            new Vector2(cx + length * 0.10f, cy + bodyWidth * 0.04f)
        };

        DrawPolygonFilled(topWing, fighterMainColor);

        DrawPolygonOutline(
            topWing,
            fighterSecondaryColor,
            Mathf.Max(3, Mathf.RoundToInt(5f * scale))
        );

        // -----------------------------------------------------
        // Нижнее крыло
        // -----------------------------------------------------

        Vector2[] bottomWing =
        {
            new Vector2(cx - length * 0.22f, cy + bodyWidth * 0.08f),
            new Vector2(cx - length * 0.01f, cy + bodyWidth * 0.38f),
            new Vector2(cx + length * 0.30f, cy + wingWidth * 0.86f),
            new Vector2(cx + length * 0.43f, cy + wingWidth),
            new Vector2(cx + length * 0.35f, cy + bodyWidth * 0.18f),
            new Vector2(cx + length * 0.10f, cy - bodyWidth * 0.04f)
        };

        DrawPolygonFilled(bottomWing, fighterMainColor);

        DrawPolygonOutline(
            bottomWing,
            fighterSecondaryColor,
            Mathf.Max(3, Mathf.RoundToInt(5f * scale))
        );

        // -----------------------------------------------------
        // Хвостовые стабилизаторы
        // -----------------------------------------------------

        Vector2[] topTail =
        {
            new Vector2(cx - length * 0.33f, cy - bodyWidth * 0.22f),
            new Vector2(cx - length * 0.54f, cy - bodyWidth * 0.82f),
            new Vector2(cx - length * 0.39f, cy - bodyWidth * 0.10f)
        };

        Vector2[] bottomTail =
        {
            new Vector2(cx - length * 0.33f, cy + bodyWidth * 0.22f),
            new Vector2(cx - length * 0.54f, cy + bodyWidth * 0.82f),
            new Vector2(cx - length * 0.39f, cy + bodyWidth * 0.10f)
        };

        DrawPolygonFilled(topTail, fighterSecondaryColor);
        DrawPolygonFilled(bottomTail, fighterSecondaryColor);

        DrawPolygonOutline(
            topTail,
            whiteBlue,
            Mathf.Max(2, Mathf.RoundToInt(3f * scale))
        );

        DrawPolygonOutline(
            bottomTail,
            whiteBlue,
            Mathf.Max(2, Mathf.RoundToInt(3f * scale))
        );

        // -----------------------------------------------------
        // Верхняя бронепластина
        // -----------------------------------------------------

        Vector2[] topArmor =
        {
            new Vector2(cx - length * 0.28f, cy - bodyWidth * 0.44f),
            new Vector2(cx - length * 0.02f, cy - bodyWidth * 0.69f),
            new Vector2(cx + length * 0.25f, cy - bodyWidth * 0.34f),
            new Vector2(cx + length * 0.10f, cy - bodyWidth * 0.12f),
            new Vector2(cx - length * 0.20f, cy - bodyWidth * 0.20f)
        };

        DrawPolygonFilled(
            topArmor,
            new Color(
                fighterSecondaryColor.r,
                fighterSecondaryColor.g,
                fighterSecondaryColor.b,
                0.42f
            )
        );

        DrawPolygonOutline(
            topArmor,
            fighterSecondaryColor,
            Mathf.Max(2, Mathf.RoundToInt(3f * scale))
        );

        // -----------------------------------------------------
        // Нижняя бронепластина
        // -----------------------------------------------------

        Vector2[] bottomArmor =
        {
            new Vector2(cx - length * 0.28f, cy + bodyWidth * 0.44f),
            new Vector2(cx - length * 0.02f, cy + bodyWidth * 0.69f),
            new Vector2(cx + length * 0.25f, cy + bodyWidth * 0.34f),
            new Vector2(cx + length * 0.10f, cy + bodyWidth * 0.12f),
            new Vector2(cx - length * 0.20f, cy + bodyWidth * 0.20f)
        };

        DrawPolygonFilled(
            bottomArmor,
            new Color(0.012f, 0.025f, 0.07f, 1f)
        );

        DrawPolygonOutline(
            bottomArmor,
            fighterGlowColor,
            Mathf.Max(2, Mathf.RoundToInt(3f * scale))
        );

        // -----------------------------------------------------
        // Кабина пилота
        // -----------------------------------------------------

        Vector2[] cockpit =
        {
            new Vector2(cx + length * 0.01f, cy - bodyWidth * 0.22f),
            new Vector2(cx + length * 0.17f, cy - bodyWidth * 0.31f),
            new Vector2(cx + length * 0.35f, cy - bodyWidth * 0.10f),
            new Vector2(cx + length * 0.35f, cy + bodyWidth * 0.10f),
            new Vector2(cx + length * 0.17f, cy + bodyWidth * 0.31f),
            new Vector2(cx + length * 0.01f, cy + bodyWidth * 0.22f)
        };

        DrawPolygonFilled(
            cockpit,
            new Color(
                fighterGlowColor.r * 0.18f,
                fighterGlowColor.g * 0.18f,
                fighterGlowColor.b * 0.24f,
                1f
            )
        );

        DrawPolygonOutline(
            cockpit,
            fighterGlowColor,
            Mathf.Max(3, Mathf.RoundToInt(5f * scale))
        );

        DrawThickLine(
            new Vector2(
                cx + length * 0.10f,
                cy - bodyWidth * 0.19f
            ),
            new Vector2(
                cx + length * 0.10f,
                cy + bodyWidth * 0.19f
            ),
            Mathf.Max(2, Mathf.RoundToInt(3f * scale)),
            whiteBlue
        );

        DrawLine(
            Mathf.RoundToInt(cx + length * 0.11f),
            Mathf.RoundToInt(cy),
            Mathf.RoundToInt(cx + length * 0.30f),
            Mathf.RoundToInt(cy),
            fighterGlowColor
        );

        // -----------------------------------------------------
        // Носовая броня
        // -----------------------------------------------------

        DrawThickLine(
            new Vector2(
                cx + length * 0.34f,
                cy - bodyWidth * 0.20f
            ),
            new Vector2(noseX, cy),
            Mathf.Max(2, Mathf.RoundToInt(4f * scale)),
            whiteBlue
        );

        DrawThickLine(
            new Vector2(
                cx + length * 0.34f,
                cy + bodyWidth * 0.20f
            ),
            new Vector2(noseX, cy),
            Mathf.Max(2, Mathf.RoundToInt(4f * scale)),
            whiteBlue
        );

        // -----------------------------------------------------
        // Двигатели
        // -----------------------------------------------------

        DrawDetailedEngine(
            cx - length * 0.46f,
            cy - bodyWidth * 0.27f,
            34f * scale
        );

        DrawDetailedEngine(
            cx - length * 0.46f,
            cy + bodyWidth * 0.27f,
            34f * scale
        );

        DrawDetailedEngine(
            cx - length * 0.34f,
            cy,
            27f * scale
        );

        // -----------------------------------------------------
        // Solana-эмблема
        // -----------------------------------------------------

        DrawSolanaEmblem(
            cx - length * 0.14f,
            cy,
            55f * scale
        );

        // -----------------------------------------------------
        // Панели
        // -----------------------------------------------------

        DrawPanelLine(
            new Vector2(
                cx - length * 0.28f,
                cy - bodyWidth * 0.12f
            ),
            new Vector2(
                cx - length * 0.02f,
                cy - bodyWidth * 0.15f
            ),
            fighterGlowColor,
            scale
        );

        DrawPanelLine(
            new Vector2(
                cx - length * 0.28f,
                cy + bodyWidth * 0.12f
            ),
            new Vector2(
                cx - length * 0.02f,
                cy + bodyWidth * 0.15f
            ),
            fighterGlowColor,
            scale
        );

        DrawPanelLine(
            new Vector2(
                cx + length * 0.30f,
                cy - bodyWidth * 0.13f
            ),
            new Vector2(
                cx + length * 0.43f,
                cy - bodyWidth * 0.06f
            ),
            whiteBlue,
            scale
        );

        DrawPanelLine(
            new Vector2(
                cx + length * 0.30f,
                cy + bodyWidth * 0.13f
            ),
            new Vector2(
                cx + length * 0.43f,
                cy + bodyWidth * 0.06f
            ),
            whiteBlue,
            scale
        );

        // -----------------------------------------------------
        // Оружие
        // -----------------------------------------------------

        DrawWingWeapon(
            cx + length * 0.12f,
            cy - wingWidth * 0.65f,
            scale,
            true
        );

        DrawWingWeapon(
            cx + length * 0.12f,
            cy + wingWidth * 0.65f,
            scale,
            false
        );

        DrawThickLine(
            new Vector2(
                cx + length * 0.31f,
                cy - wingWidth * 0.82f
            ),
            new Vector2(
                cx + length * 0.50f,
                cy - wingWidth * 0.94f
            ),
            Mathf.Max(2, Mathf.RoundToInt(4f * scale)),
            solarOrange
        );

        DrawThickLine(
            new Vector2(
                cx + length * 0.31f,
                cy + wingWidth * 0.82f
            ),
            new Vector2(
                cx + length * 0.50f,
                cy + wingWidth * 0.94f
            ),
            Mathf.Max(2, Mathf.RoundToInt(4f * scale)),
            solarOrange
        );
    }

    // =========================================================
    // FIGHTER DETAILS
    // =========================================================

    private void DrawDetailedEngine(
        float cx,
        float cy,
        float size
    )
    {
        DrawCircle(
            Mathf.RoundToInt(cx),
            Mathf.RoundToInt(cy),
            Mathf.RoundToInt(size * 1.4f),
            new Color(
                fighterGlowColor.r,
                fighterGlowColor.g,
                fighterGlowColor.b,
                0.08f
            )
        );

        DrawCircle(
            Mathf.RoundToInt(cx),
            Mathf.RoundToInt(cy),
            Mathf.RoundToInt(size),
            new Color(0.008f, 0.018f, 0.055f, 1f)
        );

        DrawCircle(
            Mathf.RoundToInt(cx),
            Mathf.RoundToInt(cy),
            Mathf.RoundToInt(size * 0.68f),
            fighterSecondaryColor
        );

        DrawCircle(
            Mathf.RoundToInt(cx),
            Mathf.RoundToInt(cy),
            Mathf.RoundToInt(size * 0.37f),
            fighterGlowColor
        );

        DrawThickLine(
            new Vector2(cx - size * 0.35f, cy),
            new Vector2(
                cx - size * RandomFloat(1.8f, 3.2f),
                cy
            ),
            Mathf.Max(3, Mathf.RoundToInt(size * 0.35f)),
            new Color(
                fighterGlowColor.r,
                fighterGlowColor.g,
                fighterGlowColor.b,
                0.55f
            )
        );

        DrawLine(
            Mathf.RoundToInt(cx - size * 0.8f),
            Mathf.RoundToInt(cy - size * 0.55f),
            Mathf.RoundToInt(cx + size * 0.4f),
            Mathf.RoundToInt(cy - size * 0.55f),
            whiteBlue
        );

        DrawLine(
            Mathf.RoundToInt(cx - size * 0.8f),
            Mathf.RoundToInt(cy + size * 0.55f),
            Mathf.RoundToInt(cx + size * 0.4f),
            Mathf.RoundToInt(cy + size * 0.55f),
            whiteBlue
        );
    }

    private void DrawWingWeapon(
        float cx,
        float cy,
        float scale,
        bool upper
    )
    {
        float length = 64f * scale;
        float width = 12f * scale;

        Color weaponColor = upper
            ? solanaGreen
            : solanaPurple;

        DrawThickLine(
            new Vector2(cx - length, cy),
            new Vector2(cx + length, cy),
            Mathf.Max(3, Mathf.RoundToInt(width)),
            new Color(0.01f, 0.025f, 0.07f, 1f)
        );

        DrawLine(
            Mathf.RoundToInt(cx - length),
            Mathf.RoundToInt(cy - width * 0.45f),
            Mathf.RoundToInt(cx + length),
            Mathf.RoundToInt(cy - width * 0.45f),
            weaponColor
        );

        DrawLine(
            Mathf.RoundToInt(cx - length),
            Mathf.RoundToInt(cy + width * 0.45f),
            Mathf.RoundToInt(cx + length),
            Mathf.RoundToInt(cy + width * 0.45f),
            weaponColor
        );

        DrawCircle(
            Mathf.RoundToInt(cx + length),
            Mathf.RoundToInt(cy),
            Mathf.Max(3, Mathf.RoundToInt(width * 0.7f)),
            solarOrange
        );

        DrawThickLine(
            new Vector2(cx + length * 0.55f, cy),
            new Vector2(cx + length * 1.45f, cy),
            Mathf.Max(2, Mathf.RoundToInt(3f * scale)),
            solarOrange
        );
    }

    private void DrawPanelLine(
        Vector2 start,
        Vector2 end,
        Color color,
        float scale
    )
    {
        DrawThickLine(
            start,
            end,
            Mathf.Max(2, Mathf.RoundToInt(3f * scale)),
            new Color(
                color.r,
                color.g,
                color.b,
                0.7f
            )
        );

        DrawCircle(
            Mathf.RoundToInt(start.x),
            Mathf.RoundToInt(start.y),
            Mathf.Max(1, Mathf.RoundToInt(3f * scale)),
            color
        );
    }

    private void DrawSolanaEmblem(
        float cx,
        float cy,
        float size
    )
    {
        float thickness = Mathf.Max(3f, size * 0.12f);

        DrawThickLine(
            new Vector2(
                cx - size * 0.48f,
                cy + size * 0.3f
            ),
            new Vector2(
                cx + size * 0.48f,
                cy + size * 0.3f
            ),
            Mathf.RoundToInt(thickness),
            solanaGreen
        );

        DrawThickLine(
            new Vector2(
                cx - size * 0.42f,
                cy
            ),
            new Vector2(
                cx + size * 0.48f,
                cy
            ),
            Mathf.RoundToInt(thickness),
            solanaPurple
        );

        DrawThickLine(
            new Vector2(
                cx - size * 0.48f,
                cy - size * 0.3f
            ),
            new Vector2(
                cx + size * 0.42f,
                cy - size * 0.3f
            ),
            Mathf.RoundToInt(thickness),
            solanaGreen
        );
    }

    // =========================================================
    // ATMOSPHERE
    // =========================================================

    private void DrawAtmosphericEffects(
        float cx,
        float cy
    )
    {
        int orbitCount = random.Next(2, 5);

        for (int i = 0; i < orbitCount; i++)
        {
            DrawOrbit(
                cx,
                cy,
                RandomFloat(260f, 500f),
                RandomFloat(90f, 230f),
                new Color(
                    fighterGlowColor.r,
                    fighterGlowColor.g,
                    fighterGlowColor.b,
                    RandomFloat(0.08f, 0.2f)
                )
            );
        }

        for (int i = 0; i < random.Next(8, 20); i++)
        {
            float x = RandomFloat(0, textureWidth);
            float y = RandomFloat(0, textureHeight);

            DrawLine(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(y),
                Mathf.RoundToInt(x + RandomFloat(-70f, 70f)),
                Mathf.RoundToInt(y + RandomFloat(-10f, 10f)),
                new Color(
                    fighterGlowColor.r,
                    fighterGlowColor.g,
                    fighterGlowColor.b,
                    RandomFloat(0.08f, 0.25f)
                )
            );
        }
    }

    private void DrawOrbit(
        float cx,
        float cy,
        float radiusX,
        float radiusY,
        Color color
    )
    {
        int segments = 160;

        Vector2 previous = new Vector2(
            cx + radiusX,
            cy
        );

        for (int i = 1; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;

            Vector2 current = new Vector2(
                cx + Mathf.Cos(angle) * radiusX,
                cy + Mathf.Sin(angle) * radiusY
            );

            DrawLine(
                Mathf.RoundToInt(previous.x),
                Mathf.RoundToInt(previous.y),
                Mathf.RoundToInt(current.x),
                Mathf.RoundToInt(current.y),
                color
            );

            previous = current;
        }
    }

    // =========================================================
    // HUD
    // =========================================================

    private void DrawHudOverlay()
    {
        int margin = random.Next(28, 65);
        int cornerSize = random.Next(35, 85);

        DrawLine(margin, margin, margin + cornerSize, margin, solanaGreen);
        DrawLine(margin, margin, margin, margin + cornerSize, solanaGreen);

        DrawLine(
            textureWidth - margin,
            margin,
            textureWidth - margin - cornerSize,
            margin,
            solanaPurple
        );

        DrawLine(
            textureWidth - margin,
            margin,
            textureWidth - margin,
            margin + cornerSize,
            solanaPurple
        );

        DrawLine(
            margin,
            textureHeight - margin,
            margin + cornerSize,
            textureHeight - margin,
            electricBlue
        );

        DrawLine(
            margin,
            textureHeight - margin,
            margin,
            textureHeight - margin - cornerSize,
            electricBlue
        );

        DrawLine(
            textureWidth - margin,
            textureHeight - margin,
            textureWidth - margin - cornerSize,
            textureHeight - margin,
            solarOrange
        );

        DrawLine(
            textureWidth - margin,
            textureHeight - margin,
            textureWidth - margin,
            textureHeight - margin - cornerSize,
            solarOrange
        );

        for (int i = 0; i < random.Next(5, 12); i++)
        {
            int x = random.Next(30, textureWidth - 220);
            int y = random.Next(30, textureHeight - 30);

            DrawRect(
                x,
                y,
                random.Next(12, 100),
                random.Next(2, 5),
                RandomNeonColor(RandomFloat(0.25f, 0.8f))
            );
        }

        DrawTargetReticle(
            random.Next(120, textureWidth - 120),
            random.Next(120, textureHeight - 120),
            random.Next(20, 55)
        );
    }

    private void DrawTargetReticle(
        int cx,
        int cy,
        int size
    )
    {
        Color color = new Color(
            solanaGreen.r,
            solanaGreen.g,
            solanaGreen.b,
            0.42f
        );

        DrawLine(cx - size, cy, cx - size / 2, cy, color);
        DrawLine(cx + size / 2, cy, cx + size, cy, color);

        DrawLine(cx, cy - size, cx, cy - size / 2, color);
        DrawLine(cx, cy + size / 2, cx, cy + size, color);

        DrawCircle(
            cx,
            cy,
            Mathf.Max(2, size / 5),
            color
        );
    }

    private void DrawFrame()
    {
        Color color = new Color(
            fighterGlowColor.r,
            fighterGlowColor.g,
            fighterGlowColor.b,
            0.4f
        );

        int border = random.Next(8, 18);

        DrawLine(
            border,
            border,
            textureWidth - border,
            border,
            color
        );

        DrawLine(
            border,
            textureHeight - border,
            textureWidth - border,
            textureHeight - border,
            color
        );

        DrawLine(
            border,
            border,
            border,
            textureHeight - border,
            color
        );

        DrawLine(
            textureWidth - border,
            border,
            textureWidth - border,
            textureHeight - border,
            color
        );
    }

    // =========================================================
    // POLYGONS
    // =========================================================

    private void DrawPolygonFilled(
        Vector2[] points,
        Color color
    )
    {
        if (points == null || points.Length < 3)
            return;

        float minX = points[0].x;
        float maxX = points[0].x;
        float minY = points[0].y;
        float maxY = points[0].y;

        for (int i = 1; i < points.Length; i++)
        {
            minX = Mathf.Min(minX, points[i].x);
            maxX = Mathf.Max(maxX, points[i].x);
            minY = Mathf.Min(minY, points[i].y);
            maxY = Mathf.Max(maxY, points[i].y);
        }

        int startX = Mathf.Max(
            0,
            Mathf.FloorToInt(minX)
        );

        int endX = Mathf.Min(
            textureWidth - 1,
            Mathf.CeilToInt(maxX)
        );

        int startY = Mathf.Max(
            0,
            Mathf.FloorToInt(minY)
        );

        int endY = Mathf.Min(
            textureHeight - 1,
            Mathf.CeilToInt(maxY)
        );

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                if (
                    IsPointInsidePolygon(
                        new Vector2(x, y),
                        points
                    )
                )
                {
                    SetPixelSafe(x, y, color);
                }
            }
        }
    }

    private bool IsPointInsidePolygon(
        Vector2 point,
        Vector2[] polygon
    )
    {
        bool inside = false;
        int j = polygon.Length - 1;

        for (int i = 0; i < polygon.Length; i++)
        {
            bool intersect =
                ((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (
                    point.x <
                    (polygon[j].x - polygon[i].x) *
                    (point.y - polygon[i].y) /
                    (polygon[j].y - polygon[i].y + 0.0001f) +
                    polygon[i].x
                );

            if (intersect)
                inside = !inside;

            j = i;
        }

        return inside;
    }

    private void DrawPolygonOutline(
        Vector2[] points,
        Color color,
        int thickness
    )
    {
        if (points == null || points.Length < 2)
            return;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[(i + 1) % points.Length];

            DrawThickLine(
                start,
                end,
                thickness,
                color
            );
        }
    }

    // =========================================================
    // PIXEL DRAWING
    // =========================================================

    private void DrawCircle(
        int centerX,
        int centerY,
        int radius,
        Color color
    )
    {
        if (radius <= 0)
            return;

        int radiusSquared = radius * radius;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > radiusSquared)
                    continue;

                SetPixelSafe(
                    centerX + x,
                    centerY + y,
                    color
                );
            }
        }
    }

    private void DrawRect(
        int x,
        int y,
        int width,
        int height,
        Color color
    )
    {
        if (width <= 0 || height <= 0)
            return;

        for (int yy = 0; yy < height; yy++)
        {
            for (int xx = 0; xx < width; xx++)
            {
                SetPixelSafe(
                    x + xx,
                    y + yy,
                    color
                );
            }
        }
    }

    private void DrawLine(
        int x1,
        int y1,
        int x2,
        int y2,
        Color color
    )
    {
        int dx = Mathf.Abs(x2 - x1);
        int dy = Mathf.Abs(y2 - y1);

        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;

        int error = dx - dy;

        while (true)
        {
            SetPixelSafe(x1, y1, color);

            if (x1 == x2 && y1 == y2)
                break;

            int e2 = error * 2;

            if (e2 > -dy)
            {
                error -= dy;
                x1 += sx;
            }

            if (e2 < dx)
            {
                error += dx;
                y1 += sy;
            }
        }
    }

    private void DrawThickLine(
        Vector2 start,
        Vector2 end,
        int thickness,
        Color color
    )
    {
        if (thickness <= 0)
            thickness = 1;

        Vector2 direction = (end - start).normalized;

        if (direction == Vector2.zero)
            return;

        Vector2 perpendicular = new Vector2(
            -direction.y,
            direction.x
        );

        for (int i = -thickness / 2; i <= thickness / 2; i++)
        {
            Vector2 offset = perpendicular * i;

            Vector2 pointA = start + offset;
            Vector2 pointB = end + offset;

            DrawLine(
                Mathf.RoundToInt(pointA.x),
                Mathf.RoundToInt(pointA.y),
                Mathf.RoundToInt(pointB.x),
                Mathf.RoundToInt(pointB.y),
                color
            );
        }
    }

    private void DrawGlowLine(
        Vector2 start,
        Vector2 end,
        float thickness,
        Color color
    )
    {
        DrawThickLine(
            start,
            end,
            Mathf.RoundToInt(thickness * 3f),
            new Color(
                color.r,
                color.g,
                color.b,
                0.06f
            )
        );

        DrawThickLine(
            start,
            end,
            Mathf.RoundToInt(thickness * 1.8f),
            new Color(
                color.r,
                color.g,
                color.b,
                0.13f
            )
        );

        DrawThickLine(
            start,
            end,
            Mathf.RoundToInt(thickness),
            color
        );
    }

    private void SetPixelSafe(
        int x,
        int y,
        Color color
    )
    {
        if (texture == null)
            return;

        if (x < 0 || x >= textureWidth)
            return;

        if (y < 0 || y >= textureHeight)
            return;

        Color oldColor = texture.GetPixel(x, y);

        Color result = Color.Lerp(
            oldColor,
            color,
            Mathf.Clamp01(color.a)
        );

        result.a = 1f;

        texture.SetPixel(
            x,
            y,
            result
        );
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void DestroyTexture()
    {
        if (texture == null)
            return;

        if (Application.isPlaying)
            Destroy(texture);
        else
            DestroyImmediate(texture);

        texture = null;
    }

    private void OnDestroy()
    {
        DestroyTexture();
    }
}