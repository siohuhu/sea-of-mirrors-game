using UnityEngine;
using UnityEngine.InputSystem;

public class TouchDampingController : MonoBehaviour
{
    [Header("移動與阻尼設定")]
    [Tooltip("移動速度（建議設定 2.0 ~ 5.0）")]
    public float moveSpeed = 3.5f;

    [Tooltip("阻尼煞車力（數字越大越快停下，建議 5.0 ~ 8.0）")]
    public float floatDamping = 6.0f;

    [Tooltip("追蹤平滑度（數字越大越跟手）")]
    public float followResponse = 15.0f;

    private Vector2 currentVelocity;
    private Vector2 lastScreenPos;
    private bool isTouching = false;

    void Update()
    {
        Vector2 currentScreenPos = Vector2.zero;
        bool hasInput = false;

        // 1. 取得當前觸控或滑鼠位置
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            hasInput = true;
            currentScreenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            hasInput = true;
            currentScreenPos = Mouse.current.position.ReadValue();
        }

        // 2. 計算移動向量
        if (hasInput)
        {
            if (!isTouching)
            {
                // 按下的第一幀：只記錄初始位置，避免第一幀瞬間拉扯暴衝
                lastScreenPos = currentScreenPos;
                isTouching = true;
            }
            else
            {
                // 計算兩幀之間的畫面位移百分比 (Normalized Delta)
                Vector2 screenDelta = (currentScreenPos - lastScreenPos) / Screen.height;
                
                // 目標速度
                Vector2 targetVelocity = screenDelta * moveSpeed * 10f;
                
                // 平滑內插到達目標速度
                currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, Time.deltaTime * followResponse);
                
                // 更新上一幀位置
                lastScreenPos = currentScreenPos;
            }
        }
        else
        {
            isTouching = false;
        }

        // 3. 手指放開後的自然阻尼煞車 (Inertia Float)
        if (!isTouching)
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, Time.deltaTime * floatDamping);
        }

        // 4. 2.5D XZ 平面位移（將螢幕 Y 映射至世界 Z 軸，不往天空飛）
        Vector3 worldMove = new Vector3(currentVelocity.x, 0, currentVelocity.y);
        transform.Translate(worldMove * Time.deltaTime, Space.World);
    }
}