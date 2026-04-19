using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ReusableButtonScript : MonoBehaviour
{
    Animator animator;

    public List<MonoBehaviour> targets;

    private bool canBePressed = true;
    private bool playerInside = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePressed) return;

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            playerInside = true;
            PressButton();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            playerInside = false;

            // Only re-arm AFTER player leaves
            StartCoroutine(RearmAfterExit());
        }
    }

    public void PressButton()
    {
        if (!canBePressed) return;

        canBePressed = false;

        animator.SetTrigger("Press");

        Debug.Log("Button Pressed!");

        foreach (var target in targets)
        {
            if (target is IActivatable activatable)
            {
                activatable.Activate();
            }
        }
    }

    IEnumerator RearmAfterExit()
    {
        // wait until player leaves + small delay (optional feel buffer)
        yield return new WaitForSeconds(0.1f);

        while (playerInside)
        {
            yield return null;
        }

        animator.SetTrigger("Rearm");
        canBePressed = true;
    }
}