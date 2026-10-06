using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle instructionStyle;
    private GUIStyle buttonStyle;

    private void EnsureStyles()
    {
        if (titleStyle != null) return;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 42,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        subtitleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18,
            normal = { textColor = new Color(0.55f, 0.9f, 1f) }
        };
        instructionStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 16,
            wordWrap = true,
            normal = { textColor = Color.white }
        };
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 23,
            fontStyle = FontStyle.Bold
        };
    }

    private void OnGUI()
    {
        EnsureStyles();

        float panelWidth = Mathf.Min(560f, Screen.width - 32f);
        float panelHeight = 450f;
        Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);

        GUI.color = new Color(0.015f, 0.04f, 0.075f, 0.94f);
        GUI.Box(panel, GUIContent.none);
        GUI.color = Color.white;

        GUI.Label(new Rect(panel.x + 24f, panel.y + 24f, panel.width - 48f, 60f), "ONE MORE TURN", titleStyle);
        GUI.Label(new Rect(panel.x + 30f, panel.y + 83f, panel.width - 60f, 34f), "Break the loop. Reach the extraction point.", subtitleStyle);

        GUI.color = new Color(0.08f, 0.9f, 1f, 0.9f);
        GUI.Box(new Rect(panel.x + 48f, panel.y + 132f, panel.width - 96f, 2f), GUIContent.none);
        GUI.color = Color.white;

        GUI.Label(new Rect(panel.x + 64f, panel.y + 150f, panel.width - 128f, 190f),
            "HOW TO PLAY\n\nWASD  Move\nMouse  Aim\nHold Left Mouse Button  Fire\nE  Activate the terminal\nR  Rewind 4 seconds and create an Echo\n\nEcho: distracts enemies; breaks the boss shield.\nCharge: +1 every 30 seconds.", instructionStyle);

        float buttonWidth = Mathf.Min(300f, panel.width - 80f);
        Rect startButton = new Rect((Screen.width - buttonWidth) * 0.5f, panel.y + panel.height - 88f, buttonWidth, 60f);
        if (GUI.Button(startButton, "START", buttonStyle))
            SceneManager.LoadScene("Main");
    }
}
