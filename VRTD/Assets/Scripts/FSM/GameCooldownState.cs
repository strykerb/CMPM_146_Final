using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCooldownState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        MaxTimeInState = 60.0f;
    }

    public override void Update(GameController controller)
    {
        Debug.Log("Cooling down.");
        if (CheckGoalReached(controller.RestingHR, controller.CurrentHR, controller.TimeInState))
        {
            controller.TransitionToState(controller.RestState);
        }
    }
}
