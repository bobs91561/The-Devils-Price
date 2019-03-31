using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankUpController : MonoBehaviour
{
    public TextMeshProUGUI Display;
    public float PromptDisplayTime;
    public float TimeBetweenPrompts;
    public List<string> PromptsText;
    private Image _image;
    private Reputable _reputable;
    private float _time_until_next_display;
    private int i;

    /*Create a level up display that shows Rank Increased, current rank, and bonuses to attributes (ie health).
There exists an Event called RankUp that tracks when the player levels up. You only need to write a responding script 
that displays to the player they have levelled up. Look at RankResponse for how events work. If you have questions about them, 
please ask! You will attach the script you write to the CustomDialogueManager prefab. */
    private void Start()
    {
        _reputable = GameManager.Player.GetComponent<Reputable>();
        _image = Display.GetComponentInParent<Image>();
        _image.enabled = false;
        Display.enabled = false;
        _time_until_next_display = 0;

        EventManager.RankUp += DisplayRankUp;
    }

    private void OnDestroy()
    {
        EventManager.RankUp -= DisplayRankUp;
    }

    public void DisplayRankUp()
    {
        _time_until_next_display = 0;
        i = 0;
        foreach (var prompt in PromptsText)
        {
            Invoke("DisplayPrompt", _time_until_next_display);
            _time_until_next_display += PromptDisplayTime + TimeBetweenPrompts;
        }
    }

    private void DisplayPrompt()
    {
        if (!_reputable) _reputable = GameManager.Player.GetComponent<Reputable>();
        Display.text = PromptsText[i];

        if (i == 0)
        {
            int currentRank = _reputable.CurrentRank;
            Display.text = Display.text + " (" + currentRank + " -> " + (currentRank + 1) + ")";
        }

        i++;
        if (i >= PromptsText.Count) i = 0;
        Display.enabled = true;
        _image.enabled = true;
        Invoke("DisablePrompt", PromptDisplayTime);
    }

    private void DisablePrompt()
    {
        Display.enabled = false;
        _image.enabled = false;
    }
}
