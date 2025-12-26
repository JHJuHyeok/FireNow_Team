using System.Collections;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float lifetime = 1f;

    private TextMeshProUGUI textMesh;
    private Color originalColor;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(float damage, Vector3 worldPosition)
    {
        // Canvas 찾기
        Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다!");
            Destroy(gameObject);
            return;
        }

        // Canvas의 자식으로 설정
        transform.SetParent(canvas.transform, false);

        // TextMeshPro 설정
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        if (textMesh == null)
        {
            Debug.LogError("TextMeshProUGUI가 없습니다!");
            Destroy(gameObject);
            return;
        }

        textMesh.text = Mathf.RoundToInt(damage).ToString();
        textMesh.fontSize = 36;
        textMesh.color = Color.red;
        originalColor = textMesh.color;

        // 월드 좌표 → UI 좌표 변환
        RectTransform rectTransform = GetComponent<RectTransform>();
        Camera cam = Camera.main;

        if (cam == null)
        {
 
            Destroy(gameObject);
            return;
        }

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);

        // Canvas의 RenderMode에 따라 처리
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Camera canvasCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // 스크린 좌표를 Canvas 로컬 좌표로 변환
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, canvasCam, out localPos);

        rectTransform.anchoredPosition = localPos;


        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        timer = 0f;

        while (timer < lifetime)
        {
            timer += Time.deltaTime;

            // 위로 이동
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // 페이드 아웃
            if (textMesh != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}