using UnityEngine;

[ExecuteAlways] // 允許在編輯模式下直接執行
public class Billboard : MonoBehaviour
{
    public Transform cameraToFace;

    void LateUpdate()
    {
        // 如果沒有手動拉相機，就自動抓取 Main Camera
        if (cameraToFace == null && Camera.main != null)
        {
            cameraToFace = Camera.main.transform;
        }

        if (cameraToFace != null)
        {
            // 讓 Z 軸方向永遠跟相機保持一致
            transform.rotation = cameraToFace.rotation;
        }
    }
}