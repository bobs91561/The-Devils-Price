using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationCheck : MonoBehaviour
{
    private ThirdPersonCharacter _mThirdPerson;
    private SkillSet _mSkillSet;
    private NavMeshAgent _mAgent;
    private Animator _mAnimator;
    private AIActionDecider _mDecider;
    private ReactionManager m_ReactionManager;

    private int _mForward = Animator.StringToHash("Forward");

    // Start is called before the first frame update
    void Start()
    {
        _mThirdPerson = GetComponent<ThirdPersonCharacter>();
        if (!_mThirdPerson) _mThirdPerson = GetComponentInParent<ThirdPersonCharacter>();
        _mSkillSet = GetComponentInParent<SkillSet>();

        _mAnimator = GetComponent<Animator>();
        if (!_mAnimator) _mAnimator = GetComponentInParent<Animator>();

        _mAgent = GetComponent<NavMeshAgent>();
        if (!_mAgent) _mAgent = GetComponentInParent<NavMeshAgent>();
        if (_mAgent) this.enabled = true;
        else enabled = false;

        _mDecider = GetComponent<AIActionDecider>();
        if (!_mDecider) _mDecider = GetComponentInParent<AIActionDecider>();
        if (_mDecider) this.enabled = true;
        else enabled = false;

        m_ReactionManager = GetComponent<ReactionManager>();
        if (!m_ReactionManager) m_ReactionManager = GetComponentInParent<ReactionManager>();
    }

    private void LateUpdate()
    {
        if (!_mAnimator) return;
        if(_mAgent)
        {
            //Ensures that if the agent is moving along a path, the AI is animating
            if(Mathf.Abs(_mAnimator.GetFloat(_mForward)) <= 0f && Mathf.Abs(_mAgent.velocity.z) > 0f)
            {
                _mAnimator.SetFloat(_mForward, _mAgent.velocity.z);
            }
        }
    }

    private void OnAnimatorMove()
    {
        if(_mThirdPerson && _mAgent)
            _mThirdPerson.OnAnimatorMove();
    }
    /*
    /// <summary>
    /// Ensure the reaction has ended after the animation.
    /// </summary>
    /// <param name="seconds"></param>
    public void CheckReact(float seconds)
    {
        StartCoroutine(SecondsAfter(seconds, true));
    }

    IEnumerator SecondsAfter(float seconds, bool react)
    {
        yield return new WaitForSeconds(seconds);
        if(react) _mThirdPerson.m_IsReacting = false;
    }
    */
    public void OnDeath()
    {
        if (!_mAnimator || !transform.parent) return;
        if (_mAnimator.GetBool("Death") && _mAgent)
        {
            //StartCoroutine(ChangeMeshPosition());
            //_mAgent.enabled = false;
            //GetComponentInParent<Rigidbody>().useGravity = false;
        }
    }
    

    IEnumerator ChangeMeshPosition()
    {
        var t = transform;
        while(transform.localPosition.y > -0.8f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime, transform.position.z);
            yield return null;

        }
    }


    /// <summary>
    /// Ensure the character is frozen during the time period.
    /// </summary>
    /// <param name="seconds"></param>
    public void CheckFreeze(float seconds)
    {

    }

    public void UseAttackWithAnim()
    {
        _mSkillSet.UseAttackWithAnim();
    }

    public void EndAttack()
    {
        _mSkillSet.EndAttack();
    }

    public void FinishReact()
    {
        m_ReactionManager.FinishReact();
    }

    public void EndDodge()
    {
        _mThirdPerson.EndDodge();
    }

    public void DoneWaiting()
    {
        _mDecider.DoneWaiting();
    }
}
