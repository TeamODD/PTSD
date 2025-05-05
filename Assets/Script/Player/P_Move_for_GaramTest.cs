using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("Walking speed")]
    public float walkSpeed = 5f;
    [Tooltip("Running speed when holding Shift")]
    public float runSpeed = 8f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isRunning;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // �߷� ����
        rb.gravityScale = 0f;
        // Y�� �̵��� ȸ�� ����
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
    }

    void Update()
    {
        // �¿� �̵� �Է�
        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1f;
        else
            moveInput = 0f;

        // Shift Ű�� �޸��� ���
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    void FixedUpdate()
    {
        // �ӵ� ��� �� ����
        float speed = isRunning ? runSpeed : walkSpeed;
        Vector2 newVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
    }
}
