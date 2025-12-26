using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BossBoundary : MonoBehaviour
{
    [Header("경계선 설정")]
    public Color boundaryColor = Color.red;
    public float lineWidth = 0.3f;
    public float lineLength = 50f;

    private LineRenderer lineRenderer;

    void Start()
    {
        CreateVisual();
    }

    private void CreateVisual()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startColor = boundaryColor;
        lineRenderer.endColor = boundaryColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(-lineLength / 2, 0, 0));
        lineRenderer.SetPosition(1, new Vector3(lineLength / 2, 0, 0));

        // Sprite Shader 사용
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sortingOrder = 10; // 위에 보이도록
    }
}