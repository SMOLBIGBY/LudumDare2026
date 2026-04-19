using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostRobotScript : MonoBehaviour
{
    private float step = 1f;
    private float moveInterval = 0.15f;
    public int maxCommands = 30;
    public bool isMoving = false;
    public int commandCount;

    public List<Vector2> commandList = new List<Vector2>();
    private CommandsTextBoxScript uiScript;
    private SpriteRenderer spriteRenderer;
    private Transform robotPos;
    private Vector2 lastDirection = Vector2.down;
    StateManager stateManager;
    private bool isResetting = false;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "ChooseLevelScreen")
        {
            audioManager.PlayMusic(audioManager.menuMusic);

        }
        else
        {
            audioManager.PlayMusic(audioManager.levelMusic);

        }



        if (currentScene.name != "ChooseLevelScreen")
        {
            stateManager = FindObjectOfType<StateManager>();
            commandCount = maxCommands;


            robotPos = FindObjectOfType<RobotScript>().transform;
            spriteRenderer = GetComponent<SpriteRenderer>();
            uiScript = FindObjectOfType<CommandsTextBoxScript>();
            transform.position = robotPos.position;
        }
    }

    void Update()
    {
        if (isResetting) return;
        commandCount = commandList.Count;
        StateManager stateManager = FindObjectOfType<StateManager>();
        if (stateManager == null) return;

        // Check this FIRST

        if (stateManager.HasStarted)
        {
            spriteRenderer.enabled = false; // Show the ghost if we haven't started
        }

        if (isMoving) return;

        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "ChooseLevelScreen")
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AddAndMove(Vector2.right);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AddAndMove(Vector2.left);

        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            AddAndMove(Vector2.up);

        }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                AddAndMove(Vector2.down);

            }
        }


        if (currentScene.name != "ChooseLevelScreen")
            if (Input.GetKeyDown(KeyCode.Z)) UndoLastCommand();
    }
    bool IsOpposite(Vector2 a, Vector2 b)
    {
        return a + b == Vector2.zero;
    }
    private void AddAndMove(Vector2 dir)
    {

        if(stateManager.HasStarted) return; // Don't allow input after starting
        {
            if (commandList.Count >= maxCommands) return;

        // ❌ BLOCK OPPOSITE DIRECTION INPUT
            if (commandList.Count > 0 && IsOpposite(dir, lastDirection))
            {
                return; // ignore input
            }

            commandList.Add(dir);
            lastDirection = dir;

            if (uiScript != null) uiScript.UpdateCommandText();
                StartCoroutine(SmoothMove(dir));
        }
    }

    public void UndoLastCommand()
    {
        if (commandList.Count > 0 && !isMoving)
        {
            Vector2 removed = commandList[commandList.Count - 1];
            commandList.RemoveAt(commandList.Count - 1);
            if (uiScript != null) uiScript.UpdateCommandText();
            StartCoroutine(SmoothMove(-removed)); // Move back
        }
    }

    public void ClearCommands()
    {
        commandList.Clear();
        if (uiScript != null) uiScript.UpdateCommandText();
    }

    IEnumerator SmoothMove(Vector2 direction)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "ChooseLevelScreen")
            audioManager.PlaySFX(audioManager.ghostMovement);
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(direction * step);
        float elapsed = 0f;

        while (elapsed < moveInterval)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveInterval);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }
    public void EnableGhostSprite()
    {
        spriteRenderer.enabled = true; // Show the ghost
    }
    public void ResetGhost()
    {
        if (!isResetting)
            StartCoroutine(ResetWhenPlayerStops());
    }
    IEnumerator ResetWhenPlayerStops()
    {
        isResetting = true;

        RobotScript robot = FindObjectOfType<RobotScript>();

        while (robot != null && robot.isMoving)
        {
            yield return null;
        }

        commandCount = 0;
        ClearCommands();

        stateManager.HasStarted = false;

        yield return StartCoroutine(GhostImageDelay());

        isResetting = false;
    }
    IEnumerator GhostImageDelay()
    {
        yield return new WaitForSeconds(0.5f);
        transform.position = robotPos.position;
        EnableGhostSprite();
    }

}