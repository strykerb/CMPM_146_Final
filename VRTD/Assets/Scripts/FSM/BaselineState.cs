using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaselineState : GameBaseState
{
    float UIFadeOutTime = 3.0f;
    float FadeTimer = 0;
    CanvasGroup UI;
    public override void EnterState(GameController controller)
    {
        MaxTimeInState = 20.0f;
        UI = Object.FindObjectOfType<Canvas>().GetComponentInChildren<CanvasGroup>();
        controller.FindRestingHR();
    }

    public override void Update(GameController controller)
    {
        if (CheckGoalReached(controller))
        {
            controller.StressHistory.Clear();
            controller.TransitionToState(controller.RestState);
        }
        
        // fade out UI when we are about to transition states
        if (controller.TimeInState > MaxTimeInState - UIFadeOutTime)
            FadeOutUI();
    }

    public override bool CheckGoalReached(GameController controller)
    {
        
        if (controller.TimeInState > MaxTimeInState)
            return true;
        return false;
    }

    void FadeOutUI()
    {
        FadeTimer += Time.deltaTime;
        if (UI.alpha > 0)
        {
            UI.alpha = Mathf.Lerp(1, 0, FadeTimer / UIFadeOutTime);
        }
    }
}
