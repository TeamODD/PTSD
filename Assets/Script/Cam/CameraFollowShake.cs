using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFollowShake : MonoBehaviour
{
    public static CameraFollowShake Instance;

    #region Follow Settings
    [Header("Follow Settings")]
    public bool followEnabled = true;              // 카메라 따라가기 기능 활성화 여부
    public float followSpeed = 2f;
    public float yOffset = 1f;
    // 단일 추격 기능은 제거하고, 오직 그룹(여러 오브젝트) 추격만 사용합니다.
    // 배열에 단 하나의 객체만 있으면, 그 객체를 따라가는 효과와 동일합니다.
    public Transform[] followTargets;
    #endregion

    #region Dead Zone Settings
    [Header("Dead Zone Settings")]
    public bool deadZoneEnabled = true;            // 데드존 기능 활성화 여부
    public Vector2 deadZoneSize = new Vector2(2f, 1f);
    #endregion

    #region Camera Border, Zoom & Shake
    [Header("Camera Border Settings")]
    public Vector2 minCameraPos;
    public Vector2 maxCameraPos;

    // 줌 관련 변수
    private float originalSize;
    private float zoomDuration = 0f;
    private float zoomSpeed = 1f;
    private bool isZooming = false;

    // 흔들림(Shake) 관련 변수
    private Vector3 shakeOffset = Vector3.zero;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;
    #endregion

    #region Fade & Dissolve Settings
    [Header("Fade & Dissolve Settings")]
    [Tooltip("화면 전체를 덮는 UI Image (Fade에 사용)")]
    public Image fadeImage;  // Canvas 상에 배치된 전체화면 Image
    [Tooltip("디졸브 효과에 사용될 Material. 셰이더에 '_DissolveAmount' 프로퍼티가 있어야 합니다.")]
    public Material dissolveMaterial; // Dissolve 효과용 Material
    [Tooltip("Fade에 사용할 기본 색상")]
    public Color defaultFadeColor = Color.black;
    #endregion

    private Camera cam;
    private Coroutine fadeCoroutine;
    private Coroutine dissolveCoroutine;

    #region Unity Lifecycle
    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollowShake 스크립트가 붙은 오브젝트에 Camera 컴포넌트가 없습니다!");
            return;
        }
        originalSize = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        if (!followEnabled)
            return;

        // followTargets가 null이 아니고, 하나 이상 있을 때만 처리
        if (followTargets == null || followTargets.Length == 0)
            return;

        // 그룹의 중심(평균 위치) 계산
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (Transform t in followTargets)
        {
            if (t != null)
            {
                center += t.position;
                count++;
            }
        }
        if (count > 0)
            center /= count;
        // center가 단 하나의 객체라면 그 객체의 위치와 동일하게 됨
        Vector3 basePos = new Vector3(center.x, center.y + yOffset, -10);

        // 흔들림(Shake) 오프셋 업데이트
        if (shakeDuration > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            shakeOffset = Vector3.zero;
        }

        Vector3 logicalPos = transform.position - shakeOffset;
        Vector3 desiredFinalPos = basePos; // 기본 타겟(그룹 중심) 위치

        if (deadZoneEnabled)
        {
            // 데드존 적용: 그룹 중심과 현재 논리적 카메라 위치의 차이가 데드존 범위를 초과할 때만 보정
            Vector3 diff = basePos - logicalPos;
            float halfDeadZoneX = deadZoneSize.x * 0.5f;
            float halfDeadZoneY = deadZoneSize.y * 0.5f;

            float offsetX = 0f;
            if (Mathf.Abs(diff.x) > halfDeadZoneX)
                offsetX = diff.x - Mathf.Sign(diff.x) * halfDeadZoneX;

            float offsetY = 0f;
            if (Mathf.Abs(diff.y) > halfDeadZoneY)
                offsetY = diff.y - Mathf.Sign(diff.y) * halfDeadZoneY;

            Vector3 correction = new Vector3(offsetX, offsetY, 0f);
            desiredFinalPos = logicalPos + correction + shakeOffset;
        }
        else
        {
            // 데드존 비활성화 시에는 그룹 중심 위치에 shake 효과만 반영
            desiredFinalPos = basePos + shakeOffset;
        }

        // 카메라 경계 적용: 미리 설정한 좌표 범위를 벗어나지 않도록 클램핑
        desiredFinalPos.x = Mathf.Clamp(desiredFinalPos.x, minCameraPos.x, maxCameraPos.x);
        desiredFinalPos.y = Mathf.Clamp(desiredFinalPos.y, minCameraPos.y, maxCameraPos.y);

        // 부드러운 이동 (Lerp)
        transform.position = Vector3.Lerp(transform.position, desiredFinalPos, followSpeed * Time.deltaTime);

        // 줌(Zoom) 효과 업데이트
        if (zoomDuration > 0)
        {
            zoomDuration -= Time.deltaTime;
        }
        else if (isZooming)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalSize, zoomSpeed * Time.deltaTime);
            if (Mathf.Abs(cam.orthographicSize - originalSize) < 0.01f)
            {
                cam.orthographicSize = originalSize;
                isZooming = false;
            }
        }
    }
    #endregion

    #region Public Methods for Existing Functionality
    public void Shake(float magnitude, float duration)
    {
        shakeMagnitude = magnitude;
        shakeDuration = Mathf.Max(shakeDuration, duration);
    }

    public void Zoom(float zoomFactor, float duration)
    {
        cam.orthographicSize = originalSize * zoomFactor;
        zoomDuration = duration;
        isZooming = true;
    }

    public void ZoomSet(float zoomFactor)
    {
        zoomDuration = 0f;
        isZooming = false;
        cam.orthographicSize = originalSize * zoomFactor;
    }

    public void SetCameraBorder(Vector2 newMin, Vector2 newMax)
    {
        minCameraPos = newMin;
        maxCameraPos = newMax;
    }

    /// <summary>
    /// 외부에서 그룹 추격 대상을 설정합니다.
    /// </summary>
    public void SetFollowTargets(Transform[] targets)
    {
        followTargets = targets;
    }

    public void SetDeadZoneEnabled(bool enabled, Vector2? newDeadZoneSize = null)
    {
        deadZoneEnabled = enabled;
        if (newDeadZoneSize.HasValue)
            deadZoneSize = newDeadZoneSize.Value;
    }
    #endregion

    #region Fade & Dissolve Methods
    public void FadeOutThenIn(float fadeOutTime, float holdTime, float fadeInTime, Color? fadeColor = null)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutThenInRoutine(fadeOutTime, holdTime, fadeInTime, fadeColor ?? defaultFadeColor));
    }

    private IEnumerator FadeOutThenInRoutine(float fadeOutTime, float holdTime, float fadeInTime, Color fadeColor)
    {
        yield return StartCoroutine(FadeRoutine(0f, 1f, fadeOutTime, fadeColor));
        yield return new WaitForSeconds(holdTime);
        yield return StartCoroutine(FadeRoutine(1f, 0f, fadeInTime, fadeColor));
        fadeCoroutine = null;
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha, float duration, Color fadeColor)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("Fade Image가 할당되지 않았습니다!");
            yield break;
        }
        Color col = fadeColor;
        col.a = startAlpha;
        fadeImage.color = col;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            col.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = col;
            yield return null;
        }
        col.a = endAlpha;
        fadeImage.color = col;
        fadeCoroutine = null;
    }

    public void Dissolve(float duration)
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);
        dissolveCoroutine = StartCoroutine(DissolveRoutine(duration));
    }

    private IEnumerator DissolveRoutine(float duration)
    {
        if (dissolveMaterial == null)
        {
            Debug.LogWarning("Dissolve Material이 할당되지 않았습니다!");
            yield break;
        }
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            dissolveMaterial.SetFloat("_DissolveAmount", t);
            yield return null;
        }
        dissolveMaterial.SetFloat("_DissolveAmount", 1f);
        dissolveCoroutine = null;
    }
    #endregion
}
