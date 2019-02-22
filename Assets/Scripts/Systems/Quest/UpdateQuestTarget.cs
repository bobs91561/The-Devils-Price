using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompassNavigatorPro;

[CreateAssetMenu(menuName ="Quest/UpdateTarget")]
public class UpdateQuestTarget : ScriptableObject
{
    public GameObject TargetPrefab;
    private GameObject m_TargetObject;
    private GameObject m_Parent;

    public void VisitTarget()
    {
        if (!m_TargetObject) return;

        m_TargetObject.GetComponent<CompassProPOI>().isVisited = true;
    }
    
    public void ChangeTarget(GameObject g)
    {
        m_Parent = g;
        if (!m_TargetObject) m_TargetObject = Instantiate(TargetPrefab);
        m_TargetObject.transform.parent = m_Parent.transform;
        m_TargetObject.transform.localPosition = new Vector3(0, 0, 0);
        m_TargetObject.GetComponent<CompassProPOI>().isVisited = false;
        
    }
}
