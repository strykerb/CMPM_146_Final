using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaselineState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        controller.FindRestingHR();
    }

    public override void Update(GameController controller)
    {
        if (controller.TimeInState > 10)
        {
            controller.TransitionToState(controller.RestState);
        }
    }
}
