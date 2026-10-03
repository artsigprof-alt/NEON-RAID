using UnityEngine;

public class ArenaGrid : MonoBehaviour
{
    [SerializeField] private int horizontalLines = 11;
    [SerializeField] private int verticalLines = 19;

    [SerializeField] private float spacing = 2f;
    [SerializeField] private float lineWidth = 0.025f;

    [SerializeField] private Color gridColor =
        new Color(0.05f, 0.5f, 1f, 0.35f);

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        float width =
            (verticalLines - 1) * spacing;

        float height =
            (horizontalLines - 1) * spacing;

        for (int x = 0; x < verticalLines; x++)
        {
            GameObject lineObject =
                new GameObject($"Vertical_{x}");

            lineObject.transform.SetParent(
                transform
            );

            LineRenderer line =
                lineObject.AddComponent<LineRenderer>();

            SetupLine(line);

            float xPosition =
                -width * 0.5f +
                x * spacing;

            line.SetPosition(
                0,
                new Vector3(
                    xPosition,
                    -height * 0.5f,
                    0f
                )
            );

            line.SetPosition(
                1,
                new Vector3(
                    xPosition,
                    height * 0.5f,
                    0f
                )
            );
        }

        for (int y = 0; y < horizontalLines; y++)
        {
            GameObject lineObject =
                new GameObject($"Horizontal_{y}");

            lineObject.transform.SetParent(
                transform
            );

            LineRenderer line =
                lineObject.AddComponent<LineRenderer>();

            SetupLine(line);

            float yPosition =
                -height * 0.5f +
                y * spacing;

            line.SetPosition(
                0,
                new Vector3(
                    -width * 0.5f,
                    yPosition,
                    0f
                )
            );

            line.SetPosition(
                1,
                new Vector3(
                    width * 0.5f,
                    yPosition,
                    0f
                )
            );
        }
    }

    private void SetupLine(LineRenderer line)
    {
        line.positionCount = 2;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.startColor = gridColor;
        line.endColor = gridColor;

        line.useWorldSpace = true;
    }
}