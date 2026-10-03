using System.Collections;
using UnityEngine;

public class CombatVFX : MonoBehaviour
{
    public static CombatVFX Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static void SpawnMuzzle(
        Vector2 position,
        Vector2 direction)
    {
        if (Instance == null)
            return;

        Instance.StartCoroutine(
            Instance.MuzzleRoutine(
                position,
                direction
            )
        );
    }

    public static void SpawnHit(
        Vector2 position)
    {
        if (Instance == null)
            return;

        Instance.StartCoroutine(
            Instance.HitRoutine(
                position
            )
        );
    }

    public static void SpawnExplosion(
        Vector2 position,
        float scale = 1f)
    {
        if (Instance == null)
            return;

        Instance.StartCoroutine(
            Instance.ExplosionRoutine(
                position,
                scale
            )
        );
    }

    public static void SpawnDamageFlash(
        Vector2 position)
    {
        if (Instance == null)
            return;

        Instance.StartCoroutine(
            Instance.DamageRoutine(
                position
            )
        );
    }

    private IEnumerator MuzzleRoutine(
        Vector2 position,
        Vector2 direction)
    {
        GameObject obj =
            CreateEffectObject(
                position,
                Color.yellow
            );

        obj.transform.localScale =
            Vector3.one * 0.16f;

        yield return
            ScaleAndDestroy(
                obj,
                0.08f,
                0.35f
            );
    }

    private IEnumerator HitRoutine(
        Vector2 position)
    {
        GameObject obj =
            CreateEffectObject(
                position,
                Color.white
            );

        obj.transform.localScale =
            Vector3.one * 0.12f;

        yield return
            ScaleAndDestroy(
                obj,
                0.1f,
                0.6f
            );
    }

    private IEnumerator DamageRoutine(
        Vector2 position)
    {
        GameObject obj =
            CreateEffectObject(
                position,
                Color.red
            );

        obj.transform.localScale =
            Vector3.one * 0.35f;

        yield return
            ScaleAndDestroy(
                obj,
                0.12f,
                0.15f
            );
    }

    private IEnumerator ExplosionRoutine(
        Vector2 position,
        float scale)
    {
        GameObject root =
            new GameObject(
                "ExplosionFX"
            );

        root.transform.position =
            position;

        for (int i = 0; i < 8; i++)
        {
            GameObject piece =
                CreateEffectObject(
                    position,
                    i % 2 == 0
                        ? Color.yellow
                        : Color.red
                );

            float angle =
                i * 45f *
                Mathf.Deg2Rad;

            Vector2 direction =
                new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                );

            piece.transform.localScale =
                Vector3.one *
                (0.12f * scale);

            StartCoroutine(
                ExplosionPieceRoutine(
                    piece,
                    direction,
                    scale
                )
            );
        }

        yield return
            new WaitForSeconds(
                0.45f
            );

        Destroy(root);
    }

    private IEnumerator ExplosionPieceRoutine(
        GameObject obj,
        Vector2 direction,
        float scale)
    {
        Vector3 start =
            obj.transform.position;

        float timer = 0f;
        float duration = 0.35f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                timer / duration;

            obj.transform.position =
                start +
                (Vector3)(
                    direction *
                    t *
                    (1.5f * scale)
                );

            obj.transform.localScale =
                Vector3.one *
                Mathf.Lerp(
                    0.15f * scale,
                    0f,
                    t
                );

            yield return null;
        }

        Destroy(obj);
    }

    private IEnumerator ScaleAndDestroy(
        GameObject obj,
        float duration,
        float endScale)
    {
        Vector3 startScale =
            obj.transform.localScale;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                timer / duration;

            obj.transform.localScale =
                Vector3.Lerp(
                    startScale,
                    startScale *
                    endScale,
                    t
                );

            yield return null;
        }

        Destroy(obj);
    }

    private GameObject CreateEffectObject(
        Vector2 position,
        Color color)
    {
        GameObject obj =
            new GameObject(
                "CombatFX"
            );

        obj.transform.position =
            position;

        SpriteRenderer renderer =
            obj.AddComponent<SpriteRenderer>();

        renderer.sprite =
            CreateSquareSprite();

        renderer.color =
            color;

        return obj;
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D texture =
            Texture2D.whiteTexture;

        return Sprite.Create(
            texture,
            new Rect(
                0,
                0,
                texture.width,
                texture.height
            ),
            new Vector2(
                0.5f,
                0.5f
            )
        );
    }
}