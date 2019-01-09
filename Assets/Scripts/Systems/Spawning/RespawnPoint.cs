using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Respawn Points play animations/cutscenes when the player is respawned
/// </summary>
public class RespawnPoint : MonoBehaviour
{
    public Vector3 offset;

    [SerializeField] private GameObject meshEffect;
    private GameObject Player;
    private Animator _mAnimator;
    private int _mAnimId = Animator.StringToHash("Respawn");

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.Player;
        _mAnimator = GetComponent<Animator>();
    }

    public void Respawn()
    {
        if (!Player) Player = GameManager.Player;
        Player.transform.position = transform.position + offset;
        Animate();
        ApplyEffect();
        EventManager.Respawn();
    }

    private void Animate()
    {
        //Trigger an animation
        if (!_mAnimator) return;
        _mAnimator.SetTrigger(_mAnimId);
    }

    private void ApplyEffect()
    {
        if (!meshEffect) return;
        meshEffect.GetComponent<PSMeshRendererUpdater>().MeshObject = Player;
        GameObject g = Instantiate(meshEffect);
        g.transform.parent = Player.transform;
        PSMeshRendererUpdater ps = g.GetComponent<PSMeshRendererUpdater>();
        ps.UpdateMeshEffect();
        
        StartCoroutine(DestroyMesh(ps.FadeTime + 5f, g));
    }

    IEnumerator DestroyMesh(float sec, GameObject g)
    {
        yield return new WaitForSeconds(sec-5f);
        g.GetComponent<PSMeshRendererUpdater>().IsActive = false;
        yield return new WaitForSeconds(sec);
        Destroy(g);
    }
}
