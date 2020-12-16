using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBaseState
{
    public GameBaseState NextState;

    public float TargetHR = 0.0f;          // Target heartrate in BPM. NYI.
    public float TargetMul = 0.0f;         // Target heartrate as a multiplier of the resting heartrate. NYI.
    public float MaxTimeInState = 0.0f;    // Maximum time to spend in state. NYI.
    public float MaxHR = 170.0f;           // The absolute maximum heartrate. NYI.
    protected int KillsLeft = -1;          // Remaining enemy kills in order to transition;
    public MusicManager musicManager;

    public abstract void EnterState(GameController controller);
    public abstract void Update(GameController controller);
    public abstract bool CheckGoalReached(GameController controller);
    
}
