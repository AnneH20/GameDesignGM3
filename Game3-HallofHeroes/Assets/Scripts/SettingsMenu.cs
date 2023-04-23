using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject slider;
    private float level;

    void Start() {
        audioMixer.GetFloat("volume", out level);
        slider.gameObject.GetComponent<Slider>().value = level;
    }
    public void Volume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void Fullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
