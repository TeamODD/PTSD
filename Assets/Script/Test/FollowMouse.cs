using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChaser : MonoBehaviour
{
    public static MouseChaser Instance;

    [Header("카메라 및 페이드 설정")]
    public float cameraZoom = 1.0f;
    public float fadeOutTime = 1.5f;
    public float fadeHoldTime = 1.0f;
    public float fadeInTime = 0.2f;

    [Header("발사 쿨타임 설정")]
    [Tooltip("발사 후 다음 발사까지 대기 시간(초)")]
    public float shootCooldown = 0.2f;

    [Header("Ejection Shell Settings")]
    [Tooltip("배출될 탄피 프리팹")]
    public GameObject shellPrefab;
    [Tooltip("탄피 위쪽 속도 최소값")]
    public float shellMinSpeedY = 2f;
    [Tooltip("탄피 위쪽 속도 최대값")]
    public float shellMaxSpeedY = 5f;
    [Tooltip("탄피 좌우 속도 최대값 (±값)")]
    public float shellMaxSpeedX = 1f;
    [Tooltip("회전 속도 최소값 (rad/s)")]
    public float shellMinAngularSpeed = 5f;
    [Tooltip("회전 속도 최대값 (rad/s)")]
    public float shellMaxAngularSpeed = 15f;

    public Animator effectAnim1;
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
        Instance = this;
    }

    private void Start()
    {
        playerMove = PMove_Garam.Instance;
        ResetBullets();  // 총알 초기화 
    }

    private void Update()
    {
        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        if (!isGameOver && bulletCount > 0 && shootTimer <= 0f && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Bang());
            shootTimer = shootCooldown;
        }
    }

    private void FixedUpdate()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y, 9f));
        if (ob2 != null)
            ob2.position = pos;

        if (bulletCount == 0 && !isGameOver)
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator Bang()
    {
        Bang_light.SetActive(true);

        // 총알 하나 소비 및 위치 저장 
        bulletCount--;
        int last = Bullets_rep.Count - 1;
        GameObject lastBullet = Bullets_rep[last];
        Vector3 bulletPos = lastBullet.transform.position;

        // 총알 비활성화 
        lastBullet.SetActive(false);
        Bullets_rep.RemoveAt(last);

        // 탄피 배출 
        if (shellPrefab != null)
        {
            GameObject shell = Instantiate(shellPrefab, bulletPos, Quaternion.identity);
            Rigidbody rb = shell.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 랜덤 속도 적용 
                float speedX = Random.Range(-shellMaxSpeedX, shellMaxSpeedX);
                float speedY = Random.Range(shellMinSpeedY, shellMaxSpeedY);
                rb.linearVelocity = new Vector3(speedX, speedY, 0f);

                // 랜덤 회전 속도 적용 (z축 회전) 
                float angSpeed = Random.Range(shellMinAngularSpeed, shellMaxAngularSpeed) * (Random.value < 0.5f ? -1f : 1f);
                rb.angularVelocity = new Vector3(0f, 0f, angSpeed);
            }
            Destroy(shell, 5f);  // 일정 시간 후 제거 
        }

        Debug.Log($"탕! 남은 탄 수: {bulletCount}");

        // 카메라 연출 
        if (bulletCount == 0)
        {
            CameraFollowShake.Instance.Shake(3f, 0.5f);
            CameraFollowShake.Instance.Zoom(0.85f, 0.5f);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            CameraFollowShake.Instance.Shake(15f, 0.02f);
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

        effectAnim1.Play("Dead_Test");

        yield return new WaitForSeconds(1.2f);
        CameraFollowShake.Instance.FadeOutThenIn(
            fadeOutTime, fadeHoldTime, fadeInTime, Color.black);
        yield return new WaitForSeconds(fadeOutTime);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.transform.position = spawn.position;

        effectAnim1.Play("Idle_Test");
        yield return new WaitForSeconds(fadeHoldTime + fadeInTime);

        ResetBullets();
        isGameOver = false;

        if (playerMove != null)
            playerMove.Ice = false;
    }

    public void ResetBullets()
    {
        foreach (var b in Bullets)
        {
            b.SetActive(true);
        }

        Bullets_rep = new List<GameObject>(Bullets);
        bulletCount = Bullets_rep.Count;
        shootTimer = 0f;
    }
}