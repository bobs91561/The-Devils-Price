using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOverride : TimelineOverrides
{
    /*
     * Activate once to play audio clip attached to this override
     * Activate again to return music to what wass originally being played
     */
    public GameManager GameManager;
    public AudioClip AudioClip;
    private AudioClip _previous_audio_clip;
    private bool _playing_override_music;
    //private AudioSource _audioSource;

    void Start()
    {
        if (!GameManager)
            GameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        _previous_audio_clip = GameManager.ClipPlaying;
        //_audioSource = GameManager.GetComponentInParent<AudioSource>();
    }

    public override void OverrideTimeline()
    {
        if (!AudioClip)
            return;
        StartCoroutine(GameManager.ChangeAudio(_playing_override_music ? _previous_audio_clip : AudioClip));
        _playing_override_music = !_playing_override_music;
    }
}
