using System;
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
    public MusicManager Jukebox;        // Music manager to manipulate based on state

    // What kind of sensor data are we using for the controller?
    public enum SensorMode
    {
        Manual, // Dev input for testing
        Simple, // ECG data only
        Complex // ECG + GSR (NYI)
    };
    public SensorMode CurrentSensorMode = SensorMode.Complex;

    public float TimeInState;   // Time since last state transition.
    public float RestingHR;     // Player's approx resting heartrate.
    public float CurrentHR;     // Player's current heartrate.
    public double RestingGSR;   // Player's approx resting GSR.
    public double CurrentGSR;   // Player's current GSR.

    public float ComplexMetric; // A metric based on HR and GSR

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

        // Read sensor data when not in manual mode
        if (CurrentSensorMode != SensorMode.Manual)
        {
            int read_hr = -1;
            double read_GSR = 0;
            string sep = "\t";

            try
            {
                string input = File.ReadLines(path).Last();
                string[] splitInput = input.Split(sep.ToCharArray());

                read_hr = Int32.Parse(splitInput[0]);
                read_GSR = Double.Parse(splitInput[1]);
            }
            catch (IOException)
            {
                // Data File was open by ShimmerAPI, pass
            }

            Debug.Log("Heart Rate: " + read_hr + " BPM | GSR: " + read_GSR + " kOhms");
            if (read_hr != -1)
            {
                CurrentHR = read_hr;

                // With HR found, get complex metric this frame
                if (CurrentSensorMode == SensorMode.Complex) ComputeComplexMetric();
            }

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
        return Spawner.AddEnemyToSpawns(NewEnemy);
    }

    public bool RemoveEnemyFromPool(GameObject Enemy)
    {
        return Spawner.RemoveEnemyFromSpawns(Enemy);
    }

    /*
     * Heart rate methods.
     */

    public void FindRestingHR()
    {
        RestingHR = Mathf.Min(60.0f, CurrentHR); // Hardcoded until we have sensor data.
    }

    private void ComputeComplexMetric() // Gets a metric based on HR and GSR data for Complex readings.
    {
        ComplexMetric = 0.0f;
    }
}
