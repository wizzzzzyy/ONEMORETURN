using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool ExtractionActive { get; private set; }
    public bool BossDefeatedFlag { get; private set; }
    private bool won, lost;
    private string toast = "";
    private float toastUntil;
    private string exitMessage = "";
    private RewindSystem rewind;
    private PlayerController player;
    private BossController boss;
    private GUIStyle heading, body, button;

    private void Awake()
    {
        Instance = this;
        rewind = FindFirstObjectByType<RewindSystem>();
        player = FindFirstObjectByType<PlayerController>();
        boss = FindFirstObjectByType<BossController>();
    }

    public void EnemyDefeated(bool echoKill)
    {
        rewind?.AddCharge();
        ShowToast(echoKill ? "ECHO ELIMINATION  +1 CHARGE" : "ENEMY ELIMINATED  +1 CHARGE");
    }

    public void ObjectiveActivated()
    {
        if (ExtractionActive || won) return;
        ExtractionActive = true;
        rewind?.AddCharge();
        ExtractionZone exit = FindFirstObjectByType<ExtractionZone>();
        if (exit) exit.Activate();
        boss?.Activate();
        ShowToast("TERMINAL ONLINE  —  ECHO SHOTS BREAK THE BOSS SHIELD");
    }

    public void BossShieldBroken() => ShowToast("SHIELD DOWN  —  PLAYER SHOTS DAMAGE THE BOSS");
    public void BossShieldRecharged() => ShowToast("BOSS SHIELD RECHARGED  —  USE THE ECHO");
    public void ShowBossActivated() => ShowToast("BOSS AWAKENED  —  SHIELD RESPONDS TO ECHOES");
    public void ShowBossPulse() => ShowToast("BOSS PULSE");
    public void ShowChargeRecharged() => ShowToast("ECHO CHARGE RECHARGED  +1");

    public void BossDefeated()
    {
        BossDefeatedFlag = true;
        ExtractionZone exit = FindFirstObjectByType<ExtractionZone>();
        if (exit) exit.Unlock();
        ShowToast("BOSS DEFEATED  —  REACH THE EXIT");
    }

    public void CompleteExtraction()
    {
        if (!ExtractionActive || !BossDefeatedFlag || won) return;
        ExtractionActive = false;
        won = true;
        ShowToast("EXTRACTION COMPLETE");
    }

    public void SetGameOver() { lost = true; ShowToast("TIMELINE COLLAPSED"); }
    public void ShowEchoCreated() => ShowToast("ECHO CREATED");
    private void ShowToast(string text) { toast = text; toastUntil = Time.unscaledTime + 2.2f; }

    private void EnsureStyles()
    {
        if (heading != null) return;
        heading = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
        body = new GUIStyle(GUI.skin.label) { fontSize = 15, normal = { textColor = Color.white } };
        button = new GUIStyle(GUI.skin.button) { fontSize = 17, fontStyle = FontStyle.Bold };
    }

    private void DrawBar(Rect rect, float value, Color color)
    {
        GUI.color = new Color(0.02f, 0.03f, 0.05f, 0.88f);
        GUI.Box(rect, GUIContent.none);
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.x + 3f, rect.y + 3f, Mathf.Max(0f, (rect.width - 6f) * Mathf.Clamp01(value)), rect.height - 6f), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void RetryRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_WEBGL && !UNITY_EDITOR
        exitMessage = "Close this browser tab to exit the game.";
#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnGUI()
    {
        EnsureStyles();
        GUI.color = new Color(0.035f, 0.07f, 0.12f, 0.9f);
        GUI.Box(new Rect(18, 18, 400, 220), GUIContent.none);
        GUI.color = Color.white;
        GUI.Label(new Rect(34, 27, 370, 34), "ONE MORE TURN", heading);
        string objective = !ExtractionActive ? "Objective: activate the terminal" :
            !BossDefeatedFlag ? "Boss: Echo breaks shield; player damages boss" : "Boss down: reach the green exit";
        GUI.Label(new Rect(34, 66, 370, 24), objective, body);
        GUI.Label(new Rect(34, 91, 300, 24), "Integrity: " + (player ? Mathf.CeilToInt(player.Health) : 0) + "%", body);
        GUI.Label(new Rect(34, 116, 360, 24), "Echo charges: " + (rewind ? rewind.Charges + "/" + rewind.MaxCharges : "0"), body);
        GUI.Label(new Rect(34, 141, 370, 23), "INSTRUCTION: Echo recharges +1 every 30 seconds", body);
        string recharge = !rewind ? "" : rewind.Charges >= rewind.MaxCharges
            ? "Charges full"
            : "Next charge in: " + Mathf.CeilToInt(rewind.SecondsUntilNextCharge) + "s";
        GUI.Label(new Rect(34, 164, 300, 23), recharge, body);
        if (boss && boss.IsActive)
        {
            GUI.Label(new Rect(34, 190, 100, 20), boss.ShieldIsUp ? "SHIELD" : "BOSS HP", body);
            DrawBar(new Rect(132, 193, 250, 17), boss.ShieldIsUp ? boss.ShieldNormalized : boss.HealthNormalized,
                boss.ShieldIsUp ? new Color(0.08f, 0.88f, 1f) : new Color(1f, 0.25f, 0.2f));
        }
        GUI.Label(new Rect(20, Screen.height - 72, 540, 52), "WASD move   Mouse aim   Hold LMB fire   E activate   R rewind", body);
        if (GUI.Button(new Rect(Screen.width - 210, Screen.height - 82, 190, 58), "REWIND  [R]", button)) rewind?.TryRewind();
        if (Time.unscaledTime < toastUntil)
        {
            GUI.color = new Color(0.12f, 0.78f, 0.94f, 0.95f);
            GUI.Box(new Rect(Screen.width * 0.5f - 250, 32, 500, 48), toast, heading);
            GUI.color = Color.white;
        }
        if (won || lost)
        {
            float panelWidth = Mathf.Min(560f, Screen.width - 32f);
            float panelHeight = 250f;
            float panelX = (Screen.width - panelWidth) * 0.5f;
            float panelY = (Screen.height - panelHeight) * 0.5f;
            GUI.color = new Color(0.01f, 0.03f, 0.06f, 0.88f);
            GUI.Box(new Rect(panelX, panelY, panelWidth, panelHeight), GUIContent.none);
            GUI.color = Color.white;
            GUI.Label(new Rect(panelX + 24f, panelY + 22f, panelWidth - 48f, 42f), won ? "EXTRACTION COMPLETE" : "RUN ENDED", heading);
            GUI.Label(new Rect(panelX + 24f, panelY + 72f, panelWidth - 48f, 30f), won ? "The Echo bought you a way out." : "The timeline collapsed. Try again?", body);

            float buttonWidth = (panelWidth - 80f) / 3f;
            float buttonY = panelY + 123f;
            if (GUI.Button(new Rect(panelX + 20f, buttonY, buttonWidth, 54f), "RETRY", button)) RetryRun();
            if (GUI.Button(new Rect(panelX + 40f + buttonWidth, buttonY, buttonWidth, 54f), "MAIN MENU", button)) ReturnToMainMenu();
            if (GUI.Button(new Rect(panelX + 60f + buttonWidth * 2f, buttonY, buttonWidth, 54f), "EXIT", button)) ExitGame();
            if (!string.IsNullOrEmpty(exitMessage))
                GUI.Label(new Rect(panelX + 24f, panelY + 190f, panelWidth - 48f, 32f), exitMessage, body);
        }
    }
}
