using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpecialDeathArea : MonoBehaviour
{
    public bool BasedOnEnable;
    public bool BasedOnEntry;
    public bool PlayCutscene;
    public bool EnableDialogueTrigger;

    public PlayableDirector playable;
    public DialogueSystemTrigger trigger;

    private bool removed = false;
    private bool added = false;

    private void Start()
    {
        BasedOnEntry = !BasedOnEnable;
    }

    #region Event Addition and Removal
    private void OnEnable()
    {
        if (BasedOnEnable) AddToEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!BasedOnEntry || !other.CompareTag("Player")) return;
        AddToEvent();
    }

    private void AddToEvent()
    {
        if (added) return;
        if (PlayCutscene)
            EventManager.SpecialDeath += Cutscene;
        if (EnableDialogueTrigger)
            EventManager.SpecialDeath += EnableTrigger;
        SpawnManager.instance.SpecialRespawnPlayer(true);
        added = true;
    }

    private void RemoveFromEvent()
    {
        if (removed) return;
        if (PlayCutscene)
            EventManager.SpecialDeath -= Cutscene;
        if (EnableDialogueTrigger)
            EventManager.SpecialDeath -= EnableTrigger;
        SpawnManager.instance.SpecialRespawnPlayer(false);
        removed = true;
    }

    private void OnDisable()
    {
        RemoveFromEvent();
    }

    private void OnDestroy()
    {
        RemoveFromEvent();
    }
    #endregion

    #region Event Methods
    private void Cutscene()
    {
        playable.Play();
    }

    private void EnableTrigger()
    {
        trigger.enabled = true;
    }
    #endregion
}
