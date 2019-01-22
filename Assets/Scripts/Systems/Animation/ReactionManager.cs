using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages when the character performs reactions to hits. Should be attached to the same object that has the animator.
/// </summary>
public class ReactionManager : MonoBehaviour
{
    private ThirdPersonCharacter m_ThirdPerson;
    private ThirdPersonUserControl m_UserControl;
    private PlayerController m_PlayerControl;

    private Animator m_Animator;
    private int m_ForwardHash = Animator.StringToHash("Forward");
    private int m_TurnAmount = Animator.StringToHash("Turn");
    private int m_DodgeKey = Animator.StringToHash("Dodge");
    private int m_JumpKey = Animator.StringToHash("JumpTrigger");
    private int m_HitDirX = Animator.StringToHash("HitDirX");
    private int m_HitDirZ = Animator.StringToHash("HitDirZ");

    private int m_MajorReact = Animator.StringToHash("MajorReact");
    private int m_React = Animator.StringToHash("React");

    private Vector3 m_HitDirection;
    private float m_HitForce;

    private float m_MinForceToConsider;

    private float m_TimeOfLastReact;
    private float m_MinimumTimeBetweenReacts;

    private bool m_ReactionStarted;

    private int m_TriggerToSet;

    // Start is called before the first frame update
    void Start()
    {
        m_ThirdPerson = GetComponent<ThirdPersonCharacter>();
        if (!m_ThirdPerson) m_ThirdPerson = GetComponentInParent<ThirdPersonCharacter>();

        m_UserControl = GetComponent<ThirdPersonUserControl>();
        if (!m_UserControl) m_UserControl = GetComponentInParent<ThirdPersonUserControl>();

        m_PlayerControl = GetComponent<PlayerController>();
        if (!m_PlayerControl) m_PlayerControl = GetComponentInParent<PlayerController>();

        m_Animator = GetComponent<Animator>();
        if (!m_Animator) m_Animator = GetComponentInChildren<Animator>();

        m_ReactionStarted = false;
        m_TimeOfLastReact = Time.time;
    }

    #region Input and Animation Control
    public void React()
    {
        m_ReactionStarted = true;

        //If user control, SetActive to false
        if (m_UserControl) m_UserControl.SetInputActive(false);
        if (m_PlayerControl) m_PlayerControl.SetInputActive(false);

        //If no user control, tell the AI to halt until reaction completes


        //Interrupt any attacks


        //Update the Animator
        UpdateAnimator();
    }

    public void FinishReact()
    {
        //If user control, SetActive to true
        if (m_UserControl) m_UserControl.SetInputActive(true);
        if (m_PlayerControl) m_PlayerControl.SetInputActive(true);

        //If no user control, allow the AI to continue

        m_TimeOfLastReact = Time.time;
        m_ReactionStarted = false;
    }

    private void UpdateAnimator()
    {
        //Set all movement values to false
        m_Animator.SetFloat(m_ForwardHash, 0f);
        m_Animator.SetFloat(m_TurnAmount, 0f);

        //Reset all triggers
        m_Animator.ResetTrigger(m_DodgeKey);
        m_Animator.ResetTrigger(m_JumpKey);

        //Set reaction values
        m_Animator.SetFloat(m_HitDirX, m_HitDirection.x);
        m_Animator.SetFloat(m_HitDirZ, m_HitDirection.z);
        m_Animator.SetTrigger(m_TriggerToSet);

    }
    #endregion
    /// <summary>
    /// Regular reaction determined by the point of contact and the damage done
    /// </summary>
    /// <param name="hitPoint"></param>
    /// <param name="hitDamage"></param>
    public void ReactHere(Vector3 hitPoint, float hitDamage = 0f)
    {
        //Check that the reaction is acceptable to pass
        if (!AcceptReaction(hitDamage)) return;

        //The reaction was accepted, proceed to animate
        m_HitDirection = hitPoint;
        m_TriggerToSet = m_React;
        React();
    }

    /// <summary>
    /// Call for special reaction, that always occurs as a part of an attack hit
    /// </summary>
    public void SpecialReact()
    {
        if (!AcceptReaction(m_MinForceToConsider*2f)) return;
        m_TriggerToSet = m_MajorReact;
        React();
    }

    #region Determine Hit Location
    public void ReactAt(RaycastHit hit)
    {
        Vector3 point = hit.point;
        point = transform.InverseTransformPoint(point);
        point.Normalize();
        ReactHere(point);
    }

    public void ReactAt(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);
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
    #endregion


    /// <summary>
    /// Returns whether the reaction should be allowed
    /// </summary>
    /// <returns></returns>
    private bool AcceptReaction(float force)
    {
        var curr = Time.time;
        if (m_ReactionStarted || (curr - m_TimeOfLastReact) < m_MinimumTimeBetweenReacts || force < m_MinForceToConsider) return false;

        return true;
    }

    public void OnDeath() {
        m_ReactionStarted = false;

        //Set all movement values to false
        m_Animator.SetFloat(m_ForwardHash, 0f);
        m_Animator.SetFloat(m_TurnAmount, 0f);

        //Reset all triggers
        m_Animator.ResetTrigger(m_DodgeKey);
        m_Animator.ResetTrigger(m_JumpKey);
        //m_Animator.ResetTrigger(m_TriggerToSet);
    }

}
