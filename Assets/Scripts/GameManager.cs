using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using UnityEngine.SceneManagement;
using com.ootii.Cameras;
using UnityEngine.UI;
using Devdog.InventoryPro.UnityStandardAssets;
using System;
using CustomManager;
using CompassNavigatorPro;

public class GameManager : MonoBehaviour {
    public static GameObject Player;

    public static Zone CurrentZone;

    private List<GameObject> ObjectsToActivate;
    public ExecuteOnComplete exec;

    public GameObject PlayerPrefab;
    public GameObject DialogueMangerPrefab;

    private GameObject DialogueManager;
    private GameObject Navigator;

    public SceneInfo CurrentSceneData;

    public AudioClip ClipPlaying;
    private AudioSource _mAudioSource;

    private Animator _mFaderAnimator;
    private int FadeInHash = Animator.StringToHash("FadeIn");
    private int FadeOutHash = Animator.StringToHash("FadeOut");

    private bool transitioning;

    public static GameManager instance;

    public static bool HandleFadeManually;

    public bool DeveloperMode;

    private void Start()
    {
        CurrentSceneData = GameObject.FindGameObjectWithTag("SceneInfo").GetComponent<SceneInfo>();
        ClipPlaying = CurrentSceneData.SceneEntryAudio;
        _mAudioSource = GetComponent<AudioSource>();
        _mFaderAnimator = GetComponentInChildren<Animator>();
        if (ClipPlaying)
        {
            _mAudioSource.clip = ClipPlaying;
            _mAudioSource.Play();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        instance = this;
        DontDestroyOnLoad(this);
        if (DeveloperMode) StartCoroutine(DevMode());
    }

    private IEnumerator DevMode()
    {
        yield return new WaitForEndOfFrame();
        SetUpPlayer();
    }

    public static void SetPlayerInput(bool active)
    {
        Player.SendMessage("SetInputActive", active);
    }
    /// <summary>
    /// Makes smooth transitions between scenes (sfx, calling transitional animations, etc.)
    /// </summary>
    public void TransitionScene()
    {
        StartCoroutine(TransitionAudio());
        if (HandleFadeManually)
        {
           _mFaderAnimator.SetTrigger(FadeOutHash);
        }
    }

    #region Fading
    public void FadeInScene()
    {
        _mFaderAnimator.SetTrigger(FadeOutHash);
        StartCoroutine(FadeScene());

    }

    private IEnumerator FadeScene()
    {
        yield return new WaitForSeconds(1f);
        _mFaderAnimator.SetTrigger(FadeInHash);
    }
    #endregion

    #region AudioTransitions
    private IEnumerator TransitionAudio()
    {
        while (_mAudioSource.volume > 0f)
        {
            _mAudioSource.volume -= Time.deltaTime;
            yield return null;
        }
    }

    private void PlayNewAudio()
    {
        //if (_mAudioSource.clip) 
        _mAudioSource.clip = CurrentSceneData.SceneEntryAudio;
        ClipPlaying = _mAudioSource.clip;
        _mAudioSource.Play();
    }

    private IEnumerator TransitionNewAudio()
    {
        //yield return new WaitUntil(()=>_mAudioSource.volume <= 0f);
        StopCoroutine(TransitionAudio());
        PlayNewAudio();
        while (_mAudioSource.volume < 1f)
        {
            if (!_mAudioSource.isPlaying) _mAudioSource.Play();
            _mAudioSource.volume += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator DoubleCheckAudio()
    {
        yield return new WaitForSeconds(1f);
        if(!_mAudioSource.isPlaying)
        {
            Debug.Log("Not playing. Starting play.");
            _mAudioSource.Play();
        }
    }
    #endregion

    public void GoToSceneEntryPoint()
    {
        if (CurrentSceneData.EntryPoint)
        {
            Player.transform.position = CurrentSceneData.EntryPoint.transform.position;
        }
    }
    
    public void SetUpPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            Destroy(players[1]);
            Player = GameObject.FindGameObjectWithTag("Player");
            Player.name = "Player";
            var cam = FindObjectOfType<CameraController>();
            cam.Anchor = Player.transform;
        }
        else if (players.Length <= 0)
        {
            GameObject g = Instantiate(PlayerPrefab);
            g.name = "Player";
            Player = g;
            FindObjectOfType<CameraController>().Anchor = g.transform;
            g.transform.position = CurrentSceneData.EntryPoint.transform.position;
        }
        else
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            Player.name = "Player";
            var cam = FindObjectOfType<CameraController>();
            cam.Anchor = Player.transform;
            cam.enabled = true;
        }
        if (CurrentSceneData.MainSceneCamera && Player)
        {
            Player.GetComponent<ThirdPersonUserControl>().SetCamera(CurrentSceneData.MainSceneCamera);
        }
        else if (Player)
            Player.GetComponent<ThirdPersonUserControl>().SetCamera();
        InputManager.SetSelectorUseKey(Player);
        SetPlayerInput(true);
    }

    private void CheckForDialogueManager()
    {
        Debug.Log("checking for dialogue manager");
        var go = GameObject.Find("Dialogue Manager");
        if (!go) go = Instantiate(DialogueMangerPrefab);
        DialogueManager = go;
        InputManager.SetDialogueCancelKeys(DialogueManager);
    }

    private void CheckForNavigator()
    {
        var go = GameObject.Find("CompassNavigatorPro");
        if (!go) return;
        Navigator = go;
        go.GetComponent<CompassPro>().cameraMain = Camera.main;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene");
        CheckForDialogueManager();
        CheckForNavigator();
        CurrentSceneData = GameObject.FindGameObjectWithTag("SceneInfo").GetComponent<SceneInfo>();
        StartCoroutine(TransitionNewAudio());
        ObjectsToActivate = CurrentSceneData.ObjectsToActivate;
        exec.AddObjects(ObjectsToActivate);
        SetUpPlayer();

        if (HandleFadeManually)
        {
            _mFaderAnimator.SetTrigger(FadeInHash);
            HandleFadeManually = false;
        }
        //StartCoroutine(DoubleCheckAudio());
    }
    

}
