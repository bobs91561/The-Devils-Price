using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PixelCrushers.DialogueSystem.UnityGUI;
using PixelCrushers.DialogueSystem;
using PixelCrushers;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void NewGame()
    {
        GameManager.HandleFadeManually = true;
        StartCoroutine(WaitToLoad());
    }

    private IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
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
}
