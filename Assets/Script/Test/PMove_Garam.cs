using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PMove_Garam : MonoBehaviour
{
    public static PMove_Garam Instance;

    [Header("이동 속도 설정")]
    public float moveSpeed = 5f;
    public float maxSpeed = 8f;

    [Header("플레이어 방향 및 상태")]
    public bool isFacingRight = true;
    public bool Ice = false;

    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // 초기 애니메이션 상태 설정
        anim.SetBool("isWalking", false);
    }

    private void FixedUpdate()
    {
        float moveHorizontal = 0f;

        if (!Ice)
        {
            // 좌우 이동 입력
            if (Input.GetKey(KeyCode.A))
                moveHorizontal = -1f;
            else if (Input.GetKey(KeyCode.D))
                moveHorizontal = 1f;

            // 이동 방향에 따른 Flip (속도 방향 기준)
            if (moveHorizontal > 0f && !isFacingRight)
                Flip();
            else if (moveHorizontal < 0f && isFacingRight)
                Flip();
        }

        // 현재 이동 속도 설정
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? maxSpeed : moveSpeed;
        Vector2 newVelocity = new Vector2(moveHorizontal * currentSpeed, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;

        // 걷기 애니메이션: 수평 입력이 있을 때만 재생
        bool isWalking = !Mathf.Approximately(moveHorizontal, 0f);
        anim.SetBool("isWalking", isWalking);
    }

    /// <summary>
    /// 플레이어의 바라보는 방향을 반전시킵니다.
    /// </summary>
    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
