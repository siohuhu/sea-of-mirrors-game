using UnityEngine;

public class DrawCameraRay : MonoBehaviour
{
    [SerializeField] private Color rayColor = Color.red;
    [SerializeField] private float rayDistance = 100f;

    private void OnDrawGizmos()
    {
        // 從相機位置沿著相機正前方 (Z 軸) 畫出一條射線
        Gizmos.color = rayColor;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
    }
}