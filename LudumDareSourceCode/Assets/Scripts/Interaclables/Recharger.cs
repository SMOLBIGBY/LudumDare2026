using System.Collections.Generic;

using UnityEngine;

public class Recharger : MonoBehaviour, IActivatable
{
    StateManager stateManager;
    GhostRobotScript ghost;
    RobotScript robotScript;
    Animator animator;
    bool isActivated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateManager = FindObjectOfType<StateManager>();
        ghost = FindObjectOfType<GhostRobotScript>();
        robotScript = FindObjectOfType<RobotScript>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame


    public void Activate()
    {

        RobotScript robot = FindObjectOfType<RobotScript>();
        if (robot != null)
        {
            robot.StopProcessing();
        }
        RobotManagerScript.CallMyFunction();
        stateManager.HasStarted = false;
        ghost.isMoving = false;
        ghost.ResetGhost();
        robotScript.hasMadeFirstMove = false;
    }

}
