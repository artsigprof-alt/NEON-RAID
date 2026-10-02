
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralLoadingVisual : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RawImage targetImage;
    [SerializeField] private TMP_Text statusText;

    [Header("Resolution")]
    [SerializeField] private int width = 640;
    [SerializeField] private int height = 360;

    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 18f;
    [SerializeField] private float pulseSpeed = 2.4f;
    [SerializeField] private float particleSpeed = 35f;
    [SerializeField] private float scanSpeed = 180f;

    [Header("Visual")]
    [SerializeField] private int starCount = 90;
    [SerializeField] private int nodeCount = 10;

    [SerializeField] private bool randomizeEveryTime = true;

    private Texture2D texture;

    private Color primaryColor;
    private Color secondaryColor;

    private Vector2 center;

    private float[] starX;
    private float[] starY;
    private float[] starSpeed;
    private float[] starSize;

    private Vector2[] nodes;

    private float rotation;
    private float pulse;
    private float scanPosition;

    private readonly string[] loadingMessages =
    {
        "INITIALIZING...",
        "CONNECTING TO NETWORK...",
        "SYNCING SYSTEM...",
        "LOADING SECTOR...",
        "CALIBRATING DRONE...",
        "ESTABLISHING LINK...",
        "PREPARING NEON RAID..."
    };

    private void OnEnable()
    {
        Generate();
    }

    public void Generate()
    {
        if (texture != null)
            Destroy(texture);

        Random.InitState(
            randomizeEveryTime
                ? System.Environment.TickCount
                : 12345
        );

        // =========================
        // RANDOM PALETTE
        // =========================

        Color[] palettes =
        {
            new Color(0.05f, 1f, 0.9f),
            new Color(0.25f, 0.55f, 1f),
            new Color(0.7f, 0.25f, 1f),
            new Color(1f, 0.2f, 0.65f),
            new Color(0.15f, 0.8f, 1f)
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
                0.35f
            );

        center =
            new Vector2(
                width * 0.5f,
                height * 0.5f
            );

        // =========================
        // TEXTURE
        // =========================

        texture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        CreateStars();
        CreateNodes();

        RedrawStaticBackground();

        texture.Apply();

        if (targetImage != null)
            targetImage.texture = texture;

        if (statusText != null)
        {
            statusText.text =
                loadingMessages[
                    Random.Range(
                        0,
                        loadingMessages.Length
                    )
                ];
        }

        rotation = 0f;
        pulse = 0f;
        scanPosition = -20f;
    }

    // =========================================================
    // STAR DATA
    // =========================================================

    private void CreateStars()
    {
        starX = new float[starCount];
        starY = new float[starCount];
        starSpeed = new float[starCount];
        starSize = new float[starCount];

        for (int i = 0; i < starCount; i++)
        {
            starX[i] =
                Random.Range(
                    0f,
                    width
                );

            starY[i] =
                Random.Range(
                    0f,
                    height
                );

            starSpeed[i] =
                Random.Range(
                    0.4f,
                    1.4f
                );

            starSize[i] =
                Random.Range(
                    0.6f,
                    2.5f
                );
        }
    }

    // =========================================================
    // NODE DATA
    // =========================================================

    private void CreateNodes()
    {
        nodes =
            new Vector2[nodeCount];

        for (int i = 0; i < nodeCount; i++)
        {
            nodes[i] =
                new Vector2(
                    Random.Range(
                        50f,
                        width - 50f
                    ),
                    Random.Range(
                        40f,
                        height - 40f
                    )
                );
        }
    }

    // =========================================================
    // STATIC BACKGROUND
    // =========================================================

    private void RedrawStaticBackground()
    {
        Color[] pixels =
            new Color[
                width *
                height
            ];

        for (int y = 0; y < height; y++)
        {
            float t =
                y / (float)(height - 1);

            Color row =
                Color.Lerp(
                    new Color(
                        0.005f,
                        0.008f,
                        0.025f
                    ),
                    new Color(
                        0.02f,
                        0.025f,
                        0.07f
                    ),
                    t
                );

            for (int x = 0; x < width; x++)
            {
                pixels[
                    y * width + x
                ] = row;
            }
        }

        texture.SetPixels(pixels);

        // =========================
        // GLOW
        // =========================

        DrawSoftGlow(
            center,
            120f,
            new Color(
                primaryColor.r,
                primaryColor.g,
                primaryColor.b,
                0.10f
            )
        );

        // =========================
        // GRID
        // =========================

        int gridStep = 40;

        for (int x = 0; x < width; x += gridStep)
        {
            DrawLine(
                x,
                0,
                x,
                height,
                new Color(
                    primaryColor.r,
                    primaryColor.g,
                    primaryColor.b,
                    0.035f
                )
            );
        }

        for (int y = 0; y < height; y += gridStep)
        {
            DrawLine(
                0,
                y,
                width,
                y,
                new Color(
                    secondaryColor.r,
                    secondaryColor.g,
                    secondaryColor.b,
                    0.035f
                )
            );
        }

        // =========================
        // RANDOM NODES
        // =========================

        for (int i = 0; i < nodes.Length; i++)
        {
            int connections =
                Random.Range(
                    1,
                    3
                );

            for (int j = 0; j < connections; j++)
            {
                int target =
                    Random.Range(
                        0,
                        nodes.Length
                    );

                if (target == i)
                    continue;

                DrawLine(
                    Mathf.RoundToInt(
                        nodes[i].x
                    ),
                    Mathf.RoundToInt(
                        nodes[i].y
                    ),
                    Mathf.RoundToInt(
                        nodes[target].x
                    ),
                    Mathf.RoundToInt(
                        nodes[target].y
                    ),
                    new Color(
                        primaryColor.r,
                        primaryColor.g,
                        primaryColor.b,
                        0.08f
                    )
                );
            }
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            DrawCircle(
                Mathf.RoundToInt(
                    nodes[i].x
                ),
                Mathf.RoundToInt(
                    nodes[i].y
                ),
                2,
                primaryColor
            );
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (texture == null)
            return;

        rotation +=
            rotationSpeed *
            Time.unscaledDeltaTime;

        pulse +=
            pulseSpeed *
            Time.unscaledDeltaTime;

        scanPosition +=
            scanSpeed *
            Time.unscaledDeltaTime;

        if (scanPosition > height + 30f)
            scanPosition = -30f;

        AnimateStars();

        DrawAnimatedCore();

        DrawOrbitRings();

        DrawScanLine();

        texture.Apply(
            false,
            false
        );

        UpdateStatusText();
    }

    // =========================================================
    // STARS
    // =========================================================

    private void AnimateStars()
    {
        for (int i = 0; i < starCount; i++)
        {
            starY[i] -=
                starSpeed[i] *
                particleSpeed *
                Time.unscaledDeltaTime;

            if (starY[i] < 0f)
            {
                starY[i] =
                    height;

                starX[i] =
                    Random.Range(
                        0f,
                        width
                    );
            }

            float flicker =
                0.55f +
                Mathf.Sin(
                    Time.unscaledTime *
                    (1.5f + starSpeed[i])
                ) *
                0.25f;

            Color starColor =
                Color.Lerp(
                    primaryColor,
                    secondaryColor,
                    Random.value * 0.2f
                );

            starColor *=
                flicker;

            DrawCircle(
                Mathf.RoundToInt(starX[i]),
                Mathf.RoundToInt(starY[i]),
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        starSize[i]
                    )
                ),
                starColor
            );
        }
    }

    // =========================================================
    // CORE
    // =========================================================

    private void DrawAnimatedCore()
    {
        float pulseValue =
            1f +
            Mathf.Sin(pulse) *
            0.15f;

        int radius =
            Mathf.RoundToInt(
                20f *
                pulseValue
            );

        DrawSoftGlow(
            center,
            radius * 2.5f,
            new Color(
                primaryColor.r,
                primaryColor.g,
                primaryColor.b,
                0.06f
            )
        );

        DrawCircle(
            Mathf.RoundToInt(center.x),
            Mathf.RoundToInt(center.y),
            radius,
            new Color(
                primaryColor.r,
                primaryColor.g,
                primaryColor.b,
                0.35f
            )
        );

        DrawCircle(
            Mathf.RoundToInt(center.x),
            Mathf.RoundToInt(center.y),
            Mathf.Max(
                4,
                radius / 2
            ),
            secondaryColor
        );

        DrawCircle(
            Mathf.RoundToInt(center.x),
            Mathf.RoundToInt(center.y),
            3,
            Color.white
        );
    }

    // =========================================================
    // ORBITS
    // =========================================================

    private void DrawOrbitRings()
    {
        float[] radii =
        {
            45f,
            75f,
            105f
        };

        for (int r = 0; r < radii.Length; r++)
        {
            float radius =
                radii[r];

            int segments = 120;

            Vector2 previous =
                GetOrbitPoint(
                    radius,
                    rotation +
                    r * 40f
                );

            for (int i = 1;
                 i <= segments;
                 i++)
            {
                float angle =
                    rotation +
                    r * 40f +
                    i *
                    (360f /
                     segments);

                Vector2 current =
                    GetOrbitPoint(
                        radius,
                        angle
                    );

                DrawLine(
                    Mathf.RoundToInt(
                        previous.x
                    ),
                    Mathf.RoundToInt(
                        previous.y
                    ),
                    Mathf.RoundToInt(
                        current.x
                    ),
                    Mathf.RoundToInt(
                        current.y
                    ),
                    new Color(
                        primaryColor.r,
                        primaryColor.g,
                        primaryColor.b,
                        0.10f
                    )
                );

                previous = current;
            }
        }

        // Moving orbit dots.
        for (int i = 0; i < radii.Length; i++)
        {
            float angle =
                rotation *
                (1f + i * 0.45f) +
                i * 110f;

            Vector2 point =
                GetOrbitPoint(
                    radii[i],
                    angle
                );

            DrawCircle(
                Mathf.RoundToInt(point.x),
                Mathf.RoundToInt(point.y),
                3,
                secondaryColor
            );
        }
    }

    private Vector2 GetOrbitPoint(
        float radius,
        float angle
    )
    {
        float radians =
            angle *
            Mathf.Deg2Rad;

        return new Vector2(
            center.x +
            Mathf.Cos(radians) *
            radius,

            center.y +
            Mathf.Sin(radians) *
            radius *
            0.45f
        );
    }

    // =========================================================
    // SCAN LINE
    // =========================================================

    private void DrawScanLine()
    {
        int y =
            Mathf.RoundToInt(
                scanPosition
            );

        if (y < 0 ||
            y >= height)
            return;

        for (int x = 0;
             x < width;
             x++)
        {
            float distance =
                Mathf.Abs(
                    x -
                    center.x
                ) /
                width;

            float alpha =
                0.20f -
                distance * 0.10f;

            Color lineColor =
                new Color(
                    primaryColor.r,
                    primaryColor.g,
                    primaryColor.b,
                    alpha
                );

            SetPixelSafe(
                x,
                y,
                lineColor
            );

            if (y + 1 < height)
            {
                lineColor.a *= 0.25f;

                SetPixelSafe(
                    x,
                    y + 1,
                    lineColor
                );
            }
        }
    }

    // =========================================================
    // STATUS TEXT
    // =========================================================

    private float lastMessageChange;

    private void UpdateStatusText()
    {
        if (statusText == null)
            return;

        if (
            Time.unscaledTime -
            lastMessageChange >
            1.8f
        )
        {
            statusText.text =
                loadingMessages[
                    Random.Range(
                        0,
                        loadingMessages.Length
                    )
                ];

            lastMessageChange =
                Time.unscaledTime;
        }

        int dots =
            Mathf.FloorToInt(
                Time.unscaledTime * 2f
            ) % 4;

        string dotString =
            new string(
                '.',
                dots
            );

        string baseText =
            statusText.text;

        int dotIndex =
            baseText.LastIndexOf('.');

        if (dotIndex >= 0)
        {
            baseText =
                baseText.Substring(
                    0,
                    dotIndex
                );
        }

        statusText.text =
            baseText +
            dotString;
    }

    // =========================================================
    // DRAWING
    // =========================================================

    private void DrawSoftGlow(
        Vector2 centerPosition,
        float radius,
        Color color
    )
    {
        int minX =
            Mathf.Max(
                0,
                Mathf.FloorToInt(
                    centerPosition.x - radius
                )
            );

        int maxX =
            Mathf.Min(
                width - 1,
                Mathf.CeilToInt(
                    centerPosition.x + radius
                )
            );

        int minY =
            Mathf.Max(
                0,
                Mathf.FloorToInt(
                    centerPosition.y - radius
                )
            );

        int maxY =
            Mathf.Min(
                height - 1,
                Mathf.CeilToInt(
                    centerPosition.y + radius
                )
            );

        for (int y = minY;
             y <= maxY;
             y++)
        {
            for (int x = minX;
                 x <= maxX;
                 x++)
            {
                float distance =
                    Vector2.Distance(
                        new Vector2(
                            x,
                            y
                        ),
                        centerPosition
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
                        2.5f
                    ) *
                    color.a;

                Color c =
                    color;

                c.a =
                    alpha;

                BlendPixel(
                    x,
                    y,
                    c
                );
            }
        }
    }

    private void DrawCircle(
        int cx,
        int cy,
        int radius,
        Color color
    )
    {
        int radiusSquared =
            radius *
            radius;

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

    private void BlendPixel(
        int x,
        int y,
        Color color
    )
    {
        if (
            x < 0 ||
            x >= width ||
            y < 0 ||
            y >= height
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

        result.a = 1f;

        texture.SetPixel(
            x,
            y,
            result
        );
    }

    private void SetPixelSafe(
        int x,
        int y,
        Color color
    )
    {
        if (
            x < 0 ||
            x >= width ||
            y < 0 ||
            y >= height
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

        result.a = 1f;

        texture.SetPixel(
            x,
            y,
            result
        );
    }

    private void OnDestroy()
    {
        if (texture != null)
            Destroy(texture);
    }
}
