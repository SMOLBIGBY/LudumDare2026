using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private BlackScreenFade blackScreenFade;
    AudioManager audioManager;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        blackScreenFade = FindObjectOfType<BlackScreenFade>();

        // 🔥 Safety check for UI system
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("EventSystem missing in scene: " + scene.name);
        }
    }

    // -------------------------
    // LEVEL SYSTEM
    // -------------------------

    public int GetUnlockedLevel()
    {

        return PlayerPrefs.GetInt("UnlockedLevel", 1);
    }

    public void UnlockNextLevel(int currentLevel)
    {
        int unlocked = GetUnlockedLevel();

        if (currentLevel >= unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            Debug.Log("Unlocked level: " + (currentLevel + 1));
            PlayerPrefs.Save();
        }
    }

    // -------------------------
    // BUTTON FRIENDLY METHOD
    // -------------------------

    // 👉 Assign THIS to Button OnClick (NO STRING NEEDED)

    // Optional: if you still want string loading
    public void LoadLevel(string sceneName)
    {


        Debug.Log("Loading scene: " + sceneName);

        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        if (blackScreenFade != null)
            blackScreenFade.FadeIn();

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(sceneName);
    }

    // -------------------------
    // MENU NAVIGATION
    // -------------------------

    public void GoToLevelSelect()
    {
        LoadLevel("ChooseLevelScreen");
    }

    public void GOToMainMenu()
    {
        LoadLevel("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // -------------------------
    // DEBUG RESET
    // -------------------------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs cleared");
        }
    }
}