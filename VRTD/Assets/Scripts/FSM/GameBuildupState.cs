using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBuildupState : GameBaseState
{
    public float maxPreludeDuration = 30f;          // Max Length of transition time between rest and scare
    bool prelude_concluded = false;
    int TransitionThreshold = 4;                    // stress reading count over last 5 windows required for transition

    public override void EnterState(GameController controller)
    {
        Debug.Log("Building up.");
        prelude_concluded = false;
        controller.Spawner.SetSpawnEnabled(true);
        controller.Jukebox.PlayMusic(1);
        //controller.Jukebox.SetSpatialStressorsOn(true);
        TargetMul = 1.2f;
        TargetHR = controller.RestingHR * TargetMul;
        MaxTimeInState = 60.0f;

        // Manipulate Spawns
        Debug.Log("Buildup Add: " + controller.AddEnemyToPool(controller.Spawner.slowZombie));
        Debug.Log("Buildup Removal: " + controller.RemoveEnemyFromPool(controller.Spawner.fastZombie));
        Debug.Log("Buildup Removal: " + controller.RemoveEnemyFromPool(controller.Spawner.crawlerZombie));
        controller.ModifyMaxEnemies(4);
        controller.ModifySpawnFrequency(3);
    }

    public override void Update(GameController controller)
    {
        if (CheckGoalReached(controller))
        {
            controller.TransitionToState(controller.ClimaxState);
        }

        // Give a buffer between rest and maximum spawn. End buffer at first stressed reading.
        if (!prelude_concluded)
        {
            // Continue prelude if max time not met and player is not stressed
            if (!(controller.TimeInState > maxPreludeDuration || controller.Stressed))
                return;

            // Only do this once
            prelude_concluded = true;

            // Play some scary sounds
            controller.Jukebox.SetSpatialStressorsOn(true);

            // Spawns are more frequent, able to spawn fast zombies, and more total zombies are allowed
            Debug.Log("Add fast: " + controller.AddEnemyToPool(controller.Spawner.fastZombie));
            Debug.Log("Add crawl: " + controller.AddEnemyToPool(controller.Spawner.crawlerZombie));
            Debug.Log("Removal: " + controller.RemoveEnemyFromPool(controller.Spawner.slowZombie));
            controller.ModifyMaxEnemies(20);
            controller.ModifySpawnFrequency(2);
        }
    }

    public override bool CheckGoalReached(GameController controller)
    {
        if (controller.TimeInState > MaxTimeInState)
            return true;

        if (controller.CurrentSensorMode != GameController.SensorMode.Complex)
            return false;

        int stress_count = 0;

        // if four out of the last five sensor readings indicate the player 
        // is relaxed, transition to build up.
        foreach (bool stressed in controller.StressHistory)
        {
            if (stressed)
                stress_count += 1;
        }

        return (stress_count >= TransitionThreshold);
    }


}
