using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PixelCrushers.DialogueSystem.UnityGUI;
using PixelCrushers.DialogueSystem;
using PixelCrushers;

public class MainMenu : MonoBehaviour
{
    [TextArea]
    public string startMessage = "Press Escape for Menu";
    public KeyCode menuKey = KeyCode.Escape;
    public bool closeWhenQuestLogOpen = true;

    public UnityEvent onOpen = new UnityEvent();
    public UnityEvent onClose = new UnityEvent();

    public GameObject mainMenu;

    private QuestLogWindow questLogWindow = null;
    private bool isMenuOpen = false;

    void Start()
    {
        if (questLogWindow == null) questLogWindow = FindObjectOfType<QuestLogWindow>();
        if (!string.IsNullOrEmpty(startMessage)) DialogueManager.ShowAlert(startMessage);
    }

    void Update()
    {
        
        if (Input.GetKeyDown(menuKey) && !DialogueManager.isConversationActive && !IsQuestLogOpen())
        {
            SetMenuStatus(!isMenuOpen);
        }
        // If you want to lock the cursor during gameplay, add ShowCursorOnConversation to the Player,
        // and uncomment the code below:
        //if (!DialogueManager.isConversationActive && !isMenuOpen && !IsQuestLogOpen ()) 
        //{
        //	Screen.lockCursor = true;
        //}
    }

   /* private void WindowFunction(int windowID)
    {
        if (GUI.Button(new Rect(10, 60, windowRect.width - 20, 48), "Quest Log"))
        {
            if (closeWhenQuestLogOpen) SetMenuStatus(false);
            OpenQuestLog();
        }
        if (GUI.Button(new Rect(10, 110, windowRect.width - 20, 48), "Save Game"))
        {
            SetMenuStatus(false);
            SaveGame();
        }
        if (GUI.Button(new Rect(10, 160, windowRect.width - 20, 48), "Load Game"))
        {
            SetMenuStatus(false);
            LoadGame();
        }
        if (GUI.Button(new Rect(10, 210, windowRect.width - 20, 48), "Clear Saved Game"))
        {
            SetMenuStatus(false);
            ClearSavedGame();
        }
        if (GUI.Button(new Rect(10, 260, windowRect.width - 20, 48), "Close Menu"))
        {
            SetMenuStatus(false);
        }
    }*/

    public void Open()
    {
        GameObject.FindGameObjectWithTag("HealthCanvas").SetActive(false);
        SetMenuStatus(true);
    }

    public void Close()
    {
        SetMenuStatus(false);
        GameObject.FindGameObjectWithTag("HealthCanvas").SetActive(true);
    }

    private void SetMenuStatus(bool open)
    {
        isMenuOpen = open;
        GameManager.SetPlayerInput(!open);
        //if (open) windowRect = scaledRect.GetPixelRect();
        Time.timeScale = open ? 0 : 1;
        if (open) onOpen.Invoke(); else onClose.Invoke();
        if (open) ActivateMenu(); else DeactivateMenu();
    }

    private void ActivateMenu()
    {
        mainMenu.SetActive(true);
    }

    private void DeactivateMenu()
    {
        mainMenu.SetActive(false);
    }

    private bool IsQuestLogOpen()
    {
        return (questLogWindow != null) && questLogWindow.isOpen;
    }

    #region Buttons
    public void OpenQuestLog()
    {
        if (closeWhenQuestLogOpen) SetMenuStatus(false);
        if ((questLogWindow != null) && !IsQuestLogOpen())
        {
            questLogWindow.Open();
        }
    }

    public void SaveGame()
    {
        SetMenuStatus(false);
        var saveSystem = FindObjectOfType<SaveSystem>();
        if (saveSystem != null)
        {
            SaveSystem.SaveToSlot(1);
        }
        else
        {
            string saveData = PersistentDataManager.GetSaveData();
            PlayerPrefs.SetString("SavedGame", saveData);
            Debug.Log("Save Game Data: " + saveData);
        }
        DialogueManager.ShowAlert("Game saved.");
    }

    public void LoadGame()
    {
        SetMenuStatus(false);
        PersistentDataManager.LevelWillBeUnloaded();
        var saveSystem = FindObjectOfType<SaveSystem>();
        if (saveSystem != null)
        {
            if (SaveSystem.HasSavedGameInSlot(1))
            {
                SaveSystem.LoadFromSlot(1);
                DialogueManager.ShowAlert("Game loaded.");
            }
            else
            {
                DialogueManager.ShowAlert("Save a game first.");
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("SavedGame"))
            {
                string saveData = PlayerPrefs.GetString("SavedGame");
                Debug.Log("Load Game Data: " + saveData);
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.LoadGame(saveData);
                }
                else
                {
                    PersistentDataManager.ApplySaveData(saveData);
                    DialogueManager.SendUpdateTracker();
                }
                DialogueManager.ShowAlert("Game loaded.");
            }
            else
            {
                DialogueManager.ShowAlert("Save a game first.");
            }
        }
    }


    public void ClearSavedGame()
    {
        SetMenuStatus(false);
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            PlayerPrefs.DeleteKey("SavedGame");
            Debug.Log("Cleared saved game data");
        }
        DialogueManager.ShowAlert("Saved Game Cleared From PlayerPrefs");
    }

    public void QuitGame()
    {
        //Tell the gamemanager to handle the fade

        //Load the main menu scene

        //Destroy all objects
    }
    #endregion
}
