using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalScript : MonoBehaviour
{
    Animator animator;
    StateManager stateManager;
    LevelManager levelManager;
    [SerializeField] int CurrentLevel = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        animator = GetComponent<Animator>();
        stateManager = FindObjectOfType<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (CurrentLevel== 10)
            {
                SceneManager.LoadScene("WinScreen");
            }
            LevelManager.Instance.UnlockNextLevel(CurrentLevel);
            stateManager.HasStopped = true;
            Debug.Log("Robot has won!");
            stateManager.HasWon = true;
            animator.SetTrigger("Teleport");
        }
    }
}
