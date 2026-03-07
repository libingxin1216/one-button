using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检测区域脚本 - 挂载在画面中央的长方形检测区域对象上
/// 使用每帧 OverlapBox 主动检测，而非依赖 Trigger 事件
/// 兼容 Play 模式下手动拖拽筹码的测试场景
/// </summary>
public class DetectionZone : MonoBehaviour
{
    [Header("检测区域设置")]
    public float zoneWidth = 4f;
    public float zoneHeight = 2f;
    public bool showDebugGizmos = true;

    [Header("视觉反馈")]
    public Color normalColor = new Color(1f, 1f, 0f, 0.3f);
    public Color frozenColor = new Color(0f, 1f, 1f, 0.5f);

    [Header("检测设置")]
    [Tooltip("筹码所在的 Layer，用于 OverlapBox 过滤")]
    public LayerMask chipLayer = ~0; // 默认检测所有层

    private List<Chip> chipsInZone = new List<Chip>();
    private BoxCollider2D detectionCollider;
    private bool isFrozen = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        detectionCollider = GetComponent<BoxCollider2D>();
        if (detectionCollider == null)
            detectionCollider = gameObject.AddComponent<BoxCollider2D>();

        detectionCollider.isTrigger = true;
        detectionCollider.size = new Vector2(zoneWidth, zoneHeight);

        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    void Update()
    {
        if (isFrozen) return;
        RefreshChipsInZone();
    }

    // =============================================
    // 核心：每帧 OverlapBox 主动扫描区域内筹码
    // =============================================

    private void RefreshChipsInZone()
    {
        foreach (Chip chip in chipsInZone)
            if (chip != null) chip.SetHighlight(false);

        chipsInZone.Clear();

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            new Vector2(zoneWidth, zoneHeight),
            0f,
            chipLayer
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Chip chip = hit.GetComponent<Chip>();
            if (chip != null)
            {
                chipsInZone.Add(chip);
                chip.SetHighlight(true);
            }
        }
    }

    // =============================================
    // 公开方法
    // =============================================

    public int CalculateScore()
    {
        int score = 0;
        chipsInZone.RemoveAll(c => c == null);
        foreach (Chip chip in chipsInZone)
            score += chip.value;
        return score;
    }

    public int GetChipCount()
    {
        chipsInZone.RemoveAll(c => c == null);
        return chipsInZone.Count;
    }

    public void ClearZone()
    {
        foreach (Chip chip in chipsInZone)
            if (chip != null) chip.SetHighlight(false);
        chipsInZone.Clear();
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
        UpdateVisual();
    }

    public void ResizeZone(float width, float height)
    {
        zoneWidth = width;
        zoneHeight = height;
        if (detectionCollider != null)
            detectionCollider.size = new Vector2(zoneWidth, zoneHeight);
    }

    // =============================================
    // 视觉 & Gizmos
    // =============================================

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = isFrozen ? frozenColor : normalColor;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        Gizmos.color = isFrozen ? Color.cyan : Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(zoneWidth, zoneHeight, 0));
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.1f);
        Gizmos.DrawCube(transform.position, new Vector3(zoneWidth, zoneHeight, 0));
    }
}