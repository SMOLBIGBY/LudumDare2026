using UnityEngine;

public class DoorScript : MonoBehaviour, IActivatable
{
    Animator animator;
    Collider2D collider2D;
    bool isActivated = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            Activate();
        }
    }

    public void Activate()
    {
        collider2D.enabled = false;
        animator.SetTrigger("OpenDoor");
    }
}
