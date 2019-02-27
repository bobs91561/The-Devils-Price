using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Systems.Combat;
using PixelCrushers.DialogueSystem;
using CustomManager;

[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SkillSet))]
public class PlayerController : AttackController {

    private Rigidbody _rb;
    private Animator _anim;

    private KeyCode RightBumper;
    private KeyCode LeftBumper;

    private string RightTrigger, LeftTrigger;

    private KeyCode DrawKey;
    private KeyCode DodgeKey;

    private KeyCode[] AttackKeys;

    public List<string> combatKeys;
    public Dictionary<string, Attack> availableAttacks;

    public PlayerMode Mode;

    private int _attackID =  Animator.StringToHash("Attack");
    private int _blockID = Animator.StringToHash("Block");
    private int _chargedID = Animator.StringToHash("ChargedMelee");


    private bool combat;

	// Use this for initialization
	void Start () {
        _anim = GetComponent<Animator>();
        ExperienceHolder.Player = AIActionDecider.Player = AIAttackController.Player = GameManager.Player = gameObject;
        ExperienceHolder.instance = new ExperienceHolder();
        Mode = PlayerMode.NORMAL;
        DontDestroyOnLoad(gameObject);
    }

    void PopulateKeys()
    {
        //Set combatKey strings to the List of key names
        //Attacks for the player should correspond to the following key:
        //RB, light
        //RT, heavy
        //LT, lightL
        //A, B, X, Y, magics

        combatKeys = new List<string>();

        var bumpers = InputManager.GetBumpers();
        LeftBumper = bumpers[0];
        RightBumper = bumpers[1];

        combatKeys.Add(RightBumper.ToString());

        //Get Triggers
        var triggers = InputManager.GetTriggerAxes();
        LeftTrigger = triggers[0];
        RightTrigger = triggers[1];

        combatKeys.Add(RightTrigger.ToString());
        combatKeys.Add(LeftTrigger.ToString());

        //Get Other Keys
        var keys = InputManager.GetOtherKeys();
        AttackKeys = keys;
        foreach (KeyCode k in keys)
        {
            combatKeys.Add(k.ToString());
        }
        DrawKey = keys[1];
        DodgeKey = keys[2];
    }
    
    public void SetAttacks()
    {
        PopulateKeys();
        Initialize();
        availableAttacks = new Dictionary<string, Attack>();
        int i = 0;
        Attack a;
        foreach (string k in combatKeys)
        {
            a = i < _skillSet.attacks.Count ? _skillSet.attacks[i] : null;
            availableAttacks.Add(k, a);
            if (a) _cooldowns.Add(a, a.coolDown);
            i++;
        }

        //Add non-regular attacks
        a = _skillSet.ChargedAttack;
        if (a) _cooldowns.Add(a, a.coolDown);

        a = _skillSet.BlockAttack;
        if(a) _cooldowns.Add(a, a.coolDown);
    }

    public void SetInputActive(bool active)
    {
        this.enabled = active;
    }

    // Update is called once per frame
    void Update () {
        // Check to see if the player is holding down the combo key!
        if (Input.GetKey(LeftBumper) && !_skillSet.CheckBlock())
        {
            Mode = PlayerMode.COMBAT;
        }
        else
        {
            Mode = PlayerMode.NORMAL;
        }
        
        switch (Mode)
        {
            case PlayerMode.NORMAL:
                NormalUpdate();
                break;
            case PlayerMode.COMBAT:
                CombatUpdate();
                break;
            case PlayerMode.SCENE:
                SceneUpdate();
                break;
        }
	}
    #region UpdateMethods
    /// <summary>
    /// Update when player is in regular mode
    /// </summary>
    void NormalUpdate()
    {
        ///     B -> Draw Weapons
        ///     X -> Dodge
        ///     RB -> Light Attack
        ///     RT -> Heavy Attack
        ///     LT -> Light Attack Left
        ///     RT + LT -> Block

        // Check for no input or input not handled by this script (Use, dodge and jump)
        if ((!Input.anyKeyDown && DetermineAxis().Count == 0) || Input.GetKeyDown(AttackKeys[0]) || Input.GetKeyDown(AttackKeys[3]) || Input.GetKeyDown(DodgeKey) || Input.GetKey(InputManager.instance.SprintKey)) return;

        if (Input.GetKeyDown(DrawKey)) _skillSet.Combat();
        else
        {
            var axes = DetermineAxis();
            Attack a = null;
            if (axes.Count == 2) AxisAction(_blockID);
            else if (_skillSet.CheckBlock()) _skillSet.EndAttack();
            //Check for no RB and zero axes, return if true
            else if (axes.Count <= 0 && !Input.GetKeyDown(RightBumper)) return;
            else if (axes.Count <= 0)
            {
                try
                {
                    a = availableAttacks[RightBumper.ToString()];
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log("Couldn't find " + RightBumper.ToString());
                }

            }
            else
            {
                try
                {
                    a = availableAttacks[axes[0]];
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log("Couldn't find " + axes[0].ToString());
                }
            }
            if (_skillSet.CheckBlock() || _skillSet.CheckAttack()) return;
            if (a && !_skillSet.CheckAttack() && a.CheckRequirements(_cooldowns[a], SoulStrength))
            {
                _skillSet.StartAttack(a);
                SendAttack(a);
                return;
            }
        }
    }

    /// <summary>
    /// Update when player is in combat mode. Checks keys with alternate purpose during combat
    /// </summary>
    void CombatUpdate()
    {
        ///     LB + RB -> Charged Attack
        ///     LB + (A, B, X, Y) -> Magic Attack
        if (_skillSet.CheckAttack()) return;

        if (Input.GetKeyDown(RightBumper))
        {
            _skillSet.StartAttack(_skillSet.ChargedAttack);
            SendAttack(_skillSet.ChargedAttack);
        }
        else
        {
            foreach(KeyCode k in AttackKeys)
            {
                try
                {
                    var a = availableAttacks[k.ToString()];
                    if (Input.GetKeyDown(k) && a && a.CheckRequirements(_cooldowns[a], SoulStrength))
                    {
                        _skillSet.StartAttack(a);
                        SendAttack(a);
                        return;
                    }
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log("Couldn't find " + k.ToString());
                }
            }
        }
    }

    /// <summary>
    /// Update for when player is in a cutscene. Controls are ignored except for dialogue.
    /// </summary>
    void SceneUpdate()
    {

    }

    #endregion


    private List<string> DetermineAxis()
    {
        var axes = new List<string>();

        if (Input.GetAxis(RightTrigger) > 0f) axes.Add(RightTrigger);
        if (Input.GetAxis(LeftTrigger) > 0f) axes.Add(LeftTrigger);
        return axes;
    }

    private void AxisAction(int id)
    {
        if (_skillSet.CheckAttack() || _skillSet.CheckBlock()) return;

        if (id == _attackID)
        {
            _skillSet.StartAttack(_skillSet.BasicAttack);
            SendAttack(_skillSet.BasicAttack);
        }
        else if (id == _chargedID)
        {
            _skillSet.StartAttack(_skillSet.ChargedAttack);
            SendAttack(_skillSet.ChargedAttack);
        }
        else if (id == _blockID)
        {
            _skillSet.StartAttack(_skillSet.BlockAttack);
            SendAttack(_skillSet.BlockAttack);
        }

    }

    public void OnDeath()
    {
        enabled = false;
        EventManager.Death();
    }

    public void OnRespawn()
    {
        this.enabled = true;
    }
    
}

public enum PlayerMode
{
    COMBAT, NORMAL, SCENE, DIALOGUE
}
