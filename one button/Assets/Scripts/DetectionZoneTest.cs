using UnityEngine;
using TMPro;

/// <summary>
/// 检测区域测试脚本 - 仅用于开发阶段手动测试
/// 挂载在 DetectionZone 同一对象上
/// 实时显示：区域内筹码数量 / 当前数值总和
/// </summary>
public class DetectionZoneTest : MonoBehaviour
{
    [Header("测试UI")]
    [Tooltip("用于显示测试信息的 TextMeshPro 对象（可为空，信息会同时输出到 Console）")]
    public TextMeshProUGUI debugText;

    private DetectionZone detectionZone;

    void Awake()
    {
        detectionZone = GetComponent<DetectionZone>();
        if (detectionZone == null)
            Debug.LogError("[测试] 未找到 DetectionZone 脚本，请确保两个脚本挂载在同一对象上！");
    }

    void Update()
    {
        if (detectionZone == null) return;

        int count = detectionZone.GetChipCount();
        int score = detectionZone.CalculateScore();

        string info = $"区域内筹码数量：{count}\n数值总和：{score}";

        // 输出到屏幕 UI（如果有绑定 TextMeshPro）
        if (debugText != null)
            debugText.text = info;

        // 同时在 Scene 视图中用 Gizmo 标注（无需 UI 也能看到）
    }

    // 在 Game 视图左上角绘制 GUI（无需任何 UI 对象，开箱即用）
    void OnGUI()
    {
        if (detectionZone == null) return;

        int count = detectionZone.GetChipCount();
        int score = detectionZone.CalculateScore();

        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;

        GUI.Label(new Rect(20, 20, 400, 50), $"区域内筹码数量：{count}", style);

        style.normal.textColor = Color.cyan;
        GUI.Label(new Rect(20, 60, 400, 50), $"数值总和：{score}", style);
    }
}