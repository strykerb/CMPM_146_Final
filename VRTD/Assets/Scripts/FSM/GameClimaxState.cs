﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClimaxState : GameBaseState
{
    public override void EnterState(GameController controller)
    {
        Debug.Log("Reached climax.");
        MaxTimeInState = 5.0f;
        controller.Jukebox.PlayMusic(2);

        //Debug.Log("Climax Removal: " + controller.RemoveEnemyFromPool(controller.Spawner.slowZombie));
        controller.ModifySpawnFrequency(5);
    }

    public override void Update(GameController controller)
    {
        if (CheckGoalReached(controller))
        {
            controller.TransitionToState(controller.CooldownState);
        }
    }

    public override bool CheckGoalReached(GameController controller)
    {
        if (controller.TimeInState > MaxTimeInState)
            return true;
        return false;
    }
}
