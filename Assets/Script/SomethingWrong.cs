using UnityEngine;

public class OverlapClickDetector : MonoBehaviour
{
    // 겹쳐있는 상태에서 클릭할 때 true로 바꿀 bool
    public bool isFeatureActivated = false;

    // 두 오브젝트가 겹쳐진 상태인지 추적
    private bool isOverlapping = false;

    private PortalManager Pmg;

    private void Start()
    {
        Pmg = PortalManager.Instance;
    }
    private void FixedUpdate()
    {
        if (isFeatureActivated)
        {
            Pmg.allowRandom = isFeatureActivated;
            isFeatureActivated = false;
        }
            
    }

    // Trigger 진입 시
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Aim"))
        {
            isOverlapping = true;
        }
    }

    // Trigger 이탈 시
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Aim"))
        {
            isOverlapping = false;
        }
    }

    // 매 프레임 마우스 입력 검사
    private void Update()
    {
        // 겹쳐져 있는 상태이고, 왼쪽 마우스 버튼을 방금 눌렀다면
        if (isOverlapping && Input.GetMouseButtonDown(0))
        {
            isFeatureActivated = true;
            Debug.Log(isFeatureActivated);
            // 필요하다면 한 번만 실행하게 막기
            // enabled = false;
        }
    }
}
