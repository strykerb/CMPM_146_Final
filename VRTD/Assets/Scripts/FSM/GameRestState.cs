using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRestState : GameBaseState
{
    
    int TransitionThreshold = 4;        // stress reading count over last 5 windows required for transition
    public override void EnterState(GameController controller)
    {
        Debug.Log("Resting.");
        MaxTimeInState = 20.0f;
        controller.Jukebox.PlayMusic(0);
    }

    public override void Update(GameController controller)
    {
        if (CheckGoalReached(controller))
        {
            controller.TransitionToState(controller.BuildupState);
        }
    }

    public override bool CheckGoalReached(GameController controller)
    {
        if (controller.TimeInState > MaxTimeInState)
            return true;

        if (controller.CurrentSensorMode != GameController.SensorMode.Complex && controller.CurrentSensorMode != GameController.SensorMode.ML)
            return false;

        int relax_count = 0;

        // if four out of the last five sensor readings indicate the player 
        // is relaxed, transition to build up.
        foreach (bool stressed in controller.StressHistory)
        {
            if (!stressed)
                relax_count += 1;
        }

        return (relax_count >= TransitionThreshold);
    }
}
