using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCooldownState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        Debug.Log("Cooling down.");
        controller.Spawner.SetSpawnEnabled(false);
        KillsLeft = controller.Spawner.GetNumAlive();
    }

    public override void Update(GameController controller)
    {
        // Update kills remaining
        KillsLeft = controller.Spawner.GetNumAlive();
        if (KillsLeft <= 0)
        {
            controller.TransitionToState(controller.RestState);
        }
    }
}
