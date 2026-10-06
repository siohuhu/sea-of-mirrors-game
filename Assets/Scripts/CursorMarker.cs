using UnityEngine;

public class CursorMarker : MonoBehaviour
{
    [Header("=== 1. 手繪 Sprite 逐格序列 ===")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] frameSprites;
    [SerializeField] private float frameRate = 12f; // 手繪風常用 8~12 fps

    [Header("=== 2. 懸浮與呼吸擠壓 (AnimationCurve) ===")]
    [SerializeField] private AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, -0.1f, 1, 0.1f);
    [SerializeField] private float floatSpeed = 1.5f;
    
    [SerializeField] private AnimationCurve squashScaleCurve = AnimationCurve.Linear(0, 0.95f, 1, 1.05f);
    [SerializeField] private float squashSpeed = 2.0f;

    [Header("=== 3. 材質透明度控管 ===")]
    [SerializeField] private Renderer targetRenderer;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private float timer;
    private int currentFrameIndex;
    private Vector3 initialLocalPosition;
    private Vector3 initialLocalScale;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();

        initialLocalPosition = transform.localPosition;
        initialLocalScale = transform.localScale;
    }

    void Update()
    {
        // --- A. 逐格 Sprite 動畫播放 ---
        if (frameSprites != null && frameSprites.Length > 0 && spriteRenderer != null)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / frameRate)
            {
                timer -= 1f / frameRate;
                currentFrameIndex = (currentFrameIndex + 1) % frameSprites.Length;
                spriteRenderer.sprite = frameSprites[currentFrameIndex];
            }
        }

        // --- B. 浮動與呼吸擠壓（代碼驅動） ---
        float time = Time.time;

        // 1. 上下微幅浮動
        float floatOffset = floatCurve.Evaluate(Mathf.PingPong(time * floatSpeed, 1.0f));
        transform.localPosition = initialLocalPosition + new Vector3(0, floatOffset, 0);

        // 2. 呼吸感的 Scale 縮放 (Y軸拉長時 X軸收縮，保持視覺體積守恆)
        float scaleFactor = squashScaleCurve.Evaluate(Mathf.PingPong(time * squashSpeed, 1.0f));
        transform.localScale = new Vector3(initialLocalScale.x / scaleFactor, initialLocalScale.y * scaleFactor, initialLocalScale.z);
    }

    /// <summary>
    /// 設定游標透明度 (0.0 ~ 1.0) - 由距離計算腳本呼叫
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (targetRenderer != null && targetRenderer.material != null)
        {
            if (targetRenderer.material.HasProperty(BaseColorID))
            {
                Color c = targetRenderer.material.GetColor(BaseColorID);
                c.a = alpha;
                targetRenderer.material.SetColor(BaseColorID, c);
            }
            else if (targetRenderer.material.HasProperty(ColorID))
            {
                Color c = targetRenderer.material.GetColor(ColorID);
                c.a = alpha;
                targetRenderer.material.SetColor(ColorID, c);
            }
        }
    }
}