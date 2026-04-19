using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TypewriterDialogue : MonoBehaviour
{
    DialogueTrigger dialogueTrigger;
    public TextMeshProUGUI dialogueText;
    public Image dialogueBox;
    public float typingSpeed = 0.03f;

    private Queue<string> sentences = new Queue<string>();
    private Coroutine typingCoroutine;

    void Start()
    {
        dialogueTrigger = FindAnyObjectByType<DialogueTrigger>();
        if (dialogueBox != null)
            dialogueBox.gameObject.SetActive(false);
    }

    public void StartDialogue(string[] dialogueLines)
    {
        dialogueTrigger.IsPlayerTalkingWithThisNPC = true;
        if (dialogueBox == null || dialogueText == null)
        {
            Debug.LogError("Dialogue UI not assigned!");
            return;
        }

        dialogueBox.gameObject.SetActive(true);
        sentences.Clear();

        foreach (string sentence in dialogueLines)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    void Update()
    {
        if (!dialogueTrigger.IsPlayerTalkingWithThisNPC)
        {
            EndDialogue();
        }

        if (dialogueBox != null && dialogueBox.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextSentence();
            }
        }
    }

    public void DisplayNextSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }

    public void EndDialogue()
    {
        dialogueTrigger.IsPlayerTalkingWithThisNPC = false;
        if (dialogueBox != null)
            dialogueBox.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        Debug.Log("Dialogue finished");
    }
}