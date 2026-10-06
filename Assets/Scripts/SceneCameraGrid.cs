using UnityEngine;

[ExecuteAlways]
public class SceneCameraGrid : MonoBehaviour
{
    public Camera targetCamera;
    public Color gridColor = new Color(1f, 1f, 0f, 0.5f); // 黃色參考線

    private void OnDrawGizmos()
    {
        if (targetCamera == null) targetCamera = GetComponent<Camera>();
        if (targetCamera == null) return;

        Gizmos.color = gridColor;

        // 取得相機近裁切面的四個角點
        Vector3[] frustumCorners = new Vector3[4];
        targetCamera.CalculateFrustumCorners(
            new Rect(0, 0, 1, 1), 
            targetCamera.nearClipPlane + 0.1f, 
            Camera.MonoOrStereoscopicEye.Mono, 
            frustumCorners
        );

        // 將角點轉為世界座標
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i] = targetCamera.transform.TransformPoint(frustumCorners[i]);
        }

        // 四個角點順序：0:左下, 1:左上, 2:右上, 3:右下
        Vector3 bottomLeft = frustumCorners[0];
        Vector3 topLeft = frustumCorners[1];
        Vector3 topRight = frustumCorners[2];
        Vector3 bottomRight = frustumCorners[3];

        // 繪製垂直三分線
        Gizmos.DrawLine(Vector3.Lerp(bottomLeft, bottomRight, 1f / 3f), Vector3.Lerp(topLeft, topRight, 1f / 3f));
        Gizmos.DrawLine(Vector3.Lerp(bottomLeft, bottomRight, 2f / 3f), Vector3.Lerp(topLeft, topRight, 2f / 3f));

        // 繪製水平三分線
        Gizmos.DrawLine(Vector3.Lerp(bottomLeft, topLeft, 1f / 3f), Vector3.Lerp(bottomRight, topRight, 1f / 3f));
        Gizmos.DrawLine(Vector3.Lerp(bottomLeft, topLeft, 2f / 3f), Vector3.Lerp(bottomRight, topRight, 2f / 3f));
    }
}