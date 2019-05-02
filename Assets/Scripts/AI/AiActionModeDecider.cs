using Assets.Scripts.Systems.Combat;
using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiActionModeDecider : AIActionDecider
{
    private AttackController m_AttackController;
    private ThirdPersonCharacter m_ThirdPerson;

    private int m_ModeNumber = 0;
    private int m_TotalModes;

    public List<ModeModifiers> Modes;
    private ModeModifiers m_CurrentMode;

    public AIAction ChangeModeAction;
    private bool changeInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        m_TotalModes = Modes.Count;
    }

    protected override void InitializeActions()
    {
        base.InitializeActions();
        if (!changeInitialized)
        {
            ChangeModeAction = Instantiate(ChangeModeAction);
            ChangeModeAction.Initialize();
            changeInitialized = true;
        }
        actions.Add(ChangeModeAction);
    }

    public void ChangeMode()
    {
        if (m_ModeNumber >= m_TotalModes) return;

        m_CurrentMode = Modes[m_ModeNumber];
        EvaluateModeModifier();

        m_ModeNumber++;
    }

    private void EvaluateModeModifier()
    {
        // Go through each of the mode's actions
        foreach (MonoBehaviour c in m_CurrentMode.BehavioursToBeDisabled)
            c.enabled = false;
        foreach (MonoBehaviour c in m_CurrentMode.BehavioursToBeEnabled)
            c.enabled = true;
        foreach (MonoBehaviour c in m_CurrentMode.BehavioursToBeDestroyed)
            Destroy(c);

        foreach (Collider c in m_CurrentMode.CollidersToChangeState)
            c.enabled = !c.enabled;

        foreach (GameObject g in m_CurrentMode.GameObjectsToChangeState)
            g.SetActive(!g.activeSelf);
        foreach (GameObject g in m_CurrentMode.GameObjectsToDestroy)
            Destroy(g);

        //For each modifier type in the modifiers to constant scripts,
        //  perform the correct action.
        // For an action change, update the list of actions for the next mode
        // For a movement type change, call ThirdPersonCharacter's ChangeMovement method
        foreach(ModifierType t in m_CurrentMode.ModifiersToConstantScripts)
        {
            switch (t)
            {
                case ModifierType.MOVEMENT:
                    m_ThirdPerson.ChangeMovementType(m_CurrentMode.movementType);
                    break;
                case ModifierType.ACTIONS:
                    actions = m_CurrentMode.ActionsForNextMode;
                    InitializeActions();
                    break;
            }
        }
    }

    [System.Serializable]
    public class ModeModifiers
    {
        public List<MonoBehaviour> BehavioursToBeDisabled;
        public List<MonoBehaviour> BehavioursToBeDestroyed;
        public List<MonoBehaviour> BehavioursToBeEnabled;
        public List<Collider> CollidersToChangeState;
        public List<GameObject> GameObjectsToChangeState;
        public List<GameObject> GameObjectsToDestroy;
        public List<ModifierType> ModifiersToConstantScripts;
        public List<AIAction> ActionsForNextMode;
        public MovementType movementType;
        
    }
}

public enum ModifierType
{
    ACTIONS, MOVEMENT
}
