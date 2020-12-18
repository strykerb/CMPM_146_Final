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
        Manual,  // Dev input for testing
        Simple,  // ECG data only
        Complex, // ECG + GSR
        ML       // Use machine learning model
    };
    public SensorMode CurrentSensorMode = SensorMode.Manual;

    public float TimeInState;       // Time since last state transition.
    public bool Stressed;           // boolean that defines if the player is actively stresed
    static float TimeFrame = 2f;    // how long each window is for averaging & combining signals
    float TimeInWindow;
    public Queue<bool> StressHistory = new Queue<bool>();   // Past Stress readings. length determined by HistoryDepth
    static int HistoryDepth = 5;    // BuildUp transition looks at past 5 stress readings
    [SerializeField] private float HRThreshold = 8;
    [SerializeField] private double GSRThreshold = 20;

    public float RestingHR;         // Player's approx resting heartrate.
    public float CurrentHR;         // Player's current heartrate.
    public float PreviousHR;        // HR from last time frame
    
    public double RestingGSR;       // Player's approx resting GSR.
    public double CurrentGSR;       // Player's current GSR.
    public double PreviousGSR;      // HR from last time frame

    // States we can use for the FSM
    public BaselineState Baseline = new BaselineState();
    public GameRestState RestState = new GameRestState();
    public GameBuildupState BuildupState = new GameBuildupState();
    public GameClimaxState ClimaxState = new GameClimaxState();
    public GameCooldownState CooldownState = new GameCooldownState();

    // Path to Heart Rate Data File
    string dataPath = Path.Combine($@"{Directory.GetCurrentDirectory()}", @"Assets\HRData\data.txt");

    // Path to ML Prediction File
    string mlPath = Path.Combine($@"{Directory.GetCurrentDirectory()}", @"Assets\HRData\predicted.txt");

    // Start is called before the first frame update
    void Start()
    {

        /*
        * When HR sensor reads -1, we don't have a HR signal for
        * that data packet. We compensate by setting the HR for
        * that interval to be equal to the previous average HR.
        * On our first reading, however, PreviousHR has not been
        * written to, so we default it to 60 bpm, a normal resting
        * heart rate.
        */
        PreviousHR = 60;
        PreviousGSR = 600;
        CurrentHR = 60;
        CurrentGSR = 600;

        // Clear irrelevant data from input file
        File.WriteAllText(dataPath, String.Empty);
        File.WriteAllText(mlPath, String.Empty);

        // Start game in the baseline state to record resting sensor data
        TransitionToState(Baseline);
    }

    // Update is called once per frame
    void Update()
    {
        TimeInState += Time.deltaTime;
        // Read sensor data when in complex mode
        if (CurrentSensorMode == SensorMode.Complex || CurrentSensorMode == SensorMode.ML)
        {
            TimeInWindow += Time.deltaTime;

            // If a full time frame has passed, update values based on sensor data
            if (TimeInWindow > TimeFrame)
                ComputeStressComplex();
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
    public void ModifySpawnFrequency(float NewRate)
    {
        Spawner.SpawnDelay = Mathf.Max(1.0f, NewRate);
    }

    // Get spawn frequency for changes
    public float GetSpawnFrequency()
    {
        return Spawner.SpawnDelay; 
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
        StartCoroutine(FindBaselineSensorValues());
    }

    public IEnumerator FindBaselineSensorValues()
    {
        RestingHR = 0;
        RestingGSR = 0;

        // How many data sections to average to generate resting values
        int BaselineIterations = 10;

        for (int i = 0; i < BaselineIterations; i++)
        {
            yield return new WaitForSeconds(TimeFrame);
            RestingHR += CurrentHR;
            RestingGSR += CurrentGSR;
        }

        Debug.Log("baseline values found");
        RestingHR /= BaselineIterations;
        RestingGSR /= BaselineIterations;

        Stressed = false;
        yield return true;
    }

    
    private void ComputeStressComplex() // Gets a metric based on HR and GSR data for Complex readings.
    {
        float read_hr = -1;
        double read_GSR = 0;
        string sep = "\t";
        string[] input;
        TimeInWindow = 0;

        try
        {
            input = File.ReadAllLines(dataPath);

            // Average all readings from this window
            foreach (string line in input)
            {
                string[] splitInput = line.Split(sep.ToCharArray());
                if (float.Parse(splitInput[0]) == -1)
                {
                    // If we failed to get a value, but we're using ML predictions
                    // then we get the latest prediction and exit early
                    if (CurrentSensorMode == GameController.SensorMode.ML)
                    { 
                        Stressed = GetLatestPrediction();
                        AddToStressHistory(Stressed);
                        return;
                    }
                    // Otherwise, we'll continue with the previous value
                    read_hr += PreviousHR;
                }
                else
                    read_hr += float.Parse(splitInput[0]);
                read_GSR += Double.Parse(splitInput[1]);
            }
            read_hr /= input.Length;
            read_GSR /= input.Length;

            // Erase the contents of the input file
            File.WriteAllText(dataPath, String.Empty);
        }
        catch (IOException)
        {
            // Data File was open by ShimmerAPI, pass
        }

        // Store previous data
        PreviousGSR = CurrentGSR;
        PreviousHR = CurrentHR;
        
        // Update new sensor data
        CurrentHR = read_hr;
        CurrentGSR = read_GSR;

        float HR_diff = CurrentHR - PreviousHR;
        double GSR_diff = CurrentGSR - PreviousGSR;

        Debug.Log("Previously stressed: " + Stressed + ". HR_diff: " + HR_diff + ". GSR_diff: " + GSR_diff + ". HR ratio: " + CurrentHR / RestingHR);

        if (!Stressed)
        {
            if (HR_diff > HRThreshold)
                Stressed = true;
            else if (GSR_diff < -GSRThreshold)
                Stressed = true;
            else if (CurrentHR / RestingHR > 1.2)
                Stressed = true;
        }
        else
        {
            if (HR_diff < -HRThreshold/2)
                Stressed = false;
            else if (GSR_diff > GSRThreshold/2)
                Stressed = false;
            else if (CurrentHR / RestingHR <= 1.05)
                Stressed = false;
        }

        // Add stress detection to history
        AddToStressHistory(Stressed);
        
        Debug.Log("Heart Rate: " + CurrentHR + " BPM | GSR: " + CurrentGSR + " kOhms | Stressed: " + Stressed);
    }

    // Get our latest ML prediction (ML MODE ONLY)
    private bool GetLatestPrediction()
    {
        string prediction = File.ReadLines(mlPath).Last();
        return prediction.Contains('1');
    }

    // Maintain a stress history of length (HistoryDepth)
    private void AddToStressHistory(bool newEntry)
    {
        StressHistory.Enqueue(newEntry);
        if (StressHistory.Count >= HistoryDepth)
        {
            StressHistory.Dequeue();
        }
    }
}
