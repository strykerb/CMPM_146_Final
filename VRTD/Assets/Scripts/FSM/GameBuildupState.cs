using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBuildupState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        Debug.Log("Building up.");
        controller.Spawner.SetSpawnEnabled(true);
        controller.Jukebox.PlayMusic(1);
        controller.FindRestingHR();
        TargetMul = 1.2f;
        TargetHR = controller.RestingHR * TargetMul;
        MaxTimeInState = 60.0f;
    }

    public override void Update(GameController controller)
    {
        if (CheckGoalReached(controller.RestingHR, controller.CurrentHR, controller.TimeInState))
        {
            controller.TransitionToState(controller.ClimaxState);
        }
    }


}
