using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIActionDecider: MonoBehaviour {
    public static GameObject Player;

    private AIController _controller;
    public SkillSet skillSet;
    public GameObject AI;

    public AIAction currentAction;

    public float currentPriority = 0f;

    public float tiredness = 0f;
    public float combatApproach = 0f;
    public bool combat = false;
    public bool combatMoveActive = false;

    public bool CombatNearby;
    public bool Friendly;

    public List<AIAction> actions;
    public List<GameObject> patrolPoints;
    public int patrolPoint;

	// Use this for initialization
	void Start()
    {
        if (!AI) AI = gameObject;
        if (actions == null)
        {
            actions = new List<AIAction>();
        }
        for(int i = 0; i < actions.Count; i++)
        {
            actions[i] = Instantiate(actions[i]);
            actions[i].Initialize(gameObject);
        }
        if (patrolPoints == null) patrolPoints = new List<GameObject>();
        skillSet = GetComponent<SkillSet>();
        _controller = GetComponent<AIController>();
        patrolPoint = 0;
        tiredness = 0f;
        combat = false;
    }

    public void Tick()
    {
        List<AIAction> feasible = new List<AIAction>();
        foreach(AIAction a in actions)
        {
            if (a.ActionFeasible())
                feasible.Add(a);
        }
        if (feasible.Count>1)
            feasible.Sort((a1,a2)=>(int)(a2.priority-a1.priority));

        int i = 0;
        bool f = false;
        while (!f && i < feasible.Count && feasible[i])
        {
            f = feasible[i].Tick();
            if (f && currentAction == null) currentAction = feasible[i];
            if (f && currentAction != feasible[i])
            {
                currentPriority = feasible[i].priority;
                currentAction.SetActive();      //Sets previous action to false
                currentAction = feasible[i];    //Set currentAction = new Action
                currentAction.SetActive();      //Sets new action to true
            }
            else if (f && !currentAction.isActive)
            {
                currentAction.SetActive();

                currentPriority = feasible[i].priority;
            }
            i++;
        }
        if (!f)
            currentAction.Tick();
    }

    public Vector3 FindNearbyPatrolPoint()
    {
        Vector3 v = new Vector3(5,0,1);
        if (patrolPoints.Count == 0)
        {
            return v;
        }
        else
        {
            v = patrolPoints[patrolPoint].transform.position;
            patrolPoint = (patrolPoint >= patrolPoints.Count-1) ? 0 : patrolPoint+1;
        }
        return v;
    }

    /// <summary>
    /// Method called when gameobject enters combat
    /// </summary>
    public void EnterCombat()
    {
        if (combat) return;
        combat = true;
        skillSet.Combat();
        skillSet.DrawWeapons();
        //GetComponent<Animator>().SetBool("Combat", true);
        //Send out combat alerts to nearby allies
        Collider[] cs = Physics.OverlapSphere(transform.position, 7f, 1 << LayerMask.NameToLayer("Enemy"));
        
        foreach (Collider c in cs)
        {
            if(c.gameObject.GetComponent<AIActionDecider>())
                c.gameObject.SendMessage("CombatIsNearby");
        }
    }

    public void ExitCombat()
    {
        if (!combat) return;
        combat = false;
        CombatNearby = false;
        skillSet.Combat();
        //GetComponent<Animator>().SetBool("Combat", false);
        GetComponent<AIAttackController>().enabled = false;
    }

    public void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3f);
    }

    public void CombatIsNearby()
    {
        CombatNearby = true;
        //EnterCombat();
        SendMessage("EnterCombat");
        FaceTarget(Player.transform.position);
    }

    private void OnDeath()
    {
        ExitCombat();
        patrolPoint = 0;
    }

    void OnDrawGizmosSelected()
    {

        var nav = GetComponent<NavMeshAgent>();
        if (nav == null || nav.path == null)
            return;

        var line = this.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            line.SetWidth(0.5f, 0.5f);
            line.SetColors(Color.yellow, Color.yellow);
        }

        var path = nav.path;

        line.SetVertexCount(path.corners.Length);

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }

    }
}
