using UnityEngine;

/// <summary>
/// Lightweight Matrix-style text animation for the computer prefab.
/// Uses Unity's built-in TextMesh component; no TextMeshPro package is required.
/// </summary>
public class MatrixRainDisplay : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 0.32f;
    [SerializeField] private float lowerLimit = -0.46f;
    [SerializeField] private float upperLimit = 0.46f;

    private TextMesh[] columns;

    private const string Characters =
        "01ABCDEFGHIJKLMNOPQRSTUVWXYZ#$%&*+-=<>";

    private void Start()
    {
        columns = GetComponentsInChildren<TextMesh>();

        for (int i = 0; i < columns.Length; i++)
        {
            columns[i].text = MakeColumnText();
        }
    }

    private void Update()
    {
        if (columns == null)
            return;

        for (int i = 0; i < columns.Length; i++)
        {
            TextMesh column = columns[i];
            if (column == null)
                continue;

            Vector3 position = column.transform.localPosition;
            position.y -= fallSpeed * Time.deltaTime;

            if (position.y < lowerLimit)
            {
                position.y = upperLimit + Random.Range(0f, 0.22f);
                column.text = MakeColumnText();
            }

            column.transform.localPosition = position;
        }
    }

    private string MakeColumnText()
    {
        const int rows = 11;
        System.Text.StringBuilder builder = new System.Text.StringBuilder(rows * 2);

        for (int row = 0; row < rows; row++)
        {
            int index = Random.Range(0, Characters.Length);
            builder.Append(Characters[index]);

            if (row < rows - 1)
                builder.Append('\n');
        }

        return builder.ToString();
    }
}
