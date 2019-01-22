using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnDeathBase : MonoBehaviour {

    public float ExperienceGainedOnDeath;
    public GameObject DeathFlourish;

    private Animator m_Animator;

    private void Start()
    {
        SetAnimator();
    }

    private void SetAnimator()
    {
        m_Animator = GetComponent<Animator>();
        if (!m_Animator) m_Animator = GetComponentInChildren<Animator>();
    }

    public virtual void OnDeath()
    {
        MatchTarget();
        ApplyExperience();
        gameObject.SetActive(false);
        //Flourish();
    }

    private void MatchTarget()
    {
        if (!m_Animator) SetAnimator();
        m_Animator.MatchTarget(transform.position, transform.rotation, AvatarTarget.Body, new MatchTargetWeightMask(Vector3.one, 1f), 0f);
    }
    public void ApplyExperience()
    {
        ExperienceHolder.AddExperience(ExperienceGainedOnDeath);
    }

    public void Flourish()
    {
        if (!DeathFlourish) return;
        GameObject g = Instantiate(DeathFlourish);
        g.transform.parent = transform;
        PSMeshRendererUpdater p = g.GetComponent<PSMeshRendererUpdater>();
        if (p)
        {
            p.MeshObject = gameObject;
            p.UpdateMeshEffect();
        }
    }
}
