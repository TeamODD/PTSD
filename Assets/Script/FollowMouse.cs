using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChaser : MonoBehaviour
{
    public Transform ob2;
    //public Transform[] followTargets;
    public GameObject Bang_light;
    Vector3 pos;

    private void Awake()
    {
        Bang_light.SetActive(false);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // ob2
        pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f));

        if (ob2 != null)
            ob2.position = pos;

    }

    // 매 프레임 마우스 입력 검사
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Bang());
        }
    }

    private IEnumerator Bang()
    {
        Debug.Log("탕!");
        Bang_light.SetActive(true);
        CameraFollowShake.Instance.Shake(10f, 0.1f); // 카메라 충격
        CameraFollowShake.Instance.Zoom(0.8f, 0.18f);  // 확대
        yield return new WaitForSeconds(0.15f);
        Bang_light.SetActive(false);
    }
}
