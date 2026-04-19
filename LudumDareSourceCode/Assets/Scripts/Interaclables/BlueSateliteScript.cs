using System.Collections.Generic;

using UnityEngine;

public class BlueSateliteScript : MonoBehaviour, IActivatable
{
    Animator animator;
    Collider2D collider2D;
    bool isActivated = false;
    public List<MonoBehaviour> targets; // assign objects in Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame


    public void Activate()
    {
        animator.SetTrigger("Activate");
        Disable();
    }
    public void Disable()
    {
        Debug.Log("Blue Satelite!");
        foreach (var target in targets)
        {
            if (target is IActivatable activatable)
            {
                activatable.Activate();
            }
        }
    }
}
