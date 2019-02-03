using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using Devdog.InventoryPro.UnityStandardAssets;

public class HealthManager : MonoBehaviour {
    public bool isAlive;
    public bool Respawns = false;
    public bool isPlayer = false;
    public float maxHealth;

    private Animator _mAnimator;
    private int m_DeathID = Animator.StringToHash("Death");
    private RespawnsOnDeath RoD;
    private SkillSet _skillSet;
    private FriendlyConditional _friendly;
    private AIActionDecider _decider;
    private ThirdPersonCharacter _mThirdPerson;
    public GameObject hitBy;
    
    public HealthBar healthBar;

    public GameObject HealthBarPrefab;

    public float Health { get; private set; }
    // Use this for initialization
    void Start () {        
        _mAnimator = GetComponent<Animator>();
        if (!_mAnimator) _mAnimator = GetComponentInChildren<Animator>();

        _mThirdPerson = GetComponent<ThirdPersonCharacter>();
        if (!_mThirdPerson) _mThirdPerson = GetComponentInChildren<ThirdPersonCharacter>();

        _skillSet = GetComponent<SkillSet>();
        _friendly = GetComponent<FriendlyConditional>();
        _decider = GetComponent<AIActionDecider>();

        if (Respawns) RoD = GetComponent<RespawnsOnDeath>();
        if (!healthBar && HealthBarPrefab) SpawnHealthBar();
        if (isPlayer) EventManager.RespawnAction += Respawn;
	}

    private void SpawnHealthBar()
    {
        healthBar = Instantiate(HealthBarPrefab).GetComponent<HealthBar>();
        healthBar.Initialize(gameObject);
    }

    private void OnEnable()
    {
        ResetHealth();
    }

    // Update is called once per frame
    void LateUpdate () {
        if (Health <= 0f) isAlive = false;

        if (!isAlive)
        {
            enabled = false;
            Death();
        }
       
	}

    public void ModifyHealth(float change)
    {
        Health += change;
        if (_friendly) SendMessage("OnAggression");
        if (_decider && !_decider.combat) SendMessage("EnterCombat");
        if (healthBar) healthBar.UpdateHealth();
    }

    public void ResetHealth()
    {
        Health = maxHealth;
        isAlive = true;
        if (healthBar) healthBar.UpdateHealth();
        else if (HealthBarPrefab)
            SpawnHealthBar();
    }

    public void EnterCombat()
    {
        if (!isPlayer && healthBar) healthBar.Activate();
    }

    public void ExitCombat()
    {
        if (!isPlayer && healthBar) healthBar.Deactivate();
    }

    private void Life()
    {
        ResetHealth();
        _mAnimator.SetBool(m_DeathID, !isAlive);
        this.enabled = true;
    }

    public void Death()
    {
        if (_skillSet.CheckAttack()) _skillSet.currentAttack.Interrupt();
        _mAnimator.SetBool("Combat", false);
        _mAnimator.SetBool(m_DeathID, !isAlive);

        if (gameObject.GetComponentInChildren<IncrementOnDestroy>())
            gameObject.GetComponentInChildren<IncrementOnDestroy>().enabled = false;

        if (!isPlayer && healthBar) healthBar.Deactivate();

        SendMessage("OnDeathParam", hitBy);
        BroadcastMessage("OnDeath");
    }

    public void Respawn()
    {
        Life();
        //SendMessage("SheatheWeapons");
        SendMessage("OnRespawn");
    }

    private void OnDestroy()
    {
        if (healthBar) Destroy(healthBar);
    }

    public void SetHealthFromSave(float health)
    {
        Health = health;
        if (healthBar) healthBar.UpdateHealth();
    }

    #region Levelling Settings
    public void ChangeMaxHealth(float newMax)
    {
        maxHealth += newMax;
    }
    #endregion
}
