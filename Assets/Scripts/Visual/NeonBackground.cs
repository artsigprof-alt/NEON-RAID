using UnityEngine;

public class NeonBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.15f;
    [SerializeField] private float parallax = 0.08f;

    private Material material;
    private Vector2 offset;

    private Transform player;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            material = renderer.material;
        }

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (material == null)
            return;

        offset.y += scrollSpeed * Time.deltaTime;

        if (player != null)
        {
            offset +=
                new Vector2(
                    player.position.x,
                    player.position.y
                ) *
                parallax *
                Time.deltaTime;
        }

        material.mainTextureOffset = offset;
    }
}