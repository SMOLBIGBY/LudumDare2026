using System;

public static class RobotManagerScript
{
    public static Action OnMyFunction;

    public static void CallMyFunction()
    {
        OnMyFunction?.Invoke();
    }
}