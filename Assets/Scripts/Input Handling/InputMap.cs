using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="InputMap")]
public class InputMap : ScriptableObject
{
    public string RightTrigger;
    public string LeftTrigger;

    public KeyCode LeftBumper;
    public KeyCode RightBumper;

    [Tooltip("For using objects")] public KeyCode KeyOne;
    [Tooltip("For drawing weapons")] public KeyCode KeyTwo;
    [Tooltip("For dodging")] public KeyCode KeyThree;
    [Tooltip("For jumping")] public KeyCode KeyFour;

    public KeyCode MenuKey;
    public KeyCode MapKey;

    public KeyCode SprintKey;
    public KeyCode JumpKey;
    public KeyCode UseKey;
    public KeyCode CancelKey;
}
