using System;
using UnityEngine;

public class RandomStarSkyTiledGenerator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private SpriteRenderer targetSprite;

    [Header("Texture")]
    [SerializeField] private int textureWidth = 512;
    [SerializeField] private int textureHeight = 512;
    [SerializeField] private float pixelsPerUnit = 100f;

    [Header("Tiling")]
    [SerializeField] private Vector2Int tileCount = new Vector2Int(2, 2);
    [SerializeField] private bool seamlessMode = true;

    [Header("Generation")]
    [SerializeField] private bool generateOnEnable = true;
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private int seed = 12345;

    [Header("Stars")]
    [SerializeField] private int starCount = 150;
    [SerializeField] private bool drawCrossStars = true;

    [Header("Space Objects")]
    [SerializeField] private bool drawNebulas = true;
    [SerializeField] private bool drawConstellations = true;
    [SerializeField] private bool drawPlanets = false;
    [SerializeField] private bool drawMeteors = true;

    [Header("Planet Settings")]
    [SerializeField] private int minPlanets = 0;
    [SerializeField] private int maxPlanets = 1;

    [Header("Colors")]
    [SerializeField] private Color topSpaceColor =
        new Color(0.003f, 0.006f, 0.025f, 1f);

    [SerializeField] private Color bottomSpaceColor =
        new Color(0.012f, 0.025f, 0.095f, 1f);

    [SerializeField] private Color blueStar =
        new Color(0.35f, 0.70f, 1f, 1f);

    [SerializeField] private Color purpleStar =
        new Color(0.75f, 0.40f, 1f, 1f);

    [SerializeField] private Color cyanStar =
        new Color(0.20f, 1f, 0.90f, 1f);

    [SerializeField] private Color whiteStar =
        new Color(0.90f, 0.97f, 1f, 1f);

    private Texture2D texture;
    private Sprite generatedSprite;
    private Color[] pixels;
    private System.Random random;
    private int seamBlendWidth = 16;

    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        if (generateOnEnable)
            Generate();
    }

    private void OnDestroy()
    {
        DestroyGeneratedObjects();
    }

    public void Generate()
    {
        ValidateSettings();

        if (randomizeSeed)
            seed = Guid.NewGuid().GetHashCode();

        random = new System.Random(seed);

        DestroyGeneratedObjects();

        texture = new Texture2D(
            textureWidth,
            textureHeight,
            TextureFormat.RGBA32,
            false
        );

        texture.name = "GeneratedStarSkyTiledTexture";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Repeat;

        pixels = new Color[textureWidth * textureHeight];

        DrawSpaceGradient();

        if (drawNebulas)
            DrawNebulas();

        DrawStars(starCount);

        if (drawConstellations)
            DrawConstellations();

        if (drawPlanets)
            DrawPlanets();

        if (drawMeteors)
            DrawMeteors();

        // Применяем seamless blending для края текстуры
        if (seamlessMode)
            ApplySeamlessBlending();

        texture.SetPixels(pixels);
        texture.Apply();

        generatedSprite = Sprite.Create(
            texture,
            new Rect(
                0f,
                0f,
                texture.width,
                texture.height
            ),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit
        );

        generatedSprite.name = "GeneratedStarSkyTiledSprite";

        if (targetSprite != null)
        {
            targetSprite.sprite = generatedSprite;
            targetSprite.color = Color.white;
            
            // Устанавливаем режим Tiled
            targetSprite.drawMode = SpriteDrawMode.Tiled;
            targetSprite.size = new Vector2(
                textureWidth / pixelsPerUnit * tileCount.x,
                textureHeight / pixelsPerUnit * tileCount.y
            );
        }
    }

    private void ValidateSettings()
    {
        textureWidth = Mathf.Clamp(textureWidth, 64, 2048);
        textureHeight = Mathf.Clamp(textureHeight, 64, 2048);
        pixelsPerUnit = Mathf.Max(1f, pixelsPerUnit);
        starCount = Mathf.Clamp(starCount, 0, 2000);
        
        tileCount = new Vector2Int(
            Mathf.Max(1, tileCount.x),
            Mathf.Max(1, tileCount.y)
        );

        minPlanets = Mathf.Max(0, minPlanets);
        maxPlanets = Mathf.Max(minPlanets, maxPlanets);
        
        seamBlendWidth = Mathf.Clamp(seamBlendWidth, 4, Mathf.Min(textureWidth, textureHeight) / 4);
    }

    private float RandomFloat(float min, float max)
    {
        if (random == null)
            random = new System.Random(seed);

        return min + (float)random.NextDouble() * (max - min);
    }

    // =========================================================
    // SEAMLESS BLENDING (Все 4 края)
    // =========================================================

    private void ApplySeamlessBlending()
    {
        // Blend LEFT край (x = 0)
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < seamBlendWidth; x++)
            {
                float t = x / (float)seamBlendWidth;
                
                Color leftColor = pixels[y * textureWidth + x];
                Color rightColor = pixels[y * textureWidth + (textureWidth - seamBlendWidth + x)];
                Color blended = Color.Lerp(rightColor, leftColor, t);
                
                pixels[y * textureWidth + x] = blended;
            }
        }

        // Blend RIGHT край (x = width-1)
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < seamBlendWidth; x++)
            {
                float t = x / (float)seamBlendWidth;
                int rightPixelX = textureWidth - seamBlendWidth + x;
                
                Color rightColor = pixels[y * textureWidth + rightPixelX];
                Color leftColor = pixels[y * textureWidth + x];
                Color blended = Color.Lerp(leftColor, rightColor, t);
                
                pixels[y * textureWidth + rightPixelX] = blended;
            }
        }

        // Blend BOTTOM край (y = 0)
        for (int y = 0; y < seamBlendWidth; y++)
        {
            float t = y / (float)seamBlendWidth;
            
            for (int x = 0; x < textureWidth; x++)
            {
                Color bottomColor = pixels[y * textureWidth + x];
                Color topColor = pixels[(textureHeight - seamBlendWidth + y) * textureWidth + x];
                Color blended = Color.Lerp(topColor, bottomColor, t);
                
                pixels[y * textureWidth + x] = blended;
            }
        }

        // Blend TOP край (y = height-1)
        for (int y = 0; y < seamBlendWidth; y++)
        {
            float t = y / (float)seamBlendWidth;
            int topPixelY = textureHeight - seamBlendWidth + y;
            
            for (int x = 0; x < textureWidth; x++)
            {
                Color topColor = pixels[topPixelY * textureWidth + x];
                Color bottomColor = pixels[y * textureWidth + x];
                Color blended = Color.Lerp(bottomColor, topColor, t);
                
                pixels[topPixelY * textureWidth + x] = blended;
            }
        }

        // Blend углы (все 4 угла)
        BlendCorner(0, 0);                                          // Bottom-Left
        BlendCorner(textureWidth - seamBlendWidth, 0);              // Bottom-Right
        BlendCorner(0, textureHeight - seamBlendWidth);             // Top-Left
        BlendCorner(textureWidth - seamBlendWidth, 
                   textureHeight - seamBlendWidth);                 // Top-Right
    }

    private void BlendCorner(int startX, int startY)
    {
        for (int y = 0; y < seamBlendWidth; y++)
        {
            for (int x = 0; x < seamBlendWidth; x++)
            {
                float tx = x / (float)seamBlendWidth;
                float ty = y / (float)seamBlendWidth;
                
                int pixelX = startX + x;
                int pixelY = startY + y;
                
                int oppositeX = (pixelX + textureWidth / 2) % textureWidth;
                int oppositeY = (pixelY + textureHeight / 2) % textureHeight;
                
                Color color = pixels[pixelY * textureWidth + pixelX];
                Color oppositeColor = pixels[oppositeY * textureWidth + oppositeX];
                Color blended = Color.Lerp(oppositeColor, color, (tx + ty) * 0.5f);
                
                pixels[pixelY * textureWidth + pixelX] = blended;
            }
        }
    }

    // =========================================================
    // BACKGROUND
    // =========================================================

    private void DrawSpaceGradient()
    {
        for (int y = 0; y < textureHeight; y++)
        {
            float vertical = y / (float)(textureHeight - 1);

            Color gradient = Color.Lerp(
                bottomSpaceColor,
                topSpaceColor,
                vertical
            );

            for (int x = 0; x < textureWidth; x++)
            {
                float noise = RandomFloat(-0.008f, 0.008f);

                Color color = new Color(
                    Mathf.Clamp01(gradient.r + noise),
                    Mathf.Clamp01(gradient.g + noise),
                    Mathf.Clamp01(gradient.b + noise),
                    1f
                );

                SetPixelDirect(x, y, color);
            }
        }
    }

    private void DrawNebulas()
    {
        int nebulaCount = random.Next(1, 4);

        for (int i = 0; i < nebulaCount; i++)
        {
            float x = RandomFloat(0f, textureWidth);
            float y = RandomFloat(0f, textureHeight);

            float radiusX = RandomFloat(
                textureWidth * 0.08f,
                textureWidth * 0.25f
            );

            float radiusY = RandomFloat(
                textureHeight * 0.08f,
                textureHeight * 0.20f
            );

            AddGlow(
                x,
                y,
                radiusX,
                radiusY,
                GetRandomSpaceColor(RandomFloat(0.025f, 0.10f))
            );
        }

        int cloudLines = random.Next(2, 6);

        for (int i = 0; i < cloudLines; i++)
        {
            float x1 = RandomFloat(0f, textureWidth);
            float y1 = RandomFloat(0f, textureHeight);

            float x2 = x1 + RandomFloat(-150f, 150f);
            float y2 = y1 + RandomFloat(-80f, 80f);

            DrawThickLine(
                new Vector2(x1, y1),
                new Vector2(x2, y2),
                random.Next(1, 4),
                GetRandomSpaceColor(RandomFloat(0.025f, 0.08f))
            );
        }
    }

    private void AddGlow(
        float centerX,
        float centerY,
        float radiusX,
        float radiusY,
        Color color
    )
    {
        if (radiusX <= 0f || radiusY <= 0f)
            return;

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

                float alpha =
                    Mathf.Pow(1f - distance, 2f) *
                    color.a;

                BlendPixel(
                    x,
                    y,
                    new Color(
                        color.r,
                        color.g,
                        color.b,
                        alpha
                    )
                );
            }
        }
    }

    // =========================================================
    // STARS
    // =========================================================

    private void DrawStars(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int x = random.Next(4, textureWidth - 4);
            int y = random.Next(4, textureHeight - 4);

            int radius;
            int sizeRoll = random.Next(0, 100);

            if (sizeRoll < 72)
                radius = 1;
            else if (sizeRoll < 95)
                radius = 2;
            else
                radius = random.Next(2, 4);

            float alpha = RandomFloat(0.35f, 1f);
            Color starColor = GetRandomStarColor(alpha);

            if (radius >= 3)
            {
                DrawCircle(
                    x,
                    y,
                    radius * 4,
                    new Color(
                        starColor.r,
                        starColor.g,
                        starColor.b,
                        0.045f
                    )
                );
            }

            DrawCircle(
                x,
                y,
                radius,
                starColor
            );

            if (
                drawCrossStars &&
                radius >= 2 &&
                random.Next(0, 4) == 0
            )
            {
                Color rayColor = new Color(
                    starColor.r,
                    starColor.g,
                    starColor.b,
                    alpha * 0.7f
                );

                DrawLine(
                    x - radius * 3,
                    y,
                    x + radius * 3,
                    y,
                    rayColor
                );

                DrawLine(
                    x,
                    y - radius * 3,
                    x,
                    y + radius * 3,
                    rayColor
                );
            }
        }
    }

    private Color GetRandomStarColor(float alpha)
    {
        Color color;

        switch (random.Next(0, 5))
        {
            case 0:
                color = blueStar;
                break;

            case 1:
                color = purpleStar;
                break;

            case 2:
                color = cyanStar;
                break;

            case 3:
                color = whiteStar;
                break;

            default:
                color = Color.Lerp(
                    whiteStar,
                    blueStar,
                    RandomFloat(0.2f, 0.8f)
                );
                break;
        }

        color.a = alpha;
        return color;
    }

    private Color GetRandomSpaceColor(float alpha)
    {
        Color color;

        switch (random.Next(0, 5))
        {
            case 0:
                color = new Color(0.12f, 0.25f, 1f, alpha);
                break;

            case 1:
                color = new Color(0.55f, 0.12f, 1f, alpha);
                break;

            case 2:
                color = new Color(0.04f, 0.75f, 0.85f, alpha);
                break;

            case 3:
                color = new Color(0.85f, 0.12f, 0.55f, alpha);
                break;

            default:
                color = new Color(0.12f, 0.38f, 0.8f, alpha);
                break;
        }

        color.a = alpha;
        return color;
    }

    // =========================================================
    // CONSTELLATIONS
    // =========================================================

    private void DrawConstellations()
    {
        int constellationCount = random.Next(1, 3);

        for (int c = 0; c < constellationCount; c++)
        {
            int pointCount = random.Next(4, 7);
            Vector2[] points = new Vector2[pointCount];

            float centerX = RandomFloat(
                textureWidth * 0.15f,
                textureWidth * 0.85f
            );

            float centerY = RandomFloat(
                textureHeight * 0.15f,
                textureHeight * 0.85f
            );

            float spread = RandomFloat(40f, 100f);

            for (int i = 0; i < pointCount; i++)
            {
                points[i] = new Vector2(
                    Mathf.Clamp(
                        centerX + RandomFloat(-spread, spread),
                        8f,
                        textureWidth - 8f
                    ),
                    Mathf.Clamp(
                        centerY + RandomFloat(-spread, spread),
                        8f,
                        textureHeight - 8f
                    )
                );
            }

            Color lineColor = new Color(
                0.35f,
                0.75f,
                1f,
                RandomFloat(0.08f, 0.20f)
            );

            for (int i = 0; i < pointCount - 1; i++)
            {
                DrawLine(
                    Mathf.RoundToInt(points[i].x),
                    Mathf.RoundToInt(points[i].y),
                    Mathf.RoundToInt(points[i + 1].x),
                    Mathf.RoundToInt(points[i + 1].y),
                    lineColor
                );
            }

            for (int i = 0; i < pointCount; i++)
            {
                DrawCircle(
                    Mathf.RoundToInt(points[i].x),
                    Mathf.RoundToInt(points[i].y),
                    random.Next(1, 3),
                    whiteStar
                );
            }
        }
    }

    // =========================================================
    // PLANETS (MINIMAL для Tiled)
    // =========================================================

    private void DrawPlanets()
    {
        int planetCount = random.Next(
            minPlanets,
            maxPlanets + 1
        );

        for (int i = 0; i < planetCount; i++)
        {
            int radius = Mathf.Max(
                16,
                textureWidth / 12
            );

            int x = random.Next(
                radius + seamBlendWidth,
                textureWidth - radius - seamBlendWidth
            );

            int y = random.Next(
                radius + seamBlendWidth,
                textureHeight - radius - seamBlendWidth
            );

            DrawPlanetAt(x, y, radius);
        }
    }

    private void DrawPlanetAt(
        int centerX,
        int centerY,
        int radius
    )
    {
        Color planetColor = GetRandomStarColor(1f);

        DrawCircle(
            centerX,
            centerY,
            radius + radius / 3,
            new Color(
                planetColor.r,
                planetColor.g,
                planetColor.b,
                0.055f
            )
        );

        DrawCircle(
            centerX,
            centerY,
            radius,
            new Color(
                planetColor.r * 0.2f,
                planetColor.g * 0.2f,
                planetColor.b * 0.3f,
                1f
            )
        );

        int detailCount = random.Next(8, 16);

        for (int i = 0; i < detailCount; i++)
        {
            int x = centerX + random.Next(-radius, radius);
            int y = centerY + random.Next(-radius, radius);

            float distance = Vector2.Distance(
                new Vector2(x, y),
                new Vector2(centerX, centerY)
            );

            if (distance > radius)
                continue;

            int detailRadius = random.Next(
                Mathf.Max(1, radius / 16),
                Mathf.Max(2, radius / 6)
            );

            DrawCircle(
                x,
                y,
                detailRadius,
                new Color(
                    planetColor.r,
                    planetColor.g,
                    planetColor.b,
                    RandomFloat(0.08f, 0.28f)
                )
            );
        }

        DrawCircleOutline(
            centerX,
            centerY,
            radius,
            new Color(
                planetColor.r,
                planetColor.g,
                planetColor.b,
                0.45f
            ),
            Mathf.Max(1, radius / 24)
        );
    }

    // =========================================================
    // METEORS
    // =========================================================

    private void DrawMeteors()
    {
        int meteorCount = random.Next(0, 3);

        for (int i = 0; i < meteorCount; i++)
        {
            float startX = RandomFloat(seamBlendWidth, textureWidth - seamBlendWidth);
            float startY = RandomFloat(seamBlendWidth, textureHeight - seamBlendWidth);

            float length = RandomFloat(40f, 100f);

            Vector2 start = new Vector2(startX, startY);
            Vector2 end = new Vector2(
                startX + length,
                startY - length * RandomFloat(0.25f, 0.65f)
            );

            Color meteorColor = GetRandomStarColor(
                RandomFloat(0.5f, 1f)
            );

            DrawThickLine(
                start,
                end,
                random.Next(2, 4),
                new Color(
                    meteorColor.r,
                    meteorColor.g,
                    meteorColor.b,
                    0.13f
                )
            );

            DrawLine(
                Mathf.RoundToInt(start.x),
                Mathf.RoundToInt(start.y),
                Mathf.RoundToInt(end.x),
                Mathf.RoundToInt(end.y),
                meteorColor
            );

            DrawCircle(
                Mathf.RoundToInt(start.x),
                Mathf.RoundToInt(start.y),
                random.Next(2, 4),
                whiteStar
            );
        }
    }

    // =========================================================
    // DRAWING
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

                BlendPixel(
                    centerX + x,
                    centerY + y,
                    color
                );
            }
        }
    }

    private void DrawCircleOutline(
        int centerX,
        int centerY,
        int radius,
        Color color,
        int thickness
    )
    {
        if (radius <= 0)
            return;

        thickness = Mathf.Max(1, thickness);
        int segments = 80;

        for (int t = 0; t < thickness; t++)
        {
            float currentRadius = radius - t;

            Vector2 previous = new Vector2(
                centerX + currentRadius,
                centerY
            );

            for (int i = 1; i <= segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;

                Vector2 current = new Vector2(
                    centerX + Mathf.Cos(angle) * currentRadius,
                    centerY + Mathf.Sin(angle) * currentRadius
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
            BlendPixel(x1, y1, color);

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
        thickness = Mathf.Max(1, thickness);

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

    // =========================================================
    // PIXELS
    // =========================================================

    private void BlendPixel(
        int x,
        int y,
        Color color
    )
    {
        if (x < 0 || x >= textureWidth)
            return;

        if (y < 0 || y >= textureHeight)
            return;

        int index = y * textureWidth + x;

        Color oldColor = pixels[index];

        float alpha = Mathf.Clamp01(color.a);

        pixels[index] = Color.Lerp(
            oldColor,
            new Color(
                color.r,
                color.g,
                color.b,
                1f
            ),
            alpha
        );
    }

    private void SetPixelDirect(
        int x,
        int y,
        Color color
    )
    {
        if (x < 0 || x >= textureWidth)
            return;

        if (y < 0 || y >= textureHeight)
            return;

        pixels[y * textureWidth + x] = color;
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void DestroyGeneratedObjects()
    {
        if (generatedSprite != null)
        {
            if (Application.isPlaying)
                Destroy(generatedSprite);
            else
                DestroyImmediate(generatedSprite);

            generatedSprite = null;
        }

        if (texture != null)
        {
            if (Application.isPlaying)
                Destroy(texture);
            else
                DestroyImmediate(texture);

            texture = null;
        }
    }
}
