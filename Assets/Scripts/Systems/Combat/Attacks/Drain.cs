using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainHitbox : BuffHitbox
{
    private BuffHitbox storedBuffHitbox;

    public void Initialize(GameObject target, bool grounded, bool singlemoment, bool reverse, float value, BuffHitbox bh = null,
        float duration = 0f)
    {
        base.Initialize(target, grounded, singlemoment, reverse, -value, duration);
        storedBuffHitbox = bh;
    }

    protected override void ApplyBuff()
    {
        base.ApplyBuff();
        storedBuffHitbox.ReceiveValue(m_BuffValue);
    }

    protected override void ApplyBuff(GameObject g)
    {
        base.ApplyBuff(g);
        storedBuffHitbox.ReceiveValue(m_BuffValue);
    }
}
