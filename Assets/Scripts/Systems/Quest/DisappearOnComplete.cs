using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DisappearOnComplete : MonoBehaviour {

    public GameObject Flourish;
    public GameObject parent;

	// Use this for initialization
	void Start () {
        if (!parent) parent = transform.parent.gameObject;
	}

    private void OnEnable()
    {
        if (!parent) parent = transform.parent.gameObject;
        if (Flourish)
        {
            GameObject g = Instantiate(Flourish);
            g.transform.parent = parent.transform;
            PSMeshRendererUpdater p = g.GetComponent<PSMeshRendererUpdater>();
            if (p)
            {
                p.MeshObject = parent;
                p.UpdateMeshEffect();
            }

        }


        StartCoroutine("DestroyIn");
    }

    public IEnumerator DestroyIn()
    {
        yield return new WaitForSeconds(2f);
        Destroy(parent);
    }
}
