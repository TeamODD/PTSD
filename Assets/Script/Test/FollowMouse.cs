using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class MouseChaser : MonoBehaviour
{
    [Header("카메라 및 페이드 설정")]
    public float cameraZoom = 1.0f;
    public float fadeOutTime = 1.5f;
    public float fadeHoldTime = 1.0f;
    public float fadeInTime = 0.2f;

    public Transform ob2;

    public Transform spawn;
    //public Transform[] followTargets;
    public GameObject Bang_light;
    Vector3 pos;
    public int bullet = 8;

    private PMove_Garam playerMove;

    private void Awake()
    {
        Bang_light.SetActive(false);
    }

    private void Start()
    {
        playerMove = PMove_Garam.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ob2
        pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f));

        if (ob2 != null)
            ob2.position = pos;

        if (bullet == 0)
        {
            StartCoroutine(GameOver());
        }

    }

    // 매 프레임 마우스 입력 검사
    private void Update()
    {
        if (bullet !=0 && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Bang());
        }
    }

    private IEnumerator Bang()
    {
        Bang_light.SetActive(true);
        bullet -= 1;
        Debug.Log($"탕!  남은 탄 수: {bullet}");
        if (bullet == 0)
        {
            CameraFollowShake.Instance.Shake(0.2f, 0.5f); // 카메라 충격
            CameraFollowShake.Instance.Zoom(0.85f, 0.5f);  // 확대
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            CameraFollowShake.Instance.Shake(3f, 0.02f); // 카메라 충격
            CameraFollowShake.Instance.Zoom(0.9f, 0.18f);  // 확대
            yield return new WaitForSeconds(0.15f);
        }
        
        Bang_light.SetActive(false);
    }

    private IEnumerator GameOver()
    {
        bullet = 8;
        Debug.Log("게임 오버");
        if (playerMove != null)
            playerMove.Ice = true;
        yield return new WaitForSeconds(0.5f);
        CameraFollowShake.Instance.FadeOutThenIn(fadeOutTime, fadeHoldTime, fadeInTime, Color.black);
        yield return new WaitForSeconds(fadeOutTime);
        //리셋//
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = spawn.position;
        yield return new WaitForSeconds(fadeHoldTime + fadeInTime);
        if (playerMove != null)
            playerMove.Ice = false;
    }
}
