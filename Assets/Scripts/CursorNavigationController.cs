using UnityEngine;
using UnityEngine.InputSystem;

public class CursorNavigationController : MonoBehaviour
{
    [Header("移動與失重阻尼設定")]
    [Tooltip("最大移動速度")]
    public float maxSpeed = 3.5f;

    [Tooltip("到達目標點的平滑時間（0.4 ~ 0.5 越絲滑）")]
    public float smoothTime = 0.45f;

    [Header("尋標器 (Cursor) 設定")]
    [Tooltip("請將 Project 視窗中的藍色 Cursor Prefab 拉入此處")]
    public GameObject cursorPrefab;

    [Tooltip("游標生成時的 Y 軸高度偏移")]
    public float cursorYOffset = 0.02f;

    [Header("游標距離淡出設定")]
    [Tooltip("一個方塊的大致尺寸/邊長（設為 2.0）")]
    public float cubeSize = 2.0f;

    [Tooltip("開始漸淡的距離（單位：方塊數量，建議 2.0，即 2 個方塊遠就開始漸淡）")]
    public float fadeStartCubeCount = 2.0f;

    [Tooltip("剛好歸零的距離（單位：方塊數量，建議 0.5，即距離剩下 0.5 個方塊時完全透明）")]
    public float fadeEndCubeMargin = 0.5f;

    private Vector3 targetWorldPosition;
    private Vector3 currentVelocity;
    private GameObject currentCursorInstance;
    private CursorMarker currentCursorMarker;
    private Camera mainCamera;
    private bool isMoving = false;

    void Start()
    {
        mainCamera = Camera.main;
        targetWorldPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        MoveToTarget();
    }

    void HandleInput()
    {
        Vector2 screenPosition = Vector2.zero;
        bool hasClicked = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            hasClicked = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPosition = Mouse.current.position.ReadValue();
            hasClicked = true;
        }

        if (hasClicked)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float enterDistance))
            {
                Vector3 hitPoint = ray.GetPoint(enterDistance);

                targetWorldPosition = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
                isMoving = true;

                SpawnNewCursor(hitPoint);
            }
        }
    }

    void MoveToTarget()
    {
        if (!isMoving) return;

        // SmoothDamp 平滑移動
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetWorldPosition,
            ref currentVelocity,
            smoothTime,
            maxSpeed
        );

        float distance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(targetWorldPosition.x, 0, targetWorldPosition.z)
        );

        // 🌟 核心：從 2.0 個方塊距離開淡，至 0.5 個方塊距離歸零
        if (currentCursorMarker != null)
        {
            float startDist = cubeSize * fadeStartCubeCount; // 開始漸淡距離 (2.0 * 2.0 = 4.0)
            float endDist = cubeSize * fadeEndCubeMargin;     // 完全歸零距離 (0.5 * 2.0 = 1.0)

            if (distance >= startDist)
            {
                // 距離 >= 2.0 個方塊：維持 100% 不透明
                currentCursorMarker.SetAlpha(1.0f);
            }
            else if (distance <= endDist)
            {
                // 距離 <= 0.5 個方塊：剛好歸零 (0% 透明)
                currentCursorMarker.SetAlpha(0.0f);
            }
            else
            {
                // 在 [2.0 方塊, 0.5 方塊] 區間內平滑過渡
                // 使用 Mathf.SmoothStep 讓開始與結束的淡出動態更柔和自然，避免線性硬切
                float t = (distance - endDist) / (startDist - endDist);
                float smoothAlpha = Mathf.SmoothStep(0f, 1f, t);
                currentCursorMarker.SetAlpha(smoothAlpha);
            }
        }

        // 到達定點判定
        if (distance < 0.005f && currentVelocity.magnitude < 0.01f)
        {
            transform.position = new Vector3(targetWorldPosition.x, transform.position.y, targetWorldPosition.z);
            isMoving = false;
            currentVelocity = Vector3.zero;

            DismissCurrentCursor();
        }
    }

    void SpawnNewCursor(Vector3 position)
    {
        DismissCurrentCursor();

        if (cursorPrefab != null)
        {
            Vector3 spawnPos = new Vector3(position.x, position.y + cursorYOffset, position.z);
            currentCursorInstance = Instantiate(cursorPrefab, spawnPos, Quaternion.identity);
            currentCursorMarker = currentCursorInstance.GetComponent<CursorMarker>();
        }
    }

    void DismissCurrentCursor()
    {
        if (currentCursorInstance != null)
        {
            Destroy(currentCursorInstance);
            currentCursorInstance = null;
            currentCursorMarker = null;
        }
    }
}