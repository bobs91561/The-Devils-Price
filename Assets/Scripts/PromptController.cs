using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomManager;
using TMPro;

public class PromptController : MonoBehaviour
{
    public TextMeshProUGUI Display;
    public float PromptDisplayTime; //only used for time-based prompts
    public float TimeBetweenPrompts;
    public List<string> PromptsText;
    public List<string> PromptsInput; //if not input dependent, set to "None"
    private Dictionary<string, KeyCode> _keycodeDict;
    private Dictionary<string, string> _triggerDict;
    private int i;
    private float _time_of_next_display;
    
    void Start()
    {
        _keycodeDict = new Dictionary<string, KeyCode> {
            {"LeftBumper", InputManager.instance.LeftBumper},
            {"RightBumper", InputManager.instance.RightBumper},
            {"KeyOne", InputManager.instance.KeyOne},
            {"KeyTwo", InputManager.instance.KeyTwo},
            {"KeyThree", InputManager.instance.KeyThree},
            {"KeyFour", InputManager.instance.KeyFour},
            {"MenuKey", InputManager.instance.MenuKey},
            {"MapKey", InputManager.instance.MapKey},
            {"SprintKey", InputManager.instance.SprintKey},
            {"JumpKey", InputManager.instance.JumpKey},
            {"UseKey", InputManager.instance.UseKey},
            {"CancelKey", InputManager.instance.CancelKey},
            {"DodgeKey", InputManager.instance.DodgeKey},
            {"None", KeyCode.None}
        };

        _triggerDict = new Dictionary<string, string>
        {
            {"LeftTrigger", InputManager.instance.LeftTrigger},
            {"RightTrigger", InputManager.instance.RightTrigger}
        };

        i = 0;
        _time_of_next_display = Time.time;
    }

    private void Update()
    {
        if (i < PromptsText.Count && Time.time >= _time_of_next_display)
        {
            Display.text = PromptsText[i];
            Display.enabled = true;
            Display.GetComponentInParent<Image>().enabled = true;
            //No input expected
            if (PromptsInput[i].Equals("None"))
            {
                _time_of_next_display = Time.time + PromptDisplayTime + TimeBetweenPrompts; //Remove TimeBetweenPrompts if needed
                Invoke("DisplayTimeBasedPrompt", PromptDisplayTime);
            }
            //Axis input expected
            else if (PromptsInput[i].Equals("LeftTrigger") || PromptsInput[i].Equals("RightTrigger"))
            {
                if (Input.GetAxis(_triggerDict[PromptsInput[i]]) > 0)
                {
                    i++;
                    _time_of_next_display = Time.time + TimeBetweenPrompts;
                    Display.enabled = false;
                    Display.GetComponentInParent<Image>().enabled = false;
                }
            }
            //Button input expected
            else
            {
                if (Input.GetKeyDown(_keycodeDict[PromptsInput[i]]))
                {
                    i++;
                    _time_of_next_display = Time.time + TimeBetweenPrompts;
                    Display.enabled = false;
                    Display.GetComponentInParent<Image>().enabled = false;
                }
            }
        }
    }

    private void DisplayTimeBasedPrompt()
    {
        i++;
        Display.enabled = false;
        Display.GetComponentInParent<Image>().enabled = false;
    }

}
