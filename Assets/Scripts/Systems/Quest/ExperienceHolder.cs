using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple experience class to hold player experience
/// </summary>
public class ExperienceHolder{
    public static ExperienceHolder instance;
    public static GameObject Player;
    public float Experience;
    
    public static void AddExperience(float exp)
    {
        instance.Experience = exp;
    }
    

    public ExperienceHolder()
    {
        instance = this;
        Experience = 0f;
    }
}

