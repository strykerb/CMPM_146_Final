﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/*
 * A concrete context for the state, which manipulates the scene accordingly.
 */

public class GameController : MonoBehaviour
{
    public GameBaseState CurrentState; // Current game state in FSM
    public SpawnManager Spawner;        // Enemy spawn manager to manipulate based on state

    public float TimeInState;  // Time since last state transition.
    public float RestingHR;    // Store player's approx resting heartrate.
    public float CurrentHR;    // Store player's current heartrate.

    // States we can use for the FSM
    public GameRestState RestState = new GameRestState();
    public GameBuildupState BuildupState = new GameBuildupState();
    public GameClimaxState ClimaxState = new GameClimaxState();
    public GameCooldownState CooldownState = new GameCooldownState();

    // Path to Heart Rate Data File
    string path = Path.Combine($@"{Directory.GetCurrentDirectory()}", @"Assets\HRData\data.txt");

    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(RestState);
    }

    // Update is called once per frame
    void Update()
    {
        TimeInState += Time.deltaTime;
        int read_hr = -1;
        
        try
        {
            read_hr = Int32.Parse(File.ReadLines(path).Last());
        }
        catch (IOException)
        {
            // Data File was open by ShimmerAPI, pass
        }

        Debug.Log("Heart Rate: " + read_hr + " BPM");
        if (read_hr != -1)
        {
            CurrentHR = read_hr;
        }

        CurrentState.Update(this);
    }

    // Transition between game states
    public void TransitionToState(GameBaseState State)
    {
        CurrentState = State;
        CurrentState.EnterState(this);
        TimeInState = 0.0f;
    }

    // Update spawn frequency
    public void ModifySpawnFrequency(int NewRate)
    {
        Spawner.SpawnDelay = NewRate;
    }

    // Update max enemies
    public void ModifyMaxEnemies(int NewMax)
    {
        Spawner.maxAtOnce = NewMax;
    }

    /*
     * Enemy pool methods
     * These allow us to modify the kinds of enemies that spawn.
     * Returns a boolean to denote the success of the operation.
     * Adding fails if it's already in the pool (or if there's no such object)
     * Removing fails if it's not in the pool (or if there's no such object)
     */

    public bool AddEnemyToPool(GameObject NewEnemy)
    {
        return false;
    }

    public bool RemoveEnemyFromPool(GameObject Enemy)
    {
        return false;
    }

    /*
     * Heart rate methods.
     */

    public void GetRestingHR()
    {
        RestingHR = Mathf.Min(60.0f, CurrentHR); // Hardcoded until we have sensor data.
    }
}
