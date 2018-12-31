using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Systems.Combat;
using PixelCrushers.DialogueSystem;
[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SkillSet))]
public class PlayerController : AttackController {

    private Rigidbody _rb;
    private Animator _anim;
    //private SkillSet _skillSet;

    public KeyCode DrawKey;

    public List<string> combatKeys;
    public Dictionary<string, Attack> availableAttacks;
    public Dictionary<string, float> cooldowns;

    public PlayerMode Mode;

    private int _attackID =  Animator.StringToHash("Attack");
    private int _dodgeID = Animator.StringToHash("Dodge");

    private KeyCode RightBumper;
    private KeyCode LeftBumper;

    private bool combat;

	// Use this for initialization
	void Start () {
        _anim = GetComponent<Animator>();
        ExperienceHolder.Player = AIActionDecider.Player = AIAttackController.Player = GameManager.Player = gameObject;
        ExperienceHolder.instance = new ExperienceHolder();
        Mode = PlayerMode.NORMAL;
        RightBumper = KeyCode.JoystickButton5;
        LeftBumper = KeyCode.JoystickButton4;
	}

    // Update is called once per frame
    void FixedUpdate () {
        
        if (Input.GetAxis("Attack")>0.1f) AxisAction(_attackID);
        if (Input.GetKeyDown(DrawKey))
        {
            _skillSet.Combat();
        }
        else if (Input.anyKeyDown) CheckKey();
        /*
        switch (Mode)
        {
            case PlayerMode.NORMAL:
                NormalUpdate();
                break;
            case PlayerMode.COMBAT:
                CombatUpdate();
                break;
        }*/
	}
    #region UpdateMethods
    /// <summary>
    /// Update when player is in regular mode
    /// </summary>
    void NormalUpdate()
    {

    }

    /// <summary>
    /// Update when player is in combat mode. Checks keys with alternate purpose during combat
    /// </summary>
    void CombatUpdate()
    {

    }

    /// <summary>
    /// Update for when player is in a cutscene. Controls are ignored except for dialogue.
    /// </summary>
    void SceneUpdate()
    {

    }

    #endregion

    public void SetAttacks()
    {
        Initialize();
        availableAttacks = new Dictionary<string, Attack>();
        int i = 0;
        Attack a;
        foreach (string k in combatKeys)
        {
            a = i < _skillSet.attacks.Count ? _skillSet.attacks[i] : null;
            availableAttacks.Add(k, a);
            if(a) _cooldowns.Add(a, a.coolDown);
            i++;
        }

        //Add non-regular attacks
        a = _skillSet.ChargedAttack;
        if(a) _cooldowns.Add(a, a.coolDown);

    }

    private void AxisAction(int id)
    {
        
        if(id == _attackID && !_skillSet.CheckAttack())
        {
            _skillSet.StartAttack(_skillSet.BasicAttack);
            SendAttack(_skillSet.BasicAttack);
        }
        /*if(id == _dodgeID)
        {
            _anim.SetTrigger(id);
        }*/

    }

    /// <summary>
    /// Checks all possible key presses
    /// </summary>
    private void CheckKey()
    {
        if (CheckDoubleKey()) return;

        if (!_skillSet.CheckAttack())
            foreach (string s in availableAttacks.Keys)
            {
                Attack a = availableAttacks[s];
                if (a && Input.GetAxis(s)>0f && a.CheckRequirements(_cooldowns[a],SoulStrength))
                {
                    _skillSet.StartAttack(a);
                    SendAttack(a);
                    return;
                }
            }
    }

    /// <summary>
    /// Checks to see if special combo keys are being pressed simultaeneously
    /// </summary>
    private bool CheckDoubleKey()
    {
        //Check if both right and left bumper are down
        //If yes, try attack
        //If not, return false
        if(Input.GetKeyDown(LeftBumper) && Input.GetKeyDown(RightBumper))
        {
            Debug.Log("Charged Attack!");
            _skillSet.StartAttack(_skillSet.ChargedAttack);
            SendAttack(_skillSet.ChargedAttack);
            return true;
        }
        return false;
    }

    public void OnDeath()
    {
        EventManager.Death();
        enabled = false;
    }
    
}

public enum PlayerMode
{
    COMBAT, NORMAL, SCENE, DIALOGUE
}
