using UnityEngine;
using TMPro;

public class CommandsTextBoxScript : MonoBehaviour
{
    RobotScript robotScript;
    GhostRobotScript ghostScript;
    public TMP_Text commandText;

    void Start()
    {
        robotScript = FindObjectOfType<RobotScript>();
        ghostScript = FindObjectOfType<GhostRobotScript>();

        UpdateCommandText();
    }

    public void UpdateCommandText()
    {
        if (ghostScript == null || robotScript == null) return;

        string text = "Commands: ";

        // Executing command (same line as "Commands:")
        if (robotScript.currentCommand != null)
        {
            text += "<color=yellow><b>Executing:</b> "
                 + DirectionToString(robotScript.currentCommand.Value)
                 + "</color>";
        }

        // New line BEFORE queue only
        text += "\n\n";

        // Queued commands
        foreach (Vector2 cmd in ghostScript.commandList)
        {
            text += DirectionToString(cmd) + "\n";
        }

        commandText.text = text;
    }

    string DirectionToString(Vector2 dir)
    {
        if (dir == Vector2.right) return "→ <b>Right</b>";
        if (dir == Vector2.left) return "← <b>Left</b>";
        if (dir == Vector2.up) return "↑ <b>Up</b>";
        if (dir == Vector2.down) return "↓ <b>Down</b>";

        return "Unknown";
    }
}