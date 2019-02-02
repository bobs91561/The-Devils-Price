using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingObject : MonoBehaviour
{
    public float CastDamageMultiplier = 1f;
    
    public float GetDamage()
    {
        return CastDamageMultiplier;
    }
}
