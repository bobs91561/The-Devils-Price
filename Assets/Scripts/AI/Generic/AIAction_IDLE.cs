using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AIAction/IDLE")]
public class AIAction_IDLE : AIAction
{
    public override bool ActionFeasible()
    {
        return true;
    }

    public override bool Tick()
    {
        if (isActive)
        {
            decider.tiredness = Mathf.Clamp01(decider.tiredness - Time.deltaTime*.1f);
            if (_animator.GetFloat("Forward") > 0f) _animator.SetFloat("Forward", 0f);
        }
        return true;
    }
}
