using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class PlaySFXOverride : TimelineOverrides
{
    public List<AudioClip> Sounds;
    private int m_CurIndex;
    private int m_CurAudSource;
    private AudioSource AudioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        m_CurIndex = 0;
        m_CurAudSource = 0;
        AudioSource = GetComponent<AudioSource>();
    }

    public override void OverrideTimeline()
    {
        if (!AudioSource) AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = Sounds[m_CurIndex];
        AudioSource.Play();
        
        m_CurIndex++;
        m_CurAudSource = m_CurAudSource == 0 ? 1 : 0;
    }
}
