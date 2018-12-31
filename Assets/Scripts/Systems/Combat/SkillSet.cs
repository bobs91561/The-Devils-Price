using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devdog.InventoryPro.UnityStandardAssets;

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

    public GameObject meleeObject;
    public GameObject meleeObjectSecondary;
    public GameObject castingObject;

    public GameObject characterCenter;

    public List<GameObject> objectsSheathe;
    public List<GameObject> objectsDrawn;

    public Attack currentAttack;

    public bool isAttacking = false;

    public bool combat;

    private bool _mTargeting;
    private GameObject _mTargetObject;

    private ThirdPersonUserControl m_userControl;

	// Use this for initialization
	void Start () {
        if (attacks == null) attacks = new List<Attack>();
        List<Attack> inputs = attacks;
        for (int i = 0; i < attacks.Count; i++)
        {
            attacks[i] = Instantiate(attacks[i]);
            attacks[i].attacker = gameObject;
        }
        if (BasicAttack) BasicAttack.attacker = gameObject;
        if (ChargedAttack) ChargedAttack.attacker = gameObject;
        SendMessage("SetAttacks");
        SheatheWeapons();
        m_userControl = GetComponent<ThirdPersonUserControl>();
	}

    public void DrawWeapons()
    {
        foreach (GameObject g in objectsSheathe)
            g.SetActive(false);
        foreach (GameObject g in objectsDrawn)
            g.SetActive(true);
    }

    public void SheatheWeapons()
    {
        foreach (GameObject g in objectsSheathe)
            g.SetActive(true);
        foreach (GameObject g in objectsDrawn)
            g.SetActive(false);
    }

    public void Combat()
    {
        combat = !combat;
        GetComponent<Animator>().SetBool("Combat", combat);
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

    public void TargetThis(GameObject g)
    {
        _mTargeting = true;
        _mTargetObject = g;
    }

    private void TargetAttack()
    {
        if (!currentAttack) return;
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
	
}
