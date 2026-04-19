using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public TypewriterDialogue typewriterDialogue;

    public string[] dialogueLines;


    [SerializeField] Image letterToPress;
    bool _detectedPlayer = false;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] string dialogueText;
    public Coroutine hideDelayCoroutine;

    public bool IsPlayerTalkingWithThisNPC = false;





    void Awake()
    {
        typewriterDialogue = FindAnyObjectByType<TypewriterDialogue>();
    }

    void Start()
    {
        letterToPress.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_detectedPlayer && Input.GetKeyDown(KeyCode.E) && !IsPlayerTalkingWithThisNPC)
        {
            typewriterDialogue.StartDialogue(dialogueLines);
            IsPlayerTalkingWithThisNPC = true;
        }

        if (typewriterDialogue == null)
        {
            typewriterDialogue = FindAnyObjectByType<TypewriterDialogue>();
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _detectedPlayer = true;
            letterToPress.gameObject.SetActive(true);
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        IsPlayerTalkingWithThisNPC = false;
        if (other.CompareTag("Player"))
        {
            _detectedPlayer = false;
            letterToPress.gameObject.SetActive(false);
        }
    }

}
