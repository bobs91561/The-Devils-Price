using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DelayActive : MonoBehaviour
{

    public GameObject WaitThing;
    public float Duration;

    private IEnumerable OnEnable()
    {
        yield return new WaitForSeconds(Duration);
        WaitThing.SetActive(true);
    }
}
