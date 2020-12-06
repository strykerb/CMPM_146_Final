using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClimaxState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        MaxTimeInState = 5.0f;
    }

    public override void Update(GameController controller)
    {
        Debug.Log("Climax state ongoing");
        if (CheckGoalReached(controller.RestingHR, controller.CurrentHR, controller.TimeInState))
        {
            controller.TransitionToState(controller.CooldownState);
        }
    }
}
