using UnityEngine;

public class AlwaysShowAxis : MonoBehaviour
{
    [Header("軸心線條長度")]
    [SerializeField] private float axisLength = 1.5f;

    // 不論是否選取該物件，Gizmos 都會持續繪製
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        // X 軸 (紅色)
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pos, transform.right * axisLength);

        // Y 軸 (綠色)
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pos, transform.up * axisLength);

        // Z 軸 (藍色)
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pos, transform.forward * axisLength);

        // 在軸心點畫一個小球體
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, 0.1f);
    }
}