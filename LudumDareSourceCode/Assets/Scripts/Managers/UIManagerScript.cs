using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManagerScript : MonoBehaviour
{
    StateManager stateManager;
    RobotScript robotScript;
    public Image winScreen;
    public Image gameOverObject;
    public Image settingsScreen;
    LevelManager levelManager;
    public TMP_Text SygnalText;
    public TMP_Text CommandCountText;
    GhostRobotScript ghost;
    BlackScreenFade blackScreenFade;
    AudioManager audioManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        blackScreenFade = FindObjectOfType<BlackScreenFade>();
        ghost = FindObjectOfType<GhostRobotScript>();
        robotScript = FindObjectOfType<RobotScript>();
        StartCoroutine(SignalLoop());

        settingsScreen.gameObject.SetActive(false);
        gameOverObject.gameObject.SetActive(false);
        stateManager = FindObjectOfType<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CommandCountText.text = ghost.commandCount + "/" + ghost.maxCommands;

        if (stateManager.HasWon)
            winScreen.gameObject.SetActive(true);

        if (stateManager.HadDied)
        {
            audioManager.PlaySFX(audioManager.deathSound);
            gameOverObject.gameObject.SetActive(true);
        }


    }

    public void StartButton()
    {
        // Call the function in RobotManagerScript to start the robot
        if (!stateManager.HasStarted)
        {
            StartCoroutine(StartButtonDelay());
            stateManager.HasStarted = true; // Set this to true to prevent multiple starts
        }
    }

    IEnumerator StartButtonDelay()
    {
        yield return new WaitForSeconds(0.5f);
        ghost.EnableGhostSprite();
        RobotManagerScript.CallMyFunction();

    }

    public void RestartButton()
    {
        // Reload the current scene to restart the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void OpenSettings()
    {
        // Implement your settings menu logic here
        settingsScreen.gameObject.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsScreen.gameObject.SetActive(false);

    }

    public void BackToIntro()
    {
        // Load the Intro scene (make sure to add it to your build settings)
        SceneManager.LoadScene("Menu");

    }

    public void ChooseLevelScreenButton()
    {
        // Use the static Instance directly!
        if (LevelManager.Instance != null)
        {
            Debug.Log("UIManager: Sending command to LevelManager Instance.");
            LevelManager.Instance.GoToLevelSelect();
        }
        else
        {
            Debug.LogError("UIManager: LevelManager.Instance is NULL!");
        }
    }

    IEnumerator SignalLoop()
    {
        bool blinkState = false;

        while (true)
        {
            if (robotScript != null && robotScript.HasSignal)
            {
                // BLINK GREEN WHEN ON
                blinkState = !blinkState;

                if (blinkState)
                    SygnalText.text = "<color=green>SIGNAL: ON</color>";
                else
                    SygnalText.text = "";

                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // SOLID RED WHEN OFF
                SygnalText.text = "<color=red>SIGNAL: OFF</color>";
                yield return null; // no blinking, just stable text
            }
        }
    }

}
