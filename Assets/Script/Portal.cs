using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("카메라 경계 (월드 좌표)")]
    public Vector2 newMinCameraPos;
    public Vector2 newMaxCameraPos;

    [Header("데드존 활성화")]
    public bool deadZoneEnabled = true;

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
        if (playerInRange && Input.GetKey(KeyCode.F))
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
        // 대상 포탈 배열 확인
        if (destinationPortals == null || destinationPortals.Length == 0)
        {
            Debug.LogWarning("No destination portals assigned!");
            yield break;
        }

        // 페이드 효과 실행
        CamFol.FadeOutThenIn(fadeOutTime, fadeHoldTime, fadeInTime, Color.black);
        yield return new WaitForSeconds(fadeOutTime);

        // 플레이어 순간이동 - 랜덤 선택
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            int idx = Random.Range(0, destinationPortals.Length);
            Transform dest = destinationPortals[idx];
            if (dest != null)
                player.transform.position = dest.position;
        }

        // 카메라 경계 및 줌 설정
        CamFol.SetCameraBorder(newMinCameraPos, newMaxCameraPos);
        nextMove = true;

        // 페이드 홀드 시간 대기
        yield return new WaitForSeconds(fadeHoldTime);
    }
}