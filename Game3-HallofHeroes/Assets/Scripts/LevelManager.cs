using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TMP_Text _textProgress;
    private float _target;
   
    void Awake()
    {
        _loaderCanvas.SetActive(false);
        PauseScreen.isPaused = false;
    }

    // Update is called once per frame
    public async void LoadScene(string _sceneName)
    {
        _progressBar.value = 0;
        var scene = SceneManager.LoadSceneAsync(_sceneName);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);
        
        do {
            await Task.Delay(100);
            _target = Mathf.Clamp01(scene.progress / 0.9f);
            _textProgress.text = "Loading... " + _target * 100f + "%";
        } while (scene.progress < 0.9f);
        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
        PauseScreen.isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        _progressBar.value = Mathf.MoveTowards(_progressBar.value, _target, 3 * Time.deltaTime);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
