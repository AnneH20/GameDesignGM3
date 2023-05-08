using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NextLevelTile : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private GameObject warning;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PlayerPrefs.GetInt("Boss Dead") == 0)
            {
                warning.SetActive(true);
                StartCoroutine(DisplayText());
            }
            else
            {
                LoadNextLevel(); 
            }
        }
    }

    protected void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    public IEnumerator DisplayText() 
   {
    yield return new WaitForSeconds(3f);
    warning.SetActive(false);
   }
}

