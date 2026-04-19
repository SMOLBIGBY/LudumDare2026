using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public GameObject chains; // <-- assign this in Inspector

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(LoadLevel);
    }

    void OnEnable()
    {
        StartCoroutine(SetupButton());
    }

    IEnumerator SetupButton()
    {
        while (LevelManager.Instance == null)
        {
            yield return null;
        }

        int unlockedLevel = LevelManager.Instance.GetUnlockedLevel();

        bool isUnlocked = levelIndex <= unlockedLevel;

        button.interactable = isUnlocked;
        UpdateChains(!isUnlocked);
    }

    void UpdateChains(bool showChains)
    {
        if (chains != null)
        {
            chains.SetActive(showChains);
        }
    }

    public void LoadLevel()
    {
        if (LevelManager.Instance != null)
        {
            Debug.Log("Loading Level " + levelIndex);
            LevelManager.Instance.LoadLevel("Level" + levelIndex);
        }
    }
}