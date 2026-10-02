
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProceduralBulletSprite : MonoBehaviour
{
    private enum BulletStyle
    {
        Plasma,
        Laser,
        Shard,
        EnergyCore,
        Ring,
        Arrow
    }

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private int textureSize = 32;

    [SerializeField] private float pixelsPerUnit = 16f;

    [Header("Randomization")]
    [SerializeField] private bool randomizeSize = true;

    [SerializeField, Range(0.7f, 1.4f)]
    private float minSize = 0.75f;

    [SerializeField, Range(0.7f, 1.8f)]
    private float maxSize = 1.25f;

    [SerializeField] private bool randomizeRotation = true;

    private Texture2D texture;
    private Sprite sprite;

    private Color primaryColor;
    private Color secondaryColor;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Generate();
    }

    public void Generate()
    {
        RandomizeColors();

        BulletStyle style =
            (BulletStyle)Random.Range(
                0,
                System.Enum.GetValues(
                    typeof(BulletStyle)
                ).Length
            );

        texture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        ClearTexture();

        switch (style)
        {
            case BulletStyle.Plasma:
                GeneratePlasma();
                break;

            case BulletStyle.Laser:
                GenerateLaser();
                break;

            case BulletStyle.Shard:
                GenerateShard();
                break;

            case BulletStyle.EnergyCore:
                GenerateEnergyCore();
                break;

            case BulletStyle.Ring:
                GenerateRing();
                break;

            case BulletStyle.Arrow:
                GenerateArrow();
                break;
        }

        texture.Apply();

        sprite = Sprite.Create(
            texture,
            new Rect(
                0,
                0,
                textureSize,
                textureSize
            ),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit
        );

        sprite.name =
            "RandomBulletSprite_" +
            Random.Range(0, 999999);

        spriteRenderer.sprite = sprite;

        if (randomizeSize)
        {
            float scale =
                Random.Range(
                    minSize,
                    maxSize
                );

            transform.localScale =
                Vector3.one * scale;
        }

        if (randomizeRotation)
        {
            transform.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    Random.Range(
                        -20f,
                        20f
                    )
                );
        }
    }

    // =========================================================
    // COLORS
    // =========================================================

    private void RandomizeColors()
    {
        Color[] palettes =
        {
            new Color(0.1f, 1f, 0.9f),
            new Color(0.2f, 0.55f, 1f),
            new Color(0.75f, 0.25f, 1f),
            new Color(1f, 0.2f, 0.65f),
            new Color(1f, 0.55f, 0.15f),
            new Color(0.85f, 1f, 0.3f),
            new Color(1f, 0.25f, 0.25f),
            new Color(0.4f, 1f, 0.45f)
        };

        primaryColor =
            palettes[
                Random.Range(
                    0,
                    palettes.Length
                )
            ];

        secondaryColor =
            Color.Lerp(
                primaryColor,
                Color.white,
                Random.Range(
                    0.25f,
                    0.7f
                )
            );
    }

    // =========================================================
    // PLASMA
    // =========================================================

    private void GeneratePlasma()
    {
        int center =
            textureSize / 2;

        int radius =
            Random.Range(
                textureSize / 5,
                textureSize / 3
            );

        DrawGlowCircle(
            center,
            center,
            radius,
            primaryColor
        );

        DrawCircle(
            center,
            center,
            Mathf.Max(
                2,
                radius / 2
            ),
            secondaryColor
        );

        // Random plasma sparks.
        int sparks =
            Random.Range(2, 6);

        for (int i = 0; i < sparks; i++)
        {
            int x =
                center +
                Random.Range(
                    -radius,
                    radius + 1
                );

            int y =
                center +
                Random.Range(
                    -radius,
                    radius + 1
                );

            DrawCircle(
                x,
                y,
                Random.Range(1, 3),
                secondaryColor
            );
        }
    }

    // =========================================================
    // LASER
    // =========================================================

    private void GenerateLaser()
    {
        int center =
            textureSize / 2;

        int length =
            Random.Range(
                textureSize / 3,
                textureSize / 2
            );

        int thickness =
            Random.Range(2, 5);

        DrawGlowLine(
            center - length,
            center,
            center + length,
            center,
            thickness,
            primaryColor
        );

        DrawLine(
            center - length,
            center,
            center + length,
            center,
            secondaryColor
        );

        // Small random side flare.
        if (Random.value > 0.45f)
        {
            DrawLine(
                center,
                center - 5,
                center,
                center + 5,
                new Color(
                    primaryColor.r,
                    primaryColor.g,
                    primaryColor.b,
                    0.7f
                )
            );
        }
    }

    // =========================================================
    // SHARD
    // =========================================================

    private void GenerateShard()
    {
        int center =
            textureSize / 2;

        int length =
            Random.Range(
                9,
                15
            );

        int width =
            Random.Range(
                3,
                6
            );

        Vector2[] points =
        {
            new Vector2(
                center - length,
                center
            ),

            new Vector2(
                center - width,
                center + width
            ),

            new Vector2(
                center + length,
                center
            ),

            new Vector2(
                center - width,
                center - width
            )
        };

        FillPolygon(
            points,
            primaryColor
        );

        DrawLine(
            center - length,
            center,
            center + length,
            center,
            secondaryColor
        );
    }

    // =========================================================
    // ENERGY CORE
    // =========================================================

    private void GenerateEnergyCore()
    {
        int center =
            textureSize / 2;

        int radius =
            Random.Range(
                5,
                9
            );

        DrawGlowCircle(
            center,
            center,
            radius + 3,
            primaryColor
        );

        DrawCircle(
            center,
            center,
            radius,
            secondaryColor
        );

        DrawCircle(
            center,
            center,
            Mathf.Max(
                2,
                radius / 3
            ),
            Color.white
        );
    }

    // =========================================================
    // RING
    // =========================================================

    private void GenerateRing()
    {
        int center =
            textureSize / 2;

        int radius =
            Random.Range(
                6,
                10
            );

        int thickness =
            Random.Range(
                2,
                4
            );

        DrawRing(
            center,
            center,
            radius,
            thickness,
            primaryColor
        );

        DrawCircle(
            center,
            center,
            Random.Range(1, 3),
            secondaryColor
        );
    }

    // =========================================================
    // ARROW
    // =========================================================

    private void GenerateArrow()
    {
        int center =
            textureSize / 2;

        int length =
            Random.Range(
                8,
                14
            );

        int arrowWidth =
            Random.Range(
                5,
                8
            );

        DrawLine(
            center - length,
            center,
            center + length,
            center,
            primaryColor
        );

        DrawLine(
            center + length,
            center,
            center + length - arrowWidth,
            center + arrowWidth,
            primaryColor
        );

        DrawLine(
            center + length,
            center,
            center + length - arrowWidth,
            center - arrowWidth,
            primaryColor
        );

        DrawLine(
            center - length + 4,
            center,
            center + length - 4,
            center,
            secondaryColor
        );
    }

    // =========================================================
    // DRAW
    // =========================================================

    private void ClearTexture()
    {
        Color[] pixels =
            new Color[
                textureSize *
                textureSize
            ];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;

        texture.SetPixels(pixels);
    }

    private void DrawCircle(
        int cx,
        int cy,
        int radius,
        Color color
    )
    {
        int radiusSquared =
            radius * radius;

        for (int y = -radius;
             y <= radius;
             y++)
        {
            for (int x = -radius;
                 x <= radius;
                 x++)
            {
                if (
                    x * x +
                    y * y <=
                    radiusSquared
                )
                {
                    BlendPixel(
                        cx + x,
                        cy + y,
                        color
                    );
                }
            }
        }
    }

    private void DrawGlowCircle(
        int cx,
        int cy,
        int radius,
        Color color
    )
    {
        for (int y = -radius;
             y <= radius;
             y++)
        {
            for (int x = -radius;
                 x <= radius;
                 x++)
            {
                float distance =
                    Mathf.Sqrt(
                        x * x +
                        y * y
                    );

                if (distance > radius)
                    continue;

                float alpha =
                    1f -
                    distance /
                    radius;

                alpha =
                    Mathf.Pow(
                        alpha,
                        2.4f
                    );

                Color glow =
                    color;

                glow.a =
                    alpha *
                    0.55f;

                BlendPixel(
                    cx + x,
                    cy + y,
                    glow
                );
            }
        }
    }

    private void DrawGlowLine(
        int x1,
        int y1,
        int x2,
        int y2,
        int thickness,
        Color color
    )
    {
        for (int i = -thickness;
             i <= thickness;
             i++)
        {
            Color glow = color;

            float alpha =
                0.25f *
                (1f -
                Mathf.Abs(i) /
                (float)(thickness + 1));

            glow.a = alpha;

            DrawLine(
                x1,
                y1 + i,
                x2,
                y2 + i,
                glow
            );
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
        int dx =
            Mathf.Abs(
                x2 - x1
            );

        int dy =
            Mathf.Abs(
                y2 - y1
            );

        int sx =
            x1 < x2 ? 1 : -1;

        int sy =
            y1 < y2 ? 1 : -1;

        int error =
            dx - dy;

        while (true)
        {
            BlendPixel(
                x1,
                y1,
                color
            );

            if (
                x1 == x2 &&
                y1 == y2
            )
                break;

            int e2 =
                error * 2;

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

    private void DrawRing(
        int cx,
        int cy,
        int radius,
        int thickness,
        Color color
    )
    {
        int outer =
            radius;

        int inner =
            radius - thickness;

        for (int y = -outer;
             y <= outer;
             y++)
        {
            for (int x = -outer;
                 x <= outer;
                 x++)
            {
                float distance =
                    Mathf.Sqrt(
                        x * x +
                        y * y
                    );

                if (
                    distance <= outer &&
                    distance >= inner
                )
                {
                    BlendPixel(
                        cx + x,
                        cy + y,
                        color
                    );
                }
            }
        }
    }

    private void FillPolygon(
        Vector2[] points,
        Color color
    )
    {
        if (points == null ||
            points.Length < 3)
            return;

        int minX = textureSize;
        int maxX = 0;
        int minY = textureSize;
        int maxY = 0;

        foreach (Vector2 point in points)
        {
            minX =
                Mathf.Min(
                    minX,
                    Mathf.RoundToInt(point.x)
                );

            maxX =
                Mathf.Max(
                    maxX,
                    Mathf.RoundToInt(point.x)
                );

            minY =
                Mathf.Min(
                    minY,
                    Mathf.RoundToInt(point.y)
                );

            maxY =
                Mathf.Max(
                    maxY,
                    Mathf.RoundToInt(point.y)
                );
        }

        for (int y = minY;
             y <= maxY;
             y++)
        {
            for (int x = minX;
                 x <= maxX;
                 x++)
            {
                if (PointInsidePolygon(
                    x,
                    y,
                    points
                ))
                {
                    BlendPixel(
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    private bool PointInsidePolygon(
        float x,
        float y,
        Vector2[] polygon
    )
    {
        bool inside = false;

        for (
            int i = 0,
            j = polygon.Length - 1;
            i < polygon.Length;
            j = i++
        )
        {
            float xi = polygon[i].x;
            float yi = polygon[i].y;

            float xj = polygon[j].x;
            float yj = polygon[j].y;

            bool intersect =
                ((yi > y) != (yj > y)) &&
                (
                    x <
                    (xj - xi) *
                    (y - yi) /
                    (yj - yi + 0.00001f) +
                    xi
                );

            if (intersect)
                inside = !inside;
        }

        return inside;
    }

    private void BlendPixel(
        int x,
        int y,
        Color color
    )
    {
        if (
            x < 0 ||
            x >= textureSize ||
            y < 0 ||
            y >= textureSize
        )
            return;

        Color old =
            texture.GetPixel(
                x,
                y
            );

        Color result =
            Color.Lerp(
                old,
                color,
                color.a
            );

        result.a =
            Mathf.Max(
                old.a,
                color.a
            );

        texture.SetPixel(
            x,
            y,
            result
        );
    }

    private void OnDestroy()
    {
        if (sprite != null)
            Destroy(sprite);

        if (texture != null)
            Destroy(texture);
    }
}

