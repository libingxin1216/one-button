using UnityEngine;
using TMPro;

/// <summary>
/// 筹码脚本 - 挂载在每个筹码对象上
/// 存储数值、显示数字、处理高亮反馈
/// </summary>
public class Chip : MonoBehaviour
{
    [Header("筹码属性")]
    [Tooltip("该筹码代表的数值")]
    public int value = 1;

    [Header("显示组件")]
    [Tooltip("显示数字的 TextMeshPro 组件（子对象）")]
    public TextMeshPro numberText;

    [Header("视觉反馈")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateDisplay();
    }

    /// <summary>
    /// 设置筹码数值（由 ChipSpawner 生成时调用）
    /// </summary>
    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateDisplay();
    }

    /// <summary>
    /// 更新筹码上显示的数字
    /// </summary>
    public void UpdateDisplay()
    {
        if (numberText != null)
            numberText.text = value.ToString();
    }

    /// <summary>
    /// 高亮显示（进入检测区域时由 DetectionZone 调用）
    /// </summary>
    public void SetHighlight(bool highlighted)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = highlighted ? highlightColor : normalColor;
    }
}