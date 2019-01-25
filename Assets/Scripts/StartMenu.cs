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
    public GameObject IntroPanel;
    public GameObject ContinueBtn;
    public List<GameObject> objectsToDisableOnIntro = new List<GameObject>();

    public void NewGame()
    {
        GameManager.HandleFadeManually = true;
        StartCoroutine(WaitToLoad());
    }

    private IEnumerator WaitToLoad()
    {
        //Begin intro scaffolding
        if (IntroPanel)
        {
            foreach (GameObject g in objectsToDisableOnIntro)
                g.SetActive(false);
            IntroPanel.SetActive(true);
        }
        yield return new WaitForSeconds(3.5f);
        if (ContinueBtn) ContinueBtn.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Joystick1Button0));
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
