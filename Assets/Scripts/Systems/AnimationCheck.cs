using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCheck : MonoBehaviour
{
    private ThirdPersonCharacter _mThirdPerson;

    // Start is called before the first frame update
    void Start()
    {
        _mThirdPerson = GetComponent<ThirdPersonCharacter>();
    }

    /// <summary>
    /// Ensure the reaction has ended after the animation.
    /// </summary>
    /// <param name="seconds"></param>
    public void CheckReact(float seconds)
    {
        StartCoroutine(SecondsAfter(seconds, true));
    }

    IEnumerator SecondsAfter(float seconds, bool react)
    {
        yield return new WaitForSeconds(seconds);
        if(react) _mThirdPerson.m_IsReacting = false;
    }

    /// <summary>
    /// Ensure the character is frozen during the time period.
    /// </summary>
    /// <param name="seconds"></param>
    public void CheckFreeze(float seconds)
    {

    }
}
