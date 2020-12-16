using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCooldownState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        Debug.Log("Cooling down.");
        controller.Spawner.SetSpawnEnabled(false);
        controller.Jukebox.PlayMusic(3);
        controller.Jukebox.SetSpatialStressorsOn(false);
        KillsLeft = controller.Spawner.GetNumAlive();
    }

    public override void Update(GameController controller)
    {
        if (CheckGoalReached(controller))
        {
            controller.TransitionToState(controller.RestState);
        }
    }

    public override bool CheckGoalReached(GameController controller)
    {

        // Update kills remaining
        KillsLeft = controller.Spawner.GetNumAlive();
        if (KillsLeft <= 0)
            return true;
        return false;
    }
}
