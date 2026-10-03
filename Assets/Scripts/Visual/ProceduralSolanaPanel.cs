using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralSolanaPanel : MonoBehaviour
{
    private enum CardTheme
    {
        SolanaCity,
        NFTGrid,
        Wallet,
        OnChain,
        NeonRaid
    }

    [Header("Main")]
    [SerializeField] private RawImage generatedImage;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private TMP_Text smallText;

    [Header("Generation")]
    [SerializeField] private int width = 1000;
    [SerializeField] private int height = 500;

    [SerializeField] private bool generateOnEnable = true;

    [Header("Colors")]
    [SerializeField] private Color background = new Color(0.015f, 0.018f, 0.04f);
    [SerializeField] private Color cyan = new Color(0.10f, 0.95f, 0.88f);
    [SerializeField] private Color purple = new Color(0.65f, 0.30f, 1f);
    [SerializeField] private Color blue = new Color(0.18f, 0.45f, 1f);
    [SerializeField] private Color white = new Color(0.85f, 0.95f, 1f);

    [Header("Seed")]
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private int seed = 12345;

    private Texture2D texture;
    private System.Random random;
    private int variation;

    private void OnEnable()
    {
        if (generateOnEnable)
            Generate();
    }

    public void Generate()
    {
        if (randomizeSeed)
            seed = Guid.NewGuid().GetHashCode();

        random = new System.Random(seed);
        variation = random.Next(0, 4);

        if (texture != null)
            Destroy(texture);

        texture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        FillBackground();

        CardTheme theme =
            (CardTheme)random.Next(
                0,
                Enum.GetValues(typeof(CardTheme)).Length
            );

        switch (theme)
        {
            case CardTheme.SolanaCity:
                DrawSolanaCity();
                break;

            case CardTheme.NFTGrid:
                DrawNFTGrid();
                break;

            case CardTheme.Wallet:
                DrawWallet();
                break;

            case CardTheme.OnChain:
                DrawOnChain();
                break;

            case CardTheme.NeonRaid:
                DrawNeonRaid();
                break;
        }

        texture.Apply();

        if (generatedImage != null)
            generatedImage.texture = texture;

        UpdateText(theme);
    }

    // =========================================================
    // RANDOM HELPERS
    // =========================================================

    private float RandomFloat(float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    private bool RandomBool()
    {
        return random.Next(0, 2) == 0;
    }

    private Color RandomThemeColor(float alpha = 1f)
    {
        Color result;

        switch (random.Next(0, 4))
        {
            case 0:
                result = cyan;
                break;
            case 1:
                result = purple;
                break;
            case 2:
                result = blue;
                break;
            default:
                result = white;
                break;
        }

        result.a = alpha;
        return result;
    }

    private Vector2 RandomPosition(float margin = 0f)
    {
        return new Vector2(
            RandomFloat(margin, width - margin),
            RandomFloat(margin, height - margin)
        );
    }

    // =========================================================
    // BACKGROUND
    // =========================================================

    private void FillBackground()
    {
        Color[] pixels = new Color[width * height];

        Color topColor = Color.Lerp(
            background,
            RandomThemeColor(1f),
            RandomFloat(0.02f, 0.18f)
        );

        Color bottomColor = Color.Lerp(
            background * RandomFloat(0.5f, 0.9f),
            RandomThemeColor(1f),
            RandomFloat(0.02f, 0.12f)
        );

        for (int y = 0; y < height; y++)
        {
            float t = y / (float)(height - 1);
            Color gradient = Color.Lerp(bottomColor, topColor, t);

            for (int x = 0; x < width; x++)
            {
                float noise = RandomFloat(-0.012f, 0.012f);

                pixels[y * width + x] = new Color(
                    Mathf.Clamp01(gradient.r + noise),
                    Mathf.Clamp01(gradient.g + noise),
                    Mathf.Clamp01(gradient.b + noise),
                    1f
                );
            }
        }

        texture.SetPixels(pixels);

        int glowCount = random.Next(2, 6);

        for (int i = 0; i < glowCount; i++)
        {
            Vector2 pos = RandomPosition(10f);

            AddGlow(
                pos.x,
                pos.y,
                RandomFloat(width * 0.12f, width * 0.55f),
                RandomFloat(height * 0.15f, height * 0.75f),
                RandomThemeColor(RandomFloat(0.04f, 0.16f))
            );
        }
    }

    private void AddGlow(
        float cx,
        float cy,
        float rx,
        float ry,
        Color glow
    )
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx));
        int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + rx));
        int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry));
        int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + ry));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = (x - cx) / rx;
                float dy = (y - cy) / ry;
                float d = Mathf.Sqrt(dx * dx + dy * dy);

                if (d > 1f)
                    continue;

                float alpha = Mathf.Pow(1f - d, 2f) * glow.a;
                Color old = texture.GetPixel(x, y);

                Color result = Color.Lerp(
                    old,
                    new Color(glow.r, glow.g, glow.b, 1f),
                    alpha
                );

                texture.SetPixel(x, y, result);
            }
        }
    }

    // =========================================================
    // THEME 1 — SOLANA CITY
    // =========================================================

    private void DrawSolanaCity()
    {
        DrawStars(random.Next(40, 120));

        int horizon = Mathf.RoundToInt(height * RandomFloat(0.68f, 0.82f));

        int buildingCount = random.Next(12, 40);

        for (int i = 0; i < buildingCount; i++)
        {
            int buildingWidth = random.Next(12, 75);
            int buildingHeight = random.Next(
                30,
                Mathf.RoundToInt(height * RandomFloat(0.25f, 0.72f))
            );

            int x = random.Next(0, Mathf.Max(1, width - buildingWidth));
            int y = horizon - buildingHeight;

            DrawRect(
                x,
                y,
                buildingWidth,
                buildingHeight,
                new Color(
                    RandomFloat(0.015f, 0.05f),
                    RandomFloat(0.02f, 0.08f),
                    RandomFloat(0.04f, 0.12f),
                    1f
                )
            );

            int windows = random.Next(2, 10);

            for (int w = 0; w < windows; w++)
            {
                int wx = x + random.Next(3, Mathf.Max(4, buildingWidth - 3));
                int wy = y + random.Next(5, Mathf.Max(6, buildingHeight - 5));

                Color windowColor = random.Next(0, 2) == 0 ? cyan : purple;

                DrawRect(
                    wx,
                    wy,
                    random.Next(2, 5),
                    random.Next(3, 9),
                    new Color(
                        windowColor.r,
                        windowColor.g,
                        windowColor.b,
                        RandomFloat(0.55f, 1f)
                    )
                );
            }
        }

        float logoX = RandomFloat(width * 0.45f, width * 0.85f);
        float logoY = RandomFloat(height * 0.18f, height * 0.55f);
        float logoSize = RandomFloat(55f, 130f);

        DrawSolanaLogo(logoX, logoY, logoSize);

        if (RandomBool())
            DrawOrbitLines(logoX, logoY);
    }

    // =========================================================
    // THEME 2 — NFT GRID
    // =========================================================

    private void DrawNFTGrid()
    {
        DrawStars(random.Next(40, 130));

        int columns = random.Next(3, 7);
        int rows = random.Next(1, 4);

        float gap = RandomFloat(12f, 32f);

        float cardWidth = RandomFloat(90f, 155f);
        float cardHeight = RandomFloat(100f, 175f);

        float totalWidth = columns * cardWidth + (columns - 1) * gap;
        float totalHeight = rows * cardHeight + (rows - 1) * gap;

        float startX = (width - totalWidth) * 0.5f;
        float startY = (height - totalHeight) * 0.5f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                float jitterX = RandomFloat(-10f, 10f);
                float jitterY = RandomFloat(-10f, 10f);

                float px = startX + x * (cardWidth + gap) + jitterX;
                float py = startY + y * (cardHeight + gap) + jitterY;

                Color cardColor = RandomThemeColor(RandomFloat(0.5f, 1f));

                DrawRectOutline(
                    Mathf.RoundToInt(px),
                    Mathf.RoundToInt(py),
                    Mathf.RoundToInt(cardWidth),
                    Mathf.RoundToInt(cardHeight),
                    cardColor
                );

                int centerX = Mathf.RoundToInt(px + cardWidth * RandomFloat(0.35f, 0.65f));
                int centerY = Mathf.RoundToInt(py + cardHeight * RandomFloat(0.40f, 0.68f));
                int radius = random.Next(16, 43);

                DrawCircle(
                    centerX,
                    centerY,
                    radius,
                    new Color(
                        cardColor.r,
                        cardColor.g,
                        cardColor.b,
                        RandomFloat(0.25f, 0.75f)
                    )
                );

                DrawRandomShape(centerX, centerY, cardColor);

                int lineCount = random.Next(1, 6);

                for (int line = 0; line < lineCount; line++)
                {
                    DrawRect(
                        Mathf.RoundToInt(px + 8),
                        Mathf.RoundToInt(py + 8 + line * RandomFloat(5f, 10f)),
                        random.Next(15, 65),
                        random.Next(1, 4),
                        new Color(
                            white.r,
                            white.g,
                            white.b,
                            RandomFloat(0.2f, 0.7f)
                        )
                    );
                }

                if (RandomBool())
                {
                    DrawLine(
                        Mathf.RoundToInt(px),
                        Mathf.RoundToInt(py),
                        Mathf.RoundToInt(px + 15),
                        Mathf.RoundToInt(py),
                        white
                    );

                    DrawLine(
                        Mathf.RoundToInt(px),
                        Mathf.RoundToInt(py),
                        Mathf.RoundToInt(px),
                        Mathf.RoundToInt(py + 15),
                        white
                    );
                }
            }
        }
    }

    // =========================================================
    // THEME 3 — WALLET
    // =========================================================

    private void DrawWallet()
    {
        DrawStars(random.Next(60, 150));

        float cx = RandomFloat(width * 0.4f, width * 0.8f);
        float cy = RandomFloat(height * 0.35f, height * 0.72f);

        int walletW = random.Next(220, 380);
        int walletH = random.Next(120, 230);

        DrawRectOutline(
            Mathf.RoundToInt(cx - walletW / 2f),
            Mathf.RoundToInt(cy - walletH / 2f),
            walletW,
            walletH,
            cyan
        );

        DrawRectOutline(
            Mathf.RoundToInt(cx - walletW / 2f + 15),
            Mathf.RoundToInt(cy - walletH / 2f + 15),
            walletW - 30,
            walletH - 30,
            new Color(
                purple.r,
                purple.g,
                purple.b,
                0.45f
            )
        );

        DrawRect(
            Mathf.RoundToInt(cx - 95),
            Mathf.RoundToInt(cy - 45),
            190,
            4,
            white
        );

        DrawRect(
            Mathf.RoundToInt(cx - 70),
            Mathf.RoundToInt(cy - 28),
            140,
            3,
            new Color(
                white.r,
                white.g,
                white.b,
                0.4f
            )
        );

        int coinRadius = random.Next(20, 45);

        DrawCircle(
            Mathf.RoundToInt(cx + 95),
            Mathf.RoundToInt(cy + 45),
            coinRadius,
            new Color(
                purple.r,
                purple.g,
                purple.b,
                0.45f
            )
        );

        DrawSolanaLogo(cx + 95, cy + 45, coinRadius + 5);

        int nodeCount = random.Next(5, 12);

        for (int i = 0; i < nodeCount; i++)
        {
            float angle = i / (float)nodeCount * Mathf.PI * 2f;
            float radius = RandomFloat(140f, 260f);

            float x = cx + Mathf.Cos(angle) * radius;
            float y = cy + Mathf.Sin(angle) * radius * 0.55f;

            DrawLine(
                Mathf.RoundToInt(cx),
                Mathf.RoundToInt(cy),
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(y),
                new Color(
                    cyan.r,
                    cyan.g,
                    cyan.b,
                    RandomFloat(0.08f, 0.18f)
                )
            );

            DrawCircle(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(y),
                4,
                cyan
            );
        }
    }

    // =========================================================
    // THEME 4 — ON CHAIN
    // =========================================================

    private void DrawOnChain()
    {
        DrawStars(random.Next(80, 180));

        int nodes = random.Next(10, 35);

        Vector2[] points = new Vector2[nodes];

        float centerX = RandomFloat(width * 0.35f, width * 0.65f);
        float centerY = RandomFloat(height * 0.35f, height * 0.65f);

        for (int i = 0; i < nodes; i++)
        {
            points[i] = new Vector2(
                Mathf.Clamp(centerX + RandomFloat(-width * 0.45f, width * 0.45f), 30f, width - 30f),
                Mathf.Clamp(centerY + RandomFloat(-height * 0.42f, height * 0.42f), 30f, height - 30f)
            );
        }

        for (int i = 0; i < nodes; i++)
        {
            int connections = random.Next(1, 4);

            for (int j = 0; j < connections; j++)
            {
                int target = random.Next(0, nodes);

                if (target == i)
                    continue;

                DrawLine(
                    Mathf.RoundToInt(points[i].x),
                    Mathf.RoundToInt(points[i].y),
                    Mathf.RoundToInt(points[target].x),
                    Mathf.RoundToInt(points[target].y),
                    new Color(
                        purple.r,
                        purple.g,
                        purple.b,
                        RandomFloat(0.08f, 0.18f)
                    )
                );
            }
        }

        for (int i = 0; i < nodes; i++)
        {
            Color nodeColor = random.Next(0, 2) == 0 ? cyan : purple;

            DrawCircle(
                Mathf.RoundToInt(points[i].x),
                Mathf.RoundToInt(points[i].y),
                random.Next(4, 9),
                nodeColor
            );
        }

        for (int i = 4; i >= 1; i--)
        {
            DrawCircle(
                Mathf.RoundToInt(centerX),
                Mathf.RoundToInt(centerY),
                i * 28,
                new Color(
                    cyan.r,
                    cyan.g,
                    cyan.b,
                    0.025f * (5 - i)
                )
            );
        }

        DrawSolanaLogo(
            centerX,
            centerY,
            RandomFloat(55f, 120f)
        );
    }

    // =========================================================
    // THEME 5 — NEON RAID
    // =========================================================

    private void DrawNeonRaid()
    {
        DrawStars(random.Next(50, 130));

        int horizon = Mathf.RoundToInt(height * RandomFloat(0.68f, 0.82f));

        for (int i = -12; i <= 12; i++)
        {
            DrawLine(
                Mathf.RoundToInt(width * 0.5f),
                horizon,
                Mathf.RoundToInt(width * 0.5f + i * RandomFloat(80f, 140f)),
                height,
                new Color(
                    cyan.r,
                    cyan.g,
                    cyan.b,
                    RandomFloat(0.08f, 0.18f)
                )
            );
        }

        for (int i = 0; i < 8; i++)
        {
            float t = i / 8f;
            int y = Mathf.RoundToInt(Mathf.Lerp(horizon, height, t * t));

            DrawLine(
                0,
                y,
                width,
                y,
                new Color(
                    purple.r,
                    purple.g,
                    purple.b,
                    RandomFloat(0.08f, 0.17f)
                )
            );
        }

        DrawDrone(
            RandomFloat(width * 0.45f, width * 0.75f),
            RandomFloat(height * 0.25f, height * 0.55f)
        );

        DrawSolanaLogo(
            RandomFloat(width * 0.15f, width * 0.35f),
            RandomFloat(height * 0.2f, height * 0.45f),
            RandomFloat(60f, 110f)
        );

        for (int i = 0; i < random.Next(5, 12); i++)
        {
            int x = random.Next(20, width / 2);
            int y = random.Next(30, height - 40);

            DrawRect(
                x,
                y,
                random.Next(15, 90),
                2,
                new Color(
                    cyan.r,
                    cyan.g,
                    cyan.b,
                    RandomFloat(0.35f, 0.75f)
                )
            );
        }
    }

    // =========================================================
    // SOLANA LOGO
    // =========================================================

    private void DrawSolanaLogo(
        float cx,
        float cy,
        float size
    )
    {
        DrawThickLine(
            new Vector2(cx - size * 0.48f, cy + size * 0.30f),
            new Vector2(cx + size * 0.48f, cy + size * 0.30f),
            11,
            cyan
        );

        DrawThickLine(
            new Vector2(cx - size * 0.42f, cy),
            new Vector2(cx + size * 0.48f, cy),
            11,
            purple
        );

        DrawThickLine(
            new Vector2(cx - size * 0.48f, cy - size * 0.30f),
            new Vector2(cx + size * 0.42f, cy - size * 0.30f),
            11,
            cyan
        );
    }

    // =========================================================
    // DRONE
    // =========================================================

    private void DrawDrone(
        float cx,
        float cy
    )
    {
        Color body = new Color(0.10f, 0.13f, 0.20f, 1f);

        DrawRect(
            Mathf.RoundToInt(cx - 55),
            Mathf.RoundToInt(cy - 12),
            110,
            24,
            body
        );

        DrawThickLine(
            new Vector2(cx - 70, cy - 8),
            new Vector2(cx - 25, cy),
            12,
            cyan
        );

        DrawThickLine(
            new Vector2(cx + 25, cy),
            new Vector2(cx + 70, cy - 8),
            12,
            cyan
        );

        DrawCircle(
            Mathf.RoundToInt(cx),
            Mathf.RoundToInt(cy),
            14,
            purple
        );

        DrawCircle(
            Mathf.RoundToInt(cx),
            Mathf.RoundToInt(cy),
            6,
            white
        );

        DrawThickLine(
            new Vector2(cx - 20, cy - 27),
            new Vector2(cx + 20, cy - 27),
            5,
            cyan
        );
    }

    // =========================================================
    // EXTRA VISUALS
    // =========================================================

    private void DrawStars(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int x = random.Next(0, width);
            int y = random.Next(0, height);

            Color star = RandomThemeColor(RandomFloat(0.25f, 0.95f));
            int radius = random.Next(1, 5);

            DrawCircle(x, y, radius, star);

            if (random.Next(0, 6) == 0)
            {
                DrawLine(
                    x - radius * 2,
                    y,
                    x + radius * 2,
                    y,
                    star
                );

                DrawLine(
                    x,
                    y - radius * 2,
                    x,
                    y + radius * 2,
                    star
                );
            }
        }
    }

    private void DrawOrbitLines(
        float cx,
        float cy
    )
    {
        for (int i = 1; i <= random.Next(3, 7); i++)
        {
            float rx = i * RandomFloat(20f, 45f);
            float ry = i * RandomFloat(10f, 25f);

            int segments = random.Next(60, 130);

            Vector2 previous = new Vector2(cx + rx, cy);

            for (int s = 1; s <= segments; s++)
            {
                float angle = s / (float)segments * Mathf.PI * 2f;
                Vector2 current = new Vector2(
                    cx + Mathf.Cos(angle) * rx,
                    cy + Mathf.Sin(angle) * ry
                );

                DrawLine(
                    Mathf.RoundToInt(previous.x),
                    Mathf.RoundToInt(previous.y),
                    Mathf.RoundToInt(current.x),
                    Mathf.RoundToInt(current.y),
                    new Color(
                        white.r,
                        white.g,
                        white.b,
                        RandomFloat(0.06f, 0.18f)
                    )
                );

                previous = current;
            }
        }
    }

    private void DrawRandomShape(
        float cx,
        float cy,
        Color color
    )
    {
        int type = random.Next(0, 4);

        if (type == 0)
        {
            DrawCircle(
                Mathf.RoundToInt(cx),
                Mathf.RoundToInt(cy),
                random.Next(20, 34),
                color
            );
        }
        else if (type == 1)
        {
            DrawRect(
                Mathf.RoundToInt(cx - 28),
                Mathf.RoundToInt(cy - 28),
                random.Next(40, 70),
                random.Next(40, 70),
                color
            );
        }
        else if (type == 2)
        {
            DrawLine(
                Mathf.RoundToInt(cx - 30),
                Mathf.RoundToInt(cy + 25),
                Mathf.RoundToInt(cx),
                Mathf.RoundToInt(cy - 35),
                color
            );

            DrawLine(
                Mathf.RoundToInt(cx),
                Mathf.RoundToInt(cy - 35),
                Mathf.RoundToInt(cx + 30),
                Mathf.RoundToInt(cy + 25),
                color
            );
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI / 3f;
                float x = cx + Mathf.Cos(angle) * RandomFloat(20f, 35f);
                float y = cy + Mathf.Sin(angle) * RandomFloat(20f, 35f);

                DrawCircle(
                    Mathf.RoundToInt(x),
                    Mathf.RoundToInt(y),
                    random.Next(6, 12),
                    color
                );
            }
        }
    }

    // =========================================================
    // TEXT
    // =========================================================

    private void UpdateText(CardTheme theme)
    {
        switch (theme)
        {
            case CardTheme.SolanaCity:
                SetTexts(
                    "SOLANA CITY",
                    "THE NETWORK NEVER SLEEPS",
                    "SOLANA // " + random.Next(1, 99).ToString("D2")
                );
                break;

            case CardTheme.NFTGrid:
                SetTexts(
                    "NFT GRID",
                    "DIGITAL ASSETS ON SOLANA",
                    "COLLECTION // " + (random.Next(0, 2) == 0 ? "ON-CHAIN" : "LIVE")
                );
                break;

            case CardTheme.Wallet:
                SetTexts(
                    "SOLANA WALLET",
                    "CONNECT • PLAY • COLLECT",
                    "WALLET // " + (random.Next(0, 2) == 0 ? "READY" : "ACTIVE")
                );
                break;

            case CardTheme.OnChain:
                SetTexts(
                    "ON-CHAIN",
                    "EVERY ACTION CAN MATTER",
                    "NETWORK // " + (random.Next(0, 2) == 0 ? "ACTIVE" : "SYNCED")
                );
                break;

            case CardTheme.NeonRaid:
                SetTexts(
                    "NEON RAID × SOLANA",
                    "POWERED BY THE SOLANA ECOSYSTEM",
                    "GAME // " + (random.Next(0, 2) == 0 ? "WEB3" : "LIVE")
                );
                break;
        }
    }

    private void SetTexts(
        string title,
        string subtitle,
        string small
    )
    {
        if (titleText != null)
            titleText.text = title;

        if (subtitleText != null)
            subtitleText.text = subtitle;

        if (smallText != null)
            smallText.text = small;
    }

    // =========================================================
    // DRAW FUNCTIONS
    // =========================================================

    private void DrawCircle(
        int cx,
        int cy,
        int radius,
        Color color
    )
    {
        int r2 = radius * radius;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > r2)
                    continue;

                SetPixelSafe(cx + x, cy + y, color);
            }
        }
    }

    private void DrawRect(
        int x,
        int y,
        int w,
        int h,
        Color color
    )
    {
        for (int yy = 0; yy < h; yy++)
        {
            for (int xx = 0; xx < w; xx++)
            {
                SetPixelSafe(x + xx, y + yy, color);
            }
        }
    }

    private void DrawRectOutline(
        int x,
        int y,
        int w,
        int h,
        Color color
    )
    {
        DrawLine(x, y, x + w, y, color);
        DrawLine(x, y, x, y + h, color);
        DrawLine(x + w, y, x + w, y + h, color);
        DrawLine(x, y + h, x + w, y + h, color);
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

        int err = dx - dy;

        while (true)
        {
            SetPixelSafe(x1, y1, color);

            if (x1 == x2 && y1 == y2)
                break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }

    private void DrawThickLine(
        Vector2 a,
        Vector2 b,
        int thickness,
        Color color
    )
    {
        Vector2 direction = (b - a).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        for (int i = -thickness / 2; i <= thickness / 2; i++)
        {
            Vector2 offset = perpendicular * i;
            Vector2 p1 = a + offset;
            Vector2 p2 = b + offset;

            DrawLine(
                Mathf.RoundToInt(p1.x),
                Mathf.RoundToInt(p1.y),
                Mathf.RoundToInt(p2.x),
                Mathf.RoundToInt(p2.y),
                color
            );
        }
    }

    private void SetPixelSafe(
        int x,
        int y,
        Color color
    )
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        Color old = texture.GetPixel(x, y);
        Color result = Color.Lerp(old, color, color.a);
        result.a = 1f;

        texture.SetPixel(x, y, result);
    }

    private Color GetRandomNeonColor()
    {
        int value = random.Next(0, 4);

        if (value == 0)
            return cyan;

        if (value == 1)
            return purple;

        if (value == 2)
            return blue;

        return white;
    }

    private void OnDestroy()
    {
        if (texture != null)
            Destroy(texture);
    }
}