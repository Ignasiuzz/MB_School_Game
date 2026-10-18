using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{   
    public GameObject menuCanvas;
    public TMP_Text saveButtonText;

    void Start()
    {
        menuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            bool isActive = !menuCanvas.activeSelf;
            menuCanvas.SetActive(isActive);

            // Pause or resume game
            Time.timeScale = isActive ? 0f : 1f;
        }
    }

    public void ToggleMenu()
    {
        bool isActive = !menuCanvas.activeSelf;
        menuCanvas.SetActive(isActive);

        // Pause or resume the game
        Time.timeScale = isActive ? 0f : 1f;
    }

    public void OnStartGame()
    {
        menuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnSaveGame()
    {
        StartCoroutine(ShowSaveMessage());
    }

    private IEnumerator ShowSaveMessage()
    {
        if (saveButtonText == null) yield break;

        saveButtonText.text = "GAME SAVED";

        yield return new WaitForSecondsRealtime(1.5f);

        saveButtonText.text = "SAVE";
    }

    public void OnExitGame()
    {
        Application.Quit();
        // UnityEditor.EditorApplication.isPlaying = false;
    }
}
