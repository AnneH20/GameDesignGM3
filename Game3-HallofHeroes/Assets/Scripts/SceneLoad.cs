using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public void gotoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Replace "MainMenu" with the name of your main menu scene
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
