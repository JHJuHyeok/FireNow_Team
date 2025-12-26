using UnityEngine;
using UnityEngine.UI;

public class InfiniteBackground : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Transform player;
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private Camera mainCamera;

    [Header("Scrolling Settings")]
    [Tooltip("값이 1이면 배경과 월드 오브젝트가 1:1로 일치")]
    [SerializeField] private float scrollMultiplier = 1.0f;

    private Vector2 savedOffset;
    private Vector3 lastPlayerPosition;

    private void Start()
    {
        // 참조가 비어있을 경우 자동 할당
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (backgroundImage == null) backgroundImage = GetComponent<RawImage>();
        if (mainCamera == null) mainCamera = Camera.main;

        if (player != null)
        {
            lastPlayerPosition = player.position;
        }

        // RawImage의 Texture Wrap Mode가 Repeat인지 확인하는 것이 좋습니다.
        if (backgroundImage != null && backgroundImage.texture != null)
        {
            if (backgroundImage.texture.wrapMode != TextureWrapMode.Repeat)
            {
                Debug.LogWarning("Background Texture의 Wrap Mode를 'Repeat'으로 설정해야 끊기지 않습니다!");
            }
        }
    }

    private void LateUpdate()
    {
        if (player == null || backgroundImage == null || mainCamera == null) return;

        // 1. 플레이어의 이동량 계산
        Vector3 movement = player.position - lastPlayerPosition;

        // 2. 카메라의 월드 시야 크기 계산
        // Orthographic Size는 화면 세로 절반의 크기입니다.
        float viewHeight = mainCamera.orthographicSize * 2f;
        float viewWidth = viewHeight * mainCamera.aspect;

        // 3. 이동량을 UV 좌표계(0~1) 비율로 변환
        // 플레이어가 '화면 크기'만큼 움직였을 때 배경 이미지도 '한 바퀴' 돌아야 일치합니다.
        float moveX = (movement.x / viewWidth) * scrollMultiplier;
        float moveY = (movement.y / viewHeight) * scrollMultiplier;

        // 4. 오프셋 적용 (아이템이 배경에 붙어있으려면 += 연산)
        savedOffset.x += moveX;
        savedOffset.y += moveY;

        // 5. 실제 RawImage에 적용
        backgroundImage.uvRect = new Rect(savedOffset.x, savedOffset.y, 1, 1);

        // 현재 위치 저장
        lastPlayerPosition = player.position;
    }
}