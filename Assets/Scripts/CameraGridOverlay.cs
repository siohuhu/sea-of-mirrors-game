using UnityEngine;

[ExecuteAlways]
public class CameraGridOverlay : MonoBehaviour
{
    public bool showGrid = true;
    public Color gridColor = new Color(1f, 1f, 1f, 0.35f); // 微透白線
    public float lineWidth = 1f;

    private void OnGUI()
    {
        if (!showGrid) return;

        Texture2D texture = Texture2D.whiteTexture;
        Color oldColor = GUI.color;
        GUI.color = gridColor;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 兩條垂直線（三分之一與三分之二處）
        float x1 = screenWidth / 3f;
        float x2 = (screenWidth / 3f) * 2f;
        GUI.DrawTexture(new Rect(x1, 0, lineWidth, screenHeight), texture);
        GUI.DrawTexture(new Rect(x2, 0, lineWidth, screenHeight), texture);

        // 兩條水平線（三分之一與三分之二處）
        float y1 = screenHeight / 3f;
        float y2 = (screenHeight / 3f) * 2f;
        GUI.DrawTexture(new Rect(0, y1, screenWidth, lineWidth), texture);
        GUI.DrawTexture(new Rect(0, y2, screenWidth, lineWidth), texture);

        GUI.color = oldColor;
    }
}