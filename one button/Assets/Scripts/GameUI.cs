using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 游戏 UI 管理器 - 挂载在 Canvas 对象上
/// 负责：血量条、回合提示、攻击数值、胜负画面
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("玩家1 UI")]
    [Tooltip("玩家1 血量条 Slider")]
    public Slider player1HPBar;
    [Tooltip("玩家1 血量数字，例如显示 '80 / 100'")]
    public TextMeshProUGUI player1HPText;

    [Header("玩家2 UI")]
    [Tooltip("玩家2 血量条 Slider")]
    public Slider player2HPBar;
    [Tooltip("玩家2 血量数字")]
    public TextMeshProUGUI player2HPText;

    [Header("回合提示")]
    [Tooltip("显示当前是哪位玩家的回合，例如 '玩家1 的回合'")]
    public TextMeshProUGUI turnText;

    [Header("操作提示")]
    [Tooltip("提示玩家按空格键，筹码飞行时显示，冻结后隐藏")]
    public TextMeshProUGUI promptText;

    [Header("攻击结算")]
    [Tooltip("冻结后显示本轮攻击数值，例如 '攻击数值：15'")]
    public TextMeshProUGUI damageText;

    [Header("胜负画面")]
    [Tooltip("胜负画面的根对象，平时隐藏，游戏结束时显示")]
    public GameObject gameOverPanel;
    [Tooltip("胜负画面上的文字，例如 '玩家1 获胜！'")]
    public TextMeshProUGUI gameOverText;
    [Tooltip("重新开始按钮")]
    public Button restartButton;

    [Header("引用")]
    public GameManager gameManager;

    private int player1MaxHP;
    private int player2MaxHP;

    // =============================================
    // 初始化
    // =============================================

    void Start()
    {
        player1MaxHP = gameManager.player1MaxHP;
        player2MaxHP = gameManager.player2MaxHP;

        // 初始化血量条范围
        if (player1HPBar != null) { player1HPBar.minValue = 0; player1HPBar.maxValue = player1MaxHP; }
        if (player2HPBar != null) { player2HPBar.minValue = 0; player2HPBar.maxValue = player2MaxHP; }

        // 胜负画面默认隐藏
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // 伤害数值默认隐藏
        if (damageText != null) damageText.gameObject.SetActive(false);

        // 重启按钮绑定事件
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        RefreshAll();
    }

    // =============================================
    // 每帧刷新
    // =============================================

    void Update()
    {
        if (gameManager == null) return;
        RefreshAll();
    }

    // =============================================
    // 刷新所有 UI
    // =============================================

    private void RefreshAll()
    {
        RefreshHP();
        RefreshTurnAndPrompt();
        RefreshGameOver();
    }

    /// <summary>
    /// 刷新双方血量条和血量数字
    /// </summary>
    private void RefreshHP()
    {
        int hp1 = gameManager.GetHP(1);
        int hp2 = gameManager.GetHP(2);

        if (player1HPBar != null) player1HPBar.value = hp1;
        if (player2HPBar != null) player2HPBar.value = hp2;

        if (player1HPText != null) player1HPText.text = $"{hp1} / {player1MaxHP}";
        if (player2HPText != null) player2HPText.text = $"{hp2} / {player2MaxHP}";
    }

    /// <summary>
    /// 刷新回合提示和操作提示
    /// </summary>
    private void RefreshTurnAndPrompt()
    {
        if (gameManager.IsGameOver()) return;

        int current = gameManager.GetCurrentPlayer();

        if (turnText != null)
            turnText.text = $"玩家 {current} 的回合";

        if (promptText != null)
        {
            if (gameManager.IsFrozen())
                promptText.gameObject.SetActive(false);
            else
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "按 Space 键停止";
            }
        }
    }

    /// <summary>
    /// 游戏结束时显示胜负画面
    /// </summary>
    private void RefreshGameOver()
    {
        if (!gameManager.IsGameOver()) return;

        if (gameOverPanel != null && !gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(true);

            int winner = gameManager.GetHP(1) > 0 ? 1 : 2;
            if (gameOverText != null)
                gameOverText.text = $"玩家 {winner} 获胜！";
        }
    }

    // =============================================
    // 公开方法：由 GameManager 主动调用
    // =============================================

    /// <summary>
    /// 显示本轮攻击数值（GameManager 冻结后调用）
    /// </summary>
    public void ShowDamage(int damage, int attackingPlayer)
    {
        if (damageText == null) return;
        damageText.gameObject.SetActive(true);
        damageText.text = $"玩家 {attackingPlayer} 攻击：{damage}";
    }

    /// <summary>
    /// 隐藏攻击数值（下一回合开始时调用）
    /// </summary>
    public void HideDamage()
    {
        if (damageText != null)
            damageText.gameObject.SetActive(false);
    }

    // =============================================
    // 重新开始
    // =============================================

    private void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}