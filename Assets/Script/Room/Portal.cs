using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("외부 목적지 매니저")]
    public PortalManager destinationMgr;
    [Tooltip("이 포탈이 속한 방의 인덱스 (0 = 태초)")]
    public int roomIndex;

    [Header("카메라 경계 (월드 좌표)")]
    public Vector2 newMinCameraPos;
    public Vector2 newMaxCameraPos;

    [Header("데드존 활성화")]
    public bool deadZoneEnabled = false;

    [Header("대상 포탈들")]
    [Tooltip("플레이어가 포탈과 상호작용 시 이동할 대상 포탈들의 Transform 리스트")]
    public Transform[] destinationPortals;

    [Header("상호작용 UI")]
    public GameObject ITKey;

    [Header("카메라 및 페이드 설정")]
    public float cameraZoom = 1.0f;
    public float fadeOutTime = 0.3f;
    public float fadeHoldTime = 1.0f;
    public float fadeInTime = 0.3f;

    private bool playerInRange = false;
    private CameraFollowShake CamFol;
    private bool nextMove = false;

    private void Start()
    {
        CamFol = CameraFollowShake.Instance;
        if (CamFol == null)
            Debug.LogError("No CameraFollowShake.Instance found in scene!");
        if (ITKey != null)
            ITKey.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (ITKey != null)
                ITKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (ITKey != null)
                ITKey.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(PortalInteract());
        }
    }

    private void FixedUpdate()
    {
        if (nextMove)
        {
            CamFol.deadZoneEnabled = deadZoneEnabled;
            nextMove = false;
        }
    }

    private IEnumerator PortalInteract()
    {
        // 1) 현재 방 설정
        destinationMgr.SetCurrentRoom(roomIndex);

        // 2) 페이드 아웃
        CamFol.FadeOutThenIn(fadeOutTime, fadeHoldTime, fadeInTime, Color.black);
        yield return new WaitForSeconds(fadeOutTime);

        // 3) 이동 대상 가져오기
        Transform dest = destinationMgr.GetNextDestination();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && dest != null)
            player.transform.position = dest.position;

        // 4) 카메라 경계 갱신
        CamFol.SetCameraBorder(newMinCameraPos, newMaxCameraPos);
        nextMove = true;

        // 5) 페이드 홀드
        yield return new WaitForSeconds(fadeHoldTime);
    }
}