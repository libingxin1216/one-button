using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏管理器 - 挂载在场景中任意空对象上
/// 负责：回合流程 / 空格键冻结 / 伤害结算 / 胜负判断
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("场景引用")]
    public DetectionZone detectionZone;
    public ChipSpawner chipSpawner;
    public GameUI gameUI;

    [Header("玩家血量")]
    public int player1MaxHP = 100;
    public int player2MaxHP = 100;

    private int player1HP;
    private int player2HP;
    private int currentPlayer = 1;

    private enum GameState
    {
        WaitingToStart,
        ChipsFlying,
        Frozen,
        GameOver
    }
    private GameState state = GameState.WaitingToStart;

    // =============================================
    // 初始化
    // =============================================

    void Start()
    {
        player1HP = player1MaxHP;
        player2HP = player2MaxHP;

        Debug.Log("===== 游戏开始 =====");
        StartCoroutine(StartRound());
    }

    // =============================================
    // 每帧监听空格键
    // =============================================

    void Update()
    {
        if (state == GameState.ChipsFlying)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                FreezeAndCalculate();
        }

        if (state == GameState.GameOver && Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // =============================================
    // 回合流程
    // =============================================

    private IEnumerator StartRound()
    {
        state = GameState.WaitingToStart;

        Debug.Log($"----- 玩家 {currentPlayer} 的回合 -----");

        chipSpawner.ClearChips();
        detectionZone.ClearZone();
        detectionZone.SetFrozen(false);
        gameUI?.HideDamage();

        Time.timeScale = 1f;

        yield return new WaitForSeconds(1f);

        chipSpawner.SpawnChips();
        state = GameState.ChipsFlying;

        Debug.Log($"玩家 {currentPlayer}：请看准时机，按 Space 键停止！");
    }

    private void FreezeAndCalculate()
    {
        state = GameState.Frozen;

        Time.timeScale = 0f;
        detectionZone.SetFrozen(true);

        int damage = detectionZone.CalculateScore();
        Debug.Log($"玩家 {currentPlayer} 的攻击数值：{damage}");

        // 通知 UI 显示攻击数值
        gameUI?.ShowDamage(damage, currentPlayer);

        StartCoroutine(ResolveRound(damage));
    }

    private IEnumerator ResolveRound(int damage)
    {
        yield return new WaitForSecondsRealtime(1.5f);

        if (currentPlayer == 1)
        {
            player2HP = Mathf.Max(player2HP - damage, 0);
            Debug.Log($"玩家2 受到 {damage} 点伤害，剩余血量：{player2HP}");
        }
        else
        {
            player1HP = Mathf.Max(player1HP - damage, 0);
            Debug.Log($"玩家1 受到 {damage} 点伤害，剩余血量：{player1HP}");
        }

        if (player1HP <= 0 || player2HP <= 0)
            EndGame();
        else
        {
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
            StartCoroutine(StartRound());
        }
    }

    private void EndGame()
    {
        state = GameState.GameOver;
        Time.timeScale = 1f;

        int winner = player1HP > 0 ? 1 : 2;
        Debug.Log($"===== 游戏结束！玩家 {winner} 获胜！=====");
    }

    // =============================================
    // 公开查询接口（供 GameUI 读取）
    // =============================================

    public int GetHP(int playerIndex) => playerIndex == 1 ? player1HP : player2HP;
    public int GetCurrentPlayer() => currentPlayer;
    public bool IsGameOver() => state == GameState.GameOver;
    public bool IsFrozen() => state == GameState.Frozen;
}