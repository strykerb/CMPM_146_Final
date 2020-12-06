using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBuildupState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        controller.GetRestingHR();
        TargetMul = 1.2f;
        TargetHR = controller.RestingHR * TargetMul;
        MaxTimeInState = 60.0f;
    }

    public override void Update(GameController controller)
    {
        Debug.Log("Building Up");
        if (CheckGoalReached(controller.RestingHR, controller.CurrentHR, controller.TimeInState))
        {
            controller.TransitionToState(controller.ClimaxState);
        }
    }
}
