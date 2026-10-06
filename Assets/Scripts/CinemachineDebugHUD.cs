using UnityEngine;
using Unity.Cinemachine;

public class CinemachineDebugHUD : MonoBehaviour
{
    public CinemachineBrain brain;
    public CinemachineCamera vcamFar;
    public CinemachineCamera vcamNear;

    void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(2, 2, 1)); // 放大字體

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("<b>== 鏡頭動態驗證 HUD ==</b>");

        // 1. 驗證當前 Live 的虛擬相機
        ICinemachineCamera activeCam = brain.ActiveVirtualCamera;
        string activeName = activeCam != null ? activeCam.Name : "None";
        GUILayout.Label($"當前相機: <color=yellow>{activeName}</color>");

        // 2. 驗證是否正在 Blend (推拉過渡中)
        if (brain.IsBlending)
        {
            float progress = brain.ActiveBlend.TimeInBlend / brain.ActiveBlend.Duration;
            GUILayout.Label($"<color=cyan>[推拉過渡中] 進度: {progress * 100:F0}% ({brain.ActiveBlend.TimeInBlend:F1}s / {brain.ActiveBlend.Duration:F1}s)</color>");
        }
        else
        {
            GUILayout.Label("狀態: 靜止中");
        }

        // 3. 驗證目前 Live 相機的 Camera Distance
        if (activeCam is CinemachineCamera cCam)
        {
            var positionComposer = cCam.GetComponent<CinemachinePositionComposer>();
            if (positionComposer != null)
            {
                GUILayout.Label($"Camera Distance: {positionComposer.CameraDistance:F1}");
            }
        }

        GUILayout.EndArea();
    }
}