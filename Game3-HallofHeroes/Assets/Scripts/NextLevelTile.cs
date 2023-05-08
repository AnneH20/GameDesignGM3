using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NextLevelTile : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    private bool test = false;
    private TextMeshPro guiText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LoadNextLevel();
            PlayerController.Instance.gameObject.transform.position = new Vector2(0, 0);
        }
    }

    protected void LoadNextLevel()
    {
        if (test)
            SceneManager.LoadScene(nextLevelName);
        else
        {
            StartCoroutine(ShowMessage("Boss still lingers on this level", 2));
        }
    }

    IEnumerator ShowMessage (string message, float delay) {
        guiText.text = message;
        guiText.enabled = true;
        yield return new WaitForSeconds(delay);
        guiText.enabled = false;
    }
}

