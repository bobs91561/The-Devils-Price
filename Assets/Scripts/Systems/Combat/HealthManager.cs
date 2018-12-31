using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using Devdog.InventoryPro.UnityStandardAssets;

[RequireComponent(typeof(Animator))]
public class HealthManager : MonoBehaviour {
    public bool isAlive;
    public bool Respawns = false;
    public bool isPlayer = false;
    public float maxHealth;

    private Animator _mAnimator;
    private int animID = Animator.StringToHash("Death");
    private RespawnsOnDeath RoD;
    private SkillSet _skillSet;
    private FriendlyConditional _friendly;
    private AIActionDecider _decider;
    public GameObject hitBy;

    public Slider healthSlider;
    public FloatingHealthBar healthBar;
    public bool hasFloatingBar;


    public GameObject HealthBarPrefab;

    public float Health { get; private set; }
    // Use this for initialization
    void Start () {        
        _mAnimator = GetComponent<Animator>();
        _skillSet = GetComponent<SkillSet>();
        _friendly = GetComponent<FriendlyConditional>();
        _decider = GetComponent<AIActionDecider>();
        if (Respawns) RoD = GetComponent<RespawnsOnDeath>();
        if (hasFloatingBar)
        {
            healthBar = Instantiate(HealthBarPrefab).GetComponent<FloatingHealthBar>();
            healthBar.Initialize(gameObject);
        }
        
	}

    private void OnEnable()
    {
        Health = maxHealth;
        isAlive = true;
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = Health;
        }
        if (healthBar)
            healthBar.UpdateHealth();
    }

    private void Life()
    {
        _mAnimator.SetBool(animID, !isAlive);
        enabled = true;
    }

    // Update is called once per frame
    void Update () {
        if (Health <= 0f) isAlive = false;

        if (!isAlive)
        {
            Death();

            enabled = false;
        }
       
	}

    public void ModifyHealth(float change)
    {
        Health += change;
        if (_friendly) SendMessage("OnAggression");
        if (_decider && !_decider.combat) SendMessage("EnterCombat");
        if (healthSlider) healthSlider.value = Health;
        if (healthBar) healthBar.UpdateHealth();
    }

    public void EnterCombat()
    {
        if (healthBar) healthBar.Activate();
    }

    public void Death()
    {
        if (_skillSet.CheckAttack()) _skillSet.currentAttack.Interrupt();
        _mAnimator.SetBool("Combat", false);
        _mAnimator.SetBool(animID, !isAlive);
        if (gameObject.GetComponentInChildren<IncrementOnDestroy>())
            gameObject.GetComponentInChildren<IncrementOnDestroy>().enabled = false;
        if (healthBar) healthBar.Deactivate();
        SendMessage("OnDeathParam", hitBy);
        SendMessage("OnDeath");
    }
    /// <summary>
    /// Takes a normalized point in local space and applies the animation
    /// </summary>
    /// <param name="point"></param>
    private void ReactHere(Vector3 point)
    {
        if (GetComponent<ThirdPersonCharacter>().m_IsReacting || GetComponent<PlayerController>()) return;
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("HitDirX", point.x);
        anim.SetFloat("HitDirZ", point.z);
        anim.SetTrigger("React");
    }

    public void ReactAt(RaycastHit hit)
    {
        Vector3 point = hit.point;
        point = transform.InverseTransformPoint(point);
        point.Normalize();
        ReactHere(point);
    }

    public void ReactAt(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];
        Vector3 point = contactPoint.point;
        point = transform.InverseTransformPoint(point);
        point.Normalize();
        ReactHere(point);  
    }

    public void ReactAt(Vector3 point)
    {
        point = transform.InverseTransformPoint(point);
        point.Normalize();
        ReactHere(point);
    }
    
}
