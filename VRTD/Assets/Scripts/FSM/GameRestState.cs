using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRestState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        Debug.Log("Resting.");
        MaxTimeInState = 20.0f;
        controller.Jukebox.PlayMusic(0);
    }

    public override void Update(GameController controller)
    {
        if (CheckGoalReached(controller.RestingHR, controller.CurrentHR, controller.TimeInState))
        {
            controller.TransitionToState(controller.BuildupState);
        }
    }
}
