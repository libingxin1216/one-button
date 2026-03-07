using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 筹码生成器 - 挂载在场景中任意空对象上
/// 每轮开始时在底部生成若干筹码，以随机角度和力度向上抛出
/// </summary>
public class ChipSpawner : MonoBehaviour
{
    [Header("筹码 Prefab")]
    [Tooltip("筹码的 Prefab，需挂载 Chip.cs、Rigidbody2D、BoxCollider2D")]
    public GameObject chipPrefab;

    [Header("生成数量")]
    [Tooltip("每轮生成的最少筹码数")]
    public int minChipCount = 3;
    [Tooltip("每轮生成的最多筹码数")]
    public int maxChipCount = 6;

    [Header("生成位置")]
    [Tooltip("筹码生成区域的水平范围（以生成器为中心，左右各延伸多少）")]
    public float spawnRangeX = 3f;
    [Tooltip("筹码生成的 Y 坐标（画面底部，低于摄像机可视范围）")]
    public float spawnY = -6f;

    [Header("抛出力度")]
    [Tooltip("向上抛出的最小力度")]
    public float minLaunchForce = 5f;
    [Tooltip("向上抛出的最大力度")]
    public float maxLaunchForce = 10f;
    [Tooltip("水平方向的最大偏移力度（让筹码散开）")]
    public float maxHorizontalForce = 3f;

    [Header("抛出时机")]
    [Tooltip("每个筹码之间的生成间隔（秒），制造错落感")]
    public float spawnInterval = 0.15f;

    [Header("筹码数值范围")]
    [Tooltip("筹码数值的最小值")]
    public int minValue = 1;
    [Tooltip("筹码数值的最大值")]
    public int maxValue = 9;

    // 本轮生成的所有筹码，回合结束后统一清理
    private List<GameObject> spawnedChips = new List<GameObject>();

    // =============================================
    // 公开方法：由 GameManager 调用
    // =============================================

    /// <summary>
    /// 开始本轮生成，由 GameManager 在回合开始时调用
    /// </summary>
    public void SpawnChips()
    {
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// 清除本轮所有筹码，由 GameManager 在回合结束后调用
    /// </summary>
    public void ClearChips()
    {
        foreach (GameObject chip in spawnedChips)
        {
            if (chip != null)
                Destroy(chip);
        }
        spawnedChips.Clear();
        Debug.Log("[ChipSpawner] 本轮筹码已清除");
    }

    // =============================================
    // 协程：错落地生成筹码
    // =============================================

    private IEnumerator SpawnRoutine()
    {
        int count = Random.Range(minChipCount, maxChipCount + 1);
        Debug.Log($"[ChipSpawner] 本轮生成 {count} 枚筹码");

        for (int i = 0; i < count; i++)
        {
            SpawnSingleChip();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSingleChip()
    {
        if (chipPrefab == null)
        {
            Debug.LogError("[ChipSpawner] chipPrefab 未赋值！请在 Inspector 中拖入筹码 Prefab");
            return;
        }

        // 随机生成位置（底部，水平随机）
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        // 实例化筹码
        GameObject chipObj = Instantiate(chipPrefab, spawnPos, Quaternion.identity);
        spawnedChips.Add(chipObj);

        // 设置随机数值
        Chip chip = chipObj.GetComponent<Chip>();
        if (chip != null)
        {
            int randomValue = Random.Range(minValue, maxValue + 1);
            chip.SetValue(randomValue);
        }

        // 施加向上的随机力
        Rigidbody2D rb = chipObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float upForce = Random.Range(minLaunchForce, maxLaunchForce);
            float sideForce = Random.Range(-maxHorizontalForce, maxHorizontalForce);
            rb.AddForce(new Vector2(sideForce, upForce), ForceMode2D.Impulse);

            // 施加随机旋转，让筹码翻滚起来更自然
            rb.angularVelocity = Random.Range(-180f, 180f);
        }

        Debug.Log($"[ChipSpawner] 生成筹码：数值={chip?.value}，位置={spawnPos}");
    }

    // =============================================
    // Scene 视图：显示生成区域范围
    // =============================================

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // 画出生成区域的横线
        Gizmos.DrawLine(
            new Vector3(-spawnRangeX, spawnY, 0),
            new Vector3(spawnRangeX, spawnY, 0)
        );
        // 两端竖线
        Gizmos.DrawLine(
            new Vector3(-spawnRangeX, spawnY, 0),
            new Vector3(-spawnRangeX, spawnY + 0.5f, 0)
        );
        Gizmos.DrawLine(
            new Vector3(spawnRangeX, spawnY, 0),
            new Vector3(spawnRangeX, spawnY + 0.5f, 0)
        );
    }
}
