using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GameOver : MonoBehaviour
{
    public static GameOver Instance;
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] public GameObject GameOverMenu;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TMP_Text _textProgress;
    private float _target;

    private GameObject battleSystem;


   
    void Awake()
    {
        _loaderCanvas.SetActive(false);
        GameOverMenu.SetActive(false);
        PauseScreen.isPaused = false;
    }
    public async void LoadScene()
    {
        PlayerController.isDead = false;
        _progressBar.value = 0;
        var scene = SceneManager.LoadSceneAsync(1);
        scene.allowSceneActivation = false;
        _loaderCanvas.SetActive(true);
        
        do {
            await Task.Delay(100);
            _target = Mathf.Clamp01(scene.progress / 0.9f);
            _textProgress.text = "Loading... " + _target * 100f + "%";
        } while (scene.progress < 0.9f);
        scene.allowSceneActivation = true;
        GameOverMenu.SetActive(false);
        _loaderCanvas.SetActive(false);
        PauseScreen.isPaused = false;
        
    }

    void Update()
    {
        if (PlayerController.isDead)
        {
            GameOverMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (!PlayerController.isDead && !PauseScreen.isPaused)
        {
            GameOverMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        _progressBar.value = Mathf.MoveTowards(_progressBar.value, _target, 3 * Time.deltaTime);
    }


    public async void MainMenu()
    {
        PlayerController.isDead = false;
        _progressBar.value = 0;
        var scene = SceneManager.LoadSceneAsync("MainMenu");
        scene.allowSceneActivation = false;
        _loaderCanvas.SetActive(true);
        
        do {
            await Task.Delay(100);
            _target = Mathf.Clamp01(scene.progress / 0.9f);
            _textProgress.text = "Loading... " + _target * 100f + "%";
        } while (scene.progress < 0.9f);
        scene.allowSceneActivation = true;
        GameOverMenu.SetActive(false);
        _loaderCanvas.SetActive(false);
        PauseScreen.isPaused = false;
    }
}