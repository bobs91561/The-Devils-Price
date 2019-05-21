using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AIAction/ChangeMode")]
public abstract class AIAction_CHANGEMODE : AIAction
{
    public float Threshold;
    public GameObject VisualEffect;
    public string ModeChangeAnimationTrigger = "";

    private bool waiting = false;
    private bool modeChanged = false;
    private Animator m_Animator;
    new AIActionModeDecider decider;

    public override bool ActionFeasible()
    {
        return CheckCondition();
    }

    public override bool Tick()
    {
        //Is the action waiting on an animation or vfx to complete?
        if (waiting) return true;

        //Has the action been called yet?
        if (!isActive)
        {
            PerformVisualEffect();
            PerformAnimation();
            return true;
        }
        else if (!modeChanged)
        {
            decider.ChangeMode();
            modeChanged = true;
            return true;
        }
        else
            return false;
    }

    protected abstract bool CheckCondition();

    private void PerformVisualEffect()
    {
        if (!VisualEffect) return;
        Instantiate(VisualEffect);
    }

    private void PerformAnimation()
    {
        if (ModeChangeAnimationTrigger == "") return;
        _animator.SetTrigger(ModeChangeAnimationTrigger);
        waiting = true;
    }

    public override void SetActive()
    {
        base.SetActive();
        waiting = false;
        modeChanged = false;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        waiting = false;
        modeChanged = false;
    }

    public void DoneWaiting()
    {
        waiting = false;
    }
}
