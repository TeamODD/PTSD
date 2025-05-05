using System.Collections;
using UnityEngine;

public class AimBorder : MonoBehaviour
{
    [Header("플레이어 트랜스폼 지정")]
    public Transform player;

    [Header("경계 영역 반지름 (2 → 가로·세로 4유닛)")]
    public float halfSize = 2f;

    void Update()
    {
        if (player == null || CameraFollowShake.Instance == null)
            return;

        // 플레이어 위치
        Vector2 p = player.position;

        // 중심으로부터 ±halfSize 만큼 오프셋
        Vector2 newMin = new Vector2(p.x - halfSize, p.y + halfSize);
        Vector2 newMax = new Vector2(p.x + halfSize, p.y + halfSize*2);

        // 카메라 보더 업데이트
        CameraFollowShake.Instance.SetCameraBorder(newMin, newMax);
    }
}
