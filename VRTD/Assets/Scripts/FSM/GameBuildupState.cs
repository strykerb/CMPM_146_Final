using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBuildupState : GameBaseState
{
    public float preludeDuration = 20f;         // Length of transition time between rest and scare
    bool prelude_concluded = false;

    public override void EnterState(GameController controller)
    {
        Debug.Log("Building up.");
        controller.Spawner.SetSpawnEnabled(true);
        controller.Jukebox.PlayMusic(1);
        controller.FindRestingHR();
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
        if (CheckGoalReached(controller.RestingHR, controller.CurrentHR, controller.TimeInState))
        {
            controller.TransitionToState(controller.ClimaxState);
        }

        // Give a buffer between rest and maximum spawn
        if (!prelude_concluded && controller.TimeInState > preludeDuration)
        {
            // Only do this once
            prelude_concluded = true;

            // Play some scary sound

            // Spawns are more frequent, able to spawn fast zombies, and more total zombies are allowed
            Debug.Log("Add fast: " + controller.AddEnemyToPool(controller.Spawner.fastZombie));
            Debug.Log("Add crawl: " + controller.AddEnemyToPool(controller.Spawner.crawlerZombie));
            controller.ModifyMaxEnemies(20);
            controller.ModifySpawnFrequency(2);
        }
    }


}
