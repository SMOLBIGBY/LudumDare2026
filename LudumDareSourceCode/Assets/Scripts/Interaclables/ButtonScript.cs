using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    Animator animator;
    public List<MonoBehaviour> targets; // assign objects in Inspector

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            PressButton();
        }
    }

    public void PressButton()
    {
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

}