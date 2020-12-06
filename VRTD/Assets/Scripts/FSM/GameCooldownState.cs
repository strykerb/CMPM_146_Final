using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCooldownState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        KillsLeft = controller.Spawner.GetNumAlive();
    }

    public override void Update(GameController controller)
    {
        Debug.Log("Cooling down.");
        // Update kills remaining
        KillsLeft = controller.Spawner.GetNumAlive();
        if (KillsLeft <= 0)
        {
            controller.TransitionToState(controller.RestState);
        }
    }
}
