using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;

    [Header("모든 침대 리스트 (방별 목적지 포탈)")]
    public Transform[] allDestinationPortals;

    [Header("태초(첫번째) 방의 대상 포탈")]
    public Transform primordialPortal;

    // 상호작용 시 현재 방(포탈)이 어떤 인덱스인지 설정
    public int currentRoomIndex = -1;

    // ture 시작 → false면 무조건 태초로, true면 랜덤
    public bool allowRandom = true;


    void Awake()
    {
        Instance = this;
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
    }


    /// <summary>
    /// 상호작용 전 호출: 지금 플레이어가 있는 방(포탈)의 인덱스를 알려줍니다.
    /// 방 인덱스 0번을 '태초'로 가정합니다.
    /// </summary>
    public void SetCurrentRoom(int roomIndex)
    {
        currentRoomIndex = roomIndex;
        // 현재 방이 태초(0)이면 다음엔 랜덤 허용
        if (roomIndex == 0)
            allowRandom = true;
    }

    /// <summary>
    /// 다음에 이동할 대상 포탈을 결정합니다.
    /// allowRandom == false → 무조건 primordialPortal 반환
    /// allowRandom == true  → (allDestinationPortals 중 currentRoomIndex 제외 + primordial 포함)에서 랜덤 선택
    /// 선택 후 allowRandom = false 로 리셋
    /// </summary>
    public Transform GetNextDestination()
    {
        // 1) allowRandom 이 false 면 → 태초로 돌려보내고, 다음엔 다시 랜덤 허용
        if (!allowRandom)
        {
            allowRandom = true;      // 태초로 돌아간 다음엔 무조건 랜덤 허용
            Debug.Log("allowRandom was false → returning primordialPortal");
            return primordialPortal;
        }

        // 2) 랜덤 허용된 경우
        var candidates = new List<Transform>(allDestinationPortals);
        if (currentRoomIndex >= 0 && currentRoomIndex < allDestinationPortals.Length)
            candidates.RemoveAt(currentRoomIndex);

        int idx = Random.Range(0, candidates.Count);
        Transform chosen = candidates[idx];
        Debug.Log($"랜덤으로 선택된 포탈: {chosen.name}");

        // 3) 선택된 게 태초면 → 다음에도 랜덤, 아니면 다음엔 태초
        allowRandom = (chosen == primordialPortal);

        return chosen;
    }
}
