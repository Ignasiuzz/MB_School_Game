using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    [Header("Settings")]
    public string mainSceneName = "MainScene";

    void Start()
    {
        Time.timeScale = 1f;
    }

    public void ReturnToGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
