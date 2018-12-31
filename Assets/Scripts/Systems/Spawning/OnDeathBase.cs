using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnDeathBase : MonoBehaviour {

    public float ExperienceGainedOnDeath;
    public GameObject DeathFlourish;

    public virtual void OnDeath()
    {
        ApplyExperience();
        gameObject.SetActive(false);
        //Flourish();
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
