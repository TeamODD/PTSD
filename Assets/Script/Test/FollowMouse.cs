using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChaser : MonoBehaviour
{
    [Header("카메라 및 페이드 설정")]
    public float cameraZoom = 1.0f;
    public float fadeOutTime = 1.5f;
    public float fadeHoldTime = 1.0f;
    public float fadeInTime = 0.2f;

    [Header("발사 쿨타임 설정")]
    [Tooltip("발사 후 다음 발사까지 대기 시간(초)")]
    public float shootCooldown = 0.2f;

    public Transform ob2;
    public Transform spawn;
    public List<GameObject> Bullets;           // 총알 원본 리스트
    private List<GameObject> Bullets_rep;       // 남은 총알 리스트
    public GameObject Bang_light;

    private int bulletCount;
    private float shootTimer;
    private bool isGameOver;
    private PMove_Garam playerMove;

    private void Awake()
    {
        Bang_light.SetActive(false);
    }

    private void Start()
    {
        playerMove = PMove_Garam.Instance;
        ResetBullets();  // 총알 초기화
    }

    private void Update()
    {
        // 쿨타이머 감소
        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        // 발사 가능 조건: 게임오버 중이 아니고, 쿨타임 지났으며, 발사 버튼 클릭 시
        if (!isGameOver && bulletCount > 0 && shootTimer <= 0f && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Bang());
            shootTimer = shootCooldown;
        }
    }

    private void FixedUpdate()
    {
        // 마우스 따라다니는 오브젝트 위치 갱신
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y, 9f));
        if (ob2 != null)
            ob2.position = pos;

        // 총알이 다 떨어지면 게임 오버
        if (bulletCount == 0 && !isGameOver)
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator Bang()
    {
        Bang_light.SetActive(true);

        // 총알 하나 소비
        bulletCount--;
        int last = Bullets_rep.Count - 1;
        Bullets_rep[last].SetActive(false);
        Bullets_rep.RemoveAt(last);

        Debug.Log($"탕! 남은 탄 수: {bulletCount}");

        // 카메라 연출
        if (bulletCount == 0)
        {
            CameraFollowShake.Instance.Shake(0.2f, 0.5f);
            CameraFollowShake.Instance.Zoom(0.85f, 0.5f);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            CameraFollowShake.Instance.Shake(3f, 0.02f);
            CameraFollowShake.Instance.Zoom(0.9f, 0.18f);
            yield return new WaitForSeconds(0.15f);
        }

        Bang_light.SetActive(false);
    }

    private IEnumerator GameOver()
    {
        isGameOver = true;
        Debug.Log("게임 오버");
        if (playerMove != null)
            playerMove.Ice = true;

        // 잠시 대기 후 페이드
        yield return new WaitForSeconds(0.5f);
        CameraFollowShake.Instance.FadeOutThenIn(
            fadeOutTime, fadeHoldTime, fadeInTime, Color.black);
        yield return new WaitForSeconds(fadeOutTime);

        // 플레이어 리셋 위치
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.transform.position = spawn.position;

        // 페이드 복구 대기
        yield return new WaitForSeconds(fadeHoldTime + fadeInTime);

        // 총알 리셋 및 발사 허용
        ResetBullets();
        isGameOver = false;

        if (playerMove != null)
            playerMove.Ice = false;
    }

    /// <summary>
    /// Bullets 원본 리스트를 활성화 후 복제, 개수를 초기화합니다.
    /// </summary>
    private void ResetBullets()
    {
        // 모든 원본 총알 오브젝트 활성화
        foreach (var b in Bullets)
        {
            b.SetActive(true);
        }

        // 잔여 리스트 재설정
        Bullets_rep = new List<GameObject>(Bullets);
        bulletCount = Bullets_rep.Count;
        shootTimer = 0f;
    }
}
