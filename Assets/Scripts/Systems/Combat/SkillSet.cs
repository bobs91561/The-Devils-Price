using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devdog.InventoryPro.UnityStandardAssets;
using Systems.Combat;

/// <summary>
/// This component is  attached to all GameObjects using the Combat System.
/// This component contains all the Attacks available to the attached character.
/// </summary>
public class SkillSet : MonoBehaviour {

    public List<Attack> attacks;
    public Attack BasicAttack;
    public Attack ChargedAttack;

    public Vector3 meleeCenter;
    public Vector3 castingPoint;

    [HideInInspector] public GameObject meleeObject;
    [HideInInspector] public GameObject HitboxToGenerate;
    [HideInInspector] public GameObject meleeObjectSecondary;
    [HideInInspector] public GameObject HitboxToGenerateSecondary;

    [HideInInspector] public float WeaponDamage1, WeaponDamage2;
    [HideInInspector] public float CastingDamage;

    public GameObject castingObject;

    public GameObject characterCenter;

    public List<GameObject> objectsSheathe;
    private List<GameObject> objectsDrawn;

    private bool _drawn = true;

    public Attack currentAttack;

    public bool isAttacking = false;

    public bool combat;

    [SerializeField]private bool _mTargeting;
    [SerializeField]private GameObject _mTargetObject;

    private ThirdPersonUserControl m_userControl;
    private Animator m_Animator;

	// Use this for initialization
	void Start () {
        FindWeapons();
        FindCastingObject();
        if (attacks == null) attacks = new List<Attack>();
        for (int i = 0; i < attacks.Count; i++)
        {
            attacks[i] = Instantiate(attacks[i]);
            attacks[i].Initialize(gameObject);
        }
        if (BasicAttack) BasicAttack.Initialize(gameObject);
        if (ChargedAttack) ChargedAttack.Initialize(gameObject);
        SendMessage("SetAttacks");
        SheatheWeapons();
        m_userControl = GetComponent<ThirdPersonUserControl>();
        m_Animator = GetComponent<Animator>();
        if (!m_Animator) m_Animator = GetComponentInChildren<Animator>();
	}

    private void FindWeapons()
    {
        objectsDrawn = new List<GameObject>();
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        int len = weapons.Length;
        if(len > 0)
        {
            if (len > 1)
            {
                meleeObjectSecondary = weapons[0].gameObject;
                HitboxToGenerateSecondary = weapons[0].GetHitbox();
                WeaponDamage2 = weapons[0].GetDamage();
                objectsDrawn.Add(weapons[0].gameObject);

                meleeObject = weapons[1].gameObject;
                HitboxToGenerate = weapons[1].GetHitbox();
                WeaponDamage1 = weapons[1].GetDamage();
                objectsDrawn.Add(weapons[1].gameObject);
                return;
            }

            meleeObject = weapons[0].gameObject;
            HitboxToGenerate = weapons[0].GetHitbox();
            WeaponDamage1 = weapons[0].GetDamage();
            objectsDrawn.Add(weapons[0].gameObject);
            
        }
    }

    private void FindCastingObject()
    {
        if (!castingObject)
            castingObject = GetComponentInChildren<CastingObject>().gameObject;
        if (!castingObject)
        {
            Debug.Log("No casting Object found on: " + gameObject.name);
            return;
        }
        CastingDamage = castingObject.GetComponent<CastingObject>().GetDamage();
    }

    public void DrawWeapons()
    {
        if (_drawn) return;
        foreach (GameObject g in objectsSheathe)
            g.SetActive(false);
        foreach (GameObject g in objectsDrawn)
            g.SetActive(true);
        _drawn = true;
    }

    public void SheatheWeapons()
    {
        if (!_drawn) return;
        foreach (GameObject g in objectsSheathe)
            g.SetActive(true);
        foreach (GameObject g in objectsDrawn)
            g.SetActive(false);

        _drawn = false;
    }

    public void Combat()
    {
        combat = !combat;
        m_Animator.SetBool("Combat", combat);
    }

    public void StartAttack(Attack a = null)
    {
        if (!combat && a is Melee)
        {
            Combat();
            DrawWeapons();
        }
        currentAttack = a;
        if (m_userControl)
        {
            m_userControl.MoveTowardCameraForward(true);
            StartCoroutine("WaitForTurnComplete");
        }
        else a.TriggerAnimation();
        isAttacking = true;
    }

    public IEnumerator WaitForTurnComplete()
    {
        yield return new WaitForSeconds(0.2f);
        currentAttack.TriggerAnimation();
    }

    public void TargetThis(Vector3 position)
    {
        _mTargeting = true;
    }

    public void TargetThis(GameObject g)
    {
        _mTargeting = true;
        _mTargetObject = g;
    }

    private void TargetAttack()
    {
        if (!currentAttack) return;
        if (!_mTargetObject)
        {
            _mTargeting = false;
            return;
        }
        currentAttack.Target(_mTargetObject);
    }

    public void UseAttackWithAnim()
    {
        currentAttack.UseAttack();
        if(_mTargeting) TargetAttack();
    }

    public void EndAttack()
    {
        if (m_userControl) m_userControl.MoveTowardCameraForward(false);
        currentAttack.End();
        isAttacking = false;
    }

    public bool CheckAttack()
    {
        return isAttacking;
    }

    public void OnRespawn()
    {
        SheatheWeapons();
        combat = false;
        m_Animator.SetBool("Combat", combat);
    }

    #region Levelling Settings
    public void ModifyDamage(float multiplier)
    {
        WeaponDamage1 += WeaponDamage1 * multiplier;
        WeaponDamage2 += WeaponDamage2 * multiplier;

        CastingDamage += multiplier;
        if (CastingDamage <= 0f) CastingDamage = 0.5f;
    }
    #endregion
}
